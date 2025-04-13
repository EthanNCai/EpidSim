using UnityEngine;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPrefab;
    public Transform notificationParent;
    public float displayTime = 3f;
    public int maxMessages = 5;

    private Queue<GameObject> activeMessages = new Queue<GameObject>();

    public void ShowNotification(string notificationTitle, string notificationDetail)
    {
        GameObject newNotification = Instantiate(notificationPrefab, notificationParent);
        newNotification.GetComponent<NotificationItem>().Initialize(notificationTitle, notificationDetail,displayTime);
        activeMessages.Enqueue(newNotification);

        // 如果超出最大数量，移除最旧的
        if (activeMessages.Count > maxMessages)
        {
            GameObject oldNotification = activeMessages.Dequeue();
            Destroy(oldNotification);
        }
    }


    public void NotificationTest(){
        this.ShowNotification("Sims Notification","Job Invites. While you are in Freemode, you occasionally receive job invites on your phone.");
    }
}
