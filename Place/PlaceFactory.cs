using System.ComponentModel;
using UnityEngine;

public class PlaceFactory : MonoBehaviour
{
    public GameObject mapRoot;
    public GameObject residentialPrefab; // 住宅类的 Prefab
    public GameObject publicPrefab;
    public GameObject officePrefab;
    public GameObject medicalPrefab;

    public ResidentialPlace CreateResidentialPlace(
        Vector2Int placeShape, Vector2Int position,
        int population,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridDebugManager gridDebugManager)
    {
        ResidentialPlace residentialPlace = CreatePlaceInstance<ResidentialPlace>(residentialPrefab, placeShape, position);
        residentialPlace.ResPlaceInit(
            placeShape,
            position,
            population,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager);
        return residentialPlace;
    }

    public OfficePlace CreateOfficePlace(
        Vector2Int placeShape, Vector2Int position,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridDebugManager gridDebugManager)
    {
        OfficePlace officePlace = CreatePlaceInstance<OfficePlace>(officePrefab, placeShape, position);
        officePlace.OfficePlaceInit(
            placeShape,
            position,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebugManager);
        return officePlace;
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
