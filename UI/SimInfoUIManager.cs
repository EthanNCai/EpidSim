using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class SimsInfoUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; // UI 面板（包含 ScrollView）
    public Transform scrollViewContent; // ScrollView 的 Content
    public TextMeshProUGUI nameTag;
    public TextMeshProUGUI balanceTag;
    public GameObject textPrefab; // 预制体
    public Button closeButton;

    // private List<string> diaryItemReprs = new List<string>();
    // private static readonly StringBuilder stringBuilder = new StringBuilder();
    private Sims currentSims;
    private readonly List<TextMeshProUGUI> textPool = new List<TextMeshProUGUI>(); // 复用的文本池

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
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
        ShowUI();
    }

    public void ShowUI()
    {
        UIRouter.Instance.SetActiveUI(this); // 让 UIManager 关闭其他 UI
        uiPanel.SetActive(true);
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

        Queue<string> diaryReprQueue = currentSims.simDiary.GetDiaryReprQueue();
        int diaryCount = diaryReprQueue.Count;

        // 确保文本池足够
        while (textPool.Count < diaryCount)
        {
            GameObject textObj = Instantiate(textPrefab, scrollViewContent);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            textPool.Add(tmp);
        }

        int index = 0;
        foreach (string diaryText in diaryReprQueue)
        {
            textPool[index].text = diaryText;
            textPool[index].gameObject.SetActive(true);
            index++;
        }

        for (int i = index; i < textPool.Count; i++)
        {
            textPool[i].gameObject.SetActive(false);
        }
    }
}
