using System.ComponentModel;
using TMPro.Examples;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SimsDieDebugger: MonoBehaviour{

    private Sims testedSim;
    public SimsManager simManager;

    public void Start(){
        SimsManager.OnSimsSpawned += Init;
    }
    public void Init(){
        testedSim = simManager.activeSimsList[0];
    }
    public void LetTestedSimDie(){
        testedSim.HandleDead();
    }



}