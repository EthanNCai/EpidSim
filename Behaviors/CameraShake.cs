using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;

    private float currentDuration = 0f;
    private Vector3 originalPos;
    private bool duringShake = false;
    // void Start()
    // {
    //     originalPos = transform.localPosition;
    // }

    void Update()
    {
        if (currentDuration > 0.1 && duringShake)
        {
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            currentDuration -= Time.deltaTime;
        }
        else if(duringShake)
        {
            transform.localPosition = originalPos;
            currentDuration = 0;
            duringShake = true;
        }
    }

    public void TriggerShake()
    {
        originalPos = transform.localPosition;
        currentDuration = shakeDuration;
        duringShake = true;
    }
}
