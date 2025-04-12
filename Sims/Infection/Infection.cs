using System;
using System.Text;
using System.Xml.Serialization;
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

public enum SicknessTag{
    Normal,
    Mild,
    Moderate,
    Severe,
    Critical,
}
public static class SicknessTagConverter{
    public static SicknessTag GetSicknessTag(float sickness){
        switch(sickness){
            case < 0.1f:
                return SicknessTag.Normal;
            case < 0.2f:
                return SicknessTag.Mild;
            case < 0.5f:
                return SicknessTag.Moderate;
            case < 0.7f:
                return SicknessTag.Severe;
            default:
                return SicknessTag.Critical;
        }
    }
    public static string SicknessTagToDescription(SicknessTag sicknessTag){
        switch(sicknessTag){
            case SicknessTag.Normal:
                return "Nothing feels wrong today";
            case SicknessTag.Mild:
                return "I felt a tiny bit uncomfortable";
            case SicknessTag.Moderate:
                return "Something doesn't feel right today";
            case SicknessTag.Severe:
                return "I felt a noticable uncomfortable";
            default:
                return "unbearable pain happend to me today";
        }
    }
}


public class Infection
{
    public InfectionPeriod currentInfectionPeriod;
    public int periodADayLen;
    public int periodBDayLen;
    public int periodCDayLen;
    public int currentPeriodDaysLeft;
    public int virusVolume = 0;
    public bool endWithDead;
    StringBuilder stringBuilder = new StringBuilder();

    Sims hostedSims;

    public Infection((int, int, int) infectionPeriod, Sims hostedSims)
    {
        int periodA = infectionPeriod.Item1;
        int periodB = infectionPeriod.Item2;
        int periodC = infectionPeriod.Item3;
        periodADayLen = periodA;
        periodBDayLen = periodB;
        periodCDayLen = periodC;
        currentPeriodDaysLeft = periodA;
        this.hostedSims = hostedSims;
    }

    public InfectionStatus Progress(bool inHospital = false)
    {
        float deathDiceMultiplier = inHospital ? 0.5f : 1f; // 50% death chance in hospital
        float virusGrowthMultiplier = inHospital ? 1.5f : 1f; // 150% virus progression in hospital

        switch (currentInfectionPeriod)
        {
            case InfectionPeriod.PeriodA:
                virusVolume = (int)Math.Round((100f / periodADayLen) * (periodADayLen - currentPeriodDaysLeft) * virusGrowthMultiplier);
                virusVolume = Mathf.Max(virusVolume, 0); // Ensure virusVolume never goes below 0
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
                    if (InfectionParams.RollTheDeathDice(InfectionParams.deathProb * deathDiceMultiplier))
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
                virusVolume = (int)Math.Round((100f / periodCDayLen) * currentPeriodDaysLeft * virusGrowthMultiplier);
                virusVolume = Mathf.Max(virusVolume, 0); // Ensure virusVolume never goes below 0

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
        stringBuilder.Clear();
        stringBuilder.Append("status ").Append(this.currentInfectionPeriod)
            .Append(", days left: ").Append(this.currentPeriodDaysLeft)
            .Append(" vol: ").Append(this.virusVolume);
        return stringBuilder.ToString();
    }

    public void CalculateSeverity()
    {
        float sevirity = this.hostedSims.infoManager.virusManager.GetVirusSeverity();
        float sickness = sevirity * this.virusVolume * 0.01f;
    }
}
