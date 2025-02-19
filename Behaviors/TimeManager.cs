using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static event Action<int> OnDayChanged;  // 事件：天数变化 (day, hour)
    public static event Action<(int, int)> OnQuarterChanged; // 事件：季度变化 (hour, quarter)

    public int day = 0;
    public int hour = 0;
    public int quarter = 0; // 1 小时 = 4 个 quarter，每个 quarter = 15 分钟

    public float speed = 1.0f; // 时间流逝速度，1.0f = 1秒推进1个quarter
    private float timeAccumulator = 0f; // 记录时间

    private void Update()
    {
        timeAccumulator += Time.deltaTime * speed;
        if (timeAccumulator >= 1f)
        {
            timeAccumulator -= 1f;
            AdvanceTime();
        }
    }

    private void AdvanceTime()
    {
        OnQuarterChanged?.Invoke((hour, quarter));
        quarter++; 
        if (quarter >= 4)
        {
            quarter = 0;
            hour++;
            if (hour >= 24)
            {
                hour = 0;
                day++;
                OnDayChanged?.Invoke(day);
            }
        }

        Debug.Log($"当前时间：第 {day} 天 {hour:D2}:{quarter * 15:D2}");
    }
}
