using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;

public class SimScheduler{
    // 这个脚本不仅是关系到模拟市民的本人意愿，更关系到政策的强制执行力
    Place pInfectionRelated = null; // infection related
    Place pLeisureRelated = null; // leisure related
    Place pJobRelated = null; // work related
    Sims hostedSim;

    public SimScheduler(Sims sim){
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
            //this.pJobRelated = hostedSim.home;
        }
        this.hostedSim.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                this.hostedSim.timeManager.GetTime(),
                SimBehaviorDetial.GotoWorkEvent(hostedSim.office)
            ));
        this.pJobRelated = hostedSim.office;
    }

    // 晚上去哪里？
    public void UpdateScheduleOnDusk(){
        this.pJobRelated = hostedSim.home;
        if(hostedSim.isTodayOff){
            this.pLeisureRelated = null;
        }
        this.hostedSim.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                this.hostedSim.timeManager.GetTime(),
                SimBehaviorDetial.GoHomeEvent(hostedSim.home)
        ));
    }
    public void UpdateScheduleOnInfectionChanged(bool justRevocerd = false, bool justDead = false){
        // read infection
        
        Debug.Assert(this.hostedSim.infection != null);
        Infection infection = this.hostedSim.infection;
        // this.hostedSim.infectionl;

        // 0 - 10
        int acutalMedicalFee = this.hostedSim.infoManager.policyManager.GetSubsidisedMedicalFee();
        // 0 - +inf
        int balance = this.hostedSim.balance;
        // 0 - 100
        int volume = infection.virusVolume;

        // 96
        int qPerday = CommonMetas.qPerday;
        
        if(!justRevocerd && !justDead){

            Debug.Assert(volume >= 0 || justDead || justRevocerd, $"sims->{hostedSim.uid}-[{hostedSim.infection.virusVolume}]-belowzero");
            float willingnessToMedical = this.CalculateHospitalWillingness(acutalMedicalFee,balance,volume,qPerday);
            bool isGoToMedicalRightNow = RandomManager.FlipTheCoin(willingnessToMedical);
            // Debug.Log("willingness to Med" + willingnessToMedical);
            if(isGoToMedicalRightNow){
                if(this.pInfectionRelated == null){
                    // 还没在医院了
                    MedicalPlace medicalPlace = this.hostedSim.placeManager.GetAvailableMedicalPlace();
                    if(medicalPlace == null){
                        Debug.Log("A sim willing to go to hospital, but no seats left");
                    }else{
                        pInfectionRelated = medicalPlace;
                    }
                }else{
                    // 已经在一个medical facility了
                    // do nothing here...
                }
            }else{
                // 放弃治疗
                this.pInfectionRelated = null;
            }
        }else{
            // 已经痊愈或者死亡
            this.pInfectionRelated = null;
        }
    }
    public void UpdateScheduleOnPolicyChanged(){
        // read policy
    }
    public void UpdateScheduleOnInfectionKowledgeChange(){
        // read policy
    }

    public void UpdateScheduleOnFeeUnaffordable(){
        // 当Sims因为非生活费的原因而破产的时候，就会触发这个函数
        Debug.Assert(hostedSim.inSite is MedicalPlace,"Sims 破产了，但是它却不再医院里面（理论上只有医院才能导致破产）");
        if(this.hostedSim.inSite is MedicalPlace){
            this.pInfectionRelated = null;
            return;
        }
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
    float CalculateHospitalWillingness(int actualMedicalFee, int balance, int volume, int qPerday)
    {
        float minWillingness = 0.0f; // 最小意愿

        // 计算每天的医疗支出
        int dailyCost = qPerday * actualMedicalFee;
        int twoDaysCost = dailyCost * 2;

            // STEP 1: 钱的判断（二极管）
        if (balance <= twoDaysCost)
        {
            return minWillingness; // 没钱
        }else{
            // STEP 2: sickness的判断（线性）
            return this.hostedSim.infoManager.virusManager.GetVirusSeverity() * volume * 0.01f;
        }
    }

    private void FlushSchedule(){
        pInfectionRelated = null; // infection related
        pJobRelated = null;
        pLeisureRelated = null;
    }
}
