using UnityEditor;
using UnityEngine;

public class UIHandlerScript : MonoBehaviour
{
    public GameObject BuildingsMenuPopUp;
    public GameObject BuildingRemoveButton;
    public static bool TileEditorOn;
    public static bool BuildingRemoverOn = false;
    //  public Square[,] GameGrid;
    private void Start()
    {
        TileEditorOn = false ;
    //  GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    //   SetGrid();
}
    public void OnRoadButtonClicked()
    {
        Debug.Log("Road button clicked");
        BuildingsMenuPopUp.SetActive(false);
        BuildingRemoveButton.SetActive(false);
        if (TileEditorOn)
        {
            TileEditorOn = false;
        }
        else
        {
            TileEditorOn= true;
        }
    }
    public void OnBuildingsButtonClick()
    {
        TileEditorOn = false;
        Debug.Log("Building button clicked");
        if (BuildingsMenuPopUp.activeInHierarchy)
        {
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn= false;
            BuildingsMenuPopUp.SetActive(false);
            BuildingRemoveButton.SetActive(false);
        }
        else
        {
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoveButton.SetActive(true);
        }
            
    }
    public void OnBuildingRemoveButtonClick()
    {
       
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
            BuildingsMenuPopUp.SetActive(false);
        }

    }
}

