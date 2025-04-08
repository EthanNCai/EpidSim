using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SunController : MonoBehaviour
{
    public TimeManager timeManager;
    public Light2D sunLight;
    public MapManager mapManager;

    private Vector3 mapCenter;
    private float mapWidth;
    private float mapHeight;
    private float constantY;
    
    // Light intensity values
    private float baseDayIntensity = 1.5f;
    private float noonIntensity = 2.5f;
    private float nightIntensity = 0.0f;
    
    // Shadow strength values
    private float noonShadowStrength = 0.5f;
    private float midnightShadowStrength = 1.0f;

    // For smooth interpolation
    private Vector3 currentPosition;
    private float currentIntensity;
    private float currentShadowStrength;
    
    // To track teleportation
    private bool isTeleporting = false;
    private float teleportTime = 0f;
    private float teleportDuration = 0.5f; // How long to keep light off during teleport

    // Smoothing factor (lower = smoother but slower)
    [Range(1f, 10f)]
    public float smoothingSpeed = 5f;

    // Time tracking for last position
    private float lastTimeOfDay = 0f;

    void Start()
    {
        if (mapManager == null || sunLight == null || timeManager == null)
        {
            Debug.LogError("Required references not assigned in SunController!");
            return;
        }

        // Get map dimensions
        mapCenter = mapManager.mapCenter;
        mapWidth = mapManager.mapsize.x;
        mapHeight = mapManager.mapsize.y;
        
        // Set constant Y position
        constantY = mapCenter.y + mapHeight;
        
        // Get current time
        var (day, hour, quarter) = timeManager.GetTime();
        lastTimeOfDay = hour + quarter / 4f;
        
        // Initialize sun position
        var targetPosition = CalculateIdealPosition(lastTimeOfDay);
        currentPosition = targetPosition;
        sunLight.transform.position = targetPosition;
        
        // Initialize intensity
        var targetIntensity = CalculateIdealIntensity(lastTimeOfDay);
        currentIntensity = targetIntensity;
        sunLight.intensity = targetIntensity;
        
        // Initialize shadow strength
        var targetShadowStrength = CalculateIdealShadowStrength(lastTimeOfDay);
        currentShadowStrength = targetShadowStrength;
        sunLight.shadowIntensity = targetShadowStrength;
    }

    void Update()
    {
        // Get current time
        var (day, hour, quarter) = timeManager.GetTime();
        float timeOfDay = hour + quarter / 4f;

        // Check for day wrap (e.g., from 23:45 to 0:00)
        // This could also catch time jumps if your game has time skipping mechanics
        if (timeOfDay < lastTimeOfDay && (lastTimeOfDay - timeOfDay) > 12)
        {
            // We've crossed midnight or had a time jump
            StartTeleportation();
        }
        
        // Handle teleportation state
        if (isTeleporting)
        {
            teleportTime += Time.deltaTime;
            
            // Keep light off during teleportation
            sunLight.intensity = 0;
            
            if (teleportTime >= teleportDuration)
            {
                // Teleportation complete
                isTeleporting = false;
                teleportTime = 0f;
                
                // Immediately set position to the correct place without smoothing
                currentPosition = CalculateIdealPosition(timeOfDay);
                sunLight.transform.position = currentPosition;
            }
            
            lastTimeOfDay = timeOfDay;
            return; // Skip normal update during teleportation
        }
        
        // Normal update (not teleporting)
        Vector3 targetPosition = CalculateIdealPosition(timeOfDay);
        float targetIntensity = CalculateIdealIntensity(timeOfDay);
        float targetShadowStrength = CalculateIdealShadowStrength(timeOfDay);
        
        // Check if we need to teleport (if position changed dramatically)
        if (Vector3.Distance(targetPosition, currentPosition) > mapWidth)
        {
            StartTeleportation();
            sunLight.intensity = 0; // Immediately turn off light
            lastTimeOfDay = timeOfDay;
            return;
        }
        
        // Smoothly interpolate current position, intensity and shadow strength toward target
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * smoothingSpeed);
        currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothingSpeed);
        currentShadowStrength = Mathf.Lerp(currentShadowStrength, targetShadowStrength, Time.deltaTime * smoothingSpeed);
        
        // Apply the smoothed values
        sunLight.transform.position = currentPosition;
        sunLight.intensity = currentIntensity;
        sunLight.shadowIntensity = currentShadowStrength;
        
        // Update time tracking
        lastTimeOfDay = timeOfDay;
    }

    private void StartTeleportation()
    {
        isTeleporting = true;
        teleportTime = 0f;
        sunLight.intensity = 0f; // Immediately turn off light
    }

    private Vector3 CalculateIdealPosition(float timeOfDay)
    {
        // Define key times
        float sunriseTime = 7f;
        float noonTime = 12f;
        float sunsetTime = 17f;
        float nightTime = 19f;
        
        // Default X position (for night)
        float xPosition = mapCenter.x - mapWidth * 0.75f;
        
        if (timeOfDay >= sunriseTime && timeOfDay <= noonTime)
        {
            // Morning to noon: left side to center
            float t = Mathf.InverseLerp(sunriseTime, noonTime, timeOfDay);
            xPosition = Mathf.Lerp(mapCenter.x - mapWidth * 0.75f, mapCenter.x, t);
        }
        else if (timeOfDay > noonTime && timeOfDay <= sunsetTime)
        {
            // Noon to sunset: center to right side
            float t = Mathf.InverseLerp(noonTime, sunsetTime, timeOfDay);
            xPosition = Mathf.Lerp(mapCenter.x, mapCenter.x + mapWidth * 0.75f, t);
        }
        else if (timeOfDay > sunsetTime && timeOfDay < nightTime)
        {
            // Sunset to night: right side to far right (gradually disappearing)
            float t = Mathf.InverseLerp(sunsetTime, nightTime, timeOfDay);
            xPosition = Mathf.Lerp(mapCenter.x + mapWidth * 0.75f, mapCenter.x + mapWidth * 1.5f, t);
        }
        else if (timeOfDay >= nightTime || timeOfDay < 5f)
        {
            // Night time - sun is at the left side, out of view
            xPosition = mapCenter.x - mapWidth * 1.5f;
        }
        else if (timeOfDay >= 5f && timeOfDay < sunriseTime)
        {
            // Before sunrise (5-7): far left to left side (gradually appearing)
            float t = Mathf.InverseLerp(5f, sunriseTime, timeOfDay);
            xPosition = Mathf.Lerp(mapCenter.x - mapWidth * 1.5f, mapCenter.x - mapWidth * 0.75f, t);
        }
        
        // Always use the constant Y position
        return new Vector3(xPosition, constantY, 0);
    }

    private float CalculateIdealIntensity(float timeOfDay)
    {
        // Define key times
        float sunriseStartTime = 5f;
        float sunriseTime = 7f;
        float noonTime = 12f;
        float sunsetTime = 17f;
        float nightTime = 19f;
        
        if (timeOfDay >= sunriseStartTime && timeOfDay < sunriseTime)
        {
            // Dawn: gradually increase intensity from night to base day
            float t = Mathf.InverseLerp(sunriseStartTime, sunriseTime, timeOfDay);
            return Mathf.Lerp(nightIntensity, baseDayIntensity, t);
        }
        else if (timeOfDay >= sunriseTime && timeOfDay <= noonTime)
        {
            // Morning to noon: gradually increase from base to peak intensity
            float t = Mathf.InverseLerp(sunriseTime, noonTime, timeOfDay);
            return Mathf.Lerp(baseDayIntensity, noonIntensity, t);
        }
        else if (timeOfDay > noonTime && timeOfDay <= sunsetTime)
        {
            // Noon to sunset: gradually decrease from peak to base intensity
            float t = Mathf.InverseLerp(noonTime, sunsetTime, timeOfDay);
            return Mathf.Lerp(noonIntensity, baseDayIntensity, t);
        }
        else if (timeOfDay > sunsetTime && timeOfDay < nightTime)
        {
            // Dusk: gradually decrease from base intensity to night
            float t = Mathf.InverseLerp(sunsetTime, nightTime, timeOfDay);
            return Mathf.Lerp(baseDayIntensity, nightIntensity, t);
        }
        else
        {
            // Night: minimum intensity
            return nightIntensity;
        }
    }
    
    private float CalculateIdealShadowStrength(float timeOfDay)
    {
        // Calculate time-based progress through the day
        // Midnight (0) to noon (12) - shadows go from strongest to weakest
        // Noon (12) to midnight (24) - shadows go from weakest to strongest
        float normalizedTime;
        
        if (timeOfDay >= 0 && timeOfDay <= 12)
        {
            // Morning to noon: shadows getting less harsh
            normalizedTime = timeOfDay / 12f; // 0 to 1
            return Mathf.Lerp(midnightShadowStrength, noonShadowStrength, normalizedTime);
        }
        else // timeOfDay > 12 && timeOfDay < 24
        {
            // Afternoon to midnight: shadows getting harsher
            normalizedTime = (timeOfDay - 12) / 12f; // 0 to 1
            return Mathf.Lerp(noonShadowStrength, midnightShadowStrength, normalizedTime);
        }
    }
}