using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public enum ContributeTypes{
    IncomeTax,
    MedicalFees,
    TestIncome,
}
public enum ExpenseTypes{
    MedicalSubsidy,
    TestExpense,
}

public class CashFlowInfoManager{
    private GameObject cashFlowDebugInfoRoot;
    private StringBuilder stringBuilder = new StringBuilder();
    private TextMesh debugInfoText;
    public int cashFlow;
    public Dictionary<ContributeTypes, int> contributeDict = new Dictionary<ContributeTypes, int>();
    public Dictionary<ExpenseTypes, int> expenseDict = new Dictionary<ExpenseTypes, int>();

    public CashFlowInfoManager(GameObject cashFlowDebugInfoRoot){
        this.cashFlow = 10000;
        contributeDict[ContributeTypes.IncomeTax] = 0;
        contributeDict[ContributeTypes.TestIncome] = 0;
        expenseDict[ExpenseTypes.MedicalSubsidy] = 0;
        expenseDict[ExpenseTypes.TestExpense] = 0;
        TimeManager.OnDayChanged += HandleDayChanged;
        this.cashFlowDebugInfoRoot = cashFlowDebugInfoRoot;    
        this.debugInfoText = Utils.SpawnTextAtRelativePosition(this.cashFlowDebugInfoRoot, new Vector2Int(1,1), "uninitialized debug text for cash flow manager.");
        UpdateDebugInfo();
    }
    public string GenerateReprString(){
        stringBuilder.Clear();
        stringBuilder.Append("Cash Flow: ").Append(cashFlow).Append("\n");
        stringBuilder.Append($"Expense Sum Today: {GetExpenseSum()}\n");
        stringBuilder.Append($"Contribute Sum Today: {GetContributeSum()}\n");
        return stringBuilder.ToString();
    }
    public void UpdateDebugInfo(){
        string newString = GenerateReprString();
        debugInfoText.text = newString;
    }
    public bool ContributeToCashFlow(int amount, ContributeTypes type){
        Debug.Assert(amount > 0, "Amount must be greater than zero");
        if (amount < 0)
            return false;
        else{
            contributeDict[type] += amount;
            cashFlow += amount;
            UpdateDebugInfo();
            return true;
        }
    }
    public bool ExpenseFromCashFlow(int amount, ExpenseTypes type){
        Debug.Assert(amount > 0, "Amount must be greater than zero");
        if (amount < 0)
            return false;
        else{
            expenseDict[type] += amount;
            cashFlow -= amount;
            UpdateDebugInfo();
            return true;
        }
    }   
    
    public int GetBalance(){
        return cashFlow;
    }
    public int GetContributeSum(){
        return contributeDict.Values.Sum();
    }
    public int GetExpenseSum(){
        return expenseDict.Values.Sum();
    }
    public void HandleDayChanged(int newDay){
        Debug.Log("Day changed detected in CashFlow Manager");
    }
}

static public class AmountMenu{
    public static int incomeTaxPerQuarter = 4;
    public static int singleSearch = 10000;
}
