using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandlerScript : MonoBehaviour
{
    public TextMeshProUGUI ReportText;
    public GameObject ReportDisplay;
    public GameObject RatingDisplay;
    public GameObject BuildingsMenuPopUp;
    public GameObject BuildingRemoveButton;
    public GameObject TransportButton;
    public GameObject TransportBuilderPopUp;
    public GameObject PauseCanvas;
    public static bool TileEditorOn;
    public static bool TransportPlacementOn=false;
    public static bool BuildingRemoverOn = false;
    public bool PauseMenuActive = false;
    //  public Square[,] GameGrid;
    private void Start()
    {
        TileEditorOn = false ;
    //  GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    //   SetGrid();
}
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenPauseMenu()
    {
        if (PauseMenuActive)
        {
            PauseCanvas.SetActive(false);
            PauseMenuActive = false;
        }
        else
        {
            PauseMenuActive = true;
            PauseCanvas.SetActive(true);
        }
    }
    public void OnSaveButtonClicked()
    {
        Debug.Log("Save button clicked");
    }
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked");
        SceneManager.LoadScene("MainMenu");
    }
    void SetUIInactive()
    {
      //  TransportButton.SetActive(false);
        BuildingsMenuPopUp.SetActive(false);
        TransportBuilderPopUp.SetActive(false);
        BuildingsListManager.BuildingCurrentlySelected = -1;
        BuildingRemoveButton.SetActive(false);

        TileEditorOn = false;
        TransportPlacementOn = false;
        BuildingRemoverOn=false;

    }
    public void OnRatingClicked()
    {
        Debug.Log(" rating section clicked");
        ReportDisplay.SetActive(true);
        List<string> Updates = GameStatusScript.GetReport();
        string Info = "";
        for(int i = 0; i < Updates.Count; i++)
        {
            Info += Updates[i] + "\n";
        }
        ReportText.text = Info;

    }
    public void OnReportClicked()
    {
        Debug.Log(" report section clicked");
        ReportDisplay.SetActive(false);
    }
    public void OnRoadButtonClicked()
    {
        Debug.Log("Road button clicked");
        SetUIInactive();
        if (TileEditorOn)
        {
            TileEditorOn = false;
        }
        else
        {
            TileEditorOn= true;
        }
    }
    public void OnTransportButtonClicked()
    {
        Debug.Log("Transport button clicked");
        SetUIInactive();
        if (TransportPlacementOn)
        {
            TransportPlacementOn = false;
        }
        else
        {
            TransportPlacementOn = true;
            TransportBuilderPopUp.SetActive(true);
        }
    }
    public void OnBuildingsButtonClick()
    {
        SetUIInactive();
        TileEditorOn = false;
        Debug.Log("Building button clicked");
        if (BuildingsMenuPopUp.activeInHierarchy)
        {
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn= false;
        }
        else
        {
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoveButton.SetActive(true);
        }
            
    }
    public void OnBuildingRemoveButtonClick()
    {
        SetUIInactive();
        TransportBuilderPopUp.SetActive(false);

        if (BuildingRemoverOn)
        {
            Debug.Log("Building remover off");
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoverOn = false;

        }
        else
        {         
            Debug.Log("Building remover on");
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn = true;
        }

    }
}

