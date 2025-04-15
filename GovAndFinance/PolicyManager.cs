using System.ComponentModel;
using UnityEngine;

public class PolicyManager: MonoBehaviour{

    // 这和类主要用于管理福利
    public float medicalSubsidyRate = 0f;
    public float testSubsidyRate = 0f;

    public int GetSubsidisedMedicalFee(){
        return (int)((float)(PriceMenu.QRawMedicalFee) - (float)(PriceMenu.QRawMedicalFee) * this.medicalSubsidyRate);
    }

}