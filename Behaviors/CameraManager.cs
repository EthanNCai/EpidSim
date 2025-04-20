using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public MapManager mapManager;
    public Camera _camera;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoomRatio = 0.3f; // 最小缩放比例
    private float initialOrthographicSize;

    private float moveSpeed = 20f;
    public float moveMarginRatio = 0.07f; // 移动越界最大百分比

    private Vector2 mapMinBound;
    private Vector2 mapMaxBound;

    void Start()
    {
        if (mapManager == null || _camera == null)
        {
            Debug.LogError("MapManager or Camera is not assigned!");
            return;
        }

        // 设置初始位置
        _camera.transform.position = new Vector3(mapManager.mapCenter.x, mapManager.mapCenter.y, _camera.transform.position.z);

        // 计算初始正交大小
        Vector2Int mapSize = mapManager.mapsize;
        float aspect = _camera.aspect;
        float sizeX = mapSize.x / 2f;
        float sizeY = mapSize.y / 2f;

        initialOrthographicSize = Mathf.Max(sizeY, sizeX / aspect);
        _camera.orthographicSize = initialOrthographicSize;

        // 计算边界（超出地图30%的范围）
        float marginX = mapSize.x * moveMarginRatio;
        float marginY = mapSize.y * moveMarginRatio;

        mapMinBound = new Vector2(-marginX, -marginY) + (Vector2)mapManager.mapRoot.transform.position;
        mapMaxBound = new Vector2(mapSize.x + marginX, mapSize.y + marginY) + (Vector2)mapManager.mapRoot.transform.position;
    }

    void Update()
    {
        HandleZoom();
        HandleMove();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            float newSize = _camera.orthographicSize - scroll * zoomSpeed;
            float minSize = initialOrthographicSize * minZoomRatio;
            float maxSize = initialOrthographicSize;

            _camera.orthographicSize = Mathf.Min(Mathf.Max(minSize, newSize),maxSize);
        }
    }

    void HandleMove()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.up;
        if (Input.GetKey(KeyCode.S)) move += Vector3.down;
        if (Input.GetKey(KeyCode.A)) move += Vector3.left;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;

        if (move != Vector3.zero)
        {
            Vector3 newPosition = _camera.transform.position + move * moveSpeed * Time.deltaTime;

            // 限制在边界范围内
            float camHalfWidth = _camera.orthographicSize * _camera.aspect;
            float camHalfHeight = _camera.orthographicSize;

            newPosition.x = Mathf.Clamp(newPosition.x, mapMinBound.x + camHalfWidth, mapMaxBound.x - camHalfWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, mapMinBound.y + camHalfHeight, mapMaxBound.y - camHalfHeight);

            _camera.transform.position = new Vector3(newPosition.x, newPosition.y, _camera.transform.position.z);
        }
    }

    public Vector3 ScreenToWorldPosition(Vector3 posIn){
        return this._camera.ScreenToWorldPoint(posIn);
    }
}
