using System.Collections.Generic;
using UnityEngine;

public class InfoDebuggerManager : MonoBehaviour{
    public GameObject rootOfTheAll;
    public InfectionInfoManager infectionInfoManager = null;
    int uidCounter = 0;
    List<GameObject> infoDebuggerRoots = new List<GameObject>();

    public void Awake(){
        this.infectionInfoManager = new InfectionInfoManager(GetListedRoot("InfectionInfo"));
    }

    private GameObject GetListedRoot(string debugName)
    {
        GameObject newRoot = new GameObject(debugName + "_INFO_DEBUG" + $"_DBGID_{uidCounter.ToString()}");
        newRoot.SetActive(false);
        newRoot.transform.parent = rootOfTheAll.transform;
        infoDebuggerRoots.Add(newRoot);
        uidCounter ++;
        return newRoot;
    }
}
