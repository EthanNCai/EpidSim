using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;



public class FlowFieldNode: IGridNode {

    public GridNodeMap<FlowFieldNode> gridMap;
    public Vector2Int cellPosition;
    private List<Vector2Int> _neighbors = new List<Vector2Int>();
    private int _raw_value;

    // getters and setters
    public List<Vector2Int> neighbors
    {
        get { return _neighbors; }  
        set { _neighbors = value; } 
    }
    public int raw_value{
        get { return _raw_value; }
        set { _raw_value = value; }
    }


    public FlowFieldNode next = null;

    int value = 0;

    public FlowFieldNode(int v, GridNodeMap<FlowFieldNode> map, Vector2Int cellPosition) {
        this.gridMap = map;
        this.cellPosition = cellPosition;
        this.value = v;
    }
    
    public void HandelClicked()  
    {
        this.value = 0;
    }

    public void ToggleWalkable(){

    }

    public override string ToString()
    {
        return value.ToString();
    }
    
}
