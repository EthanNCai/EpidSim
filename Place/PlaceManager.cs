using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;



public class PlaceManager : MonoBehaviour
{
    public GameObject placeFactoryObj;
    public MapManager mapManager;
    public GameObject flowFieldRootObject;
    public GameObject geoMapManagerObj;
    private PlaceFactory placeFactory;
    // public GameObject gridDebuggerObj;
    public GridInfoManager gridDebuggerManager;
    public List<MedicalPlace> medicalPlaces = new List<MedicalPlace>();
    public List<CommercialPlace> commercialPlaces = new List<CommercialPlace>();
    public List<ResidentialPlace> residentialPlaces = new List<ResidentialPlace>();
    public List<OfficePlace> officePlaces = new List<OfficePlace>();
    public List<TestCenterPlace> testCenterPlaces = new List<TestCenterPlace>();
    public List<QRTCentrePlace> qrtCentrePlaces = new List<QRTCentrePlace>();
    // public List<CommercialPlace> commercialPlaces = new List<CommercialPlace>();

    public InfoManager infoDebuggerManager;

    public CFEManager cfeManager;
    // public ResidentialPlace residentialPlace;
    // public OfficePlace officePlace;
    public static event Action OnPlaceSpwaned;
    public void Start()
    {
        //temporary use
        // this.gridDebuggerManager =  gridDebuggerObj.GetComponent<GridDebugManager>();
        List<Vector2Int> testCentres = new List<Vector2Int>{new Vector2Int(19,5),new Vector2Int(19,1)};
        List<Vector2Int> medicals = new List<Vector2Int>{new Vector2Int(19,3)};
        List<Vector2Int> commercials = new List<Vector2Int>{new Vector2Int(27,3), new Vector2Int(12,3)};
        List<Vector2Int> homes = new List<Vector2Int>{
            new Vector2Int(3,3),new Vector2Int(3,5),new Vector2Int(3,7), new Vector2Int(3,9),new Vector2Int(3,11),
            new Vector2Int(3,13),new Vector2Int(3,15),new Vector2Int(3,17),
            new Vector2Int(7,3),new Vector2Int(7,5),new Vector2Int(7,7), new Vector2Int(7,9),new Vector2Int(7,11),
            new Vector2Int(7,13),new Vector2Int(7,15),new Vector2Int(7,17),
            new Vector2Int(35,3),new Vector2Int(35,5),new Vector2Int(35,7), new Vector2Int(35,9),new Vector2Int(35,11),
            new Vector2Int(35,13),new Vector2Int(35,15),new Vector2Int(35,17),
            new Vector2Int(31,3),new Vector2Int(31,5),new Vector2Int(31,7), new Vector2Int(31,9),new Vector2Int(31,11),
            new Vector2Int(31,13),new Vector2Int(31,15),new Vector2Int(31,17),
             };
        List<Vector2Int> offices = new List<Vector2Int>{
            new Vector2Int(12,6),new Vector2Int(12,10),new Vector2Int(12,14),
            new Vector2Int(27,6),new Vector2Int(27,10),new Vector2Int(27,14)};
        
        // List<Vector2Int> offices = new List<Vector2Int>{
        //     new Vector2Int(12,6),new Vector2Int(12,10),new Vector2Int(12,14),
        //     new Vector2Int(27,6),new Vector2Int(27,10),new Vector2Int(27,14)};
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
                infoDebuggerManager,
                cfeManager
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
                infoDebuggerManager,
                cfeManager
                );
            newOffice.SayHi();
            this.officePlaces.Add(newOffice);   
        }
        


        foreach(var commercialPosition in commercials){
            CommercialPlace newCommercialPlace = this.placeFactory.CreateCommercialPlace(
                new Vector2Int(1, 2),
                commercialPosition,
                mapManager,
                flowFieldRootObject,
                geoMapManagerObj,
                gridDebuggerManager,
                infoDebuggerManager,
                cfeManager
                );
            this.commercialPlaces.Add(newCommercialPlace);
        }

        foreach(var medicalPosition in medicals){
            MedicalPlace newMedicalPlace = this.placeFactory.CreateMedicalPlace(
                new Vector2Int(1, 1),
                medicalPosition,
                mapManager,
                flowFieldRootObject,
                geoMapManagerObj,
                gridDebuggerManager,
                infoDebuggerManager,
                cfeManager
                );
            this.medicalPlaces.Add(newMedicalPlace);
        }

        foreach(var testCentrePos in testCentres){
            TestCenterPlace newTestCentre = this.placeFactory.CreateTestCentre(
                new Vector2Int(1, 1),
                testCentrePos,
                mapManager,
                flowFieldRootObject,
                geoMapManagerObj,
                gridDebuggerManager,
                infoDebuggerManager,
                cfeManager
                );
            this.testCenterPlaces.Add(newTestCentre);
        }
        
        OnPlaceSpwaned?.Invoke();
    }
    public OfficePlace GetRandomOffice(){
        return officePlaces[UnityEngine.Random.Range(0,officePlaces.Count)];
    }
    public ResidentialPlace GetRandomResidential(){
        return residentialPlaces[UnityEngine.Random.Range(0,residentialPlaces.Count)];
    }
    public MedicalPlace GetAvailableMedicalPlace()
    {
        foreach (var medicalPlace in medicalPlaces)
        {
            if (medicalPlace.CheckIsAvailable())
            {
                return medicalPlace;
            }
        }
        return null;
    }
    public int GetAvailableTestCentreSeats(){
        int ret = 0;
        foreach(TestCenterPlace testCentre in testCenterPlaces){
            ret += testCentre.volume;
        }
        return ret;
    }
    public int GetAvailableQuarantieCentreSeats(){
        int ret = 0;
        foreach(QRTCentrePlace qtrCentre in qrtCentrePlaces){
            ret += qtrCentre.volume;
        }
        return ret;
    }
    public void GeneratePlaceOnCell(BuildableInfo buidableInfo, Vector2Int cellPosition){
        switch(buidableInfo.placeType){
            case PlaceMeta.PlaceType.TestCentrePlace:{
                TestCenterPlace newTestCentre = this.placeFactory.CreateTestCentre(
                    buidableInfo.placeSize,
                    cellPosition,
                    mapManager,
                    flowFieldRootObject,
                    geoMapManagerObj,
                    gridDebuggerManager,
                    infoDebuggerManager,
                    cfeManager
                    );
                this.testCenterPlaces.Add(newTestCentre);
                break;
            }case PlaceMeta.PlaceType.MedicalPlace:{
                MedicalPlace newMedicalPlace = this.placeFactory.CreateMedicalPlace(
                    buidableInfo.placeSize,
                    cellPosition,
                    mapManager,
                    flowFieldRootObject,
                    geoMapManagerObj,
                    gridDebuggerManager,
                    infoDebuggerManager,
                    cfeManager
                    );
                this.medicalPlaces.Add(newMedicalPlace);
                break;
            }case PlaceMeta.PlaceType.QRTPlace:{
                QRTCentrePlace newQRTCentre = this.placeFactory.CreateQRTCentre(
                    buidableInfo.placeSize,
                    cellPosition,
                    mapManager,
                    flowFieldRootObject,
                    geoMapManagerObj,
                    gridDebuggerManager,
                    infoDebuggerManager,
                    cfeManager
                    );
                this.qrtCentrePlaces.Add(newQRTCentre);
                break;
            }
        }
    }
}
