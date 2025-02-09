using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GridNodeMap 
{
    private int[,] gridNodes;
    private TextMesh[,] debugTexts;
    private GameObject debugTextRoot;


    private int cellSize;
    private Vector2Int mapSize;
    private Vector2Int rootPosition;

    private int n_rows;
    private int n_cols;



    public GridNodeMap(int cellSize, Vector2Int mapSize, GameObject gridMapRoot)
    {
        Debug.Assert(mapSize.x % cellSize == 0 && mapSize.y % cellSize == 0, "Map size must be divisible by cell size");   
        n_rows = mapSize.x / cellSize;
        n_cols = mapSize.y / cellSize;

        this.cellSize = cellSize;
        this.mapSize = mapSize;
        this.gridNodes = new int[mapSize.x / cellSize, mapSize.y / cellSize];
        this.debugTexts = new TextMesh[mapSize.x / cellSize, mapSize.y / cellSize];


        int i = 0;
        for(int r =0; r < n_rows; r++)
        {
            for(int c = 0; c < n_cols; c++)
            {
                debugTexts[r,c] = Utils.SpawnTextAtRelativePosition(gridMapRoot,new Vector2Int(r, c), i.ToString());
                i ++ ;
            }
        }
    }

    public void TurnOffDisplay(){}
    public void TurnOnDisplay(){}




}
