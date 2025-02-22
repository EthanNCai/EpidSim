using UnityEngine;

public class MedicalPlace : Place
{
    public int populationCapacity;
    public void MedicalPlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj)
    {
        string medicalName = PlaceNameGenerator.GetMedicalName();
        // base.PlaceInit(
        //     placeShape, 
        //     basePosition,
        //     medicalName,
        //     mapManager,
        //     flowFieldRootObject,
        //     geoMapManagerObj);
        // this.populationCapacity = population;
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
}