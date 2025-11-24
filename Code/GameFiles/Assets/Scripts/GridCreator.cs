using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
public class GridCreator : MonoBehaviour
{
    public Square[,]GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    public const int WIDTH = 100;
    public const int HEIGHT = 100;
    public Tilemap GameMap;
    bool UpdateNeeded = false;

    Vector3Int PreviousMousePosition = new Vector3Int(-1, -1, 0);

    List<PlacedBuilding>PlacedBuildings=new List<PlacedBuilding>();
    List<Vector3Int> PreviousBuildingHighlight = new List<Vector3Int>();

    public RuleTile GameTile;
    public RuleTile RoadTile;

    public RuleTile SmallHouseTile;
    public RuleTile MediumHouseTile;
    public GameObject MediumHousePreFab;
    public GameObject HospitalPrefab;
    public GameObject ShopPrefab;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateGrid();
    }
    bool CheckIfBuildingCanBeplaced(int x, int y,Building building)
    {
        Debug.Log("New Y: " + (y + building.Shape.GetLength(0)));
        if (x + building.Shape.GetLength(1) > WIDTH )
        {
            Debug.Log("Building cant be placed");
            return false;
        }
        else if (y + building.Shape.GetLength(0) >= HEIGHT+1)
        {
            Debug.Log("Building cant be placed, final Y pos:"+ (y + building.Shape.GetLength(0)));
            return false;

        }

        for (int Y = 0; Y < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0); Y++)
        {
            for (int X = 0; X < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1); X++)
            {
                Vector3Int CurrentPos = GetPositionForSquare(new Vector3Int(x,y,0), BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape, X, Y, 
                    BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Origin);
                if(GameGrid[CurrentPos.x, CurrentPos.y].Contains == 2 )
                {
                    Debug.Log(" Building cant be placed, building already occupying square: " + CurrentPos.x + " , " + CurrentPos.y);
                    return false;
                }
            }
        }
        return true;
    }
    void DrawSelectedBuilding(Vector3Int MouseHoverPosition)
    {
        for (int Y = 0; Y < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0); Y++)
        {
            for (int X = 0; X < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1); X++)
            {
                if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] != -1)
                {
                    Vector3Int CurrentPos = GetPositionForSquare(MouseHoverPosition, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape, X, Y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Origin);
                    GameMap.SetColor(CurrentPos, new Color(1f, 1f, 1f, 0.5f));
                    PreviousBuildingHighlight.Add(CurrentPos);
                }

            }
        }

    }
    Vector3Int GetPositionForSquare(Vector3Int ClickPos, int[,]shape,int CurrentX,int CurrentY,int[] Origin) 
    {
        int XDiff =  System.Math.Abs( Origin[0] - CurrentX);
        int YDiff= Origin[1] - CurrentY;

        int NewX=ClickPos.x + XDiff;
        int NewY=ClickPos.y - YDiff;
        return new Vector3Int(NewX,NewY,0);

    }
    void RevertPreviousBuildingHightlight()
    {
        for(int i = 0;i<PreviousBuildingHighlight.Count;i++)
        {
            GameMap.SetTileFlags(PreviousBuildingHighlight[i], TileFlags.None);
            GameMap.SetColor(PreviousBuildingHighlight[i],Color.white);
        }
        PreviousBuildingHighlight = PreviousBuildingHighlight = new List<Vector3Int>();

    }
    void CheckForMouseHover()
    {
        Vector3Int MouseHoverPosition = GameMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (GameMap.HasTile(MouseHoverPosition) && MouseHoverPosition != PreviousMousePosition)
        {
            try
            {
                if (GameMap.HasTile(PreviousMousePosition))
                {
                    GameMap.SetTileFlags(PreviousMousePosition, TileFlags.None);
                    GameMap.SetColor(PreviousMousePosition, Color.white);
                }
                GameMap.SetTileFlags(MouseHoverPosition, TileFlags.None);
                GameMap.SetColor(MouseHoverPosition, new Color(1f, 1f, 1f, 0.5f));
                if (BuildingsListManager.BuildingCurrentlySelected != -1)
                {
                    RevertPreviousBuildingHightlight();
                    DrawSelectedBuilding(MouseHoverPosition);
                }
                PreviousMousePosition = MouseHoverPosition;
            }
            catch
            {
                Debug.Log("Hovering Over none grid square");
            }

        }
    }
    void CheckForMouseClicK() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int CellClickedPos = GameMap.WorldToCell(ClickPos);
            // Debug.Log("Click at: " + ClickPos);
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

                if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                {
                    if (CheckIfBuildingCanBeplaced(CellClickedPos.x, CellClickedPos.y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected]))
                    {
                        Debug.Log("Shape Y: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0));
                        Debug.Log("Shape X: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1));
                        for (int Y = 0; Y < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0); Y++)
                        {
                            for (int X = 0; X < BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1); X++)
                            {
                                if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] != -1)
                                {
                                    Vector3Int CurrentPos = GetPositionForSquare(CellClickedPos, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape, X, Y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Origin);
                                    GameGrid[CurrentPos.x, CurrentPos.y].Contains = 2;
                                    if (BuildingsListManager.BuildingCurrentlySelected == 1)
                                    {
                                        if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                        {
                                            Vector3 AdjustedStartPos = CurrentPos + new Vector3(1, 0.5f, 0);
                                            GameObject MediumHouse = Instantiate(MediumHousePreFab, AdjustedStartPos, Quaternion.identity);
                                        }
                                    }
                                    else if (BuildingsListManager.BuildingCurrentlySelected == 2)
                                    {
                                        if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                        {
                                            Vector3 AdjustedStartPos = CurrentPos + new Vector3(1, 0.5f, 0);
                                            GameObject Shop = Instantiate(ShopPrefab, AdjustedStartPos, Quaternion.identity);
                                        }
                                    }
                                    else if (BuildingsListManager.BuildingCurrentlySelected == 3)
                                    {
                                        if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                        {
                                            Vector3 AdjustedStartPos = CurrentPos + new Vector3(0, 0, 0);
                                            GameObject Hospital = Instantiate(HospitalPrefab, AdjustedStartPos, Quaternion.identity);
                                        }
                                    }
                                    else
                                    {
                                        GameMap.SetTile(CurrentPos, SmallHouseTile);
                                    }
                                    
                                }

                            }
                        }
                    }
                }
                RevertPreviousBuildingHightlight();
                PlacedBuildings.Add(new PlacedBuilding(BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected], new int[] { CellClickedPos.x, CellClickedPos.y }));
                BuildingsListManager.BuildingCurrentlySelected = -1;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        CheckForMouseHover();
        CheckForMouseClicK();
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
