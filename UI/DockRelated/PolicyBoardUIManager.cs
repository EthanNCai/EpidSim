using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class PolicyUIBarManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel; 
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
    }
    
    public void InitSimUI()
    {
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
    }


    // SECTION 3: UPDATE RELATED
    private void UpdateInfoQuarterly((int, int) timeNow)
    {

    }
}
