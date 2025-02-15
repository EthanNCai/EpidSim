using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;


public class SimsManager: MonoBehaviour{

    public GameObject simsFacotryObj;
    private SimsFactory simsFactory;
    public void Start()
    {
        
        this.simsFactory = this.simsFacotryObj.GetComponent<SimsFactory>();
        this.simsFactory.CreateSims(new Vector2Int(7,1));
    }
}