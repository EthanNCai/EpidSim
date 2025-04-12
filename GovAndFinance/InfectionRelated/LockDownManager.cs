using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum LockdownLevel{
    Soft,
    Hard
}

public class LockDownManager: MonoBehaviour{


    // 这个类的主要作用是： 1.记录所有已经被lockdown的Place 2.方便Global Lockdown
    public PlaceManager placeManager;
    public List<Place> lockedPlaces = new List<Place>(){};
    private List<ResidentialPlace> residentialPlaces;

    public void Start()
    {
        PlaceManager.OnPlaceSpwaned += Init;
    }

    public void Init(){
        this.residentialPlaces = placeManager.residentialPlaces;
    }

    public void GlobalLockDown(){

    }
    public void GlobalRelease(){
        
    }
    
    public void RegisterLockdown(Place place){
        if(!lockedPlaces.Contains(place)){
            lockedPlaces.Add(place);
            Debug.Log("Registered Lockdown");
        }
    }
    public void UnregisterLockdown(Place place){
        Debug.Assert(lockedPlaces.Contains(place));
        lockedPlaces.Remove(place);
        Debug.Log("Unregistered Lockdown");
    }
}