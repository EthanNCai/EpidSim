using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

/*
规则：
    destination 只能由 scheduler 设置， 只能由 navigator 取消
*/

public class Sims : MonoBehaviour
{
    // Daily Flushed INFOs
    public bool isTodayOff = false; 
    Sims maxExposedBy = null;
    int maxExposed = 0;

    // Persistance INFOs
    public StringBuilder stringBuilder = new StringBuilder();
    public int uid;
    public string simsName;
    public Place destination = null;
    public Place inSite = null;
    public ResidentialPlace home;
    public OfficePlace office;
    public SimSchedule simSchedule;
    public List<MedicalPlace> medicalPlaces = new List<MedicalPlace>(); // just a ref
    // public List<CommercialPlace> commercialPlaces = new List<CommercialPlace>(); // just a ref
    private float speed = 0f;
    public int counter = 0;
    public Rigidbody2D simsRigidbody;
    private Vector2? finalApproachPosition = null; // 使用 nullable 变量
    private static float temperature = 0.5f;
    public static (int,int) keyTimeMorningRanges = (0,16);
    public static int minWorkHours = 7; 
    public (int,int) keyTimeMorning;
    public (int,int) keyTimeNoon;
    public (int,int) keyTimeDusk;
    public HashSet<int> dayOff;
    public int toWork = 0;
    public int toWorkQ = 0;
    // General
    public int balance = 10000;

    public PlaceManager placeManager;

    // DEBUG RELATED
    public InfoManager infoManager;
    // INFECTION RELATED

    public Infection infection = null;
    InfectionStatus infectionStatus = InfectionStatus.Suscptible;
    VirusVolumeGridMapManager virusVolumeMapManager;


    // debug only
    public string infectionRepr = "";
    public string inSiteRepr = "";

    public void SimsInit(
        VirusVolumeGridMapManager virusVolumeMapManager,
        InfoManager infoManager,
        PlaceManager placeManager,
        bool infected = false
        )
    {
        this.placeManager = placeManager;
        this.infoManager = infoManager;
        this.virusVolumeMapManager = virusVolumeMapManager;
        this.uid = UniqueIDGenerator.GetUniqueID();
        this.simsName = SimsNameGenerator.GetSimsName();
        gameObject.name = this.simsName + uid;
        this.destination = home;
        this.simsRigidbody = GetComponent<Rigidbody2D>();
        this.GenerateWorkHours();
        TimeManager.OnQuarterChanged += HandleTimeChange;
        TimeManager.OnDayChanged += HandleDayChange;
        this.toWork = keyTimeMorning.Item1;
        this.toWorkQ = keyTimeMorning.Item2;
        this.speed = UnityEngine.Random.Range(4f,11f);
        this.dayOff = RandomManager.GetRandomDayOff();
        this.simSchedule = new SimSchedule(this);
        this.HandleDayChange(0);
    }
    // public void

    public void ReceivePaycheck(int paycheckIn){
        this.balance += paycheckIn;
    }
    public void DoLifeExpense(){
        if(this.balance - PriceMenu.QSimLifeExpense < 0){
            this.home.AttachSubsidyToResidential(-1 * (this.balance - PriceMenu.QSimLifeExpense));
            this.balance = 0;
        }else{
            this.balance -= PriceMenu.QSimLifeExpense;
        }
    }

    public void ManuallyInfect(){
        // DO not call infection info debugget method here
        this.infectionStatus = InfectionStatus.Infected;
        this.infection = new Infection(InfectionParams.GetInfectionPeriod(),this);
        this.infectionRepr = this.infection.ToString();
    }

    // ============== Time Handlers ==============

    private void HandleTimeChange((int,int) timeNow){
        
        HandleEveryQ(timeNow);
        if(timeNow == keyTimeMorning){
            HandleMorningKeyTime(timeNow);
        }else if(timeNow == keyTimeDusk){
            HandleDuskKeyTime(timeNow);
        }else if(timeNow == keyTimeNoon){
            HandleNoonKeyTime(timeNow);
        }else if(this.destination != null){
            HandleOnTheRoad(timeNow);
        }
    }
    
    private void HandleEveryQ((int,int) timeNow){
        
        // 这个逻辑用于处理每个Q的支出和Paycheck
        if(inSite == this.office){
            this.ReceivePaycheck(PriceMenu.QSimOfficeIncome);
        }
        this.DoLifeExpense();
    }
    private void HandleDayChange(int dayIndex){
        // infection related
        this.maxExposed = 0;
        this.isTodayOff = GetIsTodayOff(dayIndex);
    }

