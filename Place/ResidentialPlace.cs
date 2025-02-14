using UnityEngine;

public class ResidentialPlace : Place
{
    public int populationCapacity;
    public void ResPlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj)
    {
        string residentialName = PlaceNameGenerator.GetResidentialName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            residentialName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj);
        this.populationCapacity = population;
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
}