using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class SimsInfoUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; // UI 面板（包含 ScrollView）
    public Transform content; // ScrollView 的 Content
    public TextMeshProUGUI nameTag;
    public TextMeshProUGUI balanceTag;
    public GameObject textPrefab; // 预制体
    public Button closeButton;

    private readonly List<string> diaryItemReprs = new List<string>();
    private static readonly StringBuilder stringBuilder = new StringBuilder();
    private Sims currentSims;
    private readonly List<TextMeshProUGUI> textPool = new List<TextMeshProUGUI>(); // 复用的文本池

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
    }

    public void ShowUI()
    {
        UIManager.Instance.SetActiveUI(this); // 让 UIManager 关闭其他 UI
        uiPanel.SetActive(true);
    }

    public void InitSimUI(Sims sims)
    {
        if (currentSims == sims)
            return; // 避免重复打开相同的 Sims
        
        currentSims = sims;
        if (currentSims == null) return;
        
        // 更新名字和余额
        nameTag.text = $"名字：{currentSims.simsName}";
        UpdateBalance(currentSims.balance);

        // 获取并显示日记项
        currentSims.simDiary.GetDiaryEntries(diaryItemReprs);
        SetTextList(diaryItemReprs);
        ShowUI();
    }


    private void SetTextList(List<string> infos)
    {
        int i = 0;
        // 复用 textPool 中的 TextMeshProUGUI
        for (; i < infos.Count; i++)
        {
            if (i >= textPool.Count)
            {
                GameObject textObj = Instantiate(textPrefab, content);
                TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
                textPool.Add(tmp);
            }
            textPool[i].text = infos[i];
            textPool[i].gameObject.SetActive(true);
        }
        
        // 隐藏多余的 TextMeshProUGUI
        for (; i < textPool.Count; i++)
        {
            textPool[i].gameObject.SetActive(false);
        }
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        currentSims = null;
    }


    // SECTION 3: UPDATE RELATED

    private void UpdateInfoQuarterly((int, int) timeNow)
    {
        if (currentSims != null)
        {
            UpdateBalance(currentSims.balance);
            UpdateSimDiary();
        }
    }

    private void UpdateBalance(float balance)
    {
        balanceTag.text = $"存款：{balance} $";
    }

    private void UpdateSimDiary()
    {
        if (currentSims == null) return;
        
        currentSims.simDiary.GetDiaryEntries(diaryItemReprs);
        SetTextList(diaryItemReprs);
    }
}
