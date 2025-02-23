using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VirusVolumeGridMapManager : MonoBehaviour
{
    public MapManager mapManager;
    public GridNodeMap<VirusVolumeNode> virusVolumeMap = null;
    public static float selfSanitizeIndex = 0.01f;
    public GridDebugManager gridDebuggerManager;
    public GameObject geoMapManagerObj;
    private GeoMapsManager geoMapManager;

    public void Awake(){
        // Debug.LogError("Start called on instance ID: " + GetInstanceID());
        this.virusVolumeMap = new GridNodeMap<VirusVolumeNode>(
            "",
            1, 
            this.mapManager.mapsize,
            gridDebuggerManager.GetListedRoot("virusVolumeMap"), 
            (int v, GridNodeMap<VirusVolumeNode> gnm ,Vector2Int c) => new VirusVolumeNode(v,gnm,c));
        TimeManager.OnQuarterChanged += SelfSanitize;
    }

    public void PolluteTheTile(Vector2Int cellPosition, Sims sims, int incomingVolume){
        VirusVolumeNode virusVolumeNode = this.virusVolumeMap.GetNodeByCellPosition(cellPosition);
        if(virusVolumeMap == null) return;
        if (virusVolumeNode.virusVolumeAndSims.Item1 <= incomingVolume){
            virusVolumeNode.virusVolumeAndSims = (incomingVolume, sims);
        }
    }
    public void SelfSanitize((int,int) timeNow){
        foreach (VirusVolumeNode node in this.virusVolumeMap.nodeIterator()) {
            var (virusVolume, sims) = node.virusVolumeAndSims; 
            if(sims != null){
                virusVolume -= (int)(InfectionParams.maxVirusVolume * selfSanitizeIndex);
                if (virusVolume <= 0){
                    node.virusVolumeAndSims = (0, null);
                }else{
                    node.virusVolumeAndSims = (virusVolume, sims);
                }
                // node.virusVolumeAndSims = (virusVolume, sims);
            }
        }
    }
}
