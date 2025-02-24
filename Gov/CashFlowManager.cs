using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

static public class CashFlowManager {

    public static int cashFlow = 10000;
    
    public static Dictionary<ContributeTypes, int> contributeDict = new Dictionary<ContributeTypes, int>();
    public static Dictionary<ExpenseTypes, int> expenseDict = new Dictionary<ExpenseTypes, int>();

    static public void CashFlowManagerInit(){
        cashFlow = 10000;
        contributeDict[ContributeTypes.IncomeTax] = 0;
        contributeDict[ContributeTypes.TestIncome] = 0;
        expenseDict[ExpenseTypes.MedicalSubsidy] = 0;
        expenseDict[ExpenseTypes.TestExpense] = 0;
        TimeManager.OnDayChanged += HandleDayChanged;
    }

    static public bool ContributeToCashFlow(int amount, ContributeTypes type){
        Debug.Assert(amount > 0, "Amount must be greater than zero");
        if (amount < 0)
            return false;
        else{
            contributeDict[type] += amount;
            cashFlow += amount;
            return true;
        }
    }
    static public bool ExpenseToCashFlow(int amount, ExpenseTypes type){
        Debug.Assert(amount > 0, "Amount must be greater than zero");
        if (amount < 0)
            return false;
        else{
            expenseDict[type] += amount;
            cashFlow -= amount;
            return true;
        }
    }   
    static public int GetBalance(){
        return cashFlow;
    }
    static public int GetContributeSum(){
        return expenseDict.Values.Sum();
    }
    static public int GetExpenseSum(){
        return expenseDict.Values.Sum();
    }
    static public void HandleDayChanged(int newDay){
        Debug.Log("Day changed detected in CashFlow Manager");
    }
}


static public class AmountMenu{
    public static int incomeTaxPerHour = 10;
    public static int singleSearch = 10000;
}


