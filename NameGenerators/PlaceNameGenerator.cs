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
    private static List<string> officePrefixes = new List<string>{"安克创新","戴尔","美亚","苹果","多益","嘉兴","字节跳动","Gbit","北帆"};
    private static List<string> officeSuffixes = new List<string>{"产业","集团","科技","电子","网络","控股"};
    
    public static string GetResidentialName(){
        return homePrefixes[Random.Range(0,homePrefixes.Count)] + homeSuffixes[Random.Range(0,homeSuffixes.Count)];
    }
    public static string GetOfficeName(){
        return officePrefixes[Random.Range(0,officePrefixes.Count)] + officeSuffixes[Random.Range(0,officeSuffixes.Count)];
    }

}