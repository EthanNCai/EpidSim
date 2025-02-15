using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/*
规则：
    destination 只能由 scheduler 设置， 只能由 navigator 取消

*/

public class Sims : MonoBehaviour
{
    public string simsName;
    public Place destination;
    public ResidentialPlace home;
    public OfficePlace office;
    public float speed = 10.0f;
    public int counter = 0;

    public static float temperature = 0.6f;
    // infection properties

    public void SimsInit(
        ResidentialPlace home, 
        OfficePlace office,
        bool infected = false
        // float speed = 1.0f
        ){
        this.simsName = SimsNameGenerator.GetSimsName();
        this.office = office;
        this.home = home;
        this.destination = home;
    }

    public bool IsInDestination(Vector2 currentPosition)
    {
        return Utils.IsPointInsideArea( currentPosition,destination.placeLLAnchor,destination.placeURAnchor);
    }

    public void Navigate(){

        Vector3 currentPositionVctor3 = this.transform.position;
        Vector2 currentPosition = new Vector2(currentPositionVctor3.x, currentPositionVctor3.y);
        Vector2Int currentCellPosition = new Vector2Int(Mathf.FloorToInt(currentPosition.x), Mathf.FloorToInt(currentPosition.y));

        if(destination == null){
            // stand by
            return;
        }
        if (IsInDestination(currentCellPosition)){
            // **freshly** reached destination
            Debug.Log("Reached destination" + currentCellPosition.ToString() + destination.placeLLAnchor + destination.placeURAnchor);
            destination = null;
            return;
        }
            
        Debug.Log("on the road" + currentCellPosition.ToString());
        // navigate
        FlowFieldNode flowFieldNode = destination.flowFieldMapsManager.flowFieldMap.GetNodeByCellPosition(currentCellPosition);
        Vector2Int flowFieldDirection = flowFieldNode.flowFieldDirection;
        this.MoveNaturally(speed, temperature, flowFieldDirection);
        return;
    }

    public void MoveNaturally(float speed, float temperature, Vector2Int direction){
        Vector2 natureDirection = Utils.GetRandomizedDirection(direction, temperature);
        transform.localPosition += new Vector3(natureDirection.x, natureDirection.y, 0) * speed * Time.deltaTime;
    }

    private void ScheduleUpdate(){
        counter ++;
        if (counter >= 10000){
            destination = office;
        }
        if (counter >= 20000){
            destination = home;
            counter = 0;
        }
    }

    public void Update()
    {
        // navigate around
        this.Navigate();
        this.ScheduleUpdate();
        // update its infection
    }

}