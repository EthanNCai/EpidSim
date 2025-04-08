using UnityEngine;


public interface IUIManager
{
    void ShowUI(); 
    // show ui 的用途： setActive， 然后通知UI Router 关掉其他UI
    void HideUI();
    // hide ui 的同于： setInActive，卸载当前的关注对象（如有）， 但是不通知UIRouter，UIRouter在等待到下一个
}

public class UIRouter : MonoBehaviour
{
    public static UIRouter Instance { get; private set; }

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

