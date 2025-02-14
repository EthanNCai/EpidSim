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

    public static string GetResidentialName(){
        return homePrefixes[Random.Range(0,homePrefixes.Count)] + homeSuffixes[Random.Range(0,homeSuffixes.Count)];
    }
}