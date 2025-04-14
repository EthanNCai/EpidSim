using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;


class PlaceNameGenerator
{
    private static List<string> homePrefixes = new List<string> { "Cozy", "Sunny", "Bluebird", "Grand", "Maple", "Silver", "Tranquil" };
    private static List<string> homeSuffixes = new List<string> { "House", "Home", "Apartment", "Residence" };
    private static List<string> officePrefixes = new List<string> { "NexTech", "Quantum", "Skyline", "Everest", "Pioneer", "Summit", "ByteWave", "HyperCore", "Zenith" };
    private static List<string> officeSuffixes = new List<string> { "Industries", "Group", "Tech", "Solutions", "Enterprises", "Holdings", "Networks" };

    private static List<string> testCentrePrefixes = new List<string> { "GroundTruth" };
    private static List<string> testCentreSuffixes = new List<string> { "Bio" };
    

    public static string GetResidentialName()
    {
        return homePrefixes[Random.Range(0, homePrefixes.Count)] + " " + homeSuffixes[Random.Range(0, homeSuffixes.Count)];
    }
    public static string GetOfficeName()
    {
        return officePrefixes[Random.Range(0, officePrefixes.Count)] + " " + officeSuffixes[Random.Range(0, officeSuffixes.Count)];
    }
    public static string GetTestCentreName(){
        return testCentrePrefixes[Random.Range(0, testCentrePrefixes.Count)] + " " + testCentreSuffixes[Random.Range(0, testCentreSuffixes.Count)];
    }
}
