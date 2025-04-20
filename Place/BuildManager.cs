using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public struct BuildableInfo
{
    public string name;
    public string description;
    public Vector2Int placeSize;
    public Sprite icon;
    public PlaceMeta.PlaceType placeType;
     public BuildableInfo(string name, string description, Sprite icon, Vector2Int size, PlaceMeta.PlaceType placeType)
    {
        this.name = name;
        this.description = description;
        this.icon = icon;
        this.placeSize = size;
        this.placeType = placeType;
    }
}
public static class PlaceMeta
{
    public enum PlaceType
    {
        ResidentialPlace,
        OfficePlace,
        MedicalPlace,
        TestCentrePlace,
        CommercialPlace,
        QRTPlace,
    }

    static public List<PlaceType> buidableTypes = new List<PlaceType>{
        PlaceType.MedicalPlace, 
        PlaceType.TestCentrePlace,
        PlaceType.QRTPlace
    };

    static public List<BuildableInfo> buidableInfos = new List<BuildableInfo>();

    static private List<Vector2Int> GetSizesForType(PlaceType placeType)
    {
        switch (placeType)
        {
            case PlaceType.MedicalPlace:
                return new List<Vector2Int> { new Vector2Int(1, 1), new Vector2Int(2, 2) };
            case PlaceType.TestCentrePlace:
                return new List<Vector2Int> { new Vector2Int(1, 1),new Vector2Int(2, 2), new Vector2Int(3, 3) };
            case PlaceType.QRTPlace:
                return new List<Vector2Int> { new Vector2Int(1, 1),new Vector2Int(2, 2), new Vector2Int(3, 3) };
            default:
                return new List<Vector2Int> { new Vector2Int(1, 1) };
        }
    }

    static private BuildableInfo CreateBuildableInfo(PlaceType placeType, Vector2Int size)
    {
        string sizeSuffix = $" ({size.x}x{size.y})";
        switch (placeType)
        {
            case PlaceType.MedicalPlace:
                return new BuildableInfo(
                    "Clinic" + sizeSuffix,
                    "A place where sims can get medical treatment",
                    null,
                    size,
                    PlaceType.MedicalPlace
                );
            case PlaceType.TestCentrePlace:
                return new BuildableInfo(
                    "Test Centre" + sizeSuffix,
                    "A place where sims can get virus CPR test",
                    null,
                    size,
                    PlaceType.TestCentrePlace
                );
            case PlaceType.QRTPlace:
                return new BuildableInfo(
                    "Quarantine Centre" + sizeSuffix,
                    "A place where sims can get quarantined",
                    null,
                    size,
                    PlaceType.QRTPlace
                );
            default:
                throw new NotImplementedException();
        }
    }

    static public List<BuildableInfo> GetBuidableInfoList()
    {
        if (buidableInfos.Count == 0)
        {
            foreach (var buidableType in buidableTypes)
            {
                var sizes = GetSizesForType(buidableType);
                foreach (var size in sizes)
                {
                    buidableInfos.Add(CreateBuildableInfo(buidableType, size));
                }
            }
        }
        return buidableInfos;
    }
}


// using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GameObject medicalPrefab;
    public GameObject testCentrePrefab;
    public GameObject previewPrefab;
    public PlaceManager placeManager;
    public GameObject mapRoot;
    public CameraManager cameraManager;
    // private bool isPreviewing = false;
    // private GameObject storedPreviewObject = null;
    private GameObject previewingObject = null;
    public static event Action OnBuildCanceled;
    public static event Action OnBuildCompleted;
    private BuildableInfo currentBuidableInfo;

    private void Start(){
        BuidableController.OnBuildClicked += HandleBuildClicked;
    }

    private void Update(){
        if (this.previewingObject && this.previewingObject.activeSelf){
            Vector3 mouseWorldPos = GetMouseWorldPositionOnMap();
            Vector2Int cellPos = new Vector2Int(
                Mathf.FloorToInt(mouseWorldPos.x),
                Mathf.FloorToInt(mouseWorldPos.y)
            );
            previewingObject.transform.localPosition = new Vector3(cellPos.x, cellPos.y, 0);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) // 0 表示左键，1 是右键，2 是中键
            {
                // Debug.Log("Build!!!!!!!!!!!!!!");
                placeManager.GeneratePlaceOnCell(currentBuidableInfo, cellPos);
                // CameraShake.TriggerShake();
                HandelBuildFinished();
            }
        }
        // 在这里检查是不是build
    }

    public void HandleBuildClicked(BuildableInfo buildableInfo){
        Debug.Log("Place Previewing Started");
        this.currentBuidableInfo = buildableInfo;
        // isPreviewing = true;
        if(previewingObject != null){
            Debug.Assert(previewingObject.activeSelf == false, "bug here");
            // this.previewingObject = this.storedPreviewObject;
            previewingObject.SetActive(true);
            this.previewingObject.transform.localScale = new Vector3(buildableInfo.placeSize.x, buildableInfo.placeSize.y, 1);
        }else{
            this.previewingObject = Instantiate(this.previewPrefab, mapRoot.transform);
            this.previewingObject.transform.localScale = new Vector3(buildableInfo.placeSize.x, buildableInfo.placeSize.y, 1);
            this.previewingObject.SetActive(true);
        }
    }
    public void HandleBuildCancel(){
        Debug.Assert(previewingObject != null);
        OnBuildCanceled?.Invoke();
        this.previewingObject.SetActive(false);
        // this.previewingObject = null;
        // this.currentBuidableInfo = null;
    }
    public void HandelBuildFinished(){
        Debug.Assert(previewingObject != null);
        Debug.Assert(previewingObject.activeSelf == true);
        this.previewingObject.SetActive(false);
        OnBuildCompleted?.Invoke();
    }

    private Vector3 GetMouseWorldPositionOnMap(){
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPosition = cameraManager.ScreenToWorldPosition(mousePos);
        Vector3 mousePostRef = mapRoot.transform.TransformPoint(worldPosition);
        return mousePostRef;
    }
}

