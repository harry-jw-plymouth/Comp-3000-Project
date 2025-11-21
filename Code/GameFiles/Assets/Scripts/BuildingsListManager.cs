using System.Diagnostics;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsListManager : MonoBehaviour
{

    public GameObject BuildingObject; // prefab item
    public RectTransform BuildingsView; //scroll view
    int BuildingCurrentlySelected = -1;
    
    public Building[] Buildings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBuildings();
        DisplayBuildings();
    }
    void SetBuildings()
    {
        //temporarily hard coded, in future will pull more dynamically
        Buildings = new Building[]
        {
            new Building("Small House"," A small house"),
            new Building("Medium House", " A medium house for a bigger family"),
            new Building("Convenience shop", " a little shop, a bit overpriced"),
            new Building("Hospital","A hospital")
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
