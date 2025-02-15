using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;

class SimsNameGenerator
{
    private static List<string> namePrefixes = new List<string>{"蔡","郑","游","涂","林","李","韦"};
    private static List<string> nameSuffixes = new List<string>{"佳伊","俊志","江得","淑琴","挺","永佳","孜"};

    public static string GetResidentialName(){
        return namePrefixes[Random.Range(0,namePrefixes.Count)] + nameSuffixes[Random.Range(0, nameSuffixes.Count)];
    }
}