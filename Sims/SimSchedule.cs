using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SimSchedule{
    // 这个脚本不仅是关系到模拟市民的本人意愿，更关系到政策的强制执行力
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
        Debug.Log(hostedSim.isTodayOff);
        this.pLeisureRelated = null;
        if(hostedSim.isTodayOff){
            Place commercialChoosed = RandomManager.Choice(hostedSim.placeManager.commercialPlaces);
            this.pLeisureRelated = commercialChoosed;
        }else{
            this.pLeisureRelated = null;
            this.pJobRelated = hostedSim.home;
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
        int sickness = infection.virusVolume;
        // 96
        int qPerday = CommonConsts.qPerday;

        float willingnessToMedical = this.CalculateHospitalWillingness(acutalMedicalFee,balance,sickness,qPerday);
        bool isGoToMedicalRightNow = RandomManager.FlipTheCoin(willingnessToMedical);

        // this.FlushSchedule();
        if(isGoToMedicalRightNow && !justRevocerd){
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
            // just recoverd
            this.pInfectionRelated = null;
        }
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
    float CalculateHospitalWillingness(int acutalMedicalFee, int balance, int sickness, int qPerday)
    {
        float maxWillingness = 1.0f; // 最大意愿
        float minWillingness = 0.0f; // 最小意愿

        // 计算每天的医疗支出
        int dailyCost = qPerday * acutalMedicalFee;
        int fiveDayCost = dailyCost * 5;

        // **情况 1：余额不足 1 天治疗费用，直接 0**
        if (balance < dailyCost)
        {
            return minWillingness;
        }

        if (sickness <= 20){
            return minWillingness;
        }

        // **情况 2：余额足够 3 天，意愿直接 1**
        if (balance >= fiveDayCost)
        {
            return maxWillingness;
        }

        // **情况 3：余额在 1-3 天之间，意愿度线性增长**
        float balanceFactor = (float)(balance - dailyCost) / (fiveDayCost - dailyCost); // 归一化到 0-1

        // **疾病影响，越严重越想去**
        float sicknessFactor = sickness / 100f;

        // **综合意愿度 = 余额影响 + 疾病影响**
        float willingness = Mathf.Clamp01(0.5f * balanceFactor + 0.5f * sicknessFactor);

        return willingness;
    }
    private void FlushSchedule(){
        pInfectionRelated = null; // infection related
        pJobRelated = null;
        pLeisureRelated = null;
    }
}
