using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject gridMaproot;
    public Vector2Int mapsize = new Vector2Int(20, 10);
    private Vector3? _mapCenter; 

    public Vector3 mapCenter // lazy loaded
    {
        get
        {
            if (!_mapCenter.HasValue)
            {
                _mapCenter = gridMaproot.transform.position + new Vector3(mapsize.x / 2f, mapsize.y / 2f, -1);
            }
            return _mapCenter.Value;
        }
    }

    void Start()
    {
        Debug.Assert(gridMaproot != null, "GridMapRoot is not set, Abort");
    }

    void Update()
    {
        // Now you can access mapCenter lazily, and it'll be computed only when needed.
    }
}
