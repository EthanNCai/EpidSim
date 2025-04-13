using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;


public enum LockdownLevel{
    Soft,
    Hard
}

public enum LockdownStatus{
    GlobalLockdown,
    NonGlobalLockdown,
}

public class LockDownManager: MonoBehaviour{


    // 这个类的主要作用是： 1.记录所有已经被lockdown的Place 2.方便Global Lockdown
    public PlaceManager placeManager;
    public List<Place> lockedResidentialPlaces = new List<Place>(){};
    private List<ResidentialPlace> allResidentialPlaces;
    public LockdownLevel lockdownLevel = LockdownLevel.Soft;

    public void Start()
    {
        PlaceManager.OnPlaceSpwaned += Init;
    }

    public void Init(){
        this.allResidentialPlaces = placeManager.residentialPlaces;
    }

    public void GlobalLockDown(){
        foreach (ResidentialPlace place in this.allResidentialPlaces){
            place.SetLockdown(true);
        }
    }
    public void GlobalLockdownRelease(){
        foreach (ResidentialPlace place in this.allResidentialPlaces){
            place.SetLockdown(false);
        }
    }
    
    public void RegisterLockdown(Place place){
        if(!lockedResidentialPlaces.Contains(place)){
            lockedResidentialPlaces.Add(place);
            Debug.Log("Registered Lockdown");
        }
    }
    public void UnregisterLockdown(Place place){
        Debug.Assert(lockedResidentialPlaces.Contains(place));
        lockedResidentialPlaces.Remove(place);
        Debug.Log("Unregistered Lockdown");
    }
    public void SwitchLockdownLevel(string newLevel) {
        if (!System.Enum.TryParse(newLevel, true, out LockdownLevel parsedLevel)) {
            Debug.LogWarning($"喵呜~未知的 lockdown level：{newLevel}，我还不会处理这个级别呢！");
            return;
        }
        if (parsedLevel == lockdownLevel) {
            // Debug.Log($"Lockdown level 已经是 {parsedLevel} 啦，不用重复设置~");
            return;
        }
        lockdownLevel = parsedLevel;
        Debug.Log($"喵！已切换 lockdown level 为：{lockdownLevel}");
        // （你可以在这里加入根据 level 调整策略的逻辑~）
        // GlobalLockDown(); // 比如可以重新执行一次全局封锁
    }
    public int GetNumberOfAffectedResidentialBuildings(){
        return lockedResidentialPlaces.Count;
    }
    public int GetNumberOfAffectedCitizens(){
        int count = 0;
        foreach (ResidentialPlace place in this.lockedResidentialPlaces){
            count += place.registeredSims.Count;
        }
        return count;
    }   
    public string GetLockdownStatus(){
        if(this.lockedResidentialPlaces.Count == this.allResidentialPlaces.Count){
            return "Global Lockdown";
        }else if (this.lockedResidentialPlaces.Count == 0){
            return "No Lockdown";
        }else{
            return "Partial Lockdown";
        }

    }
    public List<string> GetLockedNames(){
        List<string> tmp = new List<string>(){};
        foreach (ResidentialPlace place in this.lockedResidentialPlaces){
            tmp.Add(place.placeName);
        }
        return tmp;
    }
    
}