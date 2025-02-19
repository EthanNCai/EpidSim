using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

class PlaceNameGenerator
{
    private static List<string> homePrefixes = new List<string>{"麻吉","喵喵","小游","巨型","烟民","丁真"};
    private static List<string> homeSuffixes = new List<string>{"之家","家","公寓","住宅"};
    private static List<string> officePrefixes = new List<string>{"安克创新","戴尔","美亚亿安","苹果","多益","佳芯","字节跳动","吉比特"};
    private static List<string> officeSuffixes = new List<string>{"产业","集团","科技","电子","网络"};
    private static List<string> medicalPrefixes = new List<string>{"厦门市医学院","奥梅","厦门市","孙厝"};
    private static List<string> medicalSuffixes = new List<string>{"附属第二医院","诊所","中医院","第一人民医院"};
    private static List<string> publicPrefixes = new List<string>{"九龙","敬贤","深圳湾","万象城","万达","中央"};
    private static List<string> publicSuffixes = new List<string>{"公共事务中心","运动场","公园","广场","商场"};
    
    public static string GetResidentialName(){
        return homePrefixes[Random.Range(0,homePrefixes.Count)] + homeSuffixes[Random.Range(0,homeSuffixes.Count)];
    }
    public static string GetOfficeName(){
        return officePrefixes[Random.Range(0,officePrefixes.Count)] + officeSuffixes[Random.Range(0,officeSuffixes.Count)];
    }
    public static string GetMedicalName(){
        return medicalPrefixes[Random.Range(0,medicalPrefixes.Count)] + medicalSuffixes[Random.Range(0,medicalSuffixes.Count)];
    }
    
    public static string GetPublicName(){
        return publicPrefixes[Random.Range(0,publicPrefixes.Count)] + publicSuffixes[Random.Range(0,publicSuffixes.Count)];
    }

}