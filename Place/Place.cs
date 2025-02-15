using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


public class Place : MonoBehaviour
{
    public string palaceName = "default";
    public Vector2Int placeShape;
    public Vector2Int placeLLAnchor;
    public Vector2Int placeURAnchor;
    public FlowFieldMapManager flowFieldMapsManager;

    public void PlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        string placeName,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj
        )
    {
        this.palaceName = placeName;
        this.placeShape = placeShape;
        this.placeLLAnchor = basePosition;
        this.placeURAnchor = basePosition + placeShape; // 确保边界正确
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.size = placeShape;
        }
        transform.position = new Vector3(basePosition.x, basePosition.y, 0);
        // flow field map Manager
        this.flowFieldMapsManager = gameObject.AddComponent<FlowFieldMapManager>();
        this.flowFieldMapsManager.FlowFieldMapsManagerInit(
            basePosition, 
            "placeholder",
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj
            );
    }

    public Vector2 GetRandomPositionInside()
    {
        return new Vector2(
            Random.Range(placeLLAnchor.x + 0.1f, placeURAnchor.x - 0.1f), 
            Random.Range(placeLLAnchor.y + 0.1f, placeURAnchor.y - 0.1f)
        );
    }


    public override string ToString()
    {
        return palaceName + " " + placeLLAnchor.ToString() + " " + placeURAnchor.ToString();
    }
}

