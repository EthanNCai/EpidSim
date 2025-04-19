using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

public class CPRTestUIManager : MonoBehaviour
{
    public TestManager cprTestManager;

    public Button PCRTestButton;

    public TextMeshProUGUI promptStatement;

    public TextMeshProUGUI testCentreStatement;
    public TextMeshProUGUI testSeatsStatement;
    // private LockdownStatus currentLockdownStatus;

    // GameObjects
    public TMP_Dropdown  lockdownLevelSelection;
    public PlaceManager placeManager;
    private readonly string[] testPolicies = { "Soft", "Hard" };

    public TestPolicy testPolicy;

    void Start(){
        
        lockdownLevelSelection.ClearOptions();
        lockdownLevelSelection.AddOptions(new System.Collections.Generic.List<string>(testPolicies));

        // 设置初始选项对应当前状态
        // lockdownLevelSelection.value = (int)lockDownManager.lockdownLevel;
        lockdownLevelSelection.onValueChanged.AddListener(OnDropdownValueChanged);
        testPolicy = TestPolicy.Soft;

    }

    public void OnDropdownValueChanged(int index){
        if (index >= 0 && index < testPolicies.Length){
            string selectedPolicy = testPolicies[index];
            System.Enum.TryParse(selectedPolicy, true, out TestPolicy parsedTestPolicy);
            this.testPolicy = parsedTestPolicy;
            if (this.cprTestManager.isActivePCRTestEvent()){
                cprTestManager.SwitchTestPolicy(this.testPolicy);
            }
            UpdateUIInfos();
        }
        else{
            Debug.LogWarning("喵？Dropdown 的 index 越界了，主人是不是点太快啦~");
        }
    }
    public void UpdateUIInfos()
    {
        // 首先确定是否有正在进行的test
        if(this.cprTestManager.isActivePCRTestEvent()){
            // 禁用开始的按钮
            PCRTestButton.interactable = false;
            promptStatement.text = "A Test is unfinished, you cannot start a new one";
            testPolicy = cprTestManager.currentTestEvent.testPolicy;
        }else{
            promptStatement.text = "Start a new PCR test?";
        }
        // 计算一下当前的
        // testInstitutionStatement.text
        int testSeats = placeManager.GetAvailableTestCentreSeats();
        int testCenters = placeManager.testCenterPlaces.Count;
        this.testCentreStatement.text = $"Available Test Centres: {testCenters}";
        this.testSeatsStatement.text = $"Available Test Seats: {testSeats}";

    }
    public void HandleGlobalTestButtonPress(){
        if(!this.cprTestManager.isActivePCRTestEvent()){
            this.cprTestManager.CreateTestEvent(this.testPolicy);
        }else{
            // this.cprTestManager.GlobalLockDown();
        }
        UpdateUIInfos();
    }
    
    
}
