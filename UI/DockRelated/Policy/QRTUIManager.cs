using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using TMPro;


public class QRTMeta{
    public enum QrtLenthType{
        Short,     // 半个潜伏期
        Long,     // 潜伏期
    }

    public static  int GetQrtLen(QrtLenthType qrtLen){
        switch(qrtLen){
            case QrtLenthType.Short:
                return 2;
            case QrtLenthType.Long:
                return 4;
            default:
                throw new NotImplementedException();
        }
    }

}

public class QRTUIManager : MonoBehaviour
{
    public QRTManager qrtManager;
    // public Button PCRTestButton;
    public TextMeshProUGUI availableQrtStatemnt;
    public TextMeshProUGUI availableQrtSeatsStatement;
    public TextMeshProUGUI queuedStatement;
    public TextMeshProUGUI citizenUnderQrtStatement;

    // GameObjects
    
    public TMP_Dropdown  qrtDurationTypeDropdown;
    public PlaceManager placeManager;
    private readonly string[] qrtLen = { "Short", "Long" };
    // public TestPolicy testPolicy;

    void Start(){
        
        qrtDurationTypeDropdown.ClearOptions();
        qrtDurationTypeDropdown.AddOptions(new System.Collections.Generic.List<string>(qrtLen));
        // 设置初始选项对应当前状态
        // lockdownLevelSelection.value = (int)lockDownManager.lockdownLevel;
        qrtDurationTypeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

    }

    public void OnDropdownValueChanged(int index){
        if (index >= 0 && index < qrtLen.Length){
            string selectedPolicy = qrtLen[index];
            System.Enum.TryParse(selectedPolicy, true, out QRTMeta.QrtLenthType qrtDurationType);
            // if (this.cprTestManager.isActivePCRTestEvent()){
            qrtManager.SetQrtDurationType(qrtDurationType);
            // }
            UpdateUIInfos();
        }
        else{
            Debug.LogWarning("喵？Dropdown 的 index 越界了，主人是不是点太快啦~");
        }
    }
    public void UpdateUIInfos()
    {
        // 首先确定是否有正在进行的test
        int qrtQueueLen = this.qrtManager.qrtQueue.Count;
        
        // 计算一下当前的
        // testInstitutionStatement.text
        int qrtSeats = placeManager.GetAvailableQuarantieCentreSeats();
        int qtrCenters = placeManager.qrtCentrePlaces.Count;
        int citizenUnderQrts = this.qrtManager.GetCitizenUnderQuarantine();

        this.citizenUnderQrtStatement.text = $"CitizenUnderQuarantines: {citizenUnderQrts}";
        this.queuedStatement.text = $"Queued Quarantine Citizens: {qrtQueueLen}";
        this.availableQrtStatemnt.text = $"Available Quarantine Centres: {qtrCenters}";
        this.availableQrtSeatsStatement.text = $"Available Quarantine Seats: {qrtSeats}";

    }
}
