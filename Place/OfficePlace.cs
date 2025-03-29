using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class OfficePlace : Place, IContributablePlace, ITaxPayer
{   

    public CFECommonTax<OfficePlace> commonTaxCFE;
    int accumulatedTaxLastD = 0;
    public void OfficePlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
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
            infoDebuggerManager,
            cfeManager);
        this.commonTaxCFE = cfeManager.CreateCommonTaxCFE<OfficePlace>(this);
        TimeManager.OnQuarterChanged += DistributePaycheck;
        TimeManager.OnDayChanged += LogDiaryDaily;
    }
    public void LogDiaryDaily(int timeNow){
        // Debug.Log("contributed_tx_diary");
        this.placeDiary.AppendDiaryItem(
            new PlaceDiaryItem(
                infoManager.timeManager.GetTime(),
                PlaceBehaviorsDetails.ContributeTaxEvent(this)));
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public int CalculateQContribution()
    {
        int workingSims = base.inSiteSims.Count;
        if(workingSims == 0){
          return 0;  
        } 
        else{
            int QTax = workingSims * PriceMenu.QOfficeTaxUnit;
            accumulatedTaxLastD += QTax;
            return QTax;
        }
    }
    public void DistributePaycheck((int , int ) _){
        foreach( Sims sim in inSiteSims){
            sim.ReceivePaycheck(PriceMenu.QSimOfficeIncome);
        } 
    }

    public int GetAndResetTaxContributedLastD(){
        int temp = this.accumulatedTaxLastD;
        this.accumulatedTaxLastD = 0;
        return temp;
    }
}