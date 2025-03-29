using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.UIElements;

// 这个Diary应该include是一些主观的，以Sims为视角的一些体验、得到的东西、行动以及行动的原因
// example:
/*
    身体体验：今天感觉有一些不舒服 
    情感体验：感觉最近好严重，不太敢出门了
    收获：今天赚了n块钱
    支出：今天在医院花了n块钱
    行动：因为没有钱所以没有去医院
    行动：因为医院没有位置所以没有去医院
    行动：去上班了
    行动：下班了
    行动：没有钱治下去了

*/
public class SimsDiary
{
    private readonly Queue<SimsDiaryItem> diaryQueue = new Queue<SimsDiaryItem>();
    private readonly Queue<string> diaryReprQueue = new Queue<string>(); // 可以理解为DiaryItem的数字孪生

    private readonly Sims hostedSim;
    
    public void AppendDiaryItem(SimsDiaryItem item)
    {
        if (diaryQueue.Count >= CommonMetas.diaryMaxEntries)
        {
            diaryQueue.Dequeue(); // 移除最旧的元素
            diaryReprQueue.Dequeue();
        }
        diaryQueue.Enqueue(item); // 添加新日志
        diaryReprQueue.Enqueue($"[d{item.timestamp.d:D2}, {item.timestamp.h:D2}:{item.timestamp.q:D2}] {item.simBehaviorDetial}");
    }

    public Queue<string> GetDiaryReprQueue()
    {
        return diaryReprQueue;
    }

    public SimsDiary(Sims hostedSim){
        this.hostedSim = hostedSim;
    }

    public void DailyDuskDiaryItem(){
        int paycheckToday = this.hostedSim.GetAndClearAccumulatedPaycheck();
        if(paycheckToday != 0){
            this.hostedSim.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                this.hostedSim.timeManager.GetTime(),
                SimBehaviorDetial.WorkBackHomeEvent(paycheckToday, this.hostedSim.balance)
            ));
        }
    }
    public void DailyNoonDairyItem(){
        
    }
}

public struct SimsDiaryItem{

    // 现有的SimDiary Item
    // 1-出门上班
    // 2-回家
    // 3-赚钱总结
    // 4-去医院
    // 5-用完了钱
    // 6-infection Progressed
    // 7-补贴领取总结
    public (int d, int h, int q) timestamp;
    public string simBehaviorDetial;

    // public StringBuilder stringBuilder;

    public SimsDiaryItem( (int d, int h, int q) timestamp, string simBehaviorDetial){
        this.timestamp = timestamp;
        this.simBehaviorDetial = simBehaviorDetial;
        // this.stringBuilder = new StringBuilder();
    }
}

public static class SimBehaviorDetial
{
    public static StringBuilder stringBuilder = new StringBuilder();


    public static string SicknessAwarenessEvent(SicknessTag sicknessTag)
    {
        stringBuilder.Clear();
        stringBuilder.Append(SicknessTagConverter.SicknessTagToDescription(sicknessTag));
        return stringBuilder.ToString();
    }

    public static string GoOutForFunEvent(Place place)
    {
        stringBuilder.Clear();
        stringBuilder.Append($"Today off, I'd like to go to {place.placeName} for fun");
        return stringBuilder.ToString();
    }

    public static string FaildGoOutForFunEvent(string reason)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Today off, but I'd rather stay at home because ");
        stringBuilder.Append(reason);
        return stringBuilder.ToString();
    }

    public static string GetSubsidiesEvent(int subsidiesCollected, int balance)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Gov subsidized ");
        stringBuilder.Append(subsidiesCollected);
        stringBuilder.Append("$ today, balance now: ");
        stringBuilder.Append(balance);
        return stringBuilder.ToString();
    }
    public static string WorkBackHomeEvent(int paycheck, int balance){
        stringBuilder.Clear();
        stringBuilder.Append("Work back home with");
        stringBuilder.Append(paycheck);
        stringBuilder.Append("$ today, balance now: ");
        stringBuilder.Append(balance);
        return stringBuilder.ToString();
    }
}
