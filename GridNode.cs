
using UnityEngine;
using UnityEngine.UI;



public class GridNode: IGridNode {

    public GridNodeMap<GridNode> gridMap;
    public Vector2Int cellPosition;
    int value;

    public GridNode(int v, GridNodeMap<GridNode> map, Vector2Int cellPosition) {
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
