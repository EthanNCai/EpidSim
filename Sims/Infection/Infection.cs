using System;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum InfectionPeriod
{
    PeriodA,
    PeriodB,
    PeriodC,
}
public enum InfectionStatus{
    Recovered,
    Suscptible,
    Infected,
    Dead,
}

public static class InfectionParams{

    public static float infectionProbSuscptible = 1f;
    public static float infectionProbRecoverd = 0f;
    public static int maxVirusVolume = 100;
    public static int minVirusVolume = 0;
    public static float deathProb = 0.01f;

    public static (int,int) periodADurationRanges = (1, 5);
    public static (int,int) periodBDurationRanges = (2, 3);
    public static (int,int) periodCDurationRanges = (1, 3);

    public static bool RollTheInfectionDice(int volumeExposed, InfectionStatus status)
    {
        Debug.Assert(status == InfectionStatus.Suscptible || status == InfectionStatus.Recovered, "Infection Dice Error");
        float exposedRatio = volumeExposed / maxVirusVolume;

        switch (status){
            case InfectionStatus.Suscptible:
                return UnityEngine.Random.Range(0f, 1f) < exposedRatio * infectionProbSuscptible;
            case InfectionStatus.Recovered:
                return UnityEngine.Random.Range(0f, 1f) < exposedRatio * infectionProbRecoverd; 
            default:
                return false;   
        }
    }

    public static bool RollTheDeathDice(){
        return UnityEngine.Random.Range(0f, 1f) < deathProb;
    }

    public static (int,int,int) GetInfectionPeriod(){
        int periodA = UnityEngine.Random.Range(periodADurationRanges.Item1,periodADurationRanges.Item2);
        int periodB = UnityEngine.Random.Range(periodBDurationRanges.Item1,periodBDurationRanges.Item2);
        int periodC = UnityEngine.Random.Range(periodCDurationRanges.Item1,periodCDurationRanges.Item2);
        return (periodA,periodB,periodC); 
    }
}

class Infection{
    public InfectionPeriod currentInfectionPeriod;
    public int periodADayLen;
    public int periodBDayLen;
    public int periodCDayLen;
    public int currentPeriodDaysLeft;
    public int virusVolume = 0;
    public bool endWithDead;

    public Infection((int,int,int) infectionPeriod, Sims infectedBy){
        int periodA = infectionPeriod.Item1;
        int periodB = infectionPeriod.Item2;
        int periodC =  infectionPeriod.Item3;
        periodADayLen = periodA;
        periodBDayLen = periodB;
        periodCDayLen = periodC;
        currentPeriodDaysLeft = periodA;
    }

    public InfectionStatus Progress()
    {
        switch (currentInfectionPeriod)
        {
            case InfectionPeriod.PeriodA:
                virusVolume = (int)Math.Round((100f / periodADayLen) * (periodADayLen - currentPeriodDaysLeft));
                if (currentPeriodDaysLeft <= 0)
                {
                    // PeriodA结束，进入PeriodB
                    currentInfectionPeriod = InfectionPeriod.PeriodB;
                    currentPeriodDaysLeft = periodBDayLen;
                    break;
                }
                // PeriodA的病毒体积从0到100逐渐增加
                currentPeriodDaysLeft--;
                break;

            case InfectionPeriod.PeriodB:
                if (currentPeriodDaysLeft <= 0)
                {
                    // PeriodB结束，决定是否死亡或进入PeriodC
                    if (InfectionParams.RollTheDeathDice())
                    {
                        return InfectionStatus.Dead;
                    }
                    else
                    {
                        currentInfectionPeriod = InfectionPeriod.PeriodC;
                        currentPeriodDaysLeft = periodCDayLen;
                        break;
                    }
                }
                // PeriodB的病毒体积保持为100
                currentPeriodDaysLeft--;
                break;

            case InfectionPeriod.PeriodC:
                virusVolume = (int)Math.Round((100f / periodCDayLen) * currentPeriodDaysLeft);

                if (currentPeriodDaysLeft < 0)
                {
                    // PeriodC结束，恢复
                    return InfectionStatus.Recovered;
                }
                // PeriodC的病毒体积从100减少到0
                currentPeriodDaysLeft--;
                break;

            default:
                break;
        }
        return InfectionStatus.Infected;
        
    }
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append("status ").Append(this.currentInfectionPeriod)
            .Append(", days left: ").Append(this.currentPeriodDaysLeft).Append("vol: ").Append(this.virusVolume);
        return builder.ToString();
    }
}