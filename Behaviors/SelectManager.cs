using UnityEngine;
using TMPro;

public class SelectionManager : MonoBehaviour
{
    public TextMeshProUGUI hoverText;  // 悬浮提示文本（拖入UI组件）
    public SimsInfoUIManager simsInfoUIManager; // 关联 UI 管理脚本

    private Sims highlightedSims;
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

        // **优先检测 Sims**
        RaycastHit2D hitSims = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Sims"));
        Sims newHighlightedSims = null;
        SelectableObject newHighlightedObject = null;

        if (hitSims.collider != null)
        {
            newHighlightedSims = hitSims.collider.GetComponent<Sims>();
            newHighlightedObject = hitSims.collider.GetComponent<SelectableObject>();
        }
        else
        {
            // **如果没有检测到 Sims，再检测 Place**
            RaycastHit2D hitPlace = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Place"));
            if (hitPlace.collider != null)
            {
                newHighlightedObject = hitPlace.collider.GetComponent<SelectableObject>();
            }
        }

        // **更新悬浮文本**
        if (newHighlightedObject != null && hoverText != null)
        {
            hoverText.text = newHighlightedObject.gameObject.name; // 显示物体名称
            hoverText.gameObject.SetActive(true);
        }
        else if (hoverText != null)
        {
            hoverText.gameObject.SetActive(false); // 隐藏文本
        }

        // **更新选中对象**
        highlightedSims = newHighlightedSims;
        highlightedObject = newHighlightedObject;

        // **处理鼠标点击**
        if (Input.GetMouseButtonDown(0) && highlightedSims != null)
        {
            simsInfoUIManager.ShowSimsInfo(highlightedSims);
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
