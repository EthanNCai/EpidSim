using System.ComponentModel;
using UnityEngine;

public class PlaceFactory : MonoBehaviour
{
    public GameObject mapRoot;
    public GameObject residentialPrefab; // 住宅类的 Prefab
    public GameObject commercialPrefab;
    public GameObject officePrefab;
    public GameObject medicalPrefab;
    public GameObject testCentrePrefab;
    public GameObject qrtCentrePrefab;

    public ResidentialPlace CreateResidentialPlace(
        Vector2Int placeShape, Vector2Int position,
        int population,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        ResidentialPlace residentialPlace = CreatePlaceInstance<ResidentialPlace>(residentialPrefab, placeShape, position);
        residentialPlace.ResPlaceInit(
            placeShape,
            position,
            population,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return residentialPlace;
    }

    public OfficePlace CreateOfficePlace(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        OfficePlace officePlace = CreatePlaceInstance<OfficePlace>(officePrefab, placeShape, position);
        officePlace.OfficePlaceInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return officePlace;
    }

    public CommercialPlace CreateCommercialPlace(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        CommercialPlace commercialPlace = CreatePlaceInstance<CommercialPlace>(commercialPrefab, placeShape, position);
        commercialPlace.CommercialInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return commercialPlace;
    }

    public MedicalPlace CreateMedicalPlace(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        MedicalPlace medicalPlace = CreatePlaceInstance<MedicalPlace>(medicalPrefab, placeShape, position);
        medicalPlace.CommercialInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return medicalPlace;
    }

    public TestCenterPlace CreateTestCentre(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        TestCenterPlace testCentrePlace = CreatePlaceInstance<TestCenterPlace>(testCentrePrefab, placeShape, position);
        testCentrePlace.TestCentrePlaceInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return testCentrePlace;
    }
    public QRTCentrePlace CreateQRTCentre(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebugManager,
        InfoManager infoManager,
        CFEManager cfeManager)
    {
        QRTCentrePlace testCentrePlace = CreatePlaceInstance<QRTCentrePlace>(qrtCentrePrefab, placeShape, position);
        testCentrePlace.QRTCenterInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager,
            infoManager,
            cfeManager);
        return testCentrePlace;
    }

    private T CreatePlaceInstance<T>(GameObject prefab, Vector2Int placeShape, Vector2Int position) where T : Place
    {
        GameObject obj = Instantiate(prefab, mapRoot.transform);
        obj.transform.localPosition = new Vector3(position.x, position.y, 0);
        obj.transform.localScale = new Vector3(placeShape.x, placeShape.y, 1);
        T place = obj.GetComponent<T>();
        Debug.Assert(place != null, "Prefab object cannot be null");
        return place;
    }
}
