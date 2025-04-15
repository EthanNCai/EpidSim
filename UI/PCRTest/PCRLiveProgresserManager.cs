using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;
public class PCRLiveActivityPanalManager: MonoBehaviour{

    public GameObject panal; 
    public CanvasGroup panalcanvasGroup;
    public TestManager testManager;
    public TestEvent testEventRef;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timeText;
    public Slider slider;
    // public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    void Start()
    {
        // TimeManager.OnQuarterChanged += UpdateQuarterly;
        testEventRef = this.testManager.currentTestEvent;
        panal.SetActive(false);
        TestManager.OnTestEventCreated += ShowUI;
        TestManager.OnTestEventEnd += HideUI;
        TimeManager.AfterQuarterChanged += UpdateUI;
        // 为什么要After，因为更新信息的那一帧都是在On的，所以After能够确保信息已经Uptodate了。
    }

    public void ShowUI(TestEvent testEvent){
        StartCoroutine(FadeCanvasGroup(0, 1));
        panal.SetActive(true);
        testEventRef = testEvent;
        timeText.text = $"Started at Day{testEvent.eventTimePoint.d} {testEvent.eventTimePoint.h:D2}:{testEvent.eventTimePoint.q * 15:D2}";
    }
    public void HideUI(){
        StartCoroutine(FadeCanvasGroup(1, 0));
        panal.SetActive(false);
    }

    public void UpdateUI((int day, int hour) time) {

        if(panal.activeSelf==false){return;}

        Debug.Assert(testEventRef != null, "bug here 喵！");
        
        int testedCount = testEventRef.testedSims.Count;
        int candidateCount = testEventRef.candidateSims.Count;
        int positiveCount = testEventRef.postiveSims.Count;
        int negativeCount = testEventRef.negativeSims.Count;

        // 防止除以 0 喵！
        float progress = candidateCount > 0 ? (float)testedCount / candidateCount : 0f;
        slider.value = Mathf.Min(progress, 1.0f);
        progressText.text = $"{testedCount}/{candidateCount} Tested";

        float posPercent = testedCount > 0 ? (float)positiveCount / testedCount * 100 : 0f;
        float negPercent = testedCount > 0 ? (float)negativeCount / testedCount * 100 : 0f;
        resultText.text = $"Positive {positiveCount} ({posPercent:F1}%)\nNegative {negativeCount} ({negPercent:F1}%)";

        // timeText.text = $"Started {time.day} day(s) {time.hour:D2} hour(s) ago~";
    }
    private IEnumerator FadeCanvasGroup(float start, float end)
    {
        float time = 0f;
        while (time < fadeDuration)
        {
            panalcanvasGroup.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        panalcanvasGroup.alpha = end;

        // 控制是否可交互（可选）
        panalcanvasGroup.interactable = (end == 1);
        panalcanvasGroup.blocksRaycasts = (end == 1);
    }
}