using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


// 启动单次test（Single Shot）
//      没有test还是可以去上班，不过需要中断
//      没有test就不能去上班，一直持续观察test的机会

/*
优先度定义，单次test的logic还未结束
*/



// 其实站在模拟市民的角度就是
/* 
上次检测是什么时候 > Test Policy > Right Here Right Now 要不要去做一次检测？

*/
public enum TestPolicy{
    Soft,
    Hard,
}

public class TestEvent{


    public (int d,int h,int q) eventTimePoint;
    public List<Sims> postiveSims = new List<Sims>();
    public List<Sims> negativeSims = new List<Sims>();
    public List<Sims> testedSims = new List<Sims>(); // 这个只能用作距离
    public List<Sims> candidateSims;
    public TestPolicy testPolicy;

    public TestEvent(
        List<Sims> candidateSims,
         (int d,int h,int q) timePoint,
         TestPolicy testPolicy){
        this.candidateSims = candidateSims;
        this.eventTimePoint = timePoint;
        this.testPolicy = testPolicy;
    }

    public void SubmitTestResult(Sims sim, bool isPositive){
        if(isPositive){ 
            postiveSims.Add(sim);
        }else{
            negativeSims.Add(sim);
        }
        testedSims.Add(sim);
        // Debug.Log($"testedSims: {testedSims.Count},negativeSims:{negativeSims.Count},postiveSims:{postiveSims.Count}");
    }

    // 模拟市民死亡将会影响着一个candidateSims
}

public class TestManager : MonoBehaviour
{
    // public TestPolicy currentTestPolicy = TestPolicy.Hard;
    public TestEvent currentTestEvent;
    public TimeManager timeManager;
    // public TestPolicy testPolicy;
    public SimsManager simsManager;
    private Queue<Sims> testQueue = new Queue<Sims>();

    public PlaceManager placeManager;

    // ✨ 定义一个静态事件，通知大家有测试事件开始了喵！
    public static event Action<TestEvent> OnTestEventCreated;
    public static event Action OnTestEventEnd;

    public void Start() 
    {
        SimsDeadManager.OnSimsDied += HandleSimsDead;
        TestCenterPlace.OnBookingReleased += HandleBookingReleased;
        TimeManager.OnDayChanged += FlushQueue;
        TimeManager.OnQuarterChanged += UpdateEventStatus;
    }

    // public void Update()
    // {
    //     // UpdateEventStatus();
    // }
    public void UpdateEventStatus((int h,int q) time){
        if(time.q == 0 && currentTestEvent != null){
            if (currentTestEvent.testedSims.Count >= currentTestEvent.candidateSims.Count){
                // Event 已经结束
                currentTestEvent = null;
                OnTestEventEnd?.Invoke();
            }
            
            //  

        }
    }

    public void CreateTestEvent()
    {
        currentTestEvent = new TestEvent(
            simsManager.activeSimsList,
            timeManager.GetTime(),
            TestPolicy.Soft);

        // 🎉 发出事件，告诉全世界测试开始啦喵！
        OnTestEventCreated?.Invoke(currentTestEvent);

    }

    public void HandleSimsDead(Sims deadSim)
    {
        if (currentTestEvent == null) return;

        // 💀 有市民去世了，更新测试数据列表
        currentTestEvent.testedSims.Remove(deadSim);
        currentTestEvent.candidateSims.Remove(deadSim);
    }

    public TestCenterPlace GetOrQueueTestPlace(Sims sim){
        foreach (var testCenterPlace in this.placeManager.testCenterPlaces)
        {
            if (testCenterPlace.CheckIsBookAvailable()){ return testCenterPlace; }
        }
        this.testQueue.Enqueue(sim);
        return null;
    } 
    public void HandleBookingReleased(TestCenterPlace testPlace){
        // 被通知有一个空位了

        // 从队列里面找一个
        if(this.testQueue.Count == 0) {return;};
        Sims sim = this.testQueue.Dequeue();
        sim.HandleTestQueueCall(testPlace);
    }
    public void FlushQueue(int day){
        // 每天都会重新flush一下queue， 也就是说所有的模拟市民早上就要重新拿号
        this.testQueue.Clear();
    }
    public bool isActivePCRTestEvent(){
        return currentTestEvent != null;
    }

}