using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildPanalUIManager : MonoBehaviour, IUIManager
{

    private bool isShowingDetial = false;
    public GameObject uiPanel;
    public Button closeButton;
    public GameObject detailCardObject;
    private TextMeshProUGUI detailCardName;
    private TextMeshProUGUI detailCardDescription;
    public GameObject buidableIconPrefab;
    public Transform buildableIconRootTransform;

    public Canvas rootCanvas;
    // public Canvas buidingPanalUI;

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        // TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
        detailCardObject.SetActive(false);
        if (detailCardObject != null)
        {
            // 在Start中获取子物体里的Text
            detailCardName = detailCardObject.transform.Find("backgroundImg/name").GetComponent<TextMeshProUGUI>();
            detailCardDescription = detailCardObject.transform.Find("backgroundImg/detail").GetComponent<TextMeshProUGUI>();
            detailCardObject.SetActive(false); // 默认隐藏
        }
        else
        {
            Debug.LogWarning("喵呜～你忘记给 detailPanelPrefab 赋值了喵！");
        }
        InitBuildableIconsUI();
    }

    public void InitUI()
    {
        ShowUI();

    }

    public void ShowUI()
    {
        UIRouter.Instance.SetActiveUI(this);
        uiPanel.SetActive(true);
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
    }

    // private void UpdateInfoQuarterly((int, int) timeNow)
    // {
        
    // }

    public void AttemptMakeDetialFollowMouse()
    {
        if (isShowingDetial && detailCardObject != null)
        {
            // Vector2 mousePosition = Input.mousePosition;

            // // 将屏幕坐标转为UI世界坐标
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //     rootCanvas.transform as RectTransform, 
            //     mousePosition, 
            //     rootCanvas.worldCamera, 
            //     out Vector2 localPoint);

            // // 设置 detailCard 的位置，稍微偏移防止遮挡鼠标
            // detailCardObject.GetComponent<RectTransform>().anchoredPosition = localPoint + new Vector2(20f, -20f);
        }
    }




    public void MakeDetialCardShow(BuildableInfo buidableInfo)
    {
        isShowingDetial = true;
        detailCardName.text = buidableInfo.name;
        detailCardDescription.text = buidableInfo.description;
        detailCardObject.SetActive(true);
    }

    public void MakeDetialCardHide()
    {
        isShowingDetial = false;
        detailCardObject.SetActive(false);
    }
    public void InitBuildableIconsUI()
    {
        List<BuildableInfo> buildableList = new List<BuildableInfo>{ 
            new BuildableInfo("Clinic", "A place where sims can get medical treatment", null), 
            new BuildableInfo("TestCentre", "A place where sims can get virus CPR test", null)};
        foreach (var buildable in buildableList)
        {
            Debug.Log("Initializing");
            // 一个 Buidable 图标需要知道其的基本信息 ： name\detail\ + buidableManager（需要通过它来管理detail）
            GameObject go = Instantiate(buidableIconPrefab, buildableIconRootTransform);
            BuidableController item = go.GetComponent<BuidableController>();
            item.Init(buildable, this);
        }
    }

    public void Update()
    {
        AttemptMakeDetialFollowMouse();
    }
}
