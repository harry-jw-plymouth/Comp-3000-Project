using System.Diagnostics;
using TMPro;
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
            new Home("Small House"," A small house", new int[1,1]{{0}} ,new int[]{0,0},
            false ,100, 1000,false,0,3),
            new Home("Medium House", " A medium house for a bigger family",new int[1,2]{{0,1}}  , new int[] {0,0 },
            false,100,1000,false,1,5),
            new Building("Convenience shop", " a little shop, a bit overpriced",new int[1,2]{{0,1}}, new int[] { 0, 0 },
            true,100,300,false,2),
            new Building("Hospital","A hospital",new int[2,2]{{1,1},{0,1} } ,new int[]{1,0},
            false,100,300,true,3),
            new Building("Town Hall", "The core building for your city",new int[3,3]{{1,1,1},{ 1, 1, 1 }, { 0,1,1}},new int[] { 2,0 }, 
            false,100,500,false,4)
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void DisplayBuildings()
    {
        UnityEngine.Debug.Log(" Number of buildings: " + Buildings.Length);
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

            UnityEngine.Debug.Log("Building added: " + Buildings[i].Name);


        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(BuildingsView);
    }
    void OnBuildingClicked(int BuildingPos)
    {
        UnityEngine.Debug.Log("Clicked " + Buildings[BuildingPos].Name);
        BuildingCurrentlySelected = BuildingPos;




    }
}
