using System.ComponentModel;
using System.Drawing;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
public class PlaceManager : MonoBehaviour
{
    public GameObject placeFactoryObj;
    private PlaceFactory placeFactory;
    private ResidentialPlace residentialPlace;

    public void Start()
    {
        this.placeFactory = placeFactoryObj.GetComponent<PlaceFactory>();
        this.residentialPlace = this.placeFactory.CreateResidentialPlace(new Vector2Int(1, 1), new Vector2Int(1, 3), 100);
        this.residentialPlace.SayHi();

    }
}


// placeManager.CreatePlace(new Vector2(5, 5), new Vector3(0, 0, 0));
