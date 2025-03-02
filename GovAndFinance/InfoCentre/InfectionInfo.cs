using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public class InfectionInfoManager{

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

    public InfectionInfoManager(GameObject infectionInfoRoot){
        this.infectionInfoRoot = infectionInfoRoot;
    }

    public void InitializeInfectionInfo(List<Sims> freshSims){
        
        // fresh sims got only two possible types:
        // 1) Suscptible
        // 2) Infected (Manually)

        foreach(Sims sim in freshSims){
            if(sim.infection != null){
                this.infectedSims.Add(sim);
            }else{
                this.susceptibleSims.Add(sim);
            }
        }
        debugInfoText = Utils.SpawnTextAtRelativePosition(this.infectionInfoRoot, new Vector2Int(1,1), "uninitialized debug text for infection.");
        UpdateDebugInfo();
    }


    private string GenerateReprString(){
        
        stringBuilder.Clear();
        stringBuilder.Append($"nInfection: {infectedSims.Count}");
        stringBuilder.Append($"nDead: {deadSims.Count}");
        stringBuilder.Append($"nRecoverd: {recoverdSims.Count}");
        stringBuilder.Append($"nSusceptible: {susceptibleSims.Count}");
        return stringBuilder.ToString();
    }

    public void InfectionDeletion(Sims sim,InfectionStatus status){
        switch(status){
            case InfectionStatus.Dead: 
                deadSims.Add(sim);
                infectedSims.Remove(sim);
                UpdateDebugInfo();
                return;
            case InfectionStatus.Recovered:
                recoverdSims.Add(sim);
                infectedSims.Remove(sim);
                UpdateDebugInfo();
                return;
        }
        Debug.LogError("Not suppose to reach this");
    }
    public void InfectionAddition(Sims sim,InfectionStatus status){
        switch(status){
            case InfectionStatus.Suscptible: 
                susceptibleSims.Remove(sim);
                infectedSims.Add(sim);
                UpdateDebugInfo();
                return;
            case InfectionStatus.Recovered:
                recoverdSims.Remove(sim);
                infectedSims.Add(sim);
                UpdateDebugInfo();
                return;
        }

        Debug.LogError("Not suppose to reach this");
    }
    public void UpdateDebugInfo(){
        string newString = GenerateReprString();
        debugInfoText.text = newString;
    }

    
}