using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    // 模拟市民死亡将会影响着一个candidateSims
}

public class TestManager : MonoBehaviour
{
    // public TestPolicy currentTestPolicy = TestPolicy.Hard;
    public TestEvent currentTestEvent;
    public TimeManager timeManager;
    // public TestPolicy testPolicy;
    public SimsManager simsManager;

    // ✨ 定义一个静态事件，通知大家有测试事件开始了喵！
    public static event Action<TestEvent> OnTestEventCreated;

    public void Start() 
    {
        SimsDeadManager.OnSimsDied += HandleSimsDead;
    }

    public void Update()
    {
        // TODO: 你可以在这里检测 currentTestEvent 是否结束然后清理喵～
    }

    public void CreateTestEvent()
    {
        currentTestEvent = new TestEvent(
            simsManager.activeSimsList,
            timeManager.GetTime(),
            TestPolicy.Hard);

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
}