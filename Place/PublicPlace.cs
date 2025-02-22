using UnityEngine;

public class PublicPlace : Place
{
    public int populationCapacity;
    public void PublicPlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj)
    {
        // string publicName = PlaceNameGenerator.GetPublicName();
        // base.PlaceInit(
        //     placeShape, 
        //     basePosition,
        //     publicName,
        //     mapManager,
        //     flowFieldRootObject,
        //     geoMapManagerObj);
        // this.populationCapacity = population;
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
}