using UnityEngine;

public class ResidentialPlace : Place, IContributablePlace
{
    public int populationCapacity;
    public CFEPolicyMinSub<ResidentialPlace> policyMinSub;
    public void ResPlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string residentialName = PlaceNameGenerator.GetResidentialName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            residentialName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.populationCapacity = population;
        // Debug.Log(cfeManager == null);
        this.policyMinSub = cfeManager.CreateAndRegisterPolicyMinSubCFE<ResidentialPlace>(this);
    }
    public void SayHi(){
        Debug.Log(base.ToString());
    }
    public void AttachSubsidyToResidential(int subsidyAmount){
        this.QAccumulatedSubsidies += subsidyAmount;
    }

    public int CalculateQContribution(){
        // get and clear acc subsidies
        int temp =  this.QAccumulatedSubsidies;
        this.QAccumulatedSubsidies = 0;
        return temp;
    }
}