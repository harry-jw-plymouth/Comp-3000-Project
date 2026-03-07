using System.Diagnostics;
using TMPro;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
            false,100, 1000,false,0,10,10,false,0,false,3),
            new Home("Medium House",350,4, " A medium house for a bigger family",new int[1,2]{{0,1}}  , new int[] {0,0 },
            false,100,1000,false,1,15,15,false,0,false,5),
            new Building("Convenience shop",400,6, " a little shop, a bit overpriced",new int[1,2]{{0,1}}, new int[] { 0, 0 },
            true,100,300,false,2,10,10,false,0,false),
            new Building("Hospital",1000,-10,"A hospital",new int[2,2]{{1,1},{0,1} } ,new int[]{1,0},
            false,100,300,true,3,100,80,false,0,false),
            new Building("Town Hall",800,-5, "The core building for your city",new int[3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,100,500,false,4,50,30,false,0,false),
            new PowerPlant("Coal Power plant",1200,-15, "A non renewable power plant ,efficient but bad for the environment",new int [3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,200,600,false,5,0,400,false,0,false,500,40),
            new PowerPlant("Wind Power farm",1400,-20, "A renewable power plant ,not super efficient but much better for the environment",new int [3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 },
            false,200,600,false,6,0,20,false,0,false,400,30),
            new Building("Shopping center",700,30, " a shopping centre full of shops and entertainment",new int[2,2]{{1,1},{0,1 } },new int[]{1,0 },true
            ,300,1000,false,7,100,150,true,40,false),
            new Building("Train Station",1000,20,"a stop for trains",new int[2,2]{{1,1},{0,1} } ,new int[]{1,0},false,
            100,500,false,8,100,20,false,0,true)

        }; Buildings[8].SetIsTrainStation(true);
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
      //  UnityEngine.Debug.Log(" Number of buildings: " + Buildings.Length);
        for (int i = 0; i < Buildings.Length; i++) {
            
            GameObject New = Instantiate(BuildingObject, BuildingsView,false);
            //  Text Building = New.GetComponentInChildren<Text>();
            //  if (Building != null) { 
            //    Building.text = Buildings[i].Name;
            //} 
            //   New.transform.SetParent(BuildingsView, false);
            //   New.transform.localScale = Vector3.one;
            //    New.GetComponentInChildren<Text>().text = Buildings[i].Name;
            int BuildingPos = i;
            New.GetComponent<Button>().onClick.AddListener(() => OnBuildingClicked(BuildingPos));
            //TextMeshProUGUI BuildingInfo = New.GetComponentInChildren<TextMeshProUGUI>();
            //BuildingInfo.text = Buildings[i].Name;

         //   Button BuildingButton = New.GetComponent<Button>();
         //   if (BuildingButton != null)
           // {
           //     BuildingButton.onClick.AddListener(() => OnBuildingClicked(i));
           // }

            //UnityEngine.Debug.Log("Building added: " + Buildings[i].Name);


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
}
