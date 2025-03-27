using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 先尝试获取当前 GameObject 的 SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 如果当前 GameObject 没有 SpriteRenderer，则尝试在 "PlaceBody" 子物体中寻找
        if (spriteRenderer == null){
            Transform placeBody = transform.Find("PlaceBody"); // 在子物体里找 PlaceBody
            if (placeBody != null)
            {
                spriteRenderer = placeBody.GetComponent<SpriteRenderer>();
            }
        }
        if (spriteRenderer == null){
            Debug.LogWarning($"SelectableObject: 找不到 SpriteRenderer！（{gameObject.name}）");
        }
    }
}
