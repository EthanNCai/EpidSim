using System;
using UnityEngine;
using UnityEngine.UIElements;

/*
规则：
    destination 只能由 scheduler 设置， 只能由 navigator 取消
    
*/

public class Sims : MonoBehaviour
{
    // BASIC INFOs

    public int uid;
    public string simsName;
    public Place destination = null;
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


    // INFECTION RELATED

    Infection infection = null;
    InfectionStatus infectionStatus = InfectionStatus.Suscptible;
    
    VirusVolumeMapManager virusVolumeMapManager;

    public string infectionRepr = "";

    public void SimsInit(VirusVolumeMapManager virusVolumeMapManager, bool infected = false)
    {
        this.virusVolumeMapManager = virusVolumeMapManager;
        this.uid = UniqueIDGenerator.GetUniqueID();
        this.simsName = SimsNameGenerator.GetSimsName();
        this.destination = home;
        this.simsRigidbody = GetComponent<Rigidbody2D>();
        this.GenerateWorkHours();
        TimeManager.OnQuarterChanged += HandleTimeChange;
        this.toWork = keyTimeMorning.Item1;
        this.toWorkQ = keyTimeMorning.Item2;
        this.speed = UnityEngine.Random.Range(4f,11f);
    }
    private void HandleTimeChange((int,int) timeNow){
        if(timeNow == keyTimeMorning){
            HandleMorningKeyTime(timeNow);
        }else if(timeNow == keyTimeDusk){
            HandleDuskKeyTime(timeNow);
        }else if(timeNow == keyTimeNoon){
            HandleNoonKeyTime(timeNow);
        }
    }
    private void HandleMorningKeyTime((int,int) timeNow){
        if (infection != null){
                // infection related
            this.infectionStatus = infection.Progress();
            this.infectionRepr = this.infection.ToString();
                // pollute the tile
            
        }
        else{
            // infect related
            bool isInfected = InfectionParams.RollTheInfectionDice(100, InfectionStatus.Suscptible);
            if(isInfected){
                this.infectionStatus = InfectionStatus.Infected;
                this.infection = new Infection(InfectionParams.GetInfectionPeriod());
                this.infectionRepr = this.infection.ToString();
            } 
        }
        // schedule related
        this.destination = this.office;
    }
    private void HandleDuskKeyTime((int,int) timeNow){
        if (infection != null){
        // logic of pollute the tile
            
        }else{

        }
        this.destination = this.home;
    }
    private void HandleNoonKeyTime((int,int) timeNow){
        if (infection != null){
        // logic of pollute the tile
            
        }else{

        }
        this.destination = this.home;
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
        finalApproachPosition = null; // 设为 null，表示无效
        destination = null;
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

    public void Update()
    {
        Navigate();
        // ScheduleUpdate();
    }
}
