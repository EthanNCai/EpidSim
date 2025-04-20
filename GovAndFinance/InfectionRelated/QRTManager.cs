using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class QRTManager:MonoBehaviour{


    // Quarantine不是一个短时间的任务，这个排队的队列也不会因为day change 而 flush
    public List<Sims> registerdQuarantinedSims = new List<Sims>();
    public Queue<Sims> qrtQueue = new Queue<Sims>();
    public PlaceManager placeManager;

    public QRTMeta.QrtLenthType qrtDurationType = QRTMeta.QrtLenthType.Short;
    // public static int qrtDuration = 2;

    public void Start()
    {
        QRTCentrePlace.OnBookingReleased += HandleBookingReleased;
    }
    public QRTCentrePlace GetOrQueueTestCandidate(Sims sim){
        foreach (var qrtCentrePlace in this.placeManager.qrtCentrePlaces)
        {
            if (qrtCentrePlace.CheckIsBookAvailable()){ return qrtCentrePlace; }
        }
        this.qrtQueue.Enqueue(sim);
        return null;
    }
    public void HandleBookingReleased(QRTCentrePlace qrtPlace){
        // 被通知有一个空位了

        // 从队列里面找一个
        if(this.qrtQueue.Count == 0) {return;};
        Sims sim = this.qrtQueue.Dequeue();
        sim.HandleQRTQueueCall(qrtPlace);
        // RegisterQRTSim(sim);
        
    }

    public bool CheckIsQRTQueued(Sims sim){
        return this.qrtQueue.Contains(sim);
    }
    public void SetQrtDurationType(QRTMeta.QrtLenthType qrtType){
        this.qrtDurationType = qrtType;
    }
    // public void RegisterQRTSim(Sims sim){
    //     registerdQuarantinedSims.Add(sim);
    // }
    // public void UnRegisterQRTSim(Sims sim){
    //     registerdQuarantinedSims.Remove(sim);
    // }

    public int GetCitizenUnderQuarantine(){
        int ret = 0;
        foreach (var qrtPlace in this.placeManager.qrtCentrePlaces){
            ret += qrtPlace.inSiteSims.Count;
        }
        return ret;
    }
}