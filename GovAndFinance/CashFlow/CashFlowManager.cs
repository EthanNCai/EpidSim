using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class CashFlowManager{
    private GameObject cashFlowDebugInfoRoot;
    private StringBuilder stringBuilder = new StringBuilder();
    private TextMesh debugInfoText;

    // CashFlow Cores 
    // 下面的这三个hold了所有Service，Policies， Common等等 服务对象手里的ExpenseItem 或者
    // ContributeItem 的引用
    public List<ContributeItem> contributeItemList = new List<ContributeItem>();
    public List<ExpenseItem> expenseItemList = new List<ExpenseItem>();
    public Stack<ActionExpenseItem> actionExpenseStack = new Stack<ActionExpenseItem>();

    public int cashFlow;    
    
    public CashFlowManager(GameObject cashFlowDebugInfoRoot){
        this.cashFlow = 10000;

        TimeManager.OnDayChanged += HandleDayChanged;
        this.cashFlowDebugInfoRoot = cashFlowDebugInfoRoot;    
        this.debugInfoText = Utils.SpawnTextAtRelativePosition(this.cashFlowDebugInfoRoot, new Vector2Int(1,1), "uninitialized debug text for cash flow manager.");
        UpdateDebugInfo();
    }
    public string GenerateReprString(){
        stringBuilder.Clear();
        return stringBuilder.ToString();
    }
    public void UpdateDebugInfo(){
        string newString = GenerateReprString();
        debugInfoText.text = newString;
    }

    
    public int GetBalance(){
        return cashFlow;
    }
    public void HandleDayChanged(int newDay){
        Debug.Log("Day changed detected in CashFlow Manager");
    }
}
