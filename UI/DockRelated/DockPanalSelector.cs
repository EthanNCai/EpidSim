using System.ComponentModel;
using UnityEngine;

public class DockPanalSelector: MonoBehaviour{

    public FinanceInfoUIManager financeInfoUIManager;
    public BuildPanalUIManager buildingUIBarManager;
    public PolicyUIBarManager policyUIBarManager;
    public OBUIBoardManager obUIBoardManager;
    // other ui panals

    public void HandleClickFinanceButton(){
        financeInfoUIManager.InitUI();
    }
    public void HandleClickBuildButton(){
        buildingUIBarManager.InitUI();
        
    }
    public void HandelClickPolicyButton(){

    }
    public void HandleOBPolicyButton(){

    }
}