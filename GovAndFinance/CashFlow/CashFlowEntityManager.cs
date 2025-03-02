using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using UnityEngine;


public class CashFlowEntityManager : MonoBehaviour{

    public List<CFECommon> commonCFEs = new List<CFECommon>();
    public List<CFEService> serviceCFEs = new List<CFEService>();
    // public List<Service> serviceCFEs;
    public StringBuilder stringBuilder = new StringBuilder();
    // Common
    public CFECommonTax<TPlace> CreateCommonTaxCFE<TPlace>(TPlace place) where TPlace : Place, ICommonTaxContributor
    {
        CFECommonTax<TPlace> commonTax = new CFECommonTax<TPlace>(place);
        commonCFEs.Add(commonTax);  // 假设 commonCFEs 支持泛型
        return commonTax;
    }
    public void RemoveCommonTaxCFE<TPlace>(CFECommonTax<TPlace> target) where TPlace : Place, ICommonTaxContributor
    {
        commonCFEs.Remove(target);
    }

    public void Start(){
        TimeManager.AfterQuarterChanged += QUpdateCFEs;
    }
    public void QUpdateCFEs((int,int) timeNow)
    {
        for( int i=0; i<commonCFEs.Count;i++){
            commonCFEs[i].QUpdateContributeItem();
        }
        for( int i=0; i<serviceCFEs.Count;i++){
            // serviceCFEs[i].QUpdateContribution();
        }
        Debug.Log(GetAllCFEsRepr());
    }

    public void CreateServiceBuildingMaintainingCFE(){
        
    }
    public string GetAllCFEsRepr(){
        stringBuilder.Clear();
        for( int i=0; i<commonCFEs.Count;i++){
            stringBuilder.Append("====\n");
            string CFEName = commonCFEs[i].CFEName;
            (int QuartContribute,int DayContribute) =  commonCFEs[i].contributeItem.QGetContribution();
            stringBuilder.Append(CFEName);
            stringBuilder.Append(" - Quert: ");
            stringBuilder.Append(QuartContribute);
            stringBuilder.Append(" - Day: ");
            stringBuilder.Append(DayContribute);
        }
        return stringBuilder.ToString();
    }   
}

static class CFEPriceMenu{

    


}