using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class VirusVolumeNode : IGridNode<VirusVolumeNode>
{
    
    public GridNodeMap<VirusVolumeNode> virusVolumeMap;
    public DirectionalNeighbors<VirusVolumeNode> _neighbors = new DirectionalNeighbors<VirusVolumeNode>();

    private Vector2Int _cellPosition;
    private int _raw_value;
    public GameObject visualizer;
    private (int,Sims) _virusVolumeAndSims = (0,null);
    public (int,Sims) virusVolumeAndSims
    {
        get { return _virusVolumeAndSims; }
        set { _virusVolumeAndSims = value; virusVolumeMap.InvokeValueUpdateByCell(this.cellPosition); }
    }
    public DirectionalNeighbors<VirusVolumeNode> neighbors
    {
        get { return _neighbors; }  
        set { _neighbors = value; }
    }
    public int raw_value{
        get { return _raw_value; }
        set { _raw_value = value;}
    }
    public Vector2Int cellPosition{
        get { return _cellPosition; }
        set { _cellPosition = value; }
    }

    public VirusVolumeNode(int v, GridNodeMap<VirusVolumeNode> map, Vector2Int cellPosition) {
        this.virusVolumeMap = map;
        this.cellPosition = cellPosition;
        // this.value = v;
    }

    public void HandelClicked()
    {
        // throw new System.NotImplementedException();
    }
    public override string ToString()
    {
        string simName = virusVolumeAndSims.Item2?.simsName ?? ".";
        int volume = virusVolumeAndSims.Item1;
        return $"{simName}\n{volume}";
    }
}