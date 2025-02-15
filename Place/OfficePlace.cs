using UnityEngine;

public class OfficePlace : Place
{
    public int populationCapacity;
    public void OfficePlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj)
    {
        string officeName = PlaceNameGenerator.GetResidentialName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            officeName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj);
        this.populationCapacity = population;
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
}