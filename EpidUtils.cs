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
