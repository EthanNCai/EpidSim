using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildPanalUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel;
    public Button closeButton;

    // ✨ 将整个详情面板作为预制体拖进来
    public GameObject buidableDetailTagObject;

    // 🐾 内部缓存文本组件
    private TextMeshProUGUI detailNameText;
    private TextMeshProUGUI detailDescriptionText;
    public GameObject buidableIconPrefab;
    public Transform buildableIconRootTransform;

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;

        if (buidableDetailTagObject != null)
        {
            // 在Start中获取子物体里的Text
            detailNameText = buidableDetailTagObject.transform.Find("backgroundImg/name").GetComponent<TextMeshProUGUI>();
            detailDescriptionText = buidableDetailTagObject.transform.Find("backgroundImg/detail").GetComponent<TextMeshProUGUI>();
            buidableDetailTagObject.SetActive(false); // 默认隐藏
        }
        else
        {
            Debug.LogWarning("喵呜～你忘记给 detailPanelPrefab 赋值了喵！");
        }
    }

    public void InitUI()
    {
        ShowUI();
        PopulateBuildableUI();

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

    // 🕓 更新逻辑留空
    private void UpdateInfoQuarterly((int, int) timeNow)
    {

    }

    // ✨ 详细展示逻辑
    public void ShowBuildableDetail(BuildableInfo data)
    {
        if (buidableDetailTagObject != null)
        {
            buidableDetailTagObject.SetActive(true);
            detailNameText.text = data.name;
            detailDescriptionText.text = data.description;
        }
    }

    public void HideBuildableDetail()
    {
        if (buidableDetailTagObject != null)
        {
            buidableDetailTagObject.SetActive(false);
        }
    }
    public void PopulateBuildableUI()
    {
        List<BuildableInfo> buildableList = new List<BuildableInfo>{ new BuildableInfo("小房子", "可以收容市民的基础建筑喵～", null), new BuildableInfo("第二个小房子", "可以收容市民的基础建筑喵～", null)};
        foreach (var buildable in buildableList)
        {
            Debug.Log("Initializing");
            // 一个 Buidable 图标需要知道其的基本信息 ： name\detail\ + buidableManager（需要通过它来管理detail）
            GameObject go = Instantiate(buidableIconPrefab, buildableIconRootTransform);
            BuidableController item = go.GetComponent<BuidableController>();
            item.Init(buildable, this);
        }
    }
}
