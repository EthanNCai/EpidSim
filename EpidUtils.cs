using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
public class Utils
{
    public static TextMesh SpawnTextAtRelativePosition(GameObject parent, Vector2 relativePosition, string text)
    {
        Debug.Assert(parent != null, "Parent object cannot be null");
        return UtilsClass.CreateWorldText(text, parent.transform, relativePosition, 150, Color.white, TextAnchor.MiddleCenter);
    }
    public static bool IsPointInsideArea(Vector2 point, Vector2Int bottomLeft, Vector2Int topRight)
    {
        int minX = Mathf.Min(bottomLeft.x, topRight.x);
        int maxX = Mathf.Max(bottomLeft.x, topRight.x);
        int minY = Mathf.Min(bottomLeft.y, topRight.y);
        int maxY = Mathf.Max(bottomLeft.y, topRight.y);

        return point.x >= minX && point.x <= maxX &&
            point.y >= minY && point.y <= maxY;
    }

    public static Vector2 GetRandomizedDirection(Vector2Int originalDirection, float temperature)
    {
        Vector2 direction = originalDirection; 
        float randomX = Random.Range(-temperature, temperature);
        float randomY = Random.Range(-temperature, temperature);
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
