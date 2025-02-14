using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
public class PlaceManager : MonoBehaviour
{
    public GameObject placeFactoryObj;
    public MapManager mapManager;
    public GameObject flowFieldRootObject;
    public GameObject geoMapManagerObj;
    private PlaceFactory placeFactory;
    private ResidentialPlace residentialPlace;

    public void Start()
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
    }
}
