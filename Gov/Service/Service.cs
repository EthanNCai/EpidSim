using System.ComponentModel;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

// 提示： 最小计算时间粒度是Quarter，所以这里用QuarterCost来表示

public class Service
{
    public string serviceName;
    public int qCost;
    private int cycleCost;
    private int cycleQLen;
    public CycleLen cycleLen;
    public ServiceType serviceType;
    public Service(
        string serviceName,
        CycleLen cycleLen,
        ServiceType serviceType,
        int cycleCost
    ){
        this.serviceName = serviceName;
        this.cycleLen = cycleLen;
        this.cycleCost = cycleCost;
        this.serviceType = serviceType;
        this.cycleQLen = CycleInfo.GetCycleLen(cycleLen);
        this.qCost = this.cycleCost / this.cycleQLen;
    }
}

public enum ServiceType{
    VirusInvestigate,
    BuildingMaintaining,
}

public enum CycleLen{
    Quart,
    Week,
    Hour,
    Day
}
public static class CycleInfo
{
    public static int GetCycleLen(CycleLen cycleLen)
    {
        switch (cycleLen){
            case CycleLen.Quart:
                return 1;
            case CycleLen.Hour:
                return 4;
            case CycleLen.Day:
                return 4*24;
            case CycleLen.Week:
                return 4*24*7;
            default:
               Debug.Log("Error: Invalid cycle length");
               return -1;
        }
    }

}