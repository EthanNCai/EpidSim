using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public enum PolicyTabs {
    None,
    Lockdown,
    CPRTest
}

public class PolicyPanalUIManager : MonoBehaviour, IUIManager {
    public Button closeButton;
    public GameObject lockdownUI;
    public GameObject cprTestUI;
    private GameObject activeUI;


    public Button lockdownButton;
    public Button cprTestButton;
    private Button activeButton;
    public GameObject uiPanal;

    public TimeManager timeManager;

    private PolicyTabs selectedTab = PolicyTabs.None;

    // Sub UI Manager references
    public LockdownUIManager lockdownUIManager;

    void Start() {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
    }

    public void SwitchPage(string tabName){
        // 乖乖尝试把字符串转成枚举
        if (!Enum.TryParse(tabName, true, out PolicyTabs selectedTab_in)) {
            Debug.LogWarning($"喵呜~未知的 tab 名称：{tabName}，我迷路了啦~ 🐾");
            return;
        }

        // 如果是重复 tab，就不用切啦
        // if(this.selectedTab == selectedTab_in){
        //     return;
        // }

        Debug.Log($"Switched page to {selectedTab_in}");

        // 移除旧的
        activeUI?.SetActive(false);
        if(activeButton != null) activeButton.interactable = true;

        // 记录新 tab
        selectedTab = selectedTab_in;
        switch(selectedTab){
            case PolicyTabs.Lockdown:
                activeUI = lockdownUI;
                activeButton = lockdownButton;
                activeUI.SetActive(true);
                activeButton.interactable = false;
                lockdownUIManager.UpdateLockdownInfos();
                break;
            case PolicyTabs.CPRTest:
                activeUI = cprTestUI;
                activeButton = cprTestButton;activeUI.SetActive(true);
                activeButton.interactable = false;
                break;
            default:
                Debug.LogWarning("喵喵喵？未知 tab 被选中！");
                return;
        }

        // 启用新的
        Debug.Assert(activeUI != null && activeButton != null, "Bug here, active UI or button is null");
        
    }

    public void InitUI()
    {
        ShowUI();
    }
    public void ShowUI() { 
        UIRouter.Instance.SetActiveUI(this);
        this.uiPanal.SetActive(true);
        SwitchPage("Lockdown");
        timeManager?.SetPaused(true);
    }
    public void HideUI() { 
        this.uiPanal.SetActive(false);
        timeManager?.SetPaused(false);
    }
    private void UpdateInfoQuarterly((int, int) timeNow){}
}
