using System.ComponentModel;
using UnityEngine;

public class GeoMapsManager : MonoBehaviour
{
    public MapManager mapManager;
    public GridNodeMap<GeoNode> geoMap;
    public GameObject geoRootObject;

    void Awake()
    {
        this.geoMap =  new GridNodeMap<GeoNode>(
            "tset",
            1, 
            mapManager.mapsize, geoRootObject, 
            (int v, GridNodeMap<GeoNode> gnm ,Vector2Int c) => new GeoNode(v,gnm,c));
        
    }

    void Update()
    {
        // Your implementation here.
    }
}
