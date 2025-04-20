
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class QRTCentrePlace : Place, IExpensablePlace
{   
    public int bookings;
    public int volumePerTile = 3;
    public int volume;
    public static event Action<QRTCentrePlace> OnBookingReleased;
    public CFEServiceBuildingMaintaining<QRTCentrePlace> serviceBuildingMaintainingCFE;

    public Dictionary<int,int> qrtRegisteries = new Dictionary<int, int>();


    // public CFECommonFees<MedicalPlace> commonFeesCFE;
    public void QRTCenterInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        MapManager mapManager, 
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string qrtPlaceName = GetQRTName();
        base.PlaceInit(
            placeShape, 
            basePosition,
            qrtPlaceName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.volume = volumePerTile * placeShape.x * placeShape.y;
        this.serviceBuildingMaintainingCFE = cfeManager.CreateServiceBuildingMaintainingCFE<QRTCentrePlace>(this);
        TimeManager.OnDayChanged += HandleDayChanged;
        // this.commonFeesCFE = cfeManager.CreateCommonFeesCFE<MedicalPlace>(this);
    }
    
    public void SayHi(){
        Debug.Log(base.ToString());
    }

    private static List<string> qrtPrefixes = new List<string> { "Alpha", "Beta", "Ceylan" };
    private static List<string> qrtSuffixes = new List<string> { "Quarantine" };
    public static string GetQRTName(){
        return qrtPrefixes[UnityEngine.Random.Range(0,qrtPrefixes.Count)] + qrtSuffixes[UnityEngine.Random.Range(0,qrtSuffixes.Count)];
    }
    public int CalculateQExpense(){
        return PriceMenu.QMedicalPlaceMaintaingExpense * volume;
    }
    // public int CalculateQContribution(){
    //     return inSiteSims.Count * infoManager.policyManager.GetSubsidisedMedicalFee();
    // }
    int book_called = 0;
    int book_release_called = 0;

    public bool CheckIsBookAvailable()
    {
        return this.bookings < volume;
    }
    public void Booking()
    {
        Debug.Assert(
            bookings >= 0 && bookings <= volume, 
            $"bug in release booking, booking is {bookings}, book called {book_called}, release_book_called {book_release_called}");
        this.bookings += 1;
        book_called += 1;
        Debug.Assert(
            bookings >= 0 && bookings <= volume, 
            $"bug in release booking, booking is {bookings}, book called {book_called}, release_book_called {book_release_called}");
    }

    public void ReleaseBooking()
    {
        Debug.Assert(
            bookings >= 0 && bookings <= volume, 
            $"bug in release booking, booking is {bookings}, book called {book_called}, release_book_called {book_release_called}");
        this.book_release_called += 1;
        this.bookings -= 1;
        Debug.Assert(bookings >= 0 && bookings <= volume,
         $"bug in release booking, booking is {bookings}, book called {book_called}, release_book_called {book_release_called}");
        OnBookingReleased?.Invoke(this);
    }

    public void TestSimsInsite()
    {
        // 每日检测
        List<Sims> simsCopy = new List<Sims>(this.inSiteSims);
        foreach (Sims inSiteSim in simsCopy)
        {
            // if (inSiteSim.isUnfinishedPCRQuota)
            // {
                // Debug.Log("Some One Get Tested");
            Debug.Log("here1");
            inSiteSim.GetPCRTested(); // 读取信息
                // inSiteSim.isUnfinishedPCRQuota = false;
                // this.ReleaseBooking();
                // inSiteSim.HandleTestFinished();
            // }
        }
    }
    public void CheckForLeavable(){
        // 检查每一个市民，看看其是否有离开的资格
        /* 
            判断条件:
            1. 如果有阳性就直接必须留下
            2. 如果没有阳性
                2.1 没有呆满n天 -> 留下
                2.2 呆满了n天 -> 释放
        */

        // 这里已经假设做完了一次PCR
        List<Sims> simsCopy = new List<Sims>(this.inSiteSims);
        foreach(Sims sim in simsCopy){
            if(!qrtRegisteries.ContainsKey(sim.uid)){
                qrtRegisteries.Add(sim.uid,0);
            }

            if(sim.recentPcrResult == true){
                qrtRegisteries[sim.uid] += 1;
                continue;
            }else{
                if(qrtRegisteries[sim.uid] <= QRTManager.qrtDuration){
                    qrtRegisteries[sim.uid] += 1;
                }else{
                    // 出院
                    Debug.Log($"一名在隔离所呆了{qrtRegisteries[sim.uid]}天的模拟市民今天出院了");
                    qrtRegisteries.Remove(sim.uid);
                    this.ReleaseBooking();
                    sim.HandleQRTFinished();
                }
            }
        }
    }

    public void HandleDayChanged(int day){
        this.TestSimsInsite();
        this.CheckForLeavable();
    }

}