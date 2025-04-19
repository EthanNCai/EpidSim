using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Assertions.Must;

/*
    一个隐含的假设： 所有模拟市民的日常规划都是再早上发生的（去工作、去娱乐）
*/

public class SimScheduler{

    // Section Dests
    Place lockDownDest;
    Place qurantineDest;
    Place personalMedicalDest;
    Place workRelatedDest;
    Place leisureRelatedDest;
    Place pcrTestRelatedDest;
    // End 

    Sims hostedSim;


    public SimScheduler(Sims sim){
        this.hostedSim = sim;
    }
    

    // 早晨决定去哪里？
    public void UpdateScheduleOnMorning(){
        // this.leisureRelatedDest = null;

        if (hostedSim.home.isLockedDown){
            this.lockDownDest = hostedSim.home;
        }else{
            this.lockDownDest = null;        
        }

        if(hostedSim.isTodayOff && RandomManager.FlipTheCoin(0.5)){
            Place commercialChoosed = RandomManager.Choice(hostedSim.placeManager.commercialPlaces);
            this.leisureRelatedDest = commercialChoosed;
            
        }else{
            this.leisureRelatedDest = null;
            //this.pJobRelated = hostedSim.home;
        }
        this.workRelatedDest = hostedSim.office;
        Debug.Assert(!(this.hostedSim.isUnfinishedPCRQuota == true && this.hostedSim.infoManager.testManager.isActivePCRTestEvent()==false), "bug here");
        if(this.hostedSim.infoManager.testManager.isActivePCRTestEvent()){
            if(this.hostedSim.isUnfinishedPCRQuota==true){
                // 又active的测试event 并且自己还没有测试过的话，就试图拿号或者直接去测试
                AttemptToPCRTest(); // 早上拿个号
            }
        }
        AfterUpdateCheck();
    }

    // 晚上去哪里？
    public void UpdateScheduleOnDusk(){
        if(leisureRelatedDest){
            this.leisureRelatedDest = this.hostedSim.home;
        }
        if(workRelatedDest){
            this.workRelatedDest = this.hostedSim.home;
        }
        AfterUpdateCheck();
        // 比如说这里已经确定了回家的路线，现在来看看有没有PCR的东西，如果有的话就做完PCR再回到原定路线
        // AttemptToPCRTest();
    }
    public void UpdateScheduleOnTestQueueCall(TestCenterPlace testPlace){
        this.pcrTestRelatedDest = testPlace;
        testPlace.Booking();
        AfterUpdateCheck();
    }
    public void UpdateScheduleOnTestFinished(){
        this.pcrTestRelatedDest = null;
        AfterUpdateCheck();
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
                        this.hostedSim.infoManager.notificationManager.SendHospitalFullNotification();
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
        AfterUpdateCheck();
    }
    public void UpdateScheduleOnPolicyChanged(){
        // read policy
    }
    public void UpdateScheduleOnInfectionKowledgeChange(){
        // read policy
    }
    public void AttemptToPCRTest(){
        // 请确保调用这个方法之前，已经设计好了（原定目的地）
        // 例如已经设置好了下班的去向
        // 检查是不是有PCRTest的位置
        // 如果有的话马上去，然后返回下一个级别的本来的目的地


        // 现在有一个严重的问题，就是PCRTest的预定的位置是取决于现在有没有人的，我们需要改成预定了多少个人。
        TestManager testManager = this.hostedSim.infoManager.testManager;
        if(testManager.currentTestEvent == null) return;
        Debug.Assert(testManager.currentTestEvent != null, "bug here");
        TestEvent testEvent =  testManager.currentTestEvent;
        TestPolicy testPolicy = testManager.currentTestEvent.testPolicy;

        // 试图获得一个testPosition, 成功或者失败根据Policy有不同的处理结果
        PlaceManager placeManager = hostedSim.placeManager;
        // placeManager.testCenrePlaces;
        // TestCenrePlace testCentrePlace = placeManager.GetAvailableOrQueueTestPlace();
        TestCenterPlace testCentrePlace = testManager.GetOrQueueTestPlace(this.hostedSim);

        if(testCentrePlace == null){
            // 全满, 意味着自己现在已经在排队了
            this.hostedSim.infoManager.notificationManager.SendTestCenterFullNotification();
            if(testPolicy == TestPolicy.Hard){
                pcrTestRelatedDest = hostedSim.home;
            }else if(testPolicy == TestPolicy.Soft){
                pcrTestRelatedDest = null;
            }
        }else{
            // 不用排队，有直接的位置可以坐
            testCentrePlace.Booking();
            pcrTestRelatedDest = testCentrePlace;
        }
        AfterUpdateCheck();
        // this.hostedSim.infoManager.testManager
    }

    public void UpdateSimScheduleAfterTest(){
        TestCenterPlace testcentre = this.pcrTestRelatedDest as TestCenterPlace;
        // testcentre.ReleaseBooking();
        this.pcrTestRelatedDest = null;
        AfterUpdateCheck();
    }

    public void UpdateScheduleOnFeeUnaffordable(){
        // 当Sims因为非生活费的原因而破产的时候，就会触发这个函数
        Debug.Assert(hostedSim.inSite is MedicalPlace,"Sims 破产了，但是它却不再医院里面（理论上只有医院才能导致破产）");
        if(this.hostedSim.inSite is MedicalPlace){
            this.personalMedicalDest = null;
            return;
        }
        AfterUpdateCheck();
    }

    public Place GetDestination(KeyTime keyTime){

        // 为什么要传入一个keyTime？ 这是因为有一些东西只用Insert到Diary一次

        Place destination = null;
        // DestType destType = DestType.None;

        if(qurantineDest != null){
            destination = qurantineDest;
            // destType = DestType.GovQurantine;
        }else if(pcrTestRelatedDest != null){
            destination = pcrTestRelatedDest;
        }
        else if(lockDownDest != null){
            destination =  lockDownDest;
            if(keyTime == KeyTime.Morning){ // 为什么只在morning做？因为通知只要一次就可以了
             this.hostedSim.simDiary.AppendDiaryItem(
            new SimsDiaryItem(
                this.hostedSim.timeManager.GetTime(),
                SimBehaviorDetial.LockedDownAtHomeEvent()));
            }
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
            // Debug.Assert(destination != null);
            Debug.Log("No where to go, just go back home");
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

    public void FlushDest(){
        // 今天去不成也不要把这个念想留着，明天再继续尝试！！！！
        workRelatedDest = null;
        leisureRelatedDest = null;
        qurantineDest = null;
        lockDownDest = null;
        personalMedicalDest = null;
    }

    public void AfterUpdateCheck(){
        bool orNull =  workRelatedDest != null ||
           leisureRelatedDest != null ||
           qurantineDest != null ||
           lockDownDest != null ||
           personalMedicalDest != null;
        // Debug.Assert(orNull == true, "After schedule update check not passed");
    }

}
