using System.Collections.Generic;
using UnityEngine;

public class SimsHospitalizedManager: MonoBehaviour{
    public List<Sims> hospitalizedSims = new List<Sims>();
    private bool isEverHospitalized = false; 

    public void RegisterNewHospitalizedSim(Sims targetSim){
        if(hospitalizedSims.Contains(targetSim)){
            hospitalizedSims.Add(targetSim);
        }else{
            Debug.LogError("bug here");
        }
    }
    public void UnRegisterHospitalizedSim(Sims targetSim){
        hospitalizedSims.Remove(targetSim);
    }

}