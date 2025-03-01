using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

public class GeoMapsManager : MonoBehaviour
{
    public MapManager mapManager;
    public GridNodeMap<GeoNode> geoMap;
    // public GameObject geoRootObject;
    // public GameObject gridDebuggerObj;
    public GridDebugManager gridDebuggerManager;


    public static event Action OnMapChanged;
    void Awake()
    {
        // this.gridDebuggerManager =  gridDebuggerObj.GetComponent<GridDebugManager>();
        this.geoMap =  new GridNodeMap<GeoNode>(
            "tset",
            1, 
            mapManager.mapsize, this.gridDebuggerManager.GetListedRoot("geo_map"), 
            (int v, GridNodeMap<GeoNode> gnm ,Vector2Int c) => new GeoNode(v,gnm,c));
    }

    void Update()
    {
        // Your implementation here.
    }

    public void InvokeMapChangedEvent(){
        OnMapChanged?.Invoke();
    }
}
