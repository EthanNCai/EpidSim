using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlaceInfoUIManager : MonoBehaviour, IUIManager
{
    public GameObject uiPanel;
    public TextMeshProUGUI nameTag;
    public TextMeshProUGUI pleceTypeTag;
    public Button closeButton;
    private readonly List<string> diaryItemReprs = new List<string>();
    public GameObject textPrefab; // 预制体
    private Place currentPlace;

    void Start()
    {
        closeButton.onClick.AddListener(HideUI);
        HideUI();
        TimeManager.AfterQuarterChanged += UpdateInfoQuarterly;
    }
    private void UpdateInfoQuarterly((int, int) timeNow)
    {
        if (currentPlace != null)
        {
            UpdateCFEDiary();
        }
    }

    public void ShowUI()
    {
        uiPanel.SetActive(true);
        UIManager.Instance.SetActiveUI(this);
    }

    public void InitPlaceInfoUI(Place place)
    {
        if (currentPlace == place)
            return;
        // if (currentPlace == null){
        //     return;
        // }

        currentPlace = place;
        nameTag.text = $"Name: {place.placeName}";
        pleceTypeTag.text = $"Type: {Place.GetPlaceTypeDescription(currentPlace)}";
        currentPlace.placeDiary.GetDiaryEntries(diaryItemReprs);
        ShowUI();
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        currentPlace = null;
    }

    // UPDATE RELATED
    private void UpdateCFEDiary(){
        
    }

}
