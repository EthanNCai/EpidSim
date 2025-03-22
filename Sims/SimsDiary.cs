using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SimsDiary
{
    private List<SimsDiaryItem> diaryList = new List<SimsDiaryItem>();

    public void AppendDiaryItem(SimsDiaryItem item)
    {
        if (diaryList.Count >= 50) // check if the size is over 50, if so remove the oldest entries.
        {
            diaryList.RemoveAt(0); // remove the oldest entry
        }
        diaryList.Add(item); // add new entry
    }

    public void GetDiaryEntries(List<string> entries)
    {
        entries.Clear(); // Reuse the existing list, clear previous entries
        foreach (var item in diaryList)
        {
            entries.Add($"[{item.timestamp.d}D {item.timestamp.h}H {item.timestamp.q}Q] {item.simBehaviorDetial}");
        }
        // return entries;
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
        stringBuilder.Append(office.palaceName);
        stringBuilder.Append(" for work");
        return stringBuilder.ToString();
    }

    public static string GoHomeEvent(ResidentialPlace residentialPlace)
    {
        stringBuilder.Clear();
        stringBuilder.Append("Back home at ");
        stringBuilder.Append(residentialPlace.palaceName);
        return stringBuilder.ToString();
    }

    public static string GoToMedEvent(MedicalPlace medPlace)
    {
        stringBuilder.Clear();
        if (medPlace != null)
        {
            stringBuilder.Append("Way too sick, go to ");
            stringBuilder.Append(medPlace.palaceName);
            stringBuilder.Append(" for medical treatment");
        }
        else
        {
            stringBuilder.Append("Failed to find an available medical place, staying at home instead");
        }
        return stringBuilder.ToString();
    }

    public static string Bankrupt()
    {
        stringBuilder.Clear();
        stringBuilder.Append("Spent all of the balance");
        return stringBuilder.ToString();
    }

    public static string InfectionProgressEvent(Infection infection)
    {
        // Hidden from the player
        stringBuilder.Clear();
        stringBuilder.Append("Infection progressed, ");
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
