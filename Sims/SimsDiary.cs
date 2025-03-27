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

    public static string InfectedBy(Sims infector)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Infected by ");
        stringBuilder.Append(infector.simsName);
        return stringBuilder.ToString();
    }

    public static string GotoWorkEvent(OfficePlace office)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Go to ");
        stringBuilder.Append(office.placeName);
        stringBuilder.Append(" for work");
        return stringBuilder.ToString();
    }

    public static string GoHomeEvent(ResidentialPlace residentialPlace)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Back home at ");
        stringBuilder.Append(residentialPlace.placeName);
        return stringBuilder.ToString();
    }

    public static string GoToMedEvent(MedicalPlace medPlace)
    {
        stringBuilder.Clear();
        if (medPlace != null)
        {
            stringBuilder.Append("Way too sick, go to ");
            stringBuilder.Append(medPlace.placeName);
            stringBuilder.Append(" for medical treatment");
        }
        else
        {
            stringBuilder.Append("Failed to find an available medical place, staying at home instead");
        }
        return stringBuilder.ToString();
    }

    public static string BankruptEvent(string reason)
    {
        stringBuilder.Clear();
        stringBuilder.Append($"Spent all of the balance, because {reason}");
        return stringBuilder.ToString();
    }

    public static string GoOutForFunEvent(Place place){
        stringBuilder.Clear();
        stringBuilder.Append("Today off, Go out for fun in ");
        stringBuilder.Append(place.placeName);
        return stringBuilder.ToString();
    }
    
    public static string SicknessAwarenessEvent(SicknessTag sicknessTag)
    {
        stringBuilder.Clear();
        stringBuilder.Append(SicknessTagConverter.SicknessTagToDescription(sicknessTag));
        return stringBuilder.ToString();
    }


    public static string InfectionProgressEvent(Infection infection, SicknessTag sicknessTag)
    {
        // Hidden from the player


        stringBuilder.Clear();
        stringBuilder.Append("<DEBUG ONLY>Infection progressed, ");
        stringBuilder.Append("Period: ");
        stringBuilder.Append(infection.currentInfectionPeriod);
        stringBuilder.Append(", Volume: ");
        stringBuilder.Append(infection);
        return stringBuilder.ToString();
    }

    public static string SubsidiesEvent(int subsidiesCollected, int balance)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Gov subsidized ");
        stringBuilder.Append(subsidiesCollected);
        stringBuilder.Append("$ today, balance now: ");
        stringBuilder.Append(balance);
        return stringBuilder.ToString();
    }

    public static string PaycheckEvent(int paycheck, int balance)
    {
        stringBuilder.Clear();
        if (paycheck > 0)
        {
            stringBuilder.Append("Paycheck: ");
            stringBuilder.Append(paycheck);
            stringBuilder.Append("$ today, balance now: ");
            stringBuilder.Append(balance);
        }
        else
        {
            stringBuilder.Append("No paycheck today, ");
            stringBuilder.Append("balance now: ");
            stringBuilder.Append(balance);
        }
        return stringBuilder.ToString();
    }

    
}
