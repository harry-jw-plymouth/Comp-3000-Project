using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class GridCreator : MonoBehaviour
{
    public Square[,]GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    public const int WIDTH = 100;
    public const int HEIGHT = 100;
    public Tilemap GameMap;
    bool UpdateNeeded = false;

    public RuleTile GameTile;
    public RuleTile RoadTile;

    public RuleTile SmallHouseTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateGrid();
    }
    bool CheckIfBuildingCanBeplaced(int x, int y,Building building)
    {
        return true;
    }
    Vector3Int GetPositionForSquare(Vector3Int ClickPos, int[,]shape,int CurrentX,int CurrentY,int[] Origin) 
    {
        int XDiff =  System.Math.Abs( Origin[0] - CurrentX);
        int YDiff= Origin[1] - CurrentY;

        int NewX=ClickPos.x + XDiff;
        int NewY=ClickPos.y - YDiff;
        return new Vector3Int(NewX,NewY,0);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int CellClickedPos = GameMap.WorldToCell(ClickPos);
            Debug.Log("Click at: " + ClickPos);
            Debug.Log("Click at: " + CellClickedPos);
            if (UIHandlerScript.TileEditorOn == true)
            {
                try
                {
                    if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                        GameMap.SetTile(CellClickedPos, RoadTile);
                    }
                    else
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                        GameMap.SetTile(CellClickedPos, GameTile);

                    }
                }
                catch
                {
                    Debug.Log("Click not in grid square");

                }                                             
            }
            if (BuildingsListManager.BuildingCurrentlySelected != -1)
            {

                if(GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                {
                    if (CheckIfBuildingCanBeplaced(CellClickedPos.x, CellClickedPos.y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected])){
                        Debug.Log("Shape Y: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0));
                        Debug.Log("Shape X: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1));
                        for (int Y = 0; Y < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0); Y++)
                        {
                            for (int X = 0; X < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1); X++)
                            {
                                if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] != -1)
                                {
                                    GameMap.SetTile(GetPositionForSquare(CellClickedPos, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape,
                                        X, Y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Origin), SmallHouseTile);
                                }
                                
                            }        
                        }
                    }
                }
                BuildingsListManager.BuildingCurrentlySelected = -1;
            }
        }
        
    }
    void CreateGrid()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Vector3Int CurrentPosition=new Vector3Int(x, y, 0);
                GameMap.SetTile(CurrentPosition, GameTile);
                GameGrid[x, y] = new Square(0);
            }
        }
    }
}
