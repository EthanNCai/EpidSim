using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public TextMeshProUGUI hoverText;  // 悬浮提示文本（拖入UI组件）
    public SimsInfoUIManager simsInfoUIManager; // 关联 UI 管理脚本
    public PlaceInfoUIManager placeInfoUIManager; // 关联 Place UI 管理脚本
    // private Sims highlightedSims;
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

        if (EventSystem.current.IsPointerOverGameObject())
        {
            hoverText.gameObject.SetActive(false);  // 隐藏文本
            highlightedObject = null;               // 清除高亮
            return;                                 // 不再继续检测
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // 获取所有碰撞的对象
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero, Mathf.Infinity, 
                                                LayerMask.GetMask("Sims", "Place"));
        
        // 默认为 null
        SelectableObject newHighlightedObject = null;
        
        // 优先查找 Sims 对象
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.GetComponent<Sims>() != null)
            {
                newHighlightedObject = hit.collider.GetComponent<SelectableObject>();
                break; // 找到 Sims 后立即退出循环
            }
        }
        
        // 如果没有找到 Sims 对象，再查找 Place 对象
        if (newHighlightedObject == null)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider.GetComponent<Place>() != null)
                {
                    newHighlightedObject = hit.collider.GetComponent<SelectableObject>();
                    break;
                }
            }
        }

        // 设置悬浮文本的可见性
        hoverText.gameObject.SetActive(newHighlightedObject != null);

        // 设置悬浮文本内容
        if (newHighlightedObject != null)
        {
            if (newHighlightedObject.GetComponent<Sims>())
            {
                Sims sims = newHighlightedObject.GetComponent<Sims>();
                hoverText.text = sims.simsName;
            }
            else if (newHighlightedObject.GetComponent<Place>())
            {
                Place place = newHighlightedObject.GetComponent<Place>();
                hoverText.text = place.placeName;
            }
        }

        // 更新高亮对象
        highlightedObject = newHighlightedObject;

        // 处理点击逻辑
        if (Input.GetMouseButtonDown(0) && highlightedObject != null  && !EventSystem.current.IsPointerOverGameObject())
        {
            if (highlightedObject.GetComponent<Sims>())
            {
                Sims sims = highlightedObject.GetComponent<Sims>();
                simsInfoUIManager.InitSimUI(sims);
            }
            else if (highlightedObject.GetComponent<Place>())
            {
                Place place = highlightedObject.GetComponent<Place>();
                placeInfoUIManager.InitPlaceInfoUI(place);
            }
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
