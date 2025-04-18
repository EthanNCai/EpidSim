using UnityEngine;
using UnityEngine.UIElements;

public class SimsFactory: MonoBehaviour
{
    public GameObject mapRoot;
    public GameObject simsPrefab;
    public GameObject virusVolumeMapManagerObj;
    public InfoManager infoDebuggerManager;
    public TimeManager timeManager;
    public PlaceManager placeManager;
    // public VirusVolumeMapManager virusVolumeMapManager;

    public Sims CreateSims(Vector2Int position){
        GameObject obj = Instantiate(simsPrefab, mapRoot.transform);
        obj.transform.localPosition = new Vector3(position.x, position.y, 2);
        obj.transform.SetParent(transform);
        Sims sims = obj.GetComponent<Sims>();
        sims.SimsInit(
            virusVolumeMapManagerObj.GetComponent<VirusVolumeGridMapManager>(),
            infoDebuggerManager,
            placeManager,
            timeManager,
            false);
        return sims;
    }
}