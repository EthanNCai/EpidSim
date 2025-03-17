using UnityEngine;

public class SimSchedule{
    Place pInfectionRelated = null; // infection related
    Place pLeisureRelated = null; // leisure related
    Place pJobRelated = null; // work related
    Sims hostedSim;

    public SimSchedule(Sims sim){
        this.hostedSim = sim;
    }
    

    // 早晨决定去哪里？
    public void UpdateScheduleOnMorning(){
        // flush Leisure 
        // Debug.Log(hostedSim.isTodayOff);
        this.pLeisureRelated = null;
        if(hostedSim.isTodayOff){
            Place commercialChoosed = RandomManager.Choice(hostedSim.placeManager.commercialPlaces);
            this.pLeisureRelated = commercialChoosed;
        }else{
            this.pLeisureRelated = null;
        }
        this.pJobRelated = hostedSim.office;
    }

    // 晚上去哪里？
    public void UpdateScheduleOnDusk(){
        this.pJobRelated = hostedSim.home;
        if(hostedSim.isTodayOff){
            this.pLeisureRelated = hostedSim.home;
        }
    }
    public void UpdateScheduleOnInfectionChanged(){
        // read infection
    }
    public void UpdateScheduleOnPolicyChanged(){
        // read policy
    }
    public void UpdateScheduleOnInfectionKowledgeChange(){
        // read policy
    }
    public Place GetDestination(){
        if(pInfectionRelated != null){
            return pInfectionRelated;
        }else if(pLeisureRelated != null){
            return pLeisureRelated;
        }else{
            return pJobRelated;
        }
    }
   
    public bool isTodayDayOff(int day){
        return hostedSim.dayOff.Contains(day);
    }
}