    public void HandlePolicyChange(){
        
    }
    public void HandleInfectionChange(){

    }

    // 早晨KeyTime的更新
    private void HandleMorningKeyTime((int,int) timeNow){
        this.simSchedule.UpdateScheduleOnMorning();
        if (infection != null){
            this.DailyInteractWithInfection();
        }
        else{
            this.UpdateExposureFromTile();
            this.TryToInfect();
        }
        this.SetOutMoving(simSchedule.GetDestination());
    }

    // 晚间KeyTime的更新
    private void HandleDuskKeyTime((int,int) timeNow){
        
        this.simSchedule.UpdateScheduleOnDusk();
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
        this.SetOutMoving(simSchedule.GetDestination());
    }

    // 中午KeyTime的更新
    private void HandleNoonKeyTime((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
    }

    // On the road KeyTime的更新
    private void HandleOnTheRoad((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
    }
    
    // ============== End of Time Handlers ==============
    private void TryToInfect(){
        if(this.infectionStatus == InfectionStatus.Dead){
            Debug.LogWarning("<A dead sim tring to get infected> We currently don't handle dead logic");
            return;
        }
        bool isInfected = InfectionParams.RollTheInfectionDice(maxExposed, this.infectionStatus);
        if(isInfected){
            this.infection = new Infection(InfectionParams.GetInfectionPeriod(),this.maxExposedBy);
            this.infectionRepr = this.infection.ToString();
            infoManager.infectionInfoManager.InfectionAddition(this,this.infectionStatus);
            this.infectionStatus = InfectionStatus.Infected;
        } 
    }
    private void PolluteThePosition(){
        // Debug.Log("Polluting");  
        if(this.infection.virusVolume <= 0){
            return;
        }
        Debug.Assert(this.infection != null, "the virus is null");
        Vector2 currentPosition = transform.position;
        Vector2Int currentCellPosition = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
        virusVolumeMapManager.PolluteTheTile(currentCellPosition, this, this.infection.virusVolume);
    }

    private void GenerateWorkHours()
    {
        int tmpMorningKeyTime, tmpDuskKeyTime, tmpNoonKeyTime;
        int morningKeyTimeQtr, duskKeyTimeQtr, noonKeyTimeQtr;


        int workZoneMid = (keyTimeMorningRanges.Item1 + keyTimeMorningRanges.Item2) / 2;  // 上班时间中位数

        tmpMorningKeyTime = RandomManager.NextGaussianInt(workZoneMid, 1, keyTimeMorningRanges.Item1, keyTimeMorningRanges.Item2);
        tmpDuskKeyTime = tmpMorningKeyTime + minWorkHours;
        tmpNoonKeyTime = (tmpMorningKeyTime + tmpDuskKeyTime) / 2;  // 下班时间中位数

        morningKeyTimeQtr = RandomManager.NextInt(0, 4);
        duskKeyTimeQtr = RandomManager.NextInt(0, 4);
        noonKeyTimeQtr = RandomManager.NextInt(0, 4);

        this.keyTimeMorning = (tmpMorningKeyTime, morningKeyTimeQtr);
        this.keyTimeDusk = (tmpDuskKeyTime, duskKeyTimeQtr);
        this.keyTimeNoon = (tmpNoonKeyTime, noonKeyTimeQtr);
    }

    public void AllocateHomeOffice(ResidentialPlace home, OfficePlace office){
        this.home = home;
        this.office = office;
        this.destination = home;
    }

    public bool IsInDestination(Vector2 currentPosition)
    {
        if (destination == null) return false;
        return Utils.IsPointInsideArea(currentPosition, destination.placeLLAnchor, destination.placeURAnchor);
    }

    public void DailyInteractWithInfection(){
        this.infectionStatus = infection.Progress();
        this.simSchedule.UpdateScheduleOnInfectionChanged();
        this.infectionRepr = this.infection.ToString();
        PolluteThePosition();
        if(this.infectionStatus == InfectionStatus.Recovered){
            this.infoManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
            this.infection = null;
            Debug.Log($"{this.simsName} has just recoverd!");
        }else if(this.infectionStatus == InfectionStatus.Dead){
            this.infoManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
            this.infection = null;
            Debug.Log($"{this.simsName} has just dead!");
        }
    }

    public void Navigate()
    {
        if (destination == null)
        {
            return; // 没有目标地，直接返回. Standby 状态
        }

        Vector2 currentPosition = transform.position;
        if (IsInDestination(currentPosition))
        {
            simsRigidbody.velocity = Vector2.zero;
            if (finalApproachPosition == null){
                finalApproachPosition = destination.GetRandomPositionInside();
            }

            if (Vector2.Distance(finalApproachPosition.Value, currentPosition) < 0.1f){
                FinishUpMoving();
            }else{
                NaturallyFinalApproach();
            }
            return;
        }

        // 确保 destination 仍然存在
        // Debug.Log("Long term moving...");
        if (destination != null)
        {   
            Vector2Int currentCellPosition = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
            Vector2Int mapSize = destination.flowFieldMapsManager.mapManager.mapsize;
            Vector2Int targetDirection;
            if(GridNodeMap<FlowFieldNode>.CheckIfOutside(currentCellPosition,mapSize)){
                targetDirection = GridNodeMap<FlowFieldNode>.GetReturnDirection(currentCellPosition,mapSize);
            }else{
                FlowFieldNode flowFieldNode = destination.flowFieldMapsManager.flowFieldMap.GetNodeByCellPosition(currentCellPosition);
                targetDirection = flowFieldNode.flowFieldDirection;
            }
            NaturallyMove(targetDirection);
        }
    }

    public void NaturallyMove(Vector2Int direction)
    {
        Vector2 natureDirection = Utils.GetRandomizedDirection(direction, temperature);
        Vector2 newVelocity = natureDirection.normalized * speed;
        simsRigidbody.velocity = simsRigidbody.velocity * 0.99f + newVelocity * 0.01f;
    }

    public void NaturallyFinalApproach()
    {
        if (finalApproachPosition == null) return;

        transform.position = Vector2.Lerp(transform.position, finalApproachPosition.Value, speed * 0.5f * Time.deltaTime);
    }

    public void FinishUpMoving()
    {
        this.inSite = this.destination;
        this.inSite.InsertInsiteSims(this);
        finalApproachPosition = null; // 设为 null，表示无效
        destination = null;
        UpdateInSiteRepr();
        
    }

    public bool GetIsTodayOff(int day){
        // return true;
        // today Off 需要被Sim的经济情况还有今天是不是休息日来进行一个主观决定，因此需要被放在Sim主类里面
        if(dayOff.Contains(day)){
            // return true;
            if(RandomManager.FlipTheCoin(0.5f)){
                return true;
            }
        }
        return false;
    }

    public void SetOutMoving(Place destination)
    {
        if(this.inSite != null){
            this.inSite.RemoveInsiteSims(this);
        }
        this.destination = destination;
        this.inSite = null; // 设为 null，表示无效
        UpdateInSiteRepr();
    }
    public static Vector2Int GetReturnDirection(Vector2Int position, Vector2Int mapSize)
    {
        int dirX = 0;
        int dirY = 0;
        if (position.x < 0) dirX = 1;
        else if (position.x >= mapSize.x) dirX = -1;
        
        if (position.y < 0) dirY = 1;
        else if (position.y >= mapSize.y) dirY = -1;
        
        return new Vector2Int(dirX, dirY);
    }
    public void UpdateExposureFromTile()
    {
        // 这个函数用于更新Sim被Tile所感染的情况，以及更新Repr
        Debug.Assert(infection == null, "infection is not null");
        Vector2 currentPosition = transform.position;
        Vector2Int currentCellPosition = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
        VirusVolumeNode virusVolumeNode = virusVolumeMapManager.virusVolumeMap.GetNodeByCellPosition(currentCellPosition);
        if (virusVolumeNode == null) return;
        int volumeOnTheGround = virusVolumeNode.virusVolumeAndSims.Item1;
        if (volumeOnTheGround >= this.maxExposed){
            this.maxExposedBy = virusVolumeNode.virusVolumeAndSims.Item2;
            this.maxExposed = volumeOnTheGround;
        }
        stringBuilder.Clear();
        stringBuilder.Append("exposed: ").Append(this.maxExposed).Append(", ");
        stringBuilder.Append("by: ").Append(this.maxExposedBy);
        this.infectionRepr = stringBuilder.ToString();
    }
    public void UpdateInSiteRepr(){
        stringBuilder.Clear();
        if ( this.inSite != null){
            stringBuilder.Append(this.inSite.palaceName);
        }else{
            stringBuilder.Append("Not in Site");
        }
        this.inSiteRepr = stringBuilder.ToString();
    }

    public void Update()
    {
        Navigate();
        // ScheduleUpdate();
    }
}
