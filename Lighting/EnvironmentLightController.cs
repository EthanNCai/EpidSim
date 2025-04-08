using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnvironmentColorManager : MonoBehaviour
{
    public TimeManager timeManager;
    public Light2D globalLight;
    
    // Colors for different times of day
    private Color nightColor;      //rgb(15, 17, 20)
    private Color morningDuskColor; // #B5A68D
    private Color noonColor;       // #9ED2FF
    
    // For smooth color interpolation
    private Color currentColor;
    
    // Smoothing factor (lower = smoother but slower)
    [Range(1f, 10f)]
    public float smoothingSpeed = 5f;
    
    void Start()
    {
        if (timeManager == null || globalLight == null)
        {
            Debug.LogError("Required references not assigned in EnvironmentColorManager!");
            return;
        }
        
        // Set colors using hex values
        nightColor = HexToColor("293144");
        morningDuskColor = HexToColor("B5A68D");
        noonColor = HexToColor("9ED2FF");
        
        // Initialize with proper color based on current time
        var targetColor = CalculateIdealColor();
        currentColor = targetColor;
        globalLight.color = targetColor;
    }
    
    void Update()
    {
        // Calculate target color based on current time
        Color targetColor = CalculateIdealColor();
        
        // Smoothly interpolate current color toward target
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * smoothingSpeed);
        
        // Apply the smoothed color
        globalLight.color = currentColor;
    }
    
    private Color CalculateIdealColor()
    {
        var (day, hour, quarter) = timeManager.GetTime();
        float timeOfDay = hour + quarter / 4f;
        
        // Define key times
        float preDawnTime = 5f;
        float morningTime = 7f;
        float noonTime = 12f;
        float eveningTime = 17f;
        float nightTime = 19f;
        
        if (timeOfDay >= nightTime || timeOfDay < preDawnTime)
        {
            // Night time - dark blue
            return nightColor;
        }
        else if (timeOfDay >= preDawnTime && timeOfDay < morningTime)
        {
            // Dawn transition: Night -> Morning/Dusk
            float t = Mathf.InverseLerp(preDawnTime, morningTime, timeOfDay);
            return Color.Lerp(nightColor, morningDuskColor, t);
        }
        else if (timeOfDay >= morningTime && timeOfDay < noonTime)
        {
            // Morning transition: Morning/Dusk -> Noon
            float t = Mathf.InverseLerp(morningTime, noonTime, timeOfDay);
            return Color.Lerp(morningDuskColor, noonColor, t);
        }
        else if (timeOfDay >= noonTime && timeOfDay < eveningTime)
        {
            // Afternoon transition: Noon -> Morning/Dusk
            float t = Mathf.InverseLerp(noonTime, eveningTime, timeOfDay);
            return Color.Lerp(noonColor, morningDuskColor, t);
        }
        else // timeOfDay >= eveningTime && timeOfDay < nightTime
        {
            // Dusk transition: Morning/Dusk -> Night
            float t = Mathf.InverseLerp(eveningTime, nightTime, timeOfDay);
            return Color.Lerp(morningDuskColor, nightColor, t);
        }
    }
    
    // Helper method to convert hex color to Unity Color
    private Color HexToColor(string hex)
    {
        // Remove # if present
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);
        
        // Parse the hex values
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            return color;
        
        Debug.LogWarning($"Failed to parse color: {hex}");
        return Color.white;
    }
}