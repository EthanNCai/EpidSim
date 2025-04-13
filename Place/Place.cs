using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public interface ITaxPayer{
    public int GetAndResetTaxContributedLastD();
}

public class Place : MonoBehaviour
{

    public PlaceDiary placeDiary;
    public int uid;
    public string placeName = "default";
    public string placeFullName;
    public Vector2Int placeShape;
    public Vector2Int placeLLAnchor;
    public Vector2Int placeURAnchor;
    public Vector2Int basePosition;
    public SingleFlowFieldMapManager flowFieldMapsManager;
    public List<Sims> inSiteSims = new List<Sims>();
    public List<Sims> registeredSims = new List<Sims>();
    public InfoManager infoManager;
    public CFEManager cfeManager;
    public bool isLockedDown; 

    // public bool isIsolated;
    public int QAccumulatedSubsidies = 0;

    public void PlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        string placeName,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridInfoManager,
        InfoManager infoManager,
        CFEManager cfeManager
        )
    {   
        this.cfeManager = cfeManager;
        this.infoManager = infoManager;
        this.uid = UniqueIDGenerator.GetUniqueID();
        this.placeName = placeName;
        this.placeFullName = placeName + this.uid.ToString();
        this.placeShape = placeShape;
        this.placeLLAnchor = basePosition;
        this.placeURAnchor = basePosition + placeShape; // 确保边界正确
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = placeShape;
        }
        transform.position = new Vector3(basePosition.x, basePosition.y, 0);
        // adjust geomap before generating FlowFieldMap
        GeoMapsManager geoMapManager = geoMapManagerObj.GetComponent<GeoMapsManager>();
        this.SetUpGeoMapBlocked(geoMapManager);
        geoMapManager.InvokeMapChangedEvent();
        this.flowFieldMapsManager = gameObject.AddComponent<SingleFlowFieldMapManager>();
        this.flowFieldMapsManager.FlowFieldMapsManagerInit(
            basePosition, 
            "placeholder",
            mapManager,
            gridInfoManager.GetListedRoot(placeName + uid.ToString()),
            geoMapManagerObj
            );
        this.gameObject.AddComponent<SelectableObject>();
        gameObject.name = this.placeFullName;
        placeDiary = new PlaceDiary();
    }

    // public void 1

    public Vector2 GetRandomPositionInside()
    {
        return new Vector2(
            Random.Range(placeLLAnchor.x + 0.1f, placeURAnchor.x - 0.1f), 
            Random.Range(placeLLAnchor.y + 0.1f, placeURAnchor.y - 0.1f)
        );
    }

    public void InsertInsiteSims(Sims incomingSim){
        this.inSiteSims.Add(incomingSim);   
    }
    public void RemoveInsiteSims(Sims leavingSim){
        if(this.inSiteSims.Contains(leavingSim)){
            this.inSiteSims.Remove(leavingSim);   
        }
    }

    public void SetUpGeoMapBlocked(GeoMapsManager geoMapManager){
        for ( int x = placeLLAnchor.x; x < placeURAnchor.x; x++){
            for (int y = placeLLAnchor.y; y < placeURAnchor.y; y++){
                // if (x == placeLLAnchor.x && y == placeLLAnchor.y){
                //     continue;
                // }else{
                    geoMapManager.geoMap.GetNodeByCellPosition(new Vector2Int(x, y)).SetBlocked(true);
                // }
            }
        }
    }

    // // 是的，你没有看错，这里只需要有一个flag就可以了，市民会参考建筑有没有被lockdown 而Dynamic的决定自己的Schedule
    // public void InitiateLockDown(){
    //     isLockedDown = true;
    // }
    // public void ReleaseLockDown(){
    //     isLockedDown = false;
    // }

    public void SetLockdown(bool lockdown){
        if(this.isLockedDown==false && lockdown==true){
            this.infoManager.lockdownManager.RegisterLockdown(this);
        }else if(this.isLockedDown==true && lockdown==false){
            this.infoManager.lockdownManager.UnregisterLockdown(this);
        }else{
            Debug.LogError($"unsupposed branch: old:{this.isLockedDown} new: {lockdown}");
        }
        this.isLockedDown =  lockdown;
    }

    public override string ToString()
    {
        return placeName + " " + placeLLAnchor.ToString() + " " + placeURAnchor.ToString();
    }

    public static string GetPlaceTypeDescription<T>(T place) where T : Place
    {   
       
        if (place is MedicalPlace){
            return "Medical Institution";
        }else if(place is ResidentialPlace){
            return "Residential Building";
        }else if(place is CommercialPlace){
            return "Commercial Building";
        }else if(place is OfficePlace){
            return "Office";
        }else{
            return "Unidentified Building";
        }
    }
}

public interface IContributablePlace
{
    public int CalculateQContribution();
}
public interface IExpensablePlace
{
    public int CalculateQExpense();
}

