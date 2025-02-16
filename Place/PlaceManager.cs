using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlaceManager : MonoBehaviour
{
    public GameObject placeFactoryObj;
    public MapManager mapManager;
    public GameObject flowFieldRootObject;
    public GameObject geoMapManagerObj;
    private PlaceFactory placeFactory;

    public List<ResidentialPlace> residentialPlaces = new List<ResidentialPlace>();
    public List<OfficePlace> officePlaces = new List<OfficePlace>();



    public ResidentialPlace residentialPlace;
    public OfficePlace officePlace;

    public void Awake()
    {
        this.placeFactory = placeFactoryObj.GetComponent<PlaceFactory>();
        this.residentialPlace = this.placeFactory.CreateResidentialPlace(
            new Vector2Int(2, 1), 
            new Vector2Int(1, 3), 
            100, 
            mapManager, 
            flowFieldRootObject,
            geoMapManagerObj
            );
        this.residentialPlace.SayHi();

        this.officePlace = this.placeFactory.CreateOfficePlace(
            new Vector2Int(2, 2),
            new Vector2Int(7, 7),
            mapManager,
            flowFieldRootObject,
            geoMapManagerObj
            );
        this.officePlace.SayHi();
        
    }
}
