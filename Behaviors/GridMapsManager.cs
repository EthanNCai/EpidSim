using System.ComponentModel;
using UnityEngine;

public class GridMapsManager : MonoBehaviour
{
    public GameObject gridMaproot;
    public GridNodeMap gridNodeMap;

    
    void Start()
    {
        this.gridNodeMap =  new GridNodeMap(1, new Vector2Int(10,10), gridMaproot);
        
    }

    void Update()
    {
        // Your implementation here.
    }
}
