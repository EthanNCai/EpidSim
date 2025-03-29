using System.ComponentModel;
using UnityEngine;


public class CFEPolicy{
    
    public string CFEName;
    public ExpenseItem expenseItem;
    public CFEPolicy(string CFEName, ExpenseItem expenseItem){
        // 之所以这里的 contributeItem 需要是一个传入的参数，这是因为CFECommon
        // 并不知道自己的ContributeItem此时应该是什么Type的，因为Common对应着很多种SubType
        // name 也是依赖于分支，Common对应着很多种类，例如Tax，这里的name就是建筑名字，别的就可能是其他可能，可能是Sims名字等等
        this.CFEName = CFEName;
        this.expenseItem = expenseItem;
    }
    // 由你来Update ContibuteItem
    public virtual int QUpdateExpenseItem(){return -99999999;}
}

