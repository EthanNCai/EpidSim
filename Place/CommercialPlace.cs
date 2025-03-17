using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CommercialPlace : Place, IContributablePlace
{   
    public CFECommonTax<CommercialPlace> commonTaxCFE;
    public void CommercialInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string commercialName = GetRandCommercialName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            commercialName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.commonTaxCFE = cfeManager.CreateAndRegisterCommonTaxCFE<CommercialPlace>(this);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public int CalculateQContribution(){
        int inSiteSims = base.inSiteSims.Count;
        if(inSiteSims == 0){
          return 0;  
        } 
        else{
            return inSiteSims * PriceMenu.QCommercialTaxUnit;
        }
    }

    private static List<string> publicPrefixes = new List<string>{"九龙","敬贤","深圳湾","万象城","万达","中央"};
    private static List<string> publicSuffixes = new List<string>{"公共事务中心","运动场","公园","广场","商场"};
    public static string GetRandCommercialName(){
        return publicPrefixes[Random.Range(0,publicPrefixes.Count)] + publicSuffixes[Random.Range(0,publicSuffixes.Count)];
    } 
}