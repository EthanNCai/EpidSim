using System.ComponentModel;
using UnityEngine;

public class GridMapsManager : MonoBehaviour
{
    public MapManager mapManager;

    public GridNodeMap gridNodeMap;

    
    void Start()
    {
        this.gridNodeMap =  new GridNodeMap(1, mapManager.mapsize, mapManager.gridMaproot);
        
    }

    void Update()
    {
        // Your implementation here.
    }
}
