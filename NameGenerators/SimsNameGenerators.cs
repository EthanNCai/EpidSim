using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;


class SimsNameGenerator
{
    private static List<string> namePrefixes = new List<string> { "Alex", "Chris", "Taylor", "Jordan", "Morgan", "Casey", "Riley" };
    private static List<string> nameSuffixes = new List<string> { "Smith", "Johnson", "Brown", "Davis", "Miller", "Wilson", "Anderson", "Taylor" };

    public static string GetSimsName()
    {
        return namePrefixes[Random.Range(0, namePrefixes.Count)] + " " + nameSuffixes[Random.Range(0, nameSuffixes.Count)];
    }
}
