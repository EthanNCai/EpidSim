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
    private float speed = 8.0f;
    public int counter = 0;
    public Rigidbody2D simsRigidbody;
    private Vector2? finalApproachPosition = null; // 使用 nullable 变量
    private static float temperature = 0.5f;

    public void SimsInit(ResidentialPlace home, OfficePlace office, bool infected = false)
    {
        this.uid = UniqueIDGenerator.GetUniqueID();
        this.simsName = SimsNameGenerator.GetSimsName();
        this.office = office;
        this.home = home;
        this.destination = home;
        this.simsRigidbody = GetComponent<Rigidbody2D>();
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
            Debug.Log("Standing by....");
            return; // 没有目标地，直接返回
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
        Debug.Log("Long term moving...");
        if (destination != null)
        {
            FlowFieldNode flowFieldNode = destination.flowFieldMapsManager.flowFieldMap.GetNodeByCellPosition(
                new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y))
            );
            NaturallyMove(flowFieldNode.flowFieldDirection);
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

        transform.position = Vector2.Lerp(transform.position, finalApproachPosition.Value, speed * Time.deltaTime);
    }

    public void FinishUpMoving()
    {
        finalApproachPosition = null; // 设为 null，表示无效
        destination = null;
    }

    private void ScheduleUpdate()
    {
        counter++;
        
        if (counter == 5000)
        {
            destination = office;
        }
        if (counter == 10000)
        {
            destination = home;
            counter = 0;
        }
    }

    public void Update()
    {
        Navigate();
        ScheduleUpdate();
    }
}
