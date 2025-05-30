using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Assertions;
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
    int accumulatedPaycheckToday = 0;
    int accumulatedSubsidyToday = 0;
    int dayRecord = 0;

    SicknessTag sicknessTag = SicknessTag.Normal;

    // dynamic 
    // public bool isUnfinishedQTRQuota = false;
    public bool isUnfinishedPCRQuota = false;
    SpriteRenderer spriteRenderer;
    public float pcrFreshness = 0f;  // 0-100
    private float QpcrFreshnessStaleStep = 1.5f;
    public bool recentPcrResult = false;
    private Color pcrColor;

    // Persistance INFOs
    public StringBuilder stringBuilder = new StringBuilder();
    public int uid;
    public string simsName;
    public Place destination = null;
    public Place inSite = null;
    public ResidentialPlace home;
    public OfficePlace office;
    public SimScheduler simScheduler;
    public SimsDiary simDiary;
    private float speed = 0f;
    public int counter = 0;
    public Rigidbody2D simsRigidbody;
    public Vector2 finalApproachPositionForDebug;
    public Vector2? finalApproachPosition = null; // 使用 nullable 变量
    private static float temperature = 0.4f;
    public static (int,int) keyTimeMorningRanges = (0,16);
    public static int minWorkHours = 7; 
    public (int,int) keyTimeMorning;
    public (int,int) keyTimeNoon;
    public (int,int) keyTimeDusk;
    public HashSet<int> dayOff;
    public int balance = 10000;
    public PlaceManager placeManager;
    public InfoManager infoManager;
    public Infection infection = null;
    public InfectionStatus infectionStatus = InfectionStatus.Suscptible;
    VirusVolumeGridMapManager virusVolumeMapManager;

    public TimeManager timeManager;

    // debug only
    public string infectionRepr = "";
    public string inSiteRepr = "";

    // meta 
    public Color negativeColor = Color.green;
    public Color positiveColor = Color.red;

    public Color normalColor = Color.white;

    public void SimsInit(
        VirusVolumeGridMapManager virusVolumeMapManager,
        InfoManager infoManager,
        PlaceManager placeManager,
        TimeManager timeManager,
        bool infected = false
        )
    {
        this.timeManager = timeManager;
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
        this.speed = UnityEngine.Random.Range(6f,11f);
        this.dayOff = RandomManager.GetRandomDayOff();
        this.simScheduler = new SimScheduler(this);
        this.isTodayOff = GetIsTodayOff(0);
        this.gameObject.AddComponent<SelectableObject>();
        this.simDiary = new SimsDiary(this);
        TestManager.OnTestEventCreated += HandleNewTestEventStarted;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = normalColor;
    }
    /*
        模拟市民的全部功能有下面几点(这个类只能设计的部分)

        (1) 所有（被动和主动）付钱、(被动)接受钱的接口， e.g. 接受工资、支付医疗费用、登记政府补贴
            a. 补贴的发放由所在residential统一在上午Quart分发，Sim自己提交结算
            b. 工资由所在的office统一在每一个Quart发放，Sim自己提交结算
        (2) 在每天的各个时间节点做出相应的决策 e.g. 每天早上决定要去上班，每天决定is today off 等等
        (3) 每天记录积攒的工资和补贴
    */

    public void PayMedicalFee(){
        // call by medical institution
        int medicalCost = this.infoManager.policyManager.GetSubsidisedMedicalFee();
        if(this.balance - medicalCost <= 0){
            // kick off the hospital right now
            Debug.Log($"{this.simsName} has just bankrupt because of medical cost");
            this.simScheduler.UpdateScheduleOnFeeUnaffordable();
            this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
        }else{
            this.balance -= medicalCost;
            Debug.Log($"{this.simsName} has just paid {medicalCost} for medical care");
        }
    }

    // Section: Paycheck 
    private void CommitPayCheckForToday((int,int) timeNow){
        this.balance += accumulatedPaycheckToday;
    }
    public int GetAndClearAccumulatedPaycheck(){
        int temp = this.accumulatedPaycheckToday;
        this.accumulatedPaycheckToday = 0;
        return temp;
    }
    public void ReceivePaycheck(int paycheckIn){
        accumulatedPaycheckToday += paycheckIn;
    }
    public void CheckAndGetSubsidy(){
        if ((this.balance + this.accumulatedPaycheckToday - PriceMenu.QSimLifeExpense) < 0){
            this.home.AttachSubsidyToResidential(PriceMenu.QSimLifeExpense);
            this.accumulatedSubsidyToday += PriceMenu.QSimLifeExpense;
        }
    }
 
    private void CommitSubsidyForToday((int,int) timeNow){
        this.balance += this.accumulatedSubsidyToday;
        this.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                timeManager.GetTime(),
                SimBehaviorDetial.GetSubsidiesEvent(accumulatedSubsidyToday,this.balance)));
        this.accumulatedSubsidyToday = 0;
    }

    public void QDoLifeExpense(){
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

    public void HandleTimeChange((int,int) timeNow){
        
        this.QDoLifeExpense();
        this.QMaintainPCRFreshness();
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

    public void HandleDayChange(int day){
        // infection related
        this.dayRecord = day;
        this.isTodayOff = GetIsTodayOff(day);
        this.simScheduler.FlushDest();
    }

    public void HandlePolicyChange(){
        
    }
    public void HandleInfectionChange(){

    }

    // 早晨KeyTime的更新
    private void HandleMorningKeyTime((int,int) timeNow){
        this.simScheduler.UpdateScheduleOnMorning();
        if (infection != null){
            this.DailyInteractWithInfection();
        }
        else{
            this.UpdateExposureFromTile();
            this.TryToInfect();
        }
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Morning));
    }

    // 晚间KeyTime的更新
    private void HandleDuskKeyTime((int,int) timeNow){
        
        this.simDiary.DailyDuskDiaryItem();
        this.simScheduler.UpdateScheduleOnDusk();
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Dusk));
        CommitPayCheckForToday(timeNow);
    }

    // 中午KeyTime的更新
    private void HandleNoonKeyTime((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
        UpdateSickness();
    }

    // On the road KeyTime的更新
    private void HandleOnTheRoad((int,int) timeNow){
        if (infection != null){
            PolluteThePosition();
        }else{
            UpdateExposureFromTile();
        }
    }

    
    // ============== Infection & Health Related ==============
    
    private void TryToInfect(){
        if(this.infectionStatus == InfectionStatus.Dead){
            Debug.LogWarning("<A dead sim tring to get infected> We currently don't handle dead logic");
            return;
        }
        bool isInfected = InfectionParams.RollTheInfectionDice(maxExposed, this.infectionStatus);
        if(isInfected){
            this.infection = new Infection(InfectionParams.GetInfectionPeriod(), this);
            this.infectionRepr = this.infection.ToString();
            infoManager.infectionInfoManager.InfectionAddition(this,this.infectionStatus);
            this.infectionStatus = InfectionStatus.Infected;
        } 
        this.maxExposed = 0; // 重置昨日接触
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
        this.home.registeredSims.Add(this);
        this.office.registeredSims.Add(this);
        // this.home.OnLockdownStatusUpdate += simScheduler.OnHandlingLockDownStatusChanged;
    }

    public bool IsInDestination(Vector2 currentPosition)
    {
        if (destination == null) return false;

        float tolerance = 0.5f;

        // 扩大判定区域
        Vector2 expandedLL = destination.placeLLAnchor;
        Vector2 expandedUR = destination.placeURAnchor;

        return Utils.IsPointInsideArea(currentPosition, expandedLL, expandedUR,tolerance);
    }


    public void DailyInteractWithInfection(){
        // 离开这个函数的时候是不存在脏状态的，也就是说，不存在：这个人已经痊愈了，但是infection仍然不是null
        this.infectionStatus = infection.Progress();
        this.simScheduler.UpdateScheduleOnInfectionChanged();
        this.infectionRepr = this.infection.ToString();
        PolluteThePosition();
        if(this.infectionStatus == InfectionStatus.Recovered){
            HandleRecovered();
        }else if(this.infectionStatus == InfectionStatus.Dead){
            HandleDead();
        }
    }
    public void HandleDead()
    {
        this.infoManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
        this.infection = null;
        Debug.Log($"{this.simsName} has just dead!");
        this.infoManager.simsDeadManager.HandleSimsDie(this);
    }
    public void HandleRecovered(){
        this.infoManager.infectionInfoManager.InfectionDeletion(this,this.infectionStatus);
            this.infection = null;
            Debug.Log($"{this.simsName} has just recoverd!");
    }

    // PCR Test Related
    public void HandleNewTestEventStarted(TestEvent testEvent){
        this.isUnfinishedPCRQuota = true;
    }
    public void HandleTestQueueCall(TestCenterPlace testPlace){
        this.simScheduler.UpdateScheduleOnTestQueueCall(testPlace);
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
    }
    public void HandleQRTQueueCall(QRTCentrePlace qRTCentrePlace){
        this.simScheduler.UpdateScheduleOnQRTQueueCall(qRTCentrePlace);
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
    }
    public void GetPCRTested(){
        // 这个函数只负责PCRtest result的生成
        // Debug.Log($"{this.simsName}is tested PCR, recording info...");

        // 生成一个test result
        bool isPositive = false;
        if(this.infection != null && infection.virusVolume >= 50){
            isPositive = true;
        }
        // Debug.Log("here2");
        if(isPositive){
            pcrColor = positiveColor;
            recentPcrResult = true;
        }else{
            pcrColor = negativeColor;
            recentPcrResult = false;
        }
        // 如果有TestEvent的话把这个result 提交，如果没有的话就自己把报告揣自己兜里
        if(this.infoManager.testManager.currentTestEvent != null){
            this.infoManager.testManager.currentTestEvent.SubmitTestResult(this, isPositive);
            this.pcrFreshness = 100;
            return;
        }else{
            this.pcrFreshness = 100;
            return;
        }
        
    }
    // PCR feshness 我们会有一个CPR Freshness的机制，freshness的这个数值应该是Sims自己管理，以及减淡的逻辑也是Sims自己管理
    public void QMaintainPCRFreshness(){
        if(pcrFreshness > 1){
            Debug.Assert(this.pcrColor != null,"bug here");
            pcrFreshness -= this.QpcrFreshnessStaleStep;
            float t = Mathf.Clamp01(pcrFreshness / 100f); // 转成 0~1
            Color pcrColor = Color.Lerp(normalColor, this.pcrColor, t);
            spriteRenderer.color = pcrColor;
            if(pcrFreshness <= 1){
            }
        }
    }

    public void HandleTestFinished(){
        this.simScheduler.UpdateScheduleOnTestFinished();
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
    }

    public void HandleQRTFinished(){
        this.simScheduler.UpdateScheduleOnQRTFinished();
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
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
            if (finalApproachPosition == null){
                finalApproachPositionForDebug = new Vector2(-233,-233);
            }else{
                finalApproachPositionForDebug = finalApproachPosition.Value;
            }


            if (Vector2.Distance(finalApproachPosition.Value, currentPosition) < 0.2f){
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

        transform.position = Vector2.Lerp(transform.position, finalApproachPosition.Value, speed * 0.6f * Time.deltaTime);
    }

    public void FinishUpMoving()
    {
        this.inSite = this.destination;
        this.inSite.InsertInsiteSims(this);
        finalApproachPosition = null; // 设为 null，表示无效
        destination = null;
        UpdateInSiteRepr();
    }

    public void FinishUpPCRTest(){
        Debug.Assert(isUnfinishedPCRQuota == true, "bug here");
        this.isUnfinishedPCRQuota = false;
        // this.simScheduler.
        this.simScheduler.UpdateSimScheduleAfterTest();

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
        this.finalApproachPosition = null;
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
    public void UpdateSickness(){
        if(this.infection != null){
            this.sicknessTag = SicknessTagConverter.GetSicknessTag(this.infection.virusVolume);
        }else{
            this.sicknessTag = SicknessTagConverter.GetSicknessTag(RandomManager.Get0toN(0.115f));
        }
        if(this.sicknessTag == SicknessTag.Normal){ return;}
        this.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                timeManager.GetTime(),
                SimBehaviorDetial.SicknessAwarenessEvent(this.sicknessTag)));
        }
    public void UpdateInSiteRepr(){
        stringBuilder.Clear();
        if ( this.inSite != null){
            stringBuilder.Append(this.inSite.placeName);
        }else{
            stringBuilder.Append("Not in Site");
        }
        this.inSiteRepr = stringBuilder.ToString();
    }
    private (int,int,int) GetDetailedTime((int h,int q) timeNow){
        return (this.dayRecord,timeNow.h,timeNow.q);
    }
    public void AskToQuarantine(){
        // this.isUnfinishedQTRQuota = true;
        this.simScheduler.AttemptToQuarantine();
        this.SetOutMoving(simScheduler.GetDestination(KeyTime.Random));
    }
    public bool CheckIsOnQRT(){
        return this.simScheduler.qrtDest != null;
    }

    public void Update()
    {
        Navigate();
        // ScheduleUpdate();
    }
}
