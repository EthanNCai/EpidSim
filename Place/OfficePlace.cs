using System.ComponentModel;
using UnityEngine;

public class OfficePlace : Place
{
    public void OfficePlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridDebugManager gridDebuggerManager)
    {
        string officeName = PlaceNameGenerator.GetOfficeName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            officeName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    
}