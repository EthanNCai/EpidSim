using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class TestCenterPlace : Place
{
    public int volumePerTile = 10;
    public int volume;
    public int bookings;
    public int populationCapacity;
    public List<Sims> residents;

    // ✨ 新增：静态事件！当有空位时通知所有人！
    public static event Action<TestCenterPlace> OnBookingReleased;

    public void TestCentrePlaceInit(
        Vector2Int placeShape,
        Vector2Int basePosition,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj,
        GridInfoManager gridDebuggerManager,
        InfoManager infoDebuggerManager,
        CFEManager cfeManager)
    {
        string TestCentreName = PlaceNameGenerator.GetTestCentreName();
        base.PlaceInit(
            placeShape,
            basePosition,
            TestCentreName,
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj,
            gridDebuggerManager,
            infoDebuggerManager,
            cfeManager);
        this.volume = volumePerTile * placeShape.x * placeShape.y;
        TimeManager.OnQuarterChanged += HandleQuarterChanged;
    }

    public void SayHi()
    {
        Debug.Log(base.ToString());
    }

    public void TestSimsInsite()
    {
        // 喵～先复制一份临时列表，防止被修改时爆炸💥
        List<Sims> simsCopy = new List<Sims>(this.inSiteSims);

        foreach (Sims inSiteSim in simsCopy)
        {
            if (inSiteSim.isUnfinishedPCRQuota)
            {
                Debug.Log("Some One Get Tested");
                inSiteSim.GetPCRTested(); // 读取信息
                inSiteSim.isUnfinishedPCRQuota = false;
                this.ReleaseBooking();
                inSiteSim.HandleTestFinished();
            }
        }
    }

    public bool CheckIsBookAvailable()
    {
        return this.bookings < volume;
    }

    public void Booking()
    {
        Debug.Assert(bookings >= 0 && bookings <= volume, $"bug in booking, booking is {bookings}");
        this.bookings += 1;
        Debug.Assert(bookings >= 0 && bookings <= volume, $"bug in booking, booking is {bookings}");
    }

    public void ReleaseBooking()
    {
        Debug.Assert(bookings > 0 && bookings <= volume, $"bug in release booking, booking is {bookings}");
        this.bookings -= 1;
        Debug.Assert(bookings >= 0 && bookings <= volume, $"bug in release booking,, booking is {bookings}");
        OnBookingReleased?.Invoke(this);
    }
    public void HandleQuarterChanged((int,int) time){
        if( time.Item2 == 2){
            TestSimsInsite();
        }
    }
}
