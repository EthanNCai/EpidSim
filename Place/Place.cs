using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class Place : MonoBehaviour
{
    public int uid;
    public int volumePerTile = 10;
    public string palaceName = "default";
    public Vector2Int placeShape;
    public Vector2Int placeLLAnchor;
    public Vector2Int placeURAnchor;
    public Vector2Int basePosition;
    public FlowFieldMapManager flowFieldMapsManager;
    public List<Sims> relevantSims = new List<Sims>();

    public void PlaceInit(
        Vector2Int placeShape, 
        Vector2Int basePosition, 
        string placeName,
        MapManager mapManager,
        GameObject flowFieldRootObject,
        GameObject geoMapManagerObj
        )
    {
        this.uid = UniqueIDGenerator.GetUniqueID();
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
        // adjust geomap before generating FlowFieldMap
        GeoMapsManager geoMapManager = geoMapManagerObj.GetComponent<GeoMapsManager>();
        this.SetUpGeoMapBlocked(geoMapManager);
        geoMapManager.InvokeMapChangedEvent();
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

    private bool CheckIsAvailable(){
        int tiles = this.placeShape.x * this.placeShape.y;
        int volume = tiles * volumePerTile;
        if (this.relevantSims.Count < volume){
            return true;   
        }else{
            return false;
        }
    }

    public bool InsertRelevantSimsWithAvailabilityCheck(Sims incomingSim){
        if (CheckIsAvailable()){
            this.relevantSims.Add(incomingSim);   
            return true;
        }else{
            return false;
        }
    }

    public void InsertRelevantSims(Sims incomingSim){
        this.relevantSims.Add(incomingSim);   
    }

    public void SetUpGeoMapBlocked(GeoMapsManager geoMapManager){
        for ( int x = placeLLAnchor.x; x < placeURAnchor.x; x++){
            for (int y = placeLLAnchor.y; y < placeURAnchor.y; y++){
                // if (x == placeLLAnchor.x && y == placeLLAnchor.y){
                //     continue;
                // }else{
                    geoMapManager.geoMap.GetNodeByCellPosition(new Vector2Int(x, y)).SetBlocked(true);
                // }
            }
        }
    }

    public override string ToString()
    {
        return palaceName + " " + placeLLAnchor.ToString() + " " + placeURAnchor.ToString();
    }
}

