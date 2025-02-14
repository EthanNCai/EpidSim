using UnityEngine;

public class PlaceFactory : MonoBehaviour
{
    public GameObject residentialPrefab; // 住宅类的 Prefab

    public ResidentialPlace CreateResidentialPlace(
        Vector2Int placeShape, Vector2Int position, 
        int population,
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj)
    {
        ResidentialPlace residentialPlace = CreatePlaceInstance<ResidentialPlace>(residentialPrefab, placeShape, position);
        residentialPlace.ResPlaceInit(
            placeShape, 
            position, 
            population, 
            mapManager, 
            flowFieldRootObject,
            geoMapManagerObj);
        return residentialPlace;
    }

    private T CreatePlaceInstance<T>(GameObject prefab, Vector2Int placeShape, Vector2Int position) where T : Place
    {
        GameObject obj = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.transform.localScale = new Vector3(placeShape.x, placeShape.y, 1);
        T place = obj.GetComponent<T>();
        return place;
    }

}
