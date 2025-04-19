using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using System;

public class BuidableController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // public Image iconImage;

    // ✨ 主人拖过来的！包含 name/detail 的 UI 面板预制体实例
    // public GameObject detailPanelObject;


    private BuildableInfo buidableInfo;
    private BuildPanalUIManager buildPanalManager;
    public BuildManager buildManager;
    public TextMeshProUGUI name_;

    // private bool isDetailShowing = false; 

    // private RectTransform detailRect;
    public static event Action<BuildableInfo> OnBuildClicked;

    public void Init(BuildableInfo buidableInfo, BuildPanalUIManager buildPanalManager, BuildManager buildManager)
    {
        this.buidableInfo = buidableInfo;
        this.buildPanalManager = buildPanalManager;
        name_.text = this.buidableInfo.name;
        this.buildManager = buildManager;
        // iconImage.sprite = this.buidableInfo.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MakeDetialCardShow();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MakeDetialCardHide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnBuildClicked?.Invoke(this.buidableInfo);
    }

    private void MakeDetialCardShow(){
        // call buildboard manager, don't do this logic yourself
        Debug.Log($"made {this.buidableInfo.name} show");
        this.buildPanalManager.MakeDetialCardShow(this.buidableInfo);
        // isDetailShowing = true;
    }
    private void MakeDetialCardHide(){
        // call buildboard manager, don't do this logic yourself
        Debug.Log($"made {this.buidableInfo.name} hide");
        this.buildPanalManager.MakeDetialCardHide();
        // isDetailShowing = false;
    }
    // private void makeDetialUITagFollowMouse(){
        
    // }

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
