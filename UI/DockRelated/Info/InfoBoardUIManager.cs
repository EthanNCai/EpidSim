using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InfoBoardUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; 
    public Button closeButton;
    public Transform scrollViewContent; // ScrollView 的 Content
    public GameObject textPrefab;

    public SimsDeadManager simsDeadManager;
    public InfoManager infoManager;
    public VirusManager virusManager;
    public TimeManager timeManager;

    private readonly List<GameObject> textPool = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        // TimeManager.AfterQuarterChanged += UpdateUIInfo;
    }
    
    public void InitUI()
    {
        ShowUI();
    }

    public void ShowUI()
    {
        UIRouter.Instance.SetActiveUI(this); // 让 UIManager 关闭其他 UI
        uiPanel.SetActive(true);
        UpdateUIInfo();
        timeManager?.SetPaused(true); 
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        timeManager?.SetPaused(false);
    }

    // SECTION 3: UPDATE RELATED
    private void UpdateUIInfo()
    {
        Debug.Log("Updating UI");

        // 准备要显示的所有信息喵～
        List<string> statements = new List<string>
        {
            $"Current citizen dead: {simsDeadManager.GetCurrentDeadCount()}",
            $"Virus Status: Infectiousness: {virusManager.GetVirusInffectiousness() * 100}%",
            $"Severity: {virusManager.GetVirusSeverity() * 100}%",
            $"Lethality: {virusManager.GetVirusLethality() * 100}%",
            "Hospitals Occupied: 13%",
            "Test Centre Occupied: 12%",
            "Public Mood: Cautiously Optimistic",
            "Time Elapsed: 42 days"
        };

        // 保证对象池够用，不够就多造几个小猫咪文本~
        while (textPool.Count < statements.Count)
        {
            GameObject textObj = Instantiate(textPrefab, scrollViewContent);
            // GameObject textObj = Instantiate(textPrefab, scrollViewContent);
            // TextMeshProUGUI tmp = textPool[i].GetComponent<TextMeshProUGUI>();
            textObj.SetActive(false);
            textPool.Add(textObj);
        }

        // 更新每个文本内容
        for (int i = 0; i < textPool.Count; i++)
        {
            if (i < statements.Count)
            {
                textPool[i].SetActive(true);
                TextMeshProUGUI tmp = textPool[i].GetComponent<TextMeshProUGUI>();
                tmp.text = statements[i];
            }
            else
            {
                textPool[i].SetActive(false); // 多余的就喵喵地藏起来~
            }
        }
    }
}
