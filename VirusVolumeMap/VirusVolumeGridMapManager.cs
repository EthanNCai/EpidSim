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
    public static float selfSanitizeIndex = 0.1f;
    public GridInfoManager gridInfoManager;
    public GameObject geoMapManagerObj;
    private GeoMapsManager geoMapManager;
    public GameObject virusVolumeVisualizerPrefab;

    public void Awake(){
        this.virusVolumeMap = new GridNodeMap<VirusVolumeNode>(
            "",
            1, 
            this.mapManager.mapsize,
            gridInfoManager.GetListedRoot("virusVolumeMap"), 
            (int v, GridNodeMap<VirusVolumeNode> gnm ,Vector2Int c) => new VirusVolumeNode(v,gnm,c));
            
        TimeManager.OnQuarterChanged += SelfSanitize;
        TimeManager.OnKeyTimeChanged += HandleKeyTimeChanged;

        // 生成病毒体积可视化器
        Vector2Int mapSize = mapManager.mapsize;
        Transform parentTransform = mapManager.mapRoot.transform;

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                VirusVolumeNode node = this.virusVolumeMap.GetNodeByCellPosition(new Vector2Int(x,y));
                Vector3 cellPos = new Vector3(x, y,0);
                // Vector3 worldPos = mapManager.CellToWorld(cellPos);
                GameObject instance = Instantiate(virusVolumeVisualizerPrefab, cellPos, Quaternion.identity, parentTransform);
                node.visualizer = instance;
                instance.name = $"VirusVisualizer_{x}_{y}";
            }
        }
    }


    public void PolluteTheTile(Vector2Int cellPosition, Sims sims, int incomingVolume){
        VirusVolumeNode virusVolumeNode = this.virusVolumeMap.GetNodeByCellPosition(cellPosition);
        if(virusVolumeNode == null) return;
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
    public void HandleKeyTimeChanged(KeyTime keytime){
        if(keytime == KeyTime.Noon || keytime == KeyTime.Dusk){
            ScanTheVisualizerMap();
        }
    }

    public void ScanTheVisualizerMap(){
        // 获取一份时点环境报告
        foreach (VirusVolumeNode node in this.virusVolumeMap.nodeIterator()) {
            var (virusVolume, sims) = node.virusVolumeAndSims; 
            if(sims != null){
                node.visualizer.GetComponent<VirusVolumnVisualizerController>().SetVolume(virusVolume);
            }
        }
    }
}
