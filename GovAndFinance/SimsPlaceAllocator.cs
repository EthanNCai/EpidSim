using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

class SimsPlaceAllocator : MonoBehaviour
{
    private bool simsReady = false;
    private bool placeReady = false;

    public PlaceManager placeManager;
    public SimsManager simsManager;


    private void Awake()
    {
        SimsManager.OnSimsSpawned += OnSimsSpawned;
        PlaceManager.OnPlaceSpwaned += OnPlaceSpawned;
    }

    private void OnSimsSpawned()
    {
        Debug.Log("Sims are ready");
        simsReady = true;
        TryAllocateSims();
    }

    private void OnPlaceSpawned()
    {
        Debug.Log("Places are ready");
        placeReady = true;
        TryAllocateSims();
    }

    private void TryAllocateSims()
    {
        if (simsReady && placeReady)
        {
            AllocateSims();
        }
    }

    private void AllocateSims()
    {
        Debug.Log("Allocating...");
        List<Sims> homelessSims = simsManager.activeSimsList;
        foreach(Sims homelessSim in homelessSims){
            ResidentialPlace newHome =  placeManager.GetRandomResidential();
            OfficePlace newOffice =  placeManager.GetRandomOffice();
            homelessSim.AllocateHomeOffice(newHome, newOffice);
        }
    }
}
