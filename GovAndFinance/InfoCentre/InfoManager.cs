using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour{
    public GameObject rootOfTheAll;
    public InfectionInfoManager infectionInfoManager = null;
    public CashFlowEntityManager cashFlowEntityManager = null;
    public CashFlowManager CFEManager = null;
    // public ToastManager toastManager;
    int uidCounter = 0;
    List<GameObject> infoDebuggerRoots = new List<GameObject>();
    public void Awake(){
        this.infectionInfoManager = new InfectionInfoManager(GetListedRoot("InfectionInfo"));
        this.cashFlowEntityManager.SetupDebug(GetListedRoot("CFEInfo"));
        TimeManager.OnDayChanged += ShowDaySumUp;

    }
    private GameObject GetListedRoot(string debugName){
        GameObject newRoot = new GameObject(debugName + "_INFO_DEBUG" + $"_DBGID_{uidCounter.ToString()}");
        newRoot.SetActive(false);
        newRoot.transform.parent = rootOfTheAll.transform;
        infoDebuggerRoots.Add(newRoot);
        uidCounter ++;
        return newRoot;
    }

    public void ShowDaySumUp(int day){
        // toastManager.MakeAToast(this.cashFlowManager.GenerateReprString());
    }
}
