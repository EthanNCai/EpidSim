using UnityEngine;

public class PlaceFactory : MonoBehaviour
{
    public GameObject residentialPrefab; // 住宅类的 Prefab

    public ResidentialPlace CreateResidentialPlace(Vector2Int placeShape, Vector2Int position, int population)
    {
        ResidentialPlace residentialPlace = CreatePlaceInstance<ResidentialPlace>(residentialPrefab, placeShape, position);
        residentialPlace.CustomInit(placeShape, position, population);
        return residentialPlace;
    }

    private T CreatePlaceInstance<T>(GameObject prefab, Vector2Int placeShape, Vector2Int position) where T : Place
    {
        GameObject obj = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        obj.transform.SetParent(transform);
        T place = obj.GetComponent<T>();
        return place;
    }
}
