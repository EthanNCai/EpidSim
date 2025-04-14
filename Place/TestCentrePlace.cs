using UnityEngine;

using System;
using System.Collections.Generic;
// using UnityEngine;



public class TestCenrePlace : Place
{
    public int volumePerTile = 10;
    public int volume;
    // public event Action<bool> OnLockdownStatusUpdate;
    public int populationCapacity;
    // public CFEPolicyMinSub<ResidentialPlace> policyMinSub;
    public List<Sims> residents;
    public void TestCentrePlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        // int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string TestCentreName = PlaceNameGenerator.GetTestCentreName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            TestCentreName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        // this.populationCapacity = population;
        this.volume = volumePerTile * placeShape.x * placeShape.y;
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public void TestPeopleInsite(){
        foreach( Sims inSiteSim in this.inSiteSims){
            if(inSiteSim.isUnfinishedPCRQuota == true){
                // test record logic here
                
                 // test endded
            }
        }
    }

    public bool CheckIsAvailable(){
        if (this.inSiteSims.Count < volume){
            return true;   
        }else{
            return false;
        }
    }

}