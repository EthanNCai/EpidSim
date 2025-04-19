using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour{

    // Place 和 Sim 通过InfoManager获得有关于这个世界的知识
    public GameObject rootOfTheAll;
    public InfectionInfoManager infectionInfoManager = null;
    public CFEManager cfeManager = null;
    // public CashFlowManager CFEManager = null;
    public PolicyManager policyManager = null;
    public VirusManager virusManager;
    public TimeManager timeManager;
    
    public LockDownManager lockdownManager;
    public SimsDeadManager simsDeadManager;

    public NotificationManager notificationManager;
    public TestManager testManager;

    int uidCounter = 0;
    List<GameObject> infoDebuggerRoots = new List<GameObject>();
    public void Awake(){
        this.infectionInfoManager = new InfectionInfoManager(GetListedRoot("InfectionInfo"));
        this.cfeManager.SetupDebug(GetListedRoot("CFEInfo"));
        this.virusManager.SetupDebug(GetListedRoot("VirusSevirity"));
        // TimeManager.OnDayChanged += ShowDaySumUp;

    }
    private GameObject GetListedRoot(string debugName){
        GameObject newRoot = new GameObject(debugName + "_INFO_DEBUG" + $"_DBGID_{uidCounter.ToString()}");
        newRoot.SetActive(false);
        newRoot.transform.parent = rootOfTheAll.transform;
        infoDebuggerRoots.Add(newRoot);
        uidCounter ++;
        return newRoot;
    }

    // public void ShowDaySumUp(int day){
    //     // toastManager.MakeAToast(this.cashFlowManager.GenerateReprString());
    // }
}
