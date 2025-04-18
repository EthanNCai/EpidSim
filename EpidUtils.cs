using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
public class Utils
{
    public static TextMesh SpawnTextAtRelativePosition(GameObject parent, Vector2 relativePosition, string text)
    {
        Debug.Assert(parent != null, "Parent object cannot be null");
        return UtilsClass.CreateWorldText(text, parent.transform, relativePosition, 150, Color.white, TextAnchor.MiddleCenter);
    }
    public static bool IsPointInsideArea(Vector2 point, Vector2 bottomLeft, Vector2 topRight, float tolerance = 0f)
    {
        float minX = Mathf.Min(bottomLeft.x, topRight.x) - tolerance;
        float maxX = Mathf.Max(bottomLeft.x, topRight.x) + tolerance;
        float minY = Mathf.Min(bottomLeft.y, topRight.y) - tolerance;
        float maxY = Mathf.Max(bottomLeft.y, topRight.y) + tolerance;

        return point.x >= minX && point.x <= maxX &&
            point.y >= minY && point.y <= maxY;
    }


    public static Vector2 GetRandomizedDirection(Vector2Int originalDirection, float temperature)
    {
        Vector2 direction = originalDirection; 
        float randomX = UnityEngine.Random.Range(-temperature, temperature);
        float randomY = UnityEngine.Random.Range(-temperature, temperature);
        Vector2 noise = new Vector2(randomX, randomY);
        direction += noise;
        direction.Normalize();
        return direction;
    }
}
public static class Directions
{
    public static readonly Vector2Int up = new Vector2Int(0, 1);
    public static readonly Vector2Int right = new Vector2Int(1, 0);
    public static readonly Vector2Int down = new Vector2Int(0, -1);
    public static readonly Vector2Int left = new Vector2Int(-1, 0);
    public static readonly Vector2Int upRight = new Vector2Int(1, 1);
    public static readonly Vector2Int downRight = new Vector2Int(1, -1);
    public static readonly Vector2Int upLeft = new Vector2Int(-1, 1);
    public static readonly Vector2Int downLeft = new Vector2Int(-1, -1);
}

public static class UniqueIDGenerator
{
    private static int currentId = 0;
    public static int GetUniqueID(){
        return ++currentId;
    }
    public static void Reset(){
        currentId = 0;
    }
}


public static class RandomManager
{
    private static readonly System.Random random = new System.Random();

    public static int NextInt(int min, int max)
    {
        return random.Next(min, max);
    }

    public static int NextGaussianInt(int mean, int stdDev, int min, int max)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        int result = (int)Math.Round(mean + stdDev * randStdNormal);
        return Math.Clamp(result, min, max);
    }

    public static bool FlipTheCoin(double probability)
    {
        Debug.Assert(probability >= 0 && probability <= 1, $"Probability is out of range {probability}");
        return random.NextDouble() < Math.Clamp(probability, 0.0, 1.0);
    }

    public static HashSet<int> GetRandomDayOff()
    {
        int[] weightedDays = { 0, 1, 2, 3, 4, 5, 5, 5, 6, 6, 6 };
        HashSet<int> dayOffSet = new HashSet<int>();

        while (dayOffSet.Count < 2)
        {
            dayOffSet.Add(weightedDays[random.Next(weightedDays.Length)]);
        }

        return dayOffSet;
    }

    public static T Choice<T>(List<T> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("List cannot be null or empty.");
        }

        return items[random.Next(items.Count)];
    }

    public static float Get0toN(double n)
    {
        Debug.Assert(n > 0 && n < 1, $"n should be between 0 and 1, but got {n}");
        return (float)(random.NextDouble() * n);
    }
}

