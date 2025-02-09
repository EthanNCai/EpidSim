using CodeMonkey.Utils;
using System;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public static event Action<Vector2Int> OnCellClicked;
    public GameObject gridMapRoot;  

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Debug.Assert(gridMapRoot != null, "gridMapRoot is null");
            Vector3 mouseLocalPosition = gridMapRoot.transform.InverseTransformPoint(mouseWorldPosition);
                Vector2Int cellPosition = new Vector2Int(
                    Mathf.FloorToInt(mouseLocalPosition.x),
                    Mathf.FloorToInt(mouseLocalPosition.y)
                );
                OnCellClicked?.Invoke(cellPosition);
        }
    }
}
