using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class MedicalPlace : Place, IBuilingMaintaingExpense
{   
    public CFEServiceBuildingMaintaining<MedicalPlace> serviceBuildingMaintainingCFE;
    public void CommercialInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string medicalPlaceName = GetMedicalName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            medicalPlaceName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.serviceBuildingMaintainingCFE = cfeManager.CreateAndRegisterServiceBuildingMaintainingCFE<MedicalPlace>(this);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }

    private static List<string> medicalPrefixes = new List<string>{"厦门市医学院","奥梅","厦门市","孙厝"};
    private static List<string> medicalSuffixes = new List<string>{"附属第二医院","诊所","中医院","第一人民医院"};
    public static string GetMedicalName(){
        return medicalPrefixes[Random.Range(0,medicalPrefixes.Count)] + medicalSuffixes[Random.Range(0,medicalSuffixes.Count)];
    }

    public int calculateQExpense()
    {
        return PriceMenu.QMedicalPlaceMaintaingExpense;
    }
}