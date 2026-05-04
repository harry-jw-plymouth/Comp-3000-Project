using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsListManager : MonoBehaviour
{

    public GameObject BuildingObject; // prefab item
    public RectTransform BuildingsView; //scroll view
    public static int BuildingCurrentlySelected = -1;
    
    public static Building[] Buildings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Debug.Log("Starting buildings list manager");
        SetBuildings();
        DisplayBuildings();
    }
    void SetBuildings()
    {
        //temporarily hard coded, in future will pull more dynamically
        Buildings = new Building[]
        {
            new Home("Small House",200,2," A small house", new int[1,1]{{0}} ,new int[]{0,0},
            false,100, 1000,false,0,10,10,false,0,false,false,10,3,10,10,3),
            new Home("Medium House",350,4, " A medium house for a bigger family",new int[1,2]{{0,1}}  , new int[] {0,0 },
            false,100,1000,false,1,15,15,false,0,false,false,15,5,15,15,5),
            new Building("Convenience shop",400,6, " a little shop, a bit overpriced",new int[1,2]{{0,1}}, new int[] { 0, 0 },
            true,100,300,false,2,10,10,false,0,false,false,10,3,10,17),
            new Building("Hospital",1000,-10,"A hospital",new int[2,2]{{1,1},{0,1} } ,new int[]{1,0},
            false,100,300,true,3,100,80,false,0,false,false,20,23,20,20),
            new Building("Town Hall",800,-5, "The core building for your city",new int[3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,100,500,false,4,50,30,false,0,false, false,15,10,15,13),
            new PowerPlant("Coal Power plant",1200,-15, "A non renewable power plant ,efficient but bad for the environment",new int [3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,200,600,false,5,0,400,false,0,false,false,100,100,100,100,500,40),
            new PowerPlant("Wind Power farm",1400,-20, "A renewable power plant ,not super efficient but much better for the environment",new int [3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,200,600,false,6,0,20,false,0,false,false,2,0,5,10,400,30),
            new Building("Shopping center",700,30, " a shopping centre full of shops and entertainment",new int[2,2]{{1,1},{0,1 } },new int[]{1,0 },true
            ,300,1000,false,7,100,150,true,40,false,false,30,20,50,80),
            new Building("Train Station",1000,20,"a stop for trains",new int[2,2]{{1,1},{0,1} } ,new int[]{1,0},false,
            100,500,false,8,100,20,false,0,true,false,30,10,40,30),
            new Building("Nature area" ,300,-5,"a nature area to increase enviromental value",new int[3,3]{{1,1,1},{1,1,1},{0,1,1 } } ,new int[]{2,0},false,
            100,400,false,9,0,-50, true,0,false,true,-30,0,0,0),
            new Building("Wastage center ",600, -10," A place to deal with waste, deals with waste but can cause pollution", new int[2,2]{{1,1},{0,1} } ,new int[]{1,0 },
            false, 100,500, false,10,50,30,false,0,false,false,30,100,100,0),
             new Building("Recycling center ",900, -40," A place to deal with waste, deals with waste in a way that is less harmful to the environment", new int[2,2]{{1,1},{0,1} } ,new int[]{1,0 },
            false,100,500, false,11,70,10,false,0,false,false,10,10,10,0),

        }; Buildings[8].SetIsTrainStation(true);
        Buildings[10].SetIsWatageCenter(true); Buildings[11].SetIsWatageCenter(true);
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
    public static Building[] GetBuildings()
    {
        return Buildings;
    }
    void DisplayBuildings()
    {
        for (int i = 0; i < Buildings.Length; i++) {
            
            GameObject New = Instantiate(BuildingObject, BuildingsView,false);
            int BuildingPos = i;
            New.GetComponent<Button>().onClick.AddListener(() => OnBuildingClicked(BuildingPos));

        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(BuildingsView);
    }
    void OnBuildingClicked(int BuildingPos)
    {
       // UnityEngine.Debug.Log("Clicked " + Buildings[BuildingPos].Name);
        BuildingCurrentlySelected = BuildingPos;
    }
    public void OnSmallHouseClicked()
    {
        BuildingCurrentlySelected =0;
    }
    public void OnMediumHouseClicked()
    {
        BuildingCurrentlySelected = 1;
    }
    public void OnConvenienceShopClicked()
    {
        BuildingCurrentlySelected = 2;
    }
    public void OnHospitalClicked()
    {
        BuildingCurrentlySelected = 3;
    }
    public void OnTownHallClicked()
    {
        BuildingCurrentlySelected = 4;
    }
    public void OnPowerPlantClicked()
    {
        BuildingCurrentlySelected = 5;
    }
    public void OnWindFarmClicked()
    {
        BuildingCurrentlySelected = 6;
    }
    public void OnShoppingCentreClicked()
    {
        BuildingCurrentlySelected = 7;
    }
    public void OnTrainStationClicked()
    {
        BuildingCurrentlySelected = 8;
    }
    public void OnNatureAreaSelected()
    {
        BuildingCurrentlySelected = 9;
    }
    public void OnWastageAreaSelected()
    {
        BuildingCurrentlySelected = 10;
    }
    public void OnRecyclingCenterSelected()
    {
        BuildingCurrentlySelected = 11;
    }
}
