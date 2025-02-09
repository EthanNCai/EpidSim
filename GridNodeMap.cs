using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System.Linq;
using Unity.VisualScripting;
using System;
public class GridNodeMap<TGridNodeObject>
{
    private TGridNodeObject[,] gridNodes;
    private TextMesh[,] debugTexts;
    private GameObject debugTextRoot;

    private int cellSize;
    private Vector2Int mapSize;
    private Vector2Int rootPosition;

    private int n_rows;
    private int n_cols;



    public GridNodeMap(int cellSize, Vector2Int mapSize, GameObject gridMapRoot, Func<int, TGridNodeObject> createGridNode){
        Debug.Assert(mapSize.x % cellSize == 0 && mapSize.y % cellSize == 0, "Map size must be divisible by cell size");
        n_rows = mapSize.y / cellSize;  // Rows should be based on the y dimension
        n_cols = mapSize.x / cellSize;  // Columns should be based on the x dimension

        this.cellSize = cellSize;
        this.mapSize = mapSize;

        // Initialize gridNodes and debugTexts with n_rows and n_cols
        this.gridNodes = new TGridNodeObject[n_rows, n_cols];
        this.debugTexts = new TextMesh[n_rows, n_cols];
        
        // Generate node objects
        int i = 0;
        for (int r = 0; r < n_rows; r++)
        {
            for (int c = 0; c < n_cols; c++)
            {
                gridNodes[r, c] = createGridNode(i);
                i++;
            }
        }

        // Generate debug representation
        int j = 0;
        for (int r = 0; r < n_rows; r++)
        {
            for (int c = 0; c < n_cols; c++)
            {
                debugTexts[r, c] = Utils.SpawnTextAtRelativePosition(gridMapRoot, GetNodeCenterPosition(new Vector2Int(r, c)), "Grid Node Map");
                debugTexts[r, c].text = gridNodes[r, c].ToString();
                j++;
            }
        }

        // print debug
    }

    public Vector2 GetNodeCenterPosition(Vector2Int cellPosition)
    {
        return new Vector2(cellPosition.x,cellPosition.y) + new Vector2(cellSize/2f,cellSize/2f);
    }
    public void TurnOffDisplay(){}
    public void TurnOnDisplay(){}
}

