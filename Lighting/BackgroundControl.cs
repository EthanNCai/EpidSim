using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public MapManager mapManager;  // 传入 MapManager
    public GameObject background;  // 传入背景对象（Square）

    void Start()
    {
        FitBackgroundToMap();
    }

    public void FitBackgroundToMap()
    {
        if (mapManager == null)
        {
            Debug.LogError("MapManager is not assigned!");
            return;
        }

        if (background == null)
        {
            Debug.LogError("Background GameObject is not assigned!");
            return;
        }

        // 获取地图大小（单位 1）
        Vector2Int mapSize = mapManager.mapsize;
        Vector3 mapCenter = mapManager.mapCenter;

        // 获取背景的 SpriteRenderer
        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("SpriteRenderer not found on Background object!");
            return;
        }

        // 获取背景 Sprite 的原始大小
        Vector2 spriteSize = sr.sprite.bounds.size;
        if (spriteSize.x == 0 || spriteSize.y == 0)
        {
            Debug.LogError("Sprite size is zero! Check if the background has a valid sprite.");
            return;
        }

        // **让背景大小是地图的 2 倍**
        float targetWidth = mapSize.x * 2;
        float targetHeight = mapSize.y * 2;

        // 计算缩放比例，使背景填充 2 倍地图
        background.transform.localScale = new Vector3(targetWidth / spriteSize.x, targetHeight / spriteSize.y, 10);

        // **让背景位于地图中心**
        background.transform.position = new Vector3(mapCenter.x, mapCenter.y, 10);
    }
}
