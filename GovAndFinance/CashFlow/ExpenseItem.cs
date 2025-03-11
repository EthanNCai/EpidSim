using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 提示：最小计算时间粒度是Quarter，所以这里用QuarterCost来表示
public class ExpenseItem
{
    private const int QuartsPerDay = 4 * 24;
    
    public Queue<int> DailyExpenseLog { get; private set; } = new Queue<int>();
    public int CurrentExpense { get; private set; } = 0;
    public ExpenseSubTypes ExpenseSubType { get; private set; }
    public ExpenseTypes ExpenseType { get; private set; }

    public ExpenseItem(ExpenseSubTypes expenseSubType)
    {
        this.ExpenseSubType = expenseSubType;
        this.ExpenseType = ExpenseItem.GetExpenseType(expenseSubType);
    }

    public (int QuartExpense, int DayExpense) QGetExpense()
    {
        return (this.CurrentExpense, this.DailyExpenseLog.Sum());
    }

    public void QUpdateExpense(int newQExpense)
    {
        this.CurrentExpense = newQExpense;
        if (DailyExpenseLog.Count >= QuartsPerDay)
        {
            DailyExpenseLog.Dequeue();
        }
        DailyExpenseLog.Enqueue(newQExpense);
    }

    public static ExpenseTypes GetExpenseType(ExpenseSubTypes type)
    {
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

public enum ExpenseTypes
{
    Service,
    Policy,
    Action
}

public enum ExpenseSubTypes
{
    BuildingMaintainig,
    Research,
    MedicalSubsidies,
    CoverSubsidies,
    TestExpense
}
