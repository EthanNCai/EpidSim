using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using Unity.VisualScripting;
using System;
using TMPro;



public interface IGridNode<TGridNodeObject>{
    void HandelClicked();
    public DirectionalNeighbors<TGridNodeObject> neighbors { get; set;}
    public int raw_value {get; set;}
 
    string ToString();
}

public class DirectionalNeighbors<TGridNodeObject>
{
    public TGridNodeObject up;
    public TGridNodeObject down;
    public TGridNodeObject left;
    public TGridNodeObject right;

    public DirectionalNeighbors(TGridNodeObject up = default, TGridNodeObject down = default, 
                                TGridNodeObject left = default, TGridNodeObject right = default)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
    public DirectionalNeighbors()
    {
        
    }

}




public class GridNodeMap<TGridNodeObject> where TGridNodeObject : IGridNode<TGridNodeObject>

{
    public string tag;
    private TGridNodeObject[,] gridNodes;
    private TextMesh[,] debugTexts;
    private GameObject gridMapRoot;
    private int cellSize;
    private Vector2Int mapSize;
    private int n_rows;
    private int n_cols;


    public GridNodeMap(
        string tag,
        int cellSize,
        Vector2Int mapSize,
        GameObject gridMapRoot,
        Func<int,GridNodeMap<TGridNodeObject>,Vector2Int, TGridNodeObject> createGridNode){
        Debug.Assert(mapSize.x % cellSize == 0 && mapSize.y % cellSize == 0, "Map size must be divisible by cell size");
        n_rows = mapSize.y / cellSize;  
        n_cols = mapSize.x / cellSize;  

        this.tag = tag;
        this.gridMapRoot = gridMapRoot;
        this.cellSize = cellSize;
        this.mapSize = mapSize;
        this.gridNodes = new TGridNodeObject[n_rows, n_cols];
        this.debugTexts = new TextMesh[n_rows, n_cols];
        
        // Generate node objects
        int i = 0;
        for (int r = 0; r < n_rows; r++)
        {
            for (int c = 0; c < n_cols; c++)
            {
                gridNodes[r, c] = createGridNode(i,this, new Vector2Int(r,c));
                gridNodes[r, c].neighbors = this.GetNeighbors(new Vector2Int(r, c));
                gridNodes[r, c].raw_value = i;
                i++;
            }
        }

        // Generate debug representation
        int j = 0;
        for (int r = 0; r < n_rows; r++)
        {
            for (int c = 0; c < n_cols; c++)
            {
                debugTexts[r, c] = Utils.SpawnTextAtRelativePosition(this.gridMapRoot, GetNodeCenterPosition(new Vector2Int(r, c)), "Grid Node Map");
                debugTexts[r, c].text = gridNodes[r, c].ToString();
                j++;
            }
        }

        // handle click
        ClickManager.OnCellClicked += (Vector2Int cellPosition) => {

            int r = cellPosition.x;
            int c = cellPosition.y;

            // update value
            this.gridNodes[r,c].HandelClicked();

            // update text
            debugTexts[r,c].text = gridNodes[r, c].ToString();
        };
    }

    public Vector2 GetNodeCenterPosition(Vector2Int cellPosition)
    {
        return new Vector2(cellPosition.x,cellPosition.y) + new Vector2(cellSize/2f,cellSize/2f);
    }
   private DirectionalNeighbors<TGridNodeObject> GetNeighbors(Vector2Int cellPosition)
    {
        int r = cellPosition.x;
        int c = cellPosition.y;

        TGridNodeObject up = default(TGridNodeObject),
                        down = default(TGridNodeObject),
                        left = default(TGridNodeObject),
                        right = default(TGridNodeObject); 

        if (r - 1 >= 0) {
            up = this.gridNodes[r - 1, c];
        }
        if (r + 1 < this.gridNodes.GetLength(0)) {
            down = this.gridNodes[r + 1, c];
        }
        if (c - 1 >= 0) {
            left = this.gridNodes[r, c - 1];
        }
        if (c + 1 < this.gridNodes.GetLength(1)) {
            right = this.gridNodes[r, c + 1];
        }

        return new DirectionalNeighbors<TGridNodeObject>(up, down, left, right);
    }


    public void TurnOffDisplay(){}
    public void TurnOnDisplay(){}
}

