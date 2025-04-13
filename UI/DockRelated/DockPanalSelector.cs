using System.ComponentModel;
using UnityEngine;

public class DockPanalSelector: MonoBehaviour{

    public FinanceInfoUIManager financeInfoUIManager;
    public BuildPanalUIManager buildingUIBarManager;
    public PolicyPanalUIManager policyUIBarManager;
    public OBUIBoardManager obUIBoardManager;
    // other ui panals

    public void HandleClickFinanceButton(){
        financeInfoUIManager.InitUI();
    }
    public void HandleClickBuildButton(){
        buildingUIBarManager.InitUI();
    }
    public void HandelClickPolicyButton(){
        policyUIBarManager.InitUI();
    }
    public void HandleOBPolicyButton(){

    }
}