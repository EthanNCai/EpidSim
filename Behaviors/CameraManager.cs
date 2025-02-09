using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public MapManager mapManager;
    public Camera _camera;
    void Start()
    {
        _camera.transform.position = mapManager.mapCenter;
        Debug.Log("map center: " + mapManager.mapCenter.ToString());

    }
    void Update()
    {
        
    }
}
