using UnityEditor;
using UnityEngine;

public class UIHandlerScript : MonoBehaviour
{
    public GameObject BuildingsMenuPopUp;
    public static bool TileEditorOn;
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
            BuildingsMenuPopUp.SetActive(false);
        }
        else
        {
            BuildingsMenuPopUp.SetActive(true);
        }
            
    }
}
