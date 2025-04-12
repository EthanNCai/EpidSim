using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public enum KeyTime
{
    DayChanged,
    Morning,
    Noon,
    Dusk,
    Night,
    Random
}

public class TimeManager : MonoBehaviour
{
    public static event Action<int> OnDayChanged;
    public static event Action<(int, int)> OnQuarterChanged;
    public static event Action<(int, int)> AfterQuarterChanged;
    public static event Action<KeyTime> OnKeyTimeChanged;

    public int day = 0;
    public int hour = 0;
    public int quarter = 0;
    public float speed = 4f;
    private float timeAccumulator = 0f;
    private float timeStep = 1.0f;

    private bool isPaused = false; // 💖加一个暂停状态变量～
    private float originalTimeScale = 1f;
    private void Update()
    {
        if (isPaused) return; // 💤暂停时不推进时间～

        timeAccumulator += Time.deltaTime * speed;
        if (timeAccumulator >= timeStep)
        {
            timeAccumulator = 0;
            AdvanceTime();
        }
    }

    private void AdvanceTime()
    {
        OnQuarterChanged?.Invoke((hour, quarter));
        AfterQuarterChanged?.Invoke((hour, quarter));
        
        quarter++; 
        if (quarter >= 4)
        {
            quarter = 0;
            hour++;
            
            if (hour == 7) OnKeyTimeChanged?.Invoke(KeyTime.Morning);
            else if (hour == 12) OnKeyTimeChanged?.Invoke(KeyTime.Noon);
            else if (hour == 17) OnKeyTimeChanged?.Invoke(KeyTime.Dusk);
            else if (hour == 18) OnKeyTimeChanged?.Invoke(KeyTime.Night);

            if (hour >= 24)
            {
                hour = 0;
                day++;
                OnDayChanged?.Invoke(day);
            }
        }
    }

    public (int, int, int) GetTime()
    {
        return (day, hour, quarter);
    }

    public float GetDayLen()
    {
        return (24 * 4) * timeStep / speed;
    }

    // 🎀✨主人的新功能～暂停时间✨🎀
    public void SetPaused(bool pause)
    {
        isPaused = pause;
        if (pause)
        {
            originalTimeScale = Time.timeScale; // 记住原本的速度
            Time.timeScale = 0f;                // 暂停一切
        }
        else
        {
            Time.timeScale = originalTimeScale; // 恢复之前的速度
        }
    }
    public float ConvertQuarterToRealSeconds(int q)
    {
        return q * timeStep / speed;
    }
}
