using System;
using System.Text;
using UnityEngine;

public class VirusManager: MonoBehaviour
{
    private float _infectiousnessIndex = 0.5f;
    private float _severityIndex = 0.0f;
    private float _lethalityIndex = 0.0f;
    private float _oscillationIntensity = 0.5f;
    private static System.Random _random = new System.Random();
    private GameObject VirusDebugInfoRoot;
    private TextMesh debugInfoText;

    public StringBuilder stringBuilder = new StringBuilder();

    public void SetupDebug(GameObject debugRoot){
        this.VirusDebugInfoRoot = debugRoot;
        this.debugInfoText = Utils.SpawnTextAtRelativePosition(this.VirusDebugInfoRoot, new Vector2Int(1,10), "uninitialized debug text for cash flow manager.");
        this.debugInfoText.text = GenerateVirusRepr();
    }

    public void Awake()
    {
        TimeManager.OnDayChanged += this.Evolve; 
    }

    public void Evolve(int day)
    {
        double trendShift = _random.NextDouble();
        float randomChange = (float)(_random.NextDouble() * (0.2 * _oscillationIntensity - 0.05 * _oscillationIntensity) + 0.05 * _oscillationIntensity);

        if (trendShift < 0.33)
        {
            _infectiousnessIndex = Math.Min(_infectiousnessIndex + randomChange, 1.0f);
        }
        else if (trendShift < 0.66)
        {
            _severityIndex = Math.Min(_severityIndex + randomChange, 1.0f);
        }
        else
        {
            _lethalityIndex = Math.Min(_lethalityIndex + randomChange, 1.0f);
        }

        IncreaseSeverityOverall();

        float totalIndex = _infectiousnessIndex + _severityIndex + _lethalityIndex;
        if (totalIndex > 2.0f)
        {
            float excess = totalIndex - 2.0f;
            _infectiousnessIndex -= excess * (_infectiousnessIndex / totalIndex);
            _severityIndex -= excess * (_severityIndex / totalIndex);
            _lethalityIndex -= excess * (_lethalityIndex / totalIndex);
        }
        // update debug info
        this.debugInfoText.text = GenerateVirusRepr();
    }

    private void IncreaseSeverityOverall()
    {
        _infectiousnessIndex = Math.Min(_infectiousnessIndex + (float)(_random.NextDouble() * (0.05 * _oscillationIntensity - 0.02 * _oscillationIntensity) + 0.02 * _oscillationIntensity), 1.0f);
        _severityIndex = Math.Min(_severityIndex + (float)(_random.NextDouble() * (0.05 * _oscillationIntensity - 0.02 * _oscillationIntensity) + 0.02 * _oscillationIntensity), 1.0f);
        _lethalityIndex = Math.Min(_lethalityIndex + (float)(_random.NextDouble() * (0.05 * _oscillationIntensity - 0.02 * _oscillationIntensity) + 0.02 * _oscillationIntensity), 1.0f);
    }
    private string GenerateVirusRepr(){
        stringBuilder.Clear();
        stringBuilder.Append("Virus Status: \n");
        stringBuilder.Append("Infectiousness: " );
        stringBuilder.Append(this._infectiousnessIndex);
        stringBuilder.Append("Severity: ");
        stringBuilder.Append(this._severityIndex);
        stringBuilder.Append("Lethality: ");
        stringBuilder.Append(this._lethalityIndex);
        return stringBuilder.ToString();
    }


    public (float infectiousness, float severity, float lethality) GetStatus()
    {
        return (_infectiousnessIndex, _severityIndex, _lethalityIndex);
    }
    public float GetVirusSeverity(){
        return _severityIndex;
    }
    public float GetVirusInffectiousness(){
        return _infectiousnessIndex;
    }
    public float GetVirusLethality(){
        return _lethalityIndex;
    }
}
