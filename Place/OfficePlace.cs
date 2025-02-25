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
        GridDebugManager gridDebuggerManager,
        InfoDebuggerManager infoDebuggerManager)
    {
        string officeName = PlaceNameGenerator.GetOfficeName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            officeName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager);
        TimeManager.OnQuarterChanged += ((int, int) timeNow) => {
            ContributeIncomeTaxQuarterly();
        };
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public void ContributeIncomeTaxQuarterly(){
        int workingSims = base.inSiteSims.Count;
        if(workingSims == 0) return;
        int taxes = workingSims * AmountMenu.incomeTaxPerQuarter;
        base.infoDebuggerManager.cashFlowManager.ContributeToCashFlow(taxes, ContributeTypes.IncomeTax);
    }
}