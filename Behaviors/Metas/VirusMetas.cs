using UnityEngine;

public static class InfectionParams{

    public static float infectionProbSuscptible = 1f;
    public static float infectionProbRecoverd = 0f;
    public static int maxVirusVolume = 100;
    public static int minVirusVolume = 0;
    public static float deathProb = 0.9f;

    public static (int,int) periodADurationRanges = (1, 5);
    public static (int,int) periodBDurationRanges = (2, 3);
    public static (int,int) periodCDurationRanges = (1, 3);

    public static bool RollTheInfectionDice(int volumeExposed, InfectionStatus status)
    {
        Debug.Assert(status == InfectionStatus.Suscptible || status == InfectionStatus.Recovered, "Infection Dice Error");
        float exposedRatio = (1.0f * volumeExposed) / (1.0f * maxVirusVolume);

        switch (status){
            case InfectionStatus.Suscptible:
            // Debug.Log("proab" + exposedRatio * infectionProbSuscptible);
                return UnityEngine.Random.Range(0f, 1f) <= exposedRatio * infectionProbSuscptible;
            case InfectionStatus.Recovered:
                return UnityEngine.Random.Range(0f, 1f) <= exposedRatio * infectionProbRecoverd; 
            default:
                return false;   
        }
    }

    public static bool RollTheDeathDice(float deathProab){
        return UnityEngine.Random.Range(0f, 1f) < deathProab;
    }

    public static (int,int,int) GetInfectionPeriod(){
        int periodA = UnityEngine.Random.Range(periodADurationRanges.Item1,periodADurationRanges.Item2);
        int periodB = UnityEngine.Random.Range(periodBDurationRanges.Item1,periodBDurationRanges.Item2);
        int periodC = UnityEngine.Random.Range(periodCDurationRanges.Item1,periodCDurationRanges.Item2);
        return (periodA,periodB,periodC); 
    }
}