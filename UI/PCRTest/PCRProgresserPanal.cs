using System.Runtime.InteropServices;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;
public class PCRLiveActivityPanalManager: MonoBehaviour{

    public GameObject panal; 
    public float progress;
    public TestEvent currentTestEvent;
    public TestManager testManager;
    public Slider slider;

    void Start()
    {
        TimeManager.OnQuarterChanged += UpdateQuarterly;
    }

    public void UpdateQuarterly((int,int)time){
        slider.value = 0.66f;
    }

    private string GetTestResultRepr(){
        return "";
    }
    private string GetTestTimeRepr(){
        return "";
    }
    private string GetProgressRepr(){
        return "";
    }
}