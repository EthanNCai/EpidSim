using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

public class FlowFieldMapManager : MonoBehaviour
{
    public MapManager mapManager;
    public FlowFieldNode destination;
    // FlowField Collections
    public List<GridNodeMap<FlowFieldNode>> flowFieldMaps = new List<GridNodeMap<FlowFieldNode>>();

    public GameObject flowFieldRootObject;
    // itemMapManager (for the determination of walkables)
    public GameObject GeoMapManagerObj;

    void Awake()
    {
        this.flowFieldMaps.Add(new GridNodeMap<FlowFieldNode>(
            "test",
            1, 
            mapManager.mapsize,
            flowFieldRootObject, 
            (int v, GridNodeMap<FlowFieldNode> gnm ,Vector2Int c) => new FlowFieldNode(v,gnm,c)));
        destination = this.flowFieldMaps[0].GetNodeByCellPosition(new Vector2Int(1, 4));
        // Debug.Log(flowFieldMaps[0].GetNodeByCellPosition(new Vector2Int(1, 4)).neighbors.ToString());
        
        
    }
    void Start()
    {
        ClickManager.OnAfterCellClicked += (Vector2Int cellPosition) => {
            this.UpdateFlowField();
        };
        this.UpdateFlowField();
    }


    public void UpdateFlowField()
{
    Debug.Log("Flow field update started.");

    // STEP1: BFS
    if (destination == null)
    {
        Debug.LogError("Destination is not set.");
        return;
    }
    foreach (FlowFieldNode node in this.flowFieldMaps[0].nodeIterator())
    {
        node.stepsToDestination = int.MaxValue;
    }

    Queue<FlowFieldNode> queue = new Queue<FlowFieldNode>();
    destination.stepsToDestination = 0;  
    queue.Enqueue(destination);  
    while (queue.Count > 0)
    {
        FlowFieldNode currentNode = queue.Dequeue();  
        DirectionalNeighbors<FlowFieldNode> neighbors = currentNode.neighbors;
        foreach (var neighbor in new FlowFieldNode[] { neighbors.up, neighbors.down, neighbors.left, neighbors.right })
        {
            if (neighbor != null && neighbor.stepsToDestination == int.MaxValue)
            {
                GeoMapsManager geoMapManager = GeoMapManagerObj.GetComponent<GeoMapsManager>();
                GridNodeMap<GeoNode> geoMap = geoMapManager.geoMap;
                if (!geoMap.GetNodeByCellPosition(neighbor.cellPosition).blocked)
                {
                    neighbor.stepsToDestination = currentNode.stepsToDestination + 1;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    // STEP2: Generate FlowField
    Debug.Log("start generating flowField");
    foreach (FlowFieldNode node in this.flowFieldMaps[0].nodeIterator())
    {
        FlowFieldNode currentNode = node;  
        if(this.destination == currentNode) { currentNode.flowFieldDirection = new Vector2Int(0,0); continue; }

        DirectionalNeighbors<FlowFieldNode> neighbors = currentNode.neighbors;
        FlowFieldNode minStepsToDestinationNeighbor = null;
        int minStepsToDestination = int.MaxValue;  // Correct initialization value

        foreach (var neighbor in new FlowFieldNode[] { 
            neighbors.up,
            neighbors.down,
            neighbors.left,
            neighbors.right,
            neighbors.upLeft,
            neighbors.upRight,
            neighbors.downLeft,
            neighbors.downRight
        })
        {
            if (neighbor != null && neighbor.stepsToDestination != int.MaxValue)
            {
                GeoMapsManager geoMapManager = GeoMapManagerObj.GetComponent<GeoMapsManager>();
                GridNodeMap<GeoNode> geoMap = geoMapManager.geoMap;
                if (!geoMap.GetNodeByCellPosition(neighbor.cellPosition).blocked)
                {
                    if (neighbor.stepsToDestination < minStepsToDestination)
                    {
                        minStepsToDestination = neighbor.stepsToDestination;
                        minStepsToDestinationNeighbor = neighbor;
                    }
                }
            }
        }

        if (minStepsToDestinationNeighbor != null)
        {
            currentNode.flowFieldDirection = minStepsToDestinationNeighbor.cellPosition - currentNode.cellPosition;
            Debug.Log(currentNode.flowFieldDirection);
        }
        else
        {
            currentNode.flowFieldDirection = Vector2Int.zero;
        }
    }
}

}
