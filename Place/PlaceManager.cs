using System;
using System.Collections.Generic;
using UnityEngine;
public class PlaceManager : MonoBehaviour
{
    public GameObject placeFactoryObj;
    public MapManager mapManager;
    public GameObject flowFieldRootObject;
    public GameObject geoMapManagerObj;
    private PlaceFactory placeFactory;
    // public GameObject gridDebuggerObj;
    public GridDebugManager gridDebuggerManager;

    public List<ResidentialPlace> residentialPlaces = new List<ResidentialPlace>();
    public List<OfficePlace> officePlaces = new List<OfficePlace>();
    public InfoDebuggerManager infoDebuggerManager;


    // public ResidentialPlace residentialPlace;
    // public OfficePlace officePlace;
    public static event Action OnPlaceSpwaned;
    public void Start()
    {
        //temporary use
        // this.gridDebuggerManager =  gridDebuggerObj.GetComponent<GridDebugManager>();
        List<Vector2Int> homes = new List<Vector2Int>{new Vector2Int(1,3),new Vector2Int(1,1),new Vector2Int(7,2), new Vector2Int(11,3),new Vector2Int(5,4) };
        List<Vector2Int> offices = new List<Vector2Int>{new Vector2Int(2,7), new Vector2Int(5,7), new Vector2Int(8,7), new Vector2Int(4,1) , new Vector2Int(14,7), new Vector2Int(17,7)};
        this.placeFactory = placeFactoryObj.GetComponent<PlaceFactory>();

        foreach (var homePosition in homes){
            ResidentialPlace newResidential = this.placeFactory.CreateResidentialPlace(
                new Vector2Int(2, 1), 
                homePosition, 
                100, 
                mapManager, 
                flowFieldRootObject,
                geoMapManagerObj,
                gridDebuggerManager,
                infoDebuggerManager
                );
            newResidential.SayHi();
            residentialPlaces.Add(newResidential);
        }

        foreach (var officePosition in offices){
            OfficePlace newOffice = this.placeFactory.CreateOfficePlace(
                new Vector2Int(2, 2),
                officePosition,
                mapManager,
                flowFieldRootObject,
                geoMapManagerObj,
                gridDebuggerManager,
                infoDebuggerManager
                );
            newOffice.SayHi();
            this.officePlaces.Add(newOffice);   
        }
        OnPlaceSpwaned?.Invoke();
    }
    public OfficePlace GetRandomOffice(){
        return officePlaces[UnityEngine.Random.Range(0,officePlaces.Count)];
    }
    public ResidentialPlace GetRandomResidential()
    {
        return residentialPlaces[UnityEngine.Random.Range(0,residentialPlaces.Count)];
    }
}
