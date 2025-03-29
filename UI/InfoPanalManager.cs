using JetBrains.Annotations;
using TMPro;
using UnityEngine;

class InfoPanalUIManager: MonoBehaviour{

    public TextMeshProUGUI dayTimeTag;
    public TextMeshProUGUI cashflowTag;   
    public InfoManager infoManager;
    
    public void Awake(){
        TimeManager.OnQuarterChanged += QUpdateInfo;
    }
    public void QUpdateInfo((int, int) _)
    {
        (int d, int h, int q) = infoManager.timeManager.GetTime();
        // 计算分钟数，每个 quarter 是 15 分钟
        int minutes = q * 15;
        // 格式化字符串
        this.dayTimeTag.text = $"Day {d}, {h:D2}:{minutes:D2}";
        this.cashflowTag.text = $"{infoManager.cfeManager.cashFlow.ToString()}HK$";
    }
}