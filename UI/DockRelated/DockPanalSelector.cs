using System.ComponentModel;
using UnityEngine;

public class DockPanalSelector: MonoBehaviour{

    public FinanceInfoUIManager financeInfoUIManager;
    public BuildingUIBarManager buildingUIBarManager;
    public PolicyUIBarManager policyUIBarManager;
    public OBUIBoardManager obUIBoardManager;
    // other ui panals

    public void HandleClickFinanceButton(){
        financeInfoUIManager.InitUI();
    }
    public void HandleClickBuildButton(){
        
    }
    public void HandelClickPolicyButton(){

    }
    public void HandleOBPolicyButton(){

    }
}