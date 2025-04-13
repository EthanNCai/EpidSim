using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

public class LockdownUIManager : MonoBehaviour
{
    public LockDownManager lockDownManager;
    public TextMeshProUGUI lockdownSatusStatement;
    public TextMeshProUGUI residentialBuildingAffectedStatement;
    public TextMeshProUGUI citizenAffectedStatement;
    public TextMeshProUGUI lockdownButtonText;
    private LockdownStatus currentLockdownStatus;

    // GameObjects
    public Transform scrollViewContent;
    public TMP_Dropdown  lockdownLevelSelection;

    // 
    public GameObject lockedPlaceItemPrefab; // 请在 Inspector 中绑定带有名字子物体的 prefab（类似 textPrefab）

    private readonly List<GameObject> itemPool = new(); 

    private readonly string[] levelNames = { "Soft", "Hard" };

    void Start()
    {
        lockdownLevelSelection.ClearOptions();
        lockdownLevelSelection.AddOptions(new System.Collections.Generic.List<string>(levelNames));

        // 设置初始选项对应当前状态
        // lockdownLevelSelection.value = (int)lockDownManager.lockdownLevel;
        lockdownLevelSelection.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void OnDropdownValueChanged(int index)
    {
        if (index >= 0 && index < levelNames.Length)
        {
            string selectedLevel = levelNames[index];
            lockDownManager.SwitchLockdownLevel(selectedLevel);
            UpdateLockdownInfos();
        }
        else
        {
            Debug.LogWarning("喵？Dropdown 的 index 越界了，主人是不是点太快啦~");
        }
    }

    public void UpdateLockdownInfos()
    {
        // Debug.Log("Updating Lockdown Info");
        // get 文字描述
        string lockdownStatus = lockDownManager.GetLockdownStatus();
        int affectedBuildings = lockDownManager.GetNumberOfAffectedResidentialBuildings();
        int affectedCitizens = lockDownManager.GetNumberOfAffectedCitizens();

        lockdownSatusStatement.text = $"Current Lockdown Status: <b>{lockdownStatus}</b>";
        residentialBuildingAffectedStatement.text = $"Residential Buildings Affected: <b>{affectedBuildings}</b>";
        citizenAffectedStatement.text = $"Citizens Affected: <b>{affectedCitizens}</b>";
        lockdownLevelSelection.value = (int)lockDownManager.lockdownLevel;
        lockdownButtonText.text =  lockdownStatus != "Global Lockdown" ? "Start Global Lockdown" : "Stop Global Lockdown";
        currentLockdownStatus =  lockdownStatus == "Global Lockdown" ? LockdownStatus.GlobalLockdown : LockdownStatus.NonGlobalLockdown;
        // get locked 列表
        List<string> lockedNames = lockDownManager.GetLockedNames();

        // 确保对象池足够
        while (itemPool.Count < lockedNames.Count)
        {
            GameObject item = Instantiate(lockedPlaceItemPrefab, scrollViewContent);
            itemPool.Add(item);
        }

        // 更新每个 item 的 name 显示
        for (int i = 0; i < lockedNames.Count; i++)
        {
            GameObject item = itemPool[i];
            item.SetActive(true);

            // 找到子物体 "name" 并更新文字
            Transform nameTransform = item.transform.Find("name");
            if (nameTransform != null)
            {
                TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = lockedNames[i];
                }
                else
                {
                    Debug.LogWarning("喵呜！找到了 name 但上面居然没挂 TextMeshProUGUI！");
                }
            }
            else
            {
                Debug.LogWarning("喵呜？这个 prefab 没有叫 'name' 的子物体哦～");
            }
        }

        // 多余的 item 隐藏掉
        for (int i = lockedNames.Count; i < itemPool.Count; i++)
        {
            itemPool[i].SetActive(false);
        }

    }
    public void HandleGlobalLockdownButtonPress(){
        if(this.currentLockdownStatus == LockdownStatus.GlobalLockdown){
            this.lockDownManager.GlobalLockdownRelease();
        }else{
            this.lockDownManager.GlobalLockDown();
        }
        UpdateLockdownInfos();
    }
    
}
