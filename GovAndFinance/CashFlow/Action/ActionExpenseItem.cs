using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class ActionExpenseItem
{
    public Queue<int> daywiseExpenseLog = new Queue<int>();
    public int currentExpense = 0;
    public ExpenseSubTypes expenseSubType;
    public ExpenseTypes expenseType;
    public ActionExpenseItem(ExpenseSubTypes expenseSubType){
        this.expenseSubType = expenseSubType;
        this.expenseType = ExpenseItem.GetExpenseType(expenseSubType);
    }
    public virtual int GetQExpense(){
        return -1;
    }
    
}
