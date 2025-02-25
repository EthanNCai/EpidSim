using System;
using System.Security.Cryptography;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

/*
规则：
    destination 只能由 scheduler 设置， 只能由 navigator 取消
    
*/

public class Sims : MonoBehaviour
{
    // BASIC INFOs
    public StringBuilder stringBuilder = new StringBuilder();
    public int uid;
    public string simsName;
    public Place destination = null;
    public Place inSite = null;
    public ResidentialPlace home;
    public OfficePlace office;
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
    public int toWork = 0;
    public int toWorkQ = 0;

    // DEBUG RELATED
    public InfoDebuggerManager infoDebuggerManager;

    // INFECTION RELATED

    public Infection infection = null;
    InfectionStatus infectionStatus = InfectionStatus.Suscptible;
    VirusVolumeGridMapManager virusVolumeMapManager;
    Sims maxExposedBy = null;
    int maxExposed = 0;

    // debug only
    public string infectionRepr = "";
    public string inSiteRepr = "";

    public void SimsInit(
        VirusVolumeGridMapManager virusVolumeMapManager,
        InfoDebuggerManager infoDebuggerManager,
        bool infected = false
        )
    {
        this.infoDebuggerManager = infoDebuggerManager;
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
    }

    public void ManuallyInfect(){
        // DO not call infection info debugget method here
        this.infectionStatus = InfectionStatus.Infected;
        this.infection = new Infection(InfectionParams.GetInfectionPeriod(),this);
        this.infectionRepr = this.infection.ToString();
    }
    private void HandleTimeChange((int,int) timeNow){
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
    private void HandleOnTheRoad((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateInfectionRepr();
        }
    }
    private void HandleMorningKeyTime((int,int) timeNow){
        if (infection != null){
            this.infectionStatus = infection.Progress();
            this.infectionRepr = this.infection.ToString();
            PolluteThePosition();
            if(this.infectionStatus == InfectionStatus.Recovered){
                this.infoDebuggerManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
                this.infection = null;
                Debug.Log($"{this.simsName} has just recoverd!");
            }else if(this.infectionStatus == InfectionStatus.Dead){
                this.infoDebuggerManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
                this.infection = null;
                Debug.Log($"{this.simsName} has just dead!");
            }
        }
        else{
            UpdateInfectionRepr();
            TryToInfect();
        }
        this.SetOutMoving(this.office);
    }
    private void HandleDuskKeyTime((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateInfectionRepr();
        }
        this.SetOutMoving(this.home);
    }
    private void HandleNoonKeyTime((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateInfectionRepr();
        }
    }

    private void HandleDayChange(int dayIndex){
        // infection related
        this.maxExposed = 0;
    }
    private void TryToInfect(){
        // Vector2 currentPosition = transform.position;
        // Vector2Int currentCellPosition = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));
        // VirusVolumeNode virusVolumeNode = virusVolumeMapManager.virusVolumeMap.GetNodeByCellPosition(currentCellPosition);
        // int virusVolume = virusVolumeNode.virusVolumeAndSims.Item1;
        // Sims infectedBy = virusVolumeNode.virusVolumeAndSims.Item2;
        if(this.infectionStatus == InfectionStatus.Dead){
            Debug.LogWarning("<A dead sim tring to get infected> We currently don't handle dead logic");
            return;
        }
        bool isInfected = InfectionParams.RollTheInfectionDice(maxExposed, this.infectionStatus);
        if(isInfected){
            this.infection = new Infection(InfectionParams.GetInfectionPeriod(),this.maxExposedBy);
            this.infectionRepr = this.infection.ToString();
            infoDebuggerManager.infectionInfoManager.InfectionAddition(this,this.infectionStatus);
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
    public void SetOutMoving(Place destination)
    {
        if(this.inSite == null) return;
        this.destination = destination;
        this.inSite.RemoveInsiteSims(this);
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
    public void UpdateInfectionRepr()
    {
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
