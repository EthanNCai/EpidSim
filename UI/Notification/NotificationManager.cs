using UnityEngine;
using System.Collections.Generic;
using System;

public enum NotificationType
{
    MedicalRelated,   // red
    GovernmentAction,
    FinanceRelated,     // orange
    CommentRelated      // blue
}

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform notificationParent;
    // public float displayTime = 3f;
    public int maxMessages = 5;
    private Dictionary<string, float> recentNotifications = new Dictionary<string, float>();
    private float duplicateCooldown = 10f; // 冷却时间 10 秒


    private Queue<GameObject> activeMessages = new Queue<GameObject>();

    public void ShowNotification(string notificationTitle, string notificationDetail, NotificationType type, float delay = 0f)
    {
        CleanupOldNotifications(); // 加这一行喵！
        string key = notificationTitle;

        if (recentNotifications.TryGetValue(key, out float lastTime))
        {
            if (Time.time - lastTime < duplicateCooldown)
            {
                Debug.Log($"[NotificationManager] Skipped duplicate notification: {key}");
                return; // 冷却中，不显示
            }
        }

        recentNotifications[key] = Time.time;
        StartCoroutine(DelayedShow(notificationTitle, notificationDetail, type, delay));
    }

    private IEnumerator<WaitForSeconds> DelayedShow(string title, string detail, NotificationType type, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
        Color bgColor = GetColorByNotificationType(type);

        var item = newNotification.GetComponent<NotificationItem>();

        // 🧠 计算动态显示时间（基础3秒 + 每10字加0.5秒，最多10秒）
        // 🧠 计算动态显示时间（基础5秒 + 每10字加0.5秒，最多20秒）
        // 🧠 计算动态显示时间（基础5秒 + 每10字加0.5秒，最多20秒）
        int totalLength = (title?.Length ?? 0) + (detail?.Length ?? 0);
        float extraTime = Mathf.Min(20f, 5f + (totalLength / 10f) * 0.5f);



        item.Initialize(title, detail, extraTime, bgColor);

        activeMessages.Enqueue(newNotification);

        if (activeMessages.Count > maxMessages)
        {
            GameObject oldNotification = activeMessages.Dequeue();
            Destroy(oldNotification);
        }
    }


    public Color GetColorByNotificationType(NotificationType type)
    {
        switch (type)
        {
            case NotificationType.MedicalRelated:
                return new Color(1f, 0f, 0f, 0.3f); // 红色半透明
            case NotificationType.FinanceRelated:
                return new Color(1f, 0.5f, 0f, 0.3f); // 橙色半透明
            case NotificationType.CommentRelated:
                return new Color(0f, 0.5f, 1f, 0.3f); // 蓝色半透明
            case NotificationType.GovernmentAction:
                return new Color(0.59f, 0.29f, 0.0f, 0.3f); // 棕褐色
            default:
                return new Color(1f, 1f, 1f, 0.3f); // 默认白
        }
    }
    
    private void CleanupOldNotifications()
    {
        float currentTime = Time.time;
        List<string> keysToRemove = new List<string>();

        foreach (var pair in recentNotifications)
        {
            if (currentTime - pair.Value >= duplicateCooldown)
            {
                keysToRemove.Add(pair.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            recentNotifications.Remove(key);
        }
    }

    //首例死亡病例信息

    public void SendFirstDeadCaseNotification(Sims targetsim){
        NotificationType type = NotificationType.MedicalRelated;
        string title = "Frist Death Case";
        string content = $"{targetsim.simsName}, a citizen who lived in {targetsim.home.placeDiary}, passed away due to a recent infectious disease. This caused some panic. Maybe the government should do something";
        // Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");
        ShowNotification(title, content, type, 1f);
    }

    // 医院满载消息

    private static List<string> hospitalFullStatements = new List<string>(){
        "Some citizens wish to go to the hospital, but there are no available beds in any hospitals.",
        "Hospitals are overloaded and unable to accommodate new patients",
        "Because the hospital was full, a sick citizen had to stay at home"
    }; 
    public void SendHospitalFullNotification(){
        NotificationType type = NotificationType.MedicalRelated;
        string title = "Hospital Capacity Insufficient";
        string content = RandomManager.Choice(hospitalFullStatements);
        // Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");
        ShowNotification(title, content, type, 1f);
    }

    // 和隔离有关的东西
    public void SendPartialLockdownNotification(Place targetPlace){
        NotificationType type = NotificationType.GovernmentAction;
        string title = "Partial Lockdown Started";
        string content =  $"{targetPlace.placeName} have been put into lockdown, and these citizens are unable to go to work, perhaps to reduce the spread of the plague? But who knows";
        // Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");
        ShowNotification(title, content, type, 1f);
    }

    public void SendCancelPartialLockdownNotification(Place targetPlace){
        NotificationType type = NotificationType.GovernmentAction;
        string title = "Partial Canceled";
        string content = $"{targetPlace.placeName} has ended the lockdown of infectious diseases, people can go out for fun, or work again";
        // Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");
        ShowNotification(title, content, type, 1f);
    }

    private static List<string> globalLockdownStatements = new List<string>(){
        "Some neighborhoods have been put into lockdown, and these citizens are unable to go to work, perhaps to reduce the spread of the plague? But who knows",
    }; 
    public void SendGlobalLockdownNotification(){
        NotificationType type = NotificationType.GovernmentAction;
        string title = "Government Action";
        string content = RandomManager.Choice(globalLockdownStatements);
        // Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");
        ShowNotification(title, content, type, 1f);
    }

    // 快捷测试方法
    public void NotificationTest()
    {
        List<NotificationType> types = new List<NotificationType>
        {
            NotificationType.CommentRelated,
            NotificationType.FinanceRelated,
            NotificationType.MedicalRelated
        };

        List<string> titles = new List<string>
        {
            "Citizen Alert",
            // "Economic Update",
            // "New Message",
            // "Breaking News",
            // "Health Advisory"
        };

        List<string> contents = new List<string>
        {
            "A new outbreak has been detected in the downtown area.",
            // "Your tax income has increased by 12% this month.",
            // "Someone left a comment on your city policies.",
            // "Authorities have issued a new infection control guideline.",
            // "Investment in public transport is paying off!"
        };

        NotificationType type = RandomManager.Choice(types);
        string title = RandomManager.Choice(titles);
        string content = RandomManager.Choice(contents);

        Debug.Log($"[NotificationTest] Showing: [{type}] {title} - {content}");

        ShowNotification(title, content, type, 1f);
    }
}
