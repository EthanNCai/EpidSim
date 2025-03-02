using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

// 提示： 最小计算时间粒度是Quarter，所以这里用QuarterCost来表示

public class ExpenseItem
{
    public Queue<int> daywiseExpenseLog = new Queue<int>();
    public int currentExpense = 0;
    public ExpenseSubTypes expenseSubType;
    public ExpenseTypes expenseType;

    public ExpenseItem(ExpenseSubTypes expenseSubType){
        this.expenseSubType = expenseSubType;
        this.expenseType = ExpenseItem.GetExpenseType(expenseSubType);
    }
    public virtual int GetQExpense(){
        return -1;
    }
    public static ExpenseTypes GetExpenseType(ExpenseSubTypes type){
        switch (type)
        {
            case ExpenseSubTypes.BuildingMaintainig:
                return ExpenseTypes.Service;
            case ExpenseSubTypes.Research:
                return ExpenseTypes.Service;
            case ExpenseSubTypes.MedicalSubsidies:
                return ExpenseTypes.Policy;
            case ExpenseSubTypes.CoverSubsidies:
                return ExpenseTypes.Policy;
            case ExpenseSubTypes.TestExpense:
                return ExpenseTypes.Action;
            default:
               throw new Exception("Invalid expense sub type");
        }
    }
}

public enum CycleLen{
    Quart,
    Week,
    Hour,
    Day
}
public static class CycleInfo
{
    public static int GetCycleLen(CycleLen cycleLen)
    {
        switch (cycleLen){
            case CycleLen.Quart:
                return 1;
            case CycleLen.Hour:
                return 4;
            case CycleLen.Day:
                return 4*24;
            case CycleLen.Week:
                return 4*24*7;
            default:
               Debug.Log("Error: Invalid cycle length");
               return -1;
        }
    }

}

public enum ExpenseTypes {
    Service,
    Policy,
    Action
}
public enum ExpenseSubTypes {
    BuildingMaintainig,
    Research,
    MedicalSubsidies,
    CoverSubsidies,
    TestExpense
}