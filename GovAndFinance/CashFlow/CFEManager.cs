using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public class CFEManager : MonoBehaviour{

    // For debug
    private GameObject cashFlowDebugInfoRoot;
    private TextMesh debugInfoText;
    public StringBuilder stringBuilder = new StringBuilder();
    // public StringBuilder stringBuilderForDebug = new StringBuilder();
    // Cash Flow 
    public int cashFlow; 

    // CFEs
    public List<CFEPolicy> policyCFEs = new List<CFEPolicy>();
    public List<CFECommon> commonCFEs = new List<CFECommon>();
    public List<CFEService> serviceCFEs = new List<CFEService>();

    public void Start(){
        this.cashFlow = 10000;
        TimeManager.AfterQuarterChanged += HandelAfterQuarterChanged;
    }
    public void SetupDebug(GameObject debugRoot){
        this.cashFlowDebugInfoRoot = debugRoot;
        this.debugInfoText = Utils.SpawnTextAtRelativePosition(this.cashFlowDebugInfoRoot, new Vector2Int(1,10), "uninitialized debug text for cash flow manager.");
    }

    private void HandelAfterQuarterChanged ((int,int) timeNow){
        QUpdateCFEs(timeNow);
        QUpdateDebugPanal(timeNow);
    }
    private void QUpdateDebugPanal((int,int) timeNow){
        this.debugInfoText.text = GenerateCFEsRepr();
    }

    public CFECommonTax<TPlace> CreateCommonTaxCFE<TPlace>(TPlace place) where TPlace : Place, IContributablePlace
    {
        // 这个函数的核心作用是Create并且 **注册** 到Manager的List里面
        // 并且这个注册，使用的是ContributeType来注册
        CFECommonTax<TPlace> commonTax = new CFECommonTax<TPlace>(place);
        commonCFEs.Add(commonTax);  
        return commonTax;
    }
    public CFECommonFees<TPlace> CreateCommonFeesCFE<TPlace>(TPlace place) where TPlace : Place, IContributablePlace
    {
        // 这个函数的核心作用是Create并且 **注册** 到Manager的List里面
        // 并且这个注册，使用的是ContributeType来注册
        CFECommonFees<TPlace> commonFees = new CFECommonFees<TPlace>(place);
        commonCFEs.Add(commonFees);  
        return commonFees;
    }
    public CFEServiceBuildingMaintaining<TPlace> CreateServiceBuildingMaintainingCFE<TPlace>(TPlace place) where TPlace : Place, IExpensablePlace
    {
        CFEServiceBuildingMaintaining<TPlace> serviceBuildingMaintaining = new CFEServiceBuildingMaintaining<TPlace>(place);
        serviceCFEs.Add(serviceBuildingMaintaining);  
        return serviceBuildingMaintaining;
    }

    public CFEPolicyMinSub<TPlace> CreatePolicyMinSubCFE<TPlace>(TPlace place) where TPlace : Place, IExpensablePlace{
        CFEPolicyMinSub<TPlace> policyMinSubCFE = new CFEPolicyMinSub<TPlace>(place);
        policyCFEs.Add(policyMinSubCFE);  
        return policyMinSubCFE;
    }


    public void RemoveCommonTaxCFE<TPlace>(CFECommonTax<TPlace> target) where TPlace : Place, IContributablePlace
    {
        commonCFEs.Remove(target);
    }

    public void QUpdateCFEs((int,int) _)
    {
        int accumulatedExpense = 0;
        int accumulatedContribute = 0;

        for( int i=0; i<commonCFEs.Count;i++){
            accumulatedContribute += commonCFEs[i].QUpdateContributeItem();
        }
        for( int i=0; i<serviceCFEs.Count;i++){
            accumulatedExpense += serviceCFEs[i].QUpdateExpenseItem();
        }
        for( int i=0; i<policyCFEs.Count;i++){
            accumulatedExpense += policyCFEs[i].QUpdateExpenseItem();
        }

        // commit change to cash flow
        this.cashFlow += accumulatedContribute;
        this.cashFlow -= accumulatedExpense;
    }
    

    public string GenerateCFEsRepr(){
        stringBuilder.Clear();
        // CashFlow
        stringBuilder.Append("\n==== CashFlow ====\n");
        stringBuilder.Append(this.cashFlow);

        // COMMON
        stringBuilder.Append("\n==== Commons ====\n");
        for( int i=0; i<commonCFEs.Count;i++){
            string CFEName = commonCFEs[i].CFEName;
            (int QuartContribute,int DayContribute) =  commonCFEs[i].contributeItem.QGetContribution();
            stringBuilder.Append(CFEName);
            stringBuilder.Append(" - Quert: ");
            stringBuilder.Append(QuartContribute);
            stringBuilder.Append(" - Day: ");
            stringBuilder.Append(DayContribute);
            stringBuilder.Append("\n");
        }

        // SERVICE
        stringBuilder.Append("\n==== Service ====\n");
        for( int i=0; i<serviceCFEs.Count;i++){
            string CFEName = serviceCFEs[i].CFEName;
            (int QuartExpense,int DayExpense) =  serviceCFEs[i].expenseItem.QGetExpense();
            stringBuilder.Append(CFEName);
            stringBuilder.Append(" - Quert: ");
            stringBuilder.Append(QuartExpense);
            stringBuilder.Append(" - Day: ");
            stringBuilder.Append(DayExpense);
            stringBuilder.Append("\n");
        }

        // POLICY
        stringBuilder.AppendFormat("\n==== Policy ====\n");
        for( int i=0; i<policyCFEs.Count;i++){
            string CFEName = policyCFEs[i].CFEName;
            (int QuartExpense,int DayExpense) =  policyCFEs[i].expenseItem.QGetExpense();
            stringBuilder.Append(CFEName);
            stringBuilder.Append(" - Quert: ");
            stringBuilder.Append(QuartExpense);
            stringBuilder.Append(" - Day: ");
            stringBuilder.Append(DayExpense);
            stringBuilder.Append("\n");
        }
        return stringBuilder.ToString();
    }   

    public List<string> GetExpenseStringList()
    {
        List<string> expenseStrings = new List<string>();

        foreach (var service in serviceCFEs)
        {
            string name = service.CFEName;
            var (quart, day) = service.expenseItem.QGetExpense();
            expenseStrings.Add($"{name} -{quart}/quart -{day}/day");
        }

        foreach (var policy in policyCFEs)
        {
            string name = policy.CFEName;
            var (quart, day) = policy.expenseItem.QGetExpense();
            expenseStrings.Add($"{name} -{quart}/quart -{day}/day");
        }

        return expenseStrings;
    }

    public List<string> GetContributeStringList()
    {
        List<string> contributeStrings = new List<string>();

        foreach (var common in commonCFEs)
        {
            string name = common.CFEName;
            var (quart, day) = common.contributeItem.QGetContribution();
            contributeStrings.Add($"{name} +{quart}/quart +{day}/day");
        }

        return contributeStrings;
    }
}