using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class SimsDiary{
    public StringBuilder stringBuilder = new StringBuilder();
    public List<SimsDiaryItem> diaryItems;
    public SimsDiary(){
        diaryItems = new List<SimsDiaryItem>();
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

    public StringBuilder stringBuilder;

    public SimsDiaryItem( (int d, int h, int q) timestamp, string simBehaviorDetial){
        this.timestamp = timestamp;
        this.simBehaviorDetial = simBehaviorDetial;
        this.stringBuilder = new StringBuilder();
    }
}

public static class SimBehaviorDetial{
    public static StringBuilder stringBuilder = new StringBuilder();

    public static string InfectedBy(Sims infector){
        stringBuilder.Clear();
        stringBuilder.Append("Infected by");
        stringBuilder.Append(infector.simsName);
        return stringBuilder.ToString();
    }
    public static string GotoWorkEvent(OfficePlace office){
        stringBuilder.Clear();
        stringBuilder.Append("Go to");
        stringBuilder.Append(office.palaceName);
        stringBuilder.Append("for work");
        return stringBuilder.ToString();
    }

    public static string GoHomeEvent(ResidentialPlace residentialPlace){
        stringBuilder.Clear();
        stringBuilder.Append("Back home at");
        stringBuilder.Append(residentialPlace.palaceName);
        return stringBuilder.ToString();
    }
    
    public static string GoToMedEvent(MedicalPlace medPlace){
        stringBuilder.Clear();
        if(medPlace != null){
            stringBuilder.Append("way too sick, Go to");
            stringBuilder.Append(medPlace.palaceName);
            stringBuilder.Append("for medical trreatment");
            return stringBuilder.ToString();
        }else{
            stringBuilder.Append("Faild to find a available medical place, Stay at home instead");
            return stringBuilder.ToString();
        }
    }
    public static string Bankrupt(){
        stringBuilder.Clear();
        stringBuilder.Append("spent all of the balance");
        return stringBuilder.ToString();
    }
    public static string InfectionProgressEvent(Infection infection){
        // hide for player
        stringBuilder.Clear();
        stringBuilder.Append("Infection progressed, ");
        stringBuilder.Append("Period: ");
        stringBuilder.Append(infection.currentInfectionPeriod);
        stringBuilder.Append("Volume: ");
        stringBuilder.Append(infection);
        return stringBuilder.ToString();
    }

    public static string SubsidiesEvent(int DSubsidiesCollected){
        stringBuilder.Clear();
        stringBuilder.Append("Gov Subsidised ");
        stringBuilder.Append(DSubsidiesCollected);
        stringBuilder.Append("$ today");
        return stringBuilder.ToString();

    }
    public static string PaycheckEvent(int DPaycheck){
        stringBuilder.Clear();
        if(DPaycheck > 0){
            stringBuilder.Append("Paycheck: ");
            stringBuilder.Append(DPaycheck);
            stringBuilder.Append("$ today");
            return stringBuilder.ToString();
        }else{
            stringBuilder.Append("No Paycheck today");
            return stringBuilder.ToString();
        }
    }
}