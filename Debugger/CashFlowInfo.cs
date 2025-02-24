using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public class CashFlowInfoManager{

    // private int nInfectionPA;
    // private int nInfectionPB;
    // private int nInfectionPC;

    public GameObject infectionInfoRoot;
    public int nInfection {get{return infectedSims.Count;}}
    public int nDead {get{return deadSims.Count;}}
    public int nRecoverd {get{return recoverdSims.Count;}}
    public int nSusceptible {get{return susceptibleSims.Count;}}

    private StringBuilder stringBuilder = new StringBuilder();


    private List<Sims> infectedSims = new List<Sims>();
    private List<Sims> susceptibleSims = new List<Sims>();
    private List<Sims> deadSims = new List<Sims>();
    private List<Sims> recoverdSims = new List<Sims>();

    private TextMesh debugInfoText;

    public CashFlowInfoManager(GameObject infectionInfoRoot){
        this.infectionInfoRoot = infectionInfoRoot;
    }

    private string GenerateReprString(){
        
        stringBuilder.Clear();
        stringBuilder.Append($"nInfection: {infectedSims.Count}");
        stringBuilder.Append($"nDead: {deadSims.Count}");
        stringBuilder.Append($"nRecoverd: {recoverdSims.Count}");
        stringBuilder.Append($"nSusceptible: {susceptibleSims.Count}");
        return stringBuilder.ToString();
    }
    public void UpdateDebugInfo(){
        string newString = GenerateReprString();
        debugInfoText.text = newString;
    }

}