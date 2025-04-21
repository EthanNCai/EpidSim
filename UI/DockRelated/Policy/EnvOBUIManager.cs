using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;

public class EnvOBMeta{
    public enum EnvOBStatus{
        No,
        Yes,
    }

    public static bool GetEnvOBValue(EnvOBStatus envObOps){
        switch(envObOps){
            case EnvOBStatus.No:
                return false;
            case EnvOBStatus.Yes:
                return true;
            default:
                throw new NotImplementedException();
        }
    }

}

public class EnvOBUIManager : MonoBehaviour
{
    // public QRTManager qrtManager;
    // public Button PCRTestButton;
    public TMP_Dropdown  envObOptionsDropdown;
    // public PlaceManager placeManager;
    public VirusVolumeGridMapManager virusVolumeGridMapManager;
    private readonly string[] qrtLen = { "No", "Yes" };
    // public TestPolicy testPolicy;

    void Start(){
        envObOptionsDropdown.ClearOptions();
        envObOptionsDropdown.AddOptions(new System.Collections.Generic.List<string>(qrtLen));
        envObOptionsDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void OnDropdownValueChanged(int index){
        if (index >= 0 && index < qrtLen.Length){
            string selectedEnvObStatus = qrtLen[index];
            System.Enum.TryParse(selectedEnvObStatus, true, out EnvOBMeta.EnvOBStatus newEnvStatus);
            this.virusVolumeGridMapManager.SwitchEnvOb(newEnvStatus);
            UpdateUIInfos();
        }
        else{
            Debug.LogWarning("喵？Dropdown 的 index 越界了，主人是不是点太快啦~");
        }
    }
    public void UpdateUIInfos()
    {
        // 获取当前的状态
        var currentStatus = this.virusVolumeGridMapManager.GetsEnvObStatus(); // 你要保证这个函数返回 EnvOBMeta.EnvOBStatus 类型

        // 转换为 dropdown 中对应的 index
        int dropdownIndex = System.Array.IndexOf(Enum.GetNames(typeof(EnvOBMeta.EnvOBStatus)), currentStatus.ToString());

        if (dropdownIndex >= 0 && dropdownIndex < envObOptionsDropdown.options.Count)
        {
            envObOptionsDropdown.SetValueWithoutNotify(dropdownIndex); // 不触发 onValueChanged 回调
        }
        else
        {
            Debug.LogWarning("呜呜，当前状态没有对应的 dropdown 项，哥哥你是不是又写错代码了？");
        }
    }
}
