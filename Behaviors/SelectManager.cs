using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public TextMeshProUGUI hoverText;  // 悬浮提示文本（拖入UI组件）
    
    private SelectableObject highlightedObject;  // 记录当前高亮对象

    void Start()
    {
        if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false); // 默认隐藏文本
        }
    }

    void Update()
    {
        DetectHoverAndSelect();
        UpdateHoverTextPosition();
    }

    void DetectHoverAndSelect()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hitSims = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Sims"));
        RaycastHit2D hitPlace = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Place"));

        SelectableObject newHighlighted = null;

        if (hitSims.collider != null)
        {
            newHighlighted = hitSims.collider.GetComponent<SelectableObject>();
        }
        else if (hitPlace.collider != null)
        {
            newHighlighted = hitPlace.collider.GetComponent<SelectableObject>();
        }

        // 处理悬浮文本
        if (newHighlighted != null && hoverText != null)
        {
            hoverText.text = newHighlighted.gameObject.name; // 显示选中物体的名字
            hoverText.gameObject.SetActive(true);
            highlightedObject = newHighlighted;  // 更新当前选中的对象
        }
        else if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false); // 没有选中物体就隐藏
            highlightedObject = null;
        }

        // 处理鼠标点击
        if (Input.GetMouseButtonDown(0) && highlightedObject != null)
        {
            Debug.Log("Selected: " + highlightedObject.gameObject.name);
        }
    }

    void UpdateHoverTextPosition()
    {
        if (hoverText != null && hoverText.gameObject.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            hoverText.transform.position = mousePos + new Vector2(15f, -25f); // 让文本稍微偏移，避免遮挡鼠标
        }
    }
}
