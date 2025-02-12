
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;



public class GeoNode: IGridNode<GeoNode> {

    public GridNodeMap<GeoNode> gridMap;
    private DirectionalNeighbors<GeoNode> _neighbors = new DirectionalNeighbors<GeoNode>();
    private int _raw_value;
    private Vector2Int _cellPosition;


    public bool blocked = false;

    public GeoNode next = null;
    public DirectionalNeighbors<GeoNode> neighbors
    {
        get { return _neighbors; }  
        set { _neighbors = value; } 
    }
    public int raw_value{
        get { return _raw_value; }
        set { _raw_value = value; }
    }
    public Vector2Int cellPosition{
        get { return _cellPosition; }
        set { _cellPosition = value; }
    }
    public int value;

    public GeoNode(int v, GridNodeMap<GeoNode> map, Vector2Int cellPosition) {
        this.gridMap = map;
        this.cellPosition = cellPosition;
        this.value = v;
    }


    public void HandelClicked()  
    {
        this.blocked = true;
    }

    public void ToggleWalkable(){

    }

    public override string ToString()
    {
        return blocked.ToString();
    }
    
}
