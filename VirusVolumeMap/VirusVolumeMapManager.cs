using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VirusVolumeMapManager : MonoBehaviour
{
    public MapManager mapManager;
    public GridNodeMap<VirusVolumeNode> virusVolumeMap = null;
    public static float selfSanitizeIndex = 0.2f;
    private GameObject flowFieldRootObject;
    private GameObject geoMapManagerObj;

    private GeoMapsManager geoMapManager;
    public void VirusVolumeMapManagerInit(
        string uid,
        MapManager mapManager,
        GameObject geoMapManagerObj){
        this.geoMapManagerObj = geoMapManagerObj;
        this.mapManager = mapManager;

        this.virusVolumeMap = new GridNodeMap<VirusVolumeNode>(
            uid,
            1, 
            this.mapManager.mapsize,
            this.flowFieldRootObject, 
            (int v, GridNodeMap<VirusVolumeNode> gnm ,Vector2Int c) => new VirusVolumeNode(v,gnm,c));
    }

    public void PolluteTheTile(Vector2Int cellPosition, Sims sims, int incomingVolume){
        VirusVolumeNode virusVolumeNode = this.virusVolumeMap.GetNodeByCellPosition(cellPosition);
        if (virusVolumeNode.virusVolumeAndSims.Item1 <= incomingVolume){
            virusVolumeNode.virusVolumeAndSims = (incomingVolume, sims);
        }
    }
    public void SelfSanitize(){
        foreach (VirusVolumeNode node in this.virusVolumeMap.nodeIterator()) {
            var (virusVolume, sims) = node.virusVolumeAndSims; // 解构元组
            virusVolume -= (int)(virusVolume * selfSanitizeIndex);
            node.virusVolumeAndSims = (virusVolume, sims); // 重新赋值
        }
    }
}
