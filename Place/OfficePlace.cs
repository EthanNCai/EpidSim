using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class OfficePlace : Place, IContributablePlace
{   
    
    public CFECommonTax<OfficePlace> commonTaxCFE;
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
            return workingSims * PriceMenu.QOfficeTaxUnit;
        }
    }
}