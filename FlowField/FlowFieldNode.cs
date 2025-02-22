using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class FlowFieldNode: IGridNode<FlowFieldNode> {

    public GridNodeMap<FlowFieldNode> flowFieldMap;
    private Vector2Int _flowFieldDirection = Vector2Int.zero;
    public DirectionalNeighbors<FlowFieldNode> _neighbors = new DirectionalNeighbors<FlowFieldNode>();
    private int _stepsToDestination;
    public int stepsToDestination {
        get { return _stepsToDestination; }
        set { _stepsToDestination = value; this.flowFieldMap.InvokeValueUpdateByCell(this.cellPosition);}
    }
    private Vector2Int _cellPosition;
    private int _raw_value;

    public Vector2Int flowFieldDirection{
        get { return _flowFieldDirection; }
        set { _flowFieldDirection = value; this.flowFieldMap.InvokeValueUpdateByCell(this.cellPosition); }
    }
    // getters and setters
    public DirectionalNeighbors<FlowFieldNode> neighbors
    {
        get { return _neighbors; }  
        set { _neighbors = value; }
    }
    public int raw_value{
        get { return _raw_value; }
        set { _raw_value = value;}
    }
    public Vector2Int cellPosition{
        get { return _cellPosition; }
        set { _cellPosition = value; }
    }

    public FlowFieldNode next = null;
    int value = 0;

    public FlowFieldNode(int v, GridNodeMap<FlowFieldNode> map, Vector2Int cellPosition) {
        this.flowFieldMap = map;
        this.cellPosition = cellPosition;
        this.value = v;
    }
    
    public void HandelClicked()  
    {
    }

    public void ToggleWalkable()
    {
    }
    public string GetDirectionLiteral(Vector2Int direction)
    {
        var directionMap = new Dictionary<Vector2Int, string>
        {
            { Directions.up, "↑" },
            { Directions.down, "↓" },
            { Directions.left, "←" },
            { Directions.right, "→" },
            { Directions.upLeft, "↖" },
            { Directions.upRight, "↗" },
            { Directions.downLeft, "↙" },
            { Directions.downRight, "↘" }
        };

        return directionMap.ContainsKey(direction) ? directionMap[direction] : ".";
    }
    public override string ToString()
    {
        if( stepsToDestination == int.MaxValue){
            return "x";
            
        }else{
            return stepsToDestination.ToString() + ',' + GetDirectionLiteral(flowFieldDirection);
        }
    }
}
