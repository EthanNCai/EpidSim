using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class FinanceInfoUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; 
    public Button closeButton;
    private readonly List<TextMeshProUGUI> textPool = new List<TextMeshProUGUI>(); // 复用的文本池
    public TimeManager timeManager;
    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
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
    private void UpdateInfoQuarterly((int, int) timeNow)
    {
        
    }
}
