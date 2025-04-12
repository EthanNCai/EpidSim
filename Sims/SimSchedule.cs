using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;

public class SimScheduler{

    // Section Dests
    Place residentialLockDownDest;
    Place govQurantineDest;
    Place personalMedicalDest;
    Place workRelatedDest;
    Place leisureRelatedDest;
    // End 

    Sims hostedSim;

    public SimScheduler(Sims sim){
        this.hostedSim = sim;
    }
    

    // 早晨决定去哪里？
    public void UpdateScheduleOnMorning(){
        // this.leisureRelatedDest = null;
        if(hostedSim.isTodayOff && RandomManager.FlipTheCoin(0.5)){
            Place commercialChoosed = RandomManager.Choice(hostedSim.placeManager.commercialPlaces);
            this.leisureRelatedDest = commercialChoosed;
            
        }else{
            this.leisureRelatedDest = null;
            //this.pJobRelated = hostedSim.home;
        }
        this.workRelatedDest = hostedSim.office;
    }

    // 晚上去哪里？
    public void UpdateScheduleOnDusk(){
        if(leisureRelatedDest){
            this.leisureRelatedDest = this.hostedSim.home;
        }
        if(workRelatedDest){
            this.workRelatedDest = this.hostedSim.home;
            
        }
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
                if(this.personalMedicalDest == null){
                    // 还没在医院了
                    MedicalPlace medicalPlace = this.hostedSim.placeManager.GetAvailableMedicalPlace();
                    if(medicalPlace == null){
                        Debug.Log("A sim willing to go to hospital, but no seats left");
                    }else{
                        personalMedicalDest = medicalPlace;
                    }
                }else{
                    // 已经在一个medical facility了
                    // do nothing here...
                }
            }else{
                // 放弃治疗
                this.personalMedicalDest = null;
            }
        }else{
            // 已经痊愈或者死亡
            this.personalMedicalDest = null;
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
            this.personalMedicalDest = null;
            return;
        }
    }

    public void HandelLockDownStatusUpdate(bool isLockdown){
        if (isLockdown && this.govQurantineDest == null){
            // 发出到封锁指令，并且没有其他的GovInfectionPolicy
            this.residentialLockDownDest = this.hostedSim.home;
        }else if(!isLockdown && this.govQurantineDest == hostedSim.home){
             // 收到关闭封锁的指令的同时，当前正在被封锁在家里
            this.govQurantineDest = null;
        }
    }

    public Place GetDestination(KeyTime keyTime){

        // 为什么要传入一个keyTime？ 这是因为有一些东西只用Insert到Diary一次

        Place destination = null;
        // DestType destType = DestType.None;

        if(govQurantineDest != null){
            destination = govQurantineDest;
            // destType = DestType.GovQurantine;
        }else if(residentialLockDownDest != null){
            destination =  residentialLockDownDest;
            // destType = DestType.ResidentialLockDown;
        }else if(personalMedicalDest != null){
            destination =  personalMedicalDest;
            this.hostedSim.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                this.hostedSim.timeManager.GetTime(),
                SimBehaviorDetial.GoOutForFunEvent(personalMedicalDest)));
            // destType = DestType.PersonalMedical;
        }else if(leisureRelatedDest != null){
            if(keyTime == KeyTime.Morning){
                this.hostedSim.simDiary.AppendDiaryItem(
                new SimsDiaryItem(
                    this.hostedSim.timeManager.GetTime(),
                    SimBehaviorDetial.GoOutForFunEvent(leisureRelatedDest)));
                destination =  leisureRelatedDest;
            }else{
                // 该回家了
                destination = leisureRelatedDest;
            }
        }else if(workRelatedDest != null){
            destination =  workRelatedDest;
            // destType = DestType.WorkRelated;
        }else{
            Debug.Assert(destination != null);
            destination = hostedSim.home;
        }
        return destination;
    }
   
    public bool checkIsTodayReallyOff(){
        return leisureRelatedDest != null;
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

    // private void FlushSchedule(){
    //     pGovInfectionPolicy = null; // government policy
    //     pInfectionRelated = null; // infection related
    //     pJobRelated = null;
    //     pLeisureRelated = null;
    // }

    private void FlushDest(){
        workRelatedDest = null;
        leisureRelatedDest = null;
        govQurantineDest = null;
        residentialLockDownDest = null;
    }
}
