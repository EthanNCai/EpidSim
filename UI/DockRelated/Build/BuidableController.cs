using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class BuidableController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // public Image iconImage;

    // ✨ 主人拖过来的！包含 name/detail 的 UI 面板预制体实例
    // public GameObject detailPanelObject;
    private BuildableInfo buidableInfo;
    private BuildPanalUIManager buildPanalManager;

    // private RectTransform detailRect;

    public void Init(BuildableInfo buidableInfo, BuildPanalUIManager buildPanalManager)
    {
        this.buidableInfo = buidableInfo;
        this.buildPanalManager = buildPanalManager;
        // iconImage.sprite = this.buidableInfo.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        makeDetialUITagShow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // detailPanelObject.SetActive(false);
        makeDetialUITagHide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"喵呜！你点击了 {buidableInfo.name}！");
    }

    private void makeDetialUITagShow(){
        // call buildboard manager, don't do this logic yourself
        Debug.Log($"made {this.buidableInfo.name} show");
    }
    private void makeDetialUITagHide(){
        // call buildboard manager, don't do this logic yourself
        Debug.Log($"made {this.buidableInfo.name} hide");
    }
    private void makeDetialUITagFollowMouse(){
        
    }

    // private void UpdatePanelPosition()
    // {
    //     Vector2 mousePosition = Input.mousePosition;
    //     Vector2 anchoredPos;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         manager.GetComponentInParent<Canvas>().transform as RectTransform,
    //         mousePosition,
    //         manager.GetComponentInParent<Canvas>().worldCamera,
    //         out anchoredPos
    //     );

    //     detailRect.anchoredPosition = anchoredPos;

    //     // 判断鼠标在屏幕上半部分还是下半部分
    //     bool isInUpperHalf = mousePosition.y > (Screen.height / 2f);

    //     float offsetY = isInUpperHalf ? -80f : 80f;
    //     detailRect.anchoredPosition += new Vector2(0f, offsetY);
    // }
}
