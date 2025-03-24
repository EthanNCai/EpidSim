using UnityEngine;


public interface IUIManager
{
    void ShowUI();
    void HideUI();
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private IUIManager activeUIManager;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetActiveUI(IUIManager newUIManager)
    {
        if (activeUIManager == newUIManager)
            return;

        activeUIManager?.HideUI(); // 关闭之前的 UI
        activeUIManager = newUIManager;
    }
}

