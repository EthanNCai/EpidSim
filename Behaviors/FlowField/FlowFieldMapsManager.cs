using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class FlowFieldMapManager : MonoBehaviour
{
    public MapManager mapManager;

    // FlowField Collections
    public List<GridNodeMap<GridNode>> flowFieldMaps = new List<GridNodeMap<GridNode>>();


    // itemMapManager (for the determination of walkables)
    public GameObject ItemMapManager;

    void Start()
    {
        this.flowFieldMaps.Add(new GridNodeMap<GridNode>(
            "test",
            1, 
            mapManager.mapsize, mapManager.gridMaproot, 
            (int v, GridNodeMap<GridNode> gnm ,Vector2Int c) => new GridNode(v,gnm,c)));
        
    }

    public void UpdateFlowField(){
        /*
            for i in mapManagers:
                infer()
            1. walkable determination
            2. walkable 
        */
    }
}
