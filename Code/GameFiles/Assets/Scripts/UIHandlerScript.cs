using UnityEditor;
using UnityEngine;

public class UIHandlerScript : MonoBehaviour
{
    //  public Square[,] GameGrid;
    private void Start()
    {
        //  GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
        //   SetGrid();
    }
    public void OnRoadButtonClicked()
    {
        Debug.Log("Road button clicked");
    }
}
