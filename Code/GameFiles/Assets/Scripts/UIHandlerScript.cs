using UnityEditor;
using UnityEngine;

public class UIHandlerScript : MonoBehaviour
{
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
        if (TileEditorOn)
        {
            TileEditorOn = false;
        }
        else
        {
            TileEditorOn= true;
        }
    }
}
