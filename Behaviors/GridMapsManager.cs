using System.ComponentModel;
using UnityEngine;

public class GridMapsManager : MonoBehaviour
{
    public MapManager mapManager;

    public GridNodeMap<GridNode> gridNodeMap;

    
    void Start()
    {


        
        this.gridNodeMap =  new GridNodeMap<GridNode>(
            1, 
            mapManager.mapsize, mapManager.gridMaproot, 
            (int v, GridNodeMap<GridNode> gnm ,Vector2Int c) => new GridNode(v,gnm,c));
        
    }

    void Update()
    {
        // Your implementation here.
    }
}
