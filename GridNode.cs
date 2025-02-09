
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GridNode: IGridNode {

    public GridNodeMap<GridNode> gridMap;
    private List<Vector2Int> _neighbors = new List<Vector2Int>();
    private int _raw_value;
    public List<Vector2Int> neighbors
    {
        get { return _neighbors; }  
        set { _neighbors = value; } 
    }
    public int raw_value{
        get { return _raw_value; }
        set { _raw_value = value; }
    }
    public Vector2Int cellPosition;
    public int value;

    public GridNode(int v, GridNodeMap<GridNode> map, Vector2Int cellPosition) {
        this.gridMap = map;
        this.cellPosition = cellPosition;
        this.value = v;
    }


    public void HandelClicked()  
    {
        this._raw_value = 0;
    }

    public void ToggleWalkable(){

    }

    public override string ToString()
    {
        return _raw_value.ToString();
    }
    
}
