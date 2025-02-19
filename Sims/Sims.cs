using System;
using UnityEngine;
using UnityEngine.UIElements;

/*
规则：
    destination 只能由 scheduler 设置， 只能由 navigator 取消
*/

public class Sims : MonoBehaviour
{
    
    public int uid;
    public string simsName;
    public Place destination = null;
    public ResidentialPlace home;
    public OfficePlace office;
    private float speed = 8f;
    public int counter = 0;
    public Rigidbody2D simsRigidbody;
    private Vector2? finalApproachPosition = null; // 使用 nullable 变量
    private static float temperature = 0.2f;

    public static (int,int) timeToWotkZone = (5,14);
    public static (int,int) timeToHomeZone = (12,22);
    public static int minWorkHours = 7; 
    [SerializeField]
    public (int,int) timeToWork;
    public (int,int) timeToHome;
    public int toWork = 0;
    public int toWorkQ = 0;



    public void SimsInit(bool infected = false)
    {
        this.uid = UniqueIDGenerator.GetUniqueID();
        this.simsName = SimsNameGenerator.GetSimsName();
        this.destination = home;
        this.simsRigidbody = GetComponent<Rigidbody2D>();
        this.GenerateWorkHours();
        TimeManager.OnQuarterChanged += HandleQuaterChange;
        this.toWork = timeToWork.Item1;
        this.toWorkQ = timeToWork.Item2;

    }
    private void HandleQuaterChange((int,int) timeNow){
        if(timeNow == timeToWork){
            this.destination = this.office;
        }else if(timeNow == timeToHome){
            this.destination = this.home;
        }else{
            return;   
        }
    }
    private void GenerateWorkHours()
    {
        int workStart, workEnd;
        int workStartQuarter, workEndQuarter;
        
        int workZoneMid = (timeToWotkZone.Item1 + timeToWotkZone.Item2) / 2;  // 上班时间中位数

        // 让上班时间尽量集中在 7-9 点，但仍然可能有早晚一点的情况
        workStart = RandomManager.NextGaussianInt(workZoneMid, 1, timeToWotkZone.Item1, timeToWotkZone.Item2);

        // 计算下班时间，保证固定工作时长
        workEnd = workStart + minWorkHours;
        
        // 随机增加或减少一些下班时间（模拟一些员工拖延下班）
        // workEnd += RandomManager.NextInt(-30, 31);  // 允许上下班时间有30分钟的误差（分钟）

        // 防止下班时间超过24点
        // if (workEnd > 23 * 60 + 59) workEnd = 23 * 60 + 59;

        // 随机 quarter（0, 1, 2, 3），对应 0、15、30、45 分钟
        workStartQuarter = RandomManager.NextInt(0, 4);
        workEndQuarter = RandomManager.NextInt(0, 4);

        this.timeToWork = (workStart, workStartQuarter);
        this.timeToHome = (workEnd, workEndQuarter);
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

    // private void ScheduleUpdate()
    // {
    //     counter++;
        
    //     if (counter == 7000)
    //     {
    //         destination = office;
    //     }
    //     if (counter == 14000)
    //     {
    //         destination = home;
    //         counter = 0;
    //     }
    // }

    public void Update()
    {
        Navigate();
        // ScheduleUpdate();
    }
}
