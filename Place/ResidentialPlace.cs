using System;
using System.Collections.Generic;
using UnityEngine;



public class ResidentialPlace : Place, IExpensablePlace
{
    public event Action<bool> OnLockdownStatusUpdate;
    public int populationCapacity;
    public CFEPolicyMinSub<ResidentialPlace> policyMinSub;
    public List<Sims> residents;
    public void ResPlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string residentialName = PlaceNameGenerator.GetResidentialName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            residentialName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.populationCapacity = population;
        // Debug.Log(cfeManager == null);
        this.policyMinSub = cfeManager.CreatePolicyMinSubCFE<ResidentialPlace>(this);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public void AttachSubsidyToResidential(int subsidyAmount){
        this.QAccumulatedSubsidies += subsidyAmount;
    }

    public int CalculateQExpense(){
        int temp =  this.QAccumulatedSubsidies;
        this.QAccumulatedSubsidies = 0;
        return temp;
    }
    public void StartLockdown(){
        Debug.Assert(isLockedDown == false);
        this.isLockedDown = true;
        OnLockdownStatusUpdate?.Invoke(true);
    }
    public void StopLockdown(){   
        Debug.Assert(isLockedDown == true);
        this.isLockedDown = false;
        OnLockdownStatusUpdate?.Invoke(false);
    }
}