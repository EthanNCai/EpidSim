using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class MedicalPlace : Place, IContributablePlace, IExpensablePlace
{   
    public int volumePerTile = 10;
    public int volume;
    public CFEServiceBuildingMaintaining<MedicalPlace> serviceBuildingMaintainingCFE;
    public CFECommonFees<MedicalPlace> commonFeesCFE;
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
        this.volume = volumePerTile * placeShape.x * placeShape.y;
        this.serviceBuildingMaintainingCFE = cfeManager.CreateServiceBuildingMaintainingCFE<MedicalPlace>(this);
        this.commonFeesCFE = cfeManager.CreateCommonFeesCFE<MedicalPlace>(this);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }

    private static List<string> medicalPrefixes = new List<string> { "Riverside", "Starlight", "Harmony", "Evergreen", "Summit", "Clearview", "Oakwood" };
    private static List<string> medicalSuffixes = new List<string> { "General Hospital", "Clinic", "Medical Center", "Health Institute", "Community Hospital" };
    public static string GetMedicalName(){
        return medicalPrefixes[Random.Range(0,medicalPrefixes.Count)] + medicalSuffixes[Random.Range(0,medicalSuffixes.Count)];
    }
    public bool CheckIsAvailable(){
        if (this.inSiteSims.Count < volume){
            return true;   
        }else{
            return false;
        }
    }

    public int CalculateQExpense(){
        return PriceMenu.QMedicalPlaceMaintaingExpense;
    }
    public int CalculateQContribution(){
        return inSiteSims.Count * infoManager.policyManager.GetSubsidisedMedicalFee();
    }
}