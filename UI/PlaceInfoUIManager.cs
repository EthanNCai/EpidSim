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
    public Button lockDownButton;
    public Transform scrollViewContent; // ScrollView 的 Content
    public GameObject textPrefab; // 预制体
    
    private List<string> diaryItemReprs = new List<string>();
    private Place currentPlace;
    private readonly List<TextMeshProUGUI> textPool = new List<TextMeshProUGUI>(); // 复用的文本池

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

        currentPlace = place;
        lockDownButton.interactable = currentPlace is ILockDownable;
        nameTag.text = $"Name: {place.placeName}";
        pleceTypeTag.text = $"Type: {Place.GetPlaceTypeDescription(currentPlace)}";
        ShowUI();
        UpdateCFEDiary(); // 初始化时更新一次
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        currentPlace = null;
    }

    private void UpdateCFEDiary()
    {
        if (currentPlace == null) return;

        Queue<string> diaryReprQueue = currentPlace.placeDiary.GetDiaryReprQueue();
        int diaryCount = diaryReprQueue.Count;

        // 确保文本池足够
        while (textPool.Count < diaryCount)
        {
            GameObject textObj = Instantiate(textPrefab, scrollViewContent);
            TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
            textPool.Add(tmp);
        }

        int index = 0;
        foreach (string diaryText in diaryReprQueue)
        {
            textPool[index].text = diaryText;
            textPool[index].gameObject.SetActive(true);
            index++;
        }

        for (int i = index; i < textPool.Count; i++)
        {
            textPool[i].gameObject.SetActive(false);
        }
    }
}
