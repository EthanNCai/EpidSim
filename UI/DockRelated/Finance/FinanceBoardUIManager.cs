using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class FinanceInfoUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; 
    public Button closeButton;
    public GameObject textPrefab;
    public Transform scrollViewExpenseContent;
    public Transform scrollViewContributeContent;
    public CFEManager cFEManager;
    private readonly List<TextMeshProUGUI> textPool = new List<TextMeshProUGUI>(); // 复用的文本池
    public TimeManager timeManager;
    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        // TimeManager.AfterQuarterChanged += UpdateUI;
    }
    
    // SECTION 1： LIFETIME MANAGERMENT
    public void InitUI(){
        ShowUI();
    }
    public void ShowUI()
    {
        UIRouter.Instance.SetActiveUI(this);
        uiPanel.SetActive(true);
        timeManager?.SetPaused(true); // ⏸️暂停时间！
        UpdateUI();
        // LayoutRebuilder.ForceRebuildLayoutImmediate();
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        timeManager?.SetPaused(false); // ▶️恢复时间！
    }

    // public void ShowUI(){
    //     UIRouter.Instance.SetActiveUI(this); // 让 UIManager 关闭其他 UI
    //     uiPanel.SetActive(true);
    // }
    // public void HideUI()
    // {
    //     uiPanel.SetActive(false);
    //     // currentTarget = null;
    // }

    // SECTION 2: UPDATE RELATED
    private void UpdateUI()
    {
        List<string> expenseList = cFEManager.GetExpenseStringList();
        List<string> contributeList = cFEManager.GetContributeStringList();

        int poolIndex = 0;

        // 先清空两个ScrollView的已有Text（不销毁，只是移除 parent）
        foreach (Transform child in scrollViewExpenseContent)
        {
            child.SetParent(null); // 断开 parent 关系以便复用
        }
        foreach (Transform child in scrollViewContributeContent)
        {
            child.SetParent(null);
        }

        // Helper 函数：展示一批数据到某个 content 容器里
        void ShowTextList(List<string> dataList, Transform content)
        {
            foreach (string text in dataList)
            {
                TextMeshProUGUI tmp;

                // 如果池子够用，复用
                if (poolIndex < textPool.Count)
                {
                    tmp = textPool[poolIndex];
                    tmp.gameObject.SetActive(true);
                }
                else
                {
                    // 不够用就生成新的
                    GameObject textObj = Instantiate(textPrefab, content);
                    tmp = textObj.GetComponent<TextMeshProUGUI>();
                    textPool.Add(tmp);
                }

                // 设置parent和内容
                tmp.transform.SetParent(content, false);
                tmp.text = text;
                poolIndex++;
            }
        }

        ShowTextList(expenseList, scrollViewExpenseContent);
        ShowTextList(contributeList, scrollViewContributeContent);

        // 多余的Text隐藏，不销毁
        for (int i = poolIndex; i < textPool.Count; i++)
        {
            textPool[i].gameObject.SetActive(false);
        }
    }
}
