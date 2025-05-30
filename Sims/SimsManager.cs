
using System;
using System.Collections.Generic;
using UnityEngine;


public class SimsManager: MonoBehaviour{

    public MapManager mapManager;
    public static event Action OnSimsSpawned;
    public GameObject simsFacotryObj;
    private SimsFactory simsFactory;
    private Dictionary<int, Sims> simsDictionary = new Dictionary<int, Sims>();
    public List<Sims> activeSimsList = new List<Sims>();
    public InfoManager infoDebuggerManager;

    // public PlaceManager placeManager();
    public void Start()
    {
        Vector2Int mapSize = mapManager.mapsize;
        int numSims = 200;
        for(int i =0; i < numSims; i++)
        {
            this.simsFactory = this.simsFacotryObj.GetComponent<SimsFactory>();
            Sims newSims = this.simsFactory.CreateSims(new Vector2Int((int)mapManager.mapCenter.x,(int)mapManager.mapCenter.y));
            int uid = newSims.uid;
            this.simsDictionary[uid] = newSims;
            this.activeSimsList.Add(newSims);
        }
        // initial infect
        activeSimsList[0].ManuallyInfect();
        infoDebuggerManager.infectionInfoManager.InitializeInfectionInfo(this.activeSimsList);
        OnSimsSpawned?.Invoke();
    }
    public Sims GetSimsByUID(int uid)
    {
        return simsDictionary.TryGetValue(uid, out Sims sims) ? sims : null;
    }
}