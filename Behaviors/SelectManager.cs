using UnityEngine;
using TMPro;

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
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // **检测 Sims 和 Place**
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Sims", "Place"));
        SelectableObject newHighlightedObject = hit.collider != null ? hit.collider.GetComponent<SelectableObject>() : null;

        // **初步检查是否有高亮对象**
        hoverText.gameObject.SetActive(newHighlightedObject != null);

        if (newHighlightedObject != null)
        {
            // 先检查是否是 Sims 或 Place，决定如何设置名称
            if (newHighlightedObject.GetComponent<Sims>())
            {
                Sims sims = newHighlightedObject.GetComponent<Sims>();
                hoverText.text = sims.simsName;  // 使用 Sims 的名字
            }
            else if (newHighlightedObject.GetComponent<Place>())
            {
                Place place = newHighlightedObject.GetComponent<Place>();
                Debug.Log(hoverText.text = place.placeName);
                hoverText.text = place.placeName;  // 使用 Place 的名字
            }
        }

        // **更新高亮对象**
        highlightedObject = newHighlightedObject;

        // **处理点击逻辑**
        if (Input.GetMouseButtonDown(0) && highlightedObject != null)
        {
            // 判断具体是 Sims 还是 Place
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
