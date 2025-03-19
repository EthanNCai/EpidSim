using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

public class SingleFlowFieldMapManager : MonoBehaviour
{
    public MapManager mapManager;
    public FlowFieldNode destination;
    public GridNodeMap<FlowFieldNode> flowFieldMap = null;

    private GameObject flowFieldRootObject;
    private GameObject geoMapManagerObj;

    private GeoMapsManager geoMapManager;
    public void FlowFieldMapsManagerInit(
        Vector2Int destination, 
        string uid,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj){
        this.geoMapManagerObj = geoMapManagerObj;
        this.mapManager = mapManager;
        this.flowFieldRootObject = flowFieldRootObject;
        this.flowFieldMap = new GridNodeMap<FlowFieldNode>(
            uid,
            1, 
            this.mapManager.mapsize,
            this.flowFieldRootObject, 
            
            (int v, GridNodeMap<FlowFieldNode> gnm ,Vector2Int c) => new FlowFieldNode(v,gnm,c));
        this.destination = this.flowFieldMap.GetNodeByCellPosition(destination);
        this.geoMapManager = geoMapManagerObj.GetComponent<GeoMapsManager>();
        ClickManager.OnAfterCellClicked += (Vector2Int cellPosition) => {
            this.UpdateFlowField();
        };
        GeoMapsManager.OnMapChanged += () => {
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
        foreach (FlowFieldNode node in this.flowFieldMap.nodeIterator())
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
                    GeoMapsManager geoMapManager = geoMapManagerObj.GetComponent<GeoMapsManager>();
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
        foreach (FlowFieldNode node in this.flowFieldMap.nodeIterator())
        {
            FlowFieldNode currentNode = node;  


            if(this.destination == currentNode) { currentNode.flowFieldDirection = new Vector2Int(0,0); continue; }

            DirectionalNeighbors<FlowFieldNode> neighbors = currentNode.neighbors;
            FlowFieldNode minStepsToDestinationNeighbor = null;
            int minStepsToDestination = int.MaxValue;  // Correct initialization value
            GeoMapsManager geoMapManager = geoMapManagerObj.GetComponent<GeoMapsManager>();
            GridNodeMap<GeoNode> geoMap = geoMapManager.geoMap;

            if (geoMap.GetNodeByCellPosition(currentNode.cellPosition).blocked)
            {
                FlowFieldNode leftNeighbor = neighbors.left;
                FlowFieldNode rightNeighbor = neighbors.right;

                bool leftValid = (leftNeighbor != null) && !geoMap.GetNodeByCellPosition(leftNeighbor.cellPosition).blocked;
                bool rightValid = (rightNeighbor != null) && !geoMap.GetNodeByCellPosition(rightNeighbor.cellPosition).blocked;

                if (leftValid && rightValid)
                {
                    // 计算左右邻居到目标的距离
                    int leftDistance = Mathf.Abs(leftNeighbor.cellPosition.x - destination.cellPosition.x);
                    int rightDistance = Mathf.Abs(rightNeighbor.cellPosition.x - destination.cellPosition.x);

                    // 选择距离较小的方向
                    minStepsToDestinationNeighbor = (leftDistance < rightDistance) ? leftNeighbor : rightNeighbor;
                }
                else if (leftValid)
                {
                    minStepsToDestinationNeighbor = leftNeighbor;
                }
                else if (rightValid)
                {
                    minStepsToDestinationNeighbor = rightNeighbor;
                }

                // 设定 flowFieldDirection
                if (minStepsToDestinationNeighbor != null)
                {
                    currentNode.flowFieldDirection = minStepsToDestinationNeighbor.cellPosition - currentNode.cellPosition;
                }
                else
                {
                    currentNode.flowFieldDirection = Vector2Int.left; // 如果都不行，默认向左
                }
            }
            else{
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
                currentNode.flowFieldDirection = minStepsToDestinationNeighbor.cellPosition - currentNode.cellPosition;
            }
        }
    }
}
