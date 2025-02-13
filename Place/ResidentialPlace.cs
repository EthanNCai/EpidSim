using UnityEngine;

public class ResidentialPlace : Place
{
    public int populationCapacity;
    public void CustomInit(Vector2Int placeShape, Vector2Int basePosition, int population)
    {
        base.CustomInit(placeShape, basePosition);
        this.populationCapacity = population;
    }
    public void SayHi(){
        Debug.Log("Hello from ResidentialPlace");
    }
}