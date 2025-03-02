using System.ComponentModel;
using UnityEngine;


public class CFEService{
    
    public string name;
    public int contributeItemUid;
    public ExpenseItem expenseItem;
    public PlaceManager placeManager;

    public CFEService(){

    }

    public virtual void QUpdateContributeItem(){
        
    }

    private static string CFECommonNameGenerator(){
        return "";
    }
}