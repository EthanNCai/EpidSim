
using System;
using System.Collections.Generic;
using UnityEngine;


public class SimsManager: MonoBehaviour{

    public MapManager mapManager;
    public static event Action OnSimsSpawned;
    public GameObject simsFacotryObj;
    private SimsFactory simsFactory;

    public List<Sims> simsList = new List<Sims>();

    public void Start()
    {

        Vector2Int mapSize = mapManager.mapsize;
        int numSims = 2;
        for(int i =0; i < numSims; i++)
        {
            this.simsFactory = this.simsFacotryObj.GetComponent<SimsFactory>();
            Sims newSims = this.simsFactory.CreateSims(new Vector2Int(7,1));
            this.simsList.Add(newSims);
        }
        OnSimsSpawned?.Invoke();
    }
}