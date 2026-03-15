using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
public class GridCreator : MonoBehaviour
{
    [SerializeField] NPChandler npcHandler;
    [SerializeField] TransportPlacementScript TransportHandler;
    public static Square[,]GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    public const int WIDTH = 100;
    public const int HEIGHT = 100;
    [SerializeField] private Tilemap GameMapReference;
    public static Tilemap GameMap;
    public UIHandlerScript uiHandler;
    public int NumberOfRoads = 0;

    public Camera MainCamera;
    bool UpdateNeeded = false;

    Vector3Int PreviousMousePosition = new Vector3Int(-1, -1, 0);

    public static List<PlacedBuilding>PlacedBuildings=new List<PlacedBuilding>();
    List<GameObject> Sprites = new List<GameObject>();
    List<Vector3Int> PreviousBuildingHighlight = new List<Vector3Int>();

   public static  List<PlacedBuilding> HomesPlaced = new List<PlacedBuilding>();
 

    public static Building RecentlySelectedBuilding;

    public RuleTile GameTile;
    public RuleTile RoadTile;
    public RuleTile BusStopTile;

    public RuleTile SmallHouseTile;
    public RuleTile MediumHouseTile;

    public GameObject SmallHousePreFab;
    public GameObject MediumHousePreFab;
    public GameObject HospitalPrefab;
    public GameObject ShopPrefab;
    public GameObject TownHallPrefab;
    public GameObject PowerPlantPrefab;
    public GameObject WindFarmPrefab;
    public GameObject ShoppingCenterPrefab;
    public GameObject TrainStationPrefab;

    public GameStatusScript GameStatusScript;


    public GameObject NoPowerForBuildingWarning;
    public int NumberOfBusStops = 0;

    public static List<Vector3> RoadPositions=new List<Vector3>();
    public List<GameObject> PowerIcons=new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        CreateGrid();
        CenterCamera();
    }
    private void Awake()
    {
        GameGrid = new Square[WIDTH, HEIGHT];
        GameMap = GameMapReference;
    }
    void CenterCamera()
    {
        Debug.Log("Camera centered");
        Vector3 CenterPos = GameMap.CellToWorld(new Vector3Int(WIDTH / 2, HEIGHT / 2, 0));
        MainCamera.transform.position = new Vector3(CenterPos.x, CenterPos.y, MainCamera.transform.position.z);
    }
    public static List<PlacedBuilding> GetAllBuildings()
    {
        return PlacedBuildings;
    }
    
    bool CheckIfBuildingCanBeplaced(int x, int y,Building building)
    {
        //Debug.Log("New Y: " + (y + building.Shape.GetLength(0)));
        if (x + building.Shape.GetLength(1) > WIDTH )
        {
          //  Debug.Log("Building cant be placed");
            return false;
        }
        else if (y + building.Shape.GetLength(0) >= HEIGHT+1)
        {
          //  Debug.Log("Building cant be placed, final Y pos:"+ (y + building.Shape.GetLength(0)));
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
                  //  Debug.Log(" Building cant be placed, building already occupying square: " + CurrentPos.x + " , " + CurrentPos.y);
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
    public int GetNumberOfBuildings()
    {
        return PlacedBuildings.Count;
    }
    public static int GetPowerGeneration()
    {
        int Total = 0;
        foreach (PlacedBuilding building in PlacedBuildings)
        {
            // && building.GetIfInRangeOfPowerPlant()
            if (building.buildingType is PowerPlant Powerplant)
            {
              //  Debug.Log("Power plant found:" + Powerplant.PowerGeneration);
                Total += Powerplant.GetPowerGeneration();
            }
        }
        return Total;
    }
    public void DisplayPowerAvailabilityOnBuilding()
    {
        for (int i = 0; i < PlacedBuildings.Count; ++i)
        {
            if (!PlacedBuildings[i].GetIfInRangeOfPowerPlant())
            {
                PlacedBuildings[i].DisplayWarning(true);
               
            }
            else
            {
                PlacedBuildings[i].DisplayWarning(false);
               
            }
        }
    }
    public static int GetPowerUsage()
    {
        int total = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            total += PlacedBuildings[i].GetType().PowerUsage;
        }
        return total;
    }
    public int GetNumberOfRoads()
    {
        return NumberOfRoads;
    }
    public static int GetNumberOfHospitals()
    {
        int Number = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsHospital())
            {
                Number++;
            }
        }
        return Number;
    }
    public static int GetNumberOfShops()
    {
        int Number = 0;
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsHospital()) 
            {
                Number++;
            }
        }
        return Number;
    }
    public static int GetNumberOfTrainStations()
    {
        int Number = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfTrainStation())
            {
                Number++;
            }
        }
        return Number;
    }
    public static bool GetIfRoadExists()
    {
        for(int Y = 0; Y < HEIGHT; Y++)
        {
            for(int X = 0; X < WIDTH; X++)
            {
               // Debug.Log("COntains:"+ GameGrid[X, Y].Contains);
                if (GameGrid[X, Y].Contains == 1 || GameGrid[X,Y].Contains==5)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public static bool GetIfTrainStationExists()
    {
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfTrainStation())
            {
                return true;
            }
        }
        return false;
    }
    static int GetDistanceBetweenPostions(Vector3Int Pos1, Vector3Int Pos2)
    {
        int XDiff=0, YDiff=0;
        if (Pos1.x > Pos2.x)
        {
            XDiff = Pos1.x - Pos2.x;
        }
        else
        {
            XDiff = Pos2.x - Pos1.x;
        }
        if (Pos1.y > Pos2.y) { 
            YDiff= Pos1.y - Pos2.y;
        }
        else
        {
            YDiff = Pos2.y - Pos1.y;
        }
        return XDiff + YDiff;
    }
    public static Vector3 GetPosOfNearestRoad(Vector3 CurrentPos)
    { 
        Vector3 CurrentClosest=new Vector3(0,0,0);
        int CurrentMinDistance=100000;
        for (int Y = 0; Y < HEIGHT; Y++)
        {
            for (int X = 0; X < WIDTH; X++)
            {
                if (GameGrid[X, Y].Contains == 1 || GameGrid[X,Y].Contains==5)
                {
                    int Distance = GetDistanceBetweenPostions(GameMap.WorldToCell(new Vector3(X, Y, 0)),GameMap.WorldToCell(CurrentPos));
                    if (Distance < CurrentMinDistance)
                    {
                        CurrentMinDistance = Distance;
                        CurrentClosest=new Vector3(X, Y, 0);
                    }
                }
            }
        }
        return CurrentClosest;
    }
    public static Vector3 GetPosOfNearestShop(Vector3 CurrentPos)
    {
        Vector3 CurrentClosest = new Vector3(0, 0, 0);
        bool ShopFound = false;
        int CurrentMinDistance = 100000;
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsShop())
            {
                int Distance = GetDistanceBetweenPostions(GameMap.WorldToCell(PlacedBuildings[i].GetBuildingPos()), GameMap.WorldToCell(CurrentPos));
                if (Distance < CurrentMinDistance)
                {
                    RecentlySelectedBuilding = PlacedBuildings[i].buildingType;
                    ShopFound= true;
                    CurrentMinDistance = Distance;
                    CurrentClosest = PlacedBuildings[i].GetBuildingPos();
                }
            }
        }

        if (!ShopFound)
        {
            return new Vector3(-1,-1,-1);
        }
        return CurrentClosest;
    }
    public static int GetTotalEnviormentalEffects()
    {
        int EnviromentalEffects = 0;
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            EnviromentalEffects += PlacedBuildings[i].GetEnviromentalValue();
        }
        return EnviromentalEffects;
    }
    public static Vector3 GetPosOfNearestEntertainment(Vector3 CurrentPos)
    {
        Vector3 CurrentClosest = new Vector3(0, 0, 0);
        bool EntFound = false;
        int CurrentMinDistance = 100000;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetType().GetIfEntertainment())
            {
                int Distance = GetDistanceBetweenPostions(GameMap.WorldToCell(PlacedBuildings[i].GetBuildingPos()), GameMap.WorldToCell(CurrentPos));
                if (Distance < CurrentMinDistance)
                {
                    RecentlySelectedBuilding = PlacedBuildings[i].buildingType;
                    EntFound = true;
                    CurrentMinDistance = Distance;
                    CurrentClosest = PlacedBuildings[i].GetBuildingPos();
                }
            }
        }

        if (!EntFound)
        {
            return new Vector3(-1, -1, -1);
        }
        return CurrentClosest;
    }
    public static Vector3 GetPosOfNearestHospital(Vector3 CurrentPos)
    {
        Vector3 CurrentClosest = new Vector3(0, 0, 0);
        bool HospitalFound = false;
        int CurrentMinDistance = 100000;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsHospital())
            {
                int Distance = GetDistanceBetweenPostions(GameMap.WorldToCell(PlacedBuildings[i].GetBuildingPos()), GameMap.WorldToCell(CurrentPos));
                if (Distance < CurrentMinDistance)
                {
                    RecentlySelectedBuilding = PlacedBuildings[i].buildingType;
                    HospitalFound= true;
                    CurrentMinDistance = Distance;
                    CurrentClosest = PlacedBuildings[i].GetBuildingPos();
                }
            }
        }

        if (!HospitalFound)
        {
            return new Vector3(-1, -1, -1);
        }
        return CurrentClosest;
    }
    public static Building GetSelectedBuilding()
    {
        return RecentlySelectedBuilding;
    }
    void RemoveSelectedBuilding(Building RemovedBuilding, Vector3Int Origin)
    {
        for (int Y = 0; Y < RemovedBuilding.Shape.GetLength(0); Y++)
        {
            for (int X = 0; X < RemovedBuilding.Shape.GetLength(1); X++)
            {
                if (RemovedBuilding.Shape[Y, X] != -1)
                {
                    Vector3Int CurrentPos = GetPositionForSquare(Origin, RemovedBuilding.Shape, X, Y, RemovedBuilding.Origin);

                    GameGrid[CurrentPos.x, CurrentPos.y].Contains = 0;

                    GameMap.SetColor(CurrentPos, new Color(1f, 1f, 1f, 0.5f));
                    GameMap.SetTile(CurrentPos, GameTile);

                    GameMap.SetTileFlags(CurrentPos, TileFlags.None);
                    GameMap.SetColor(CurrentPos, Color.white);

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

    public int GetBuildingClicked(Vector3Int MousePos)
    {
        for (int i = 0; PlacedBuildings.Count > i; i++)
        {
            PlacedBuilding Currentbuilding = PlacedBuildings[i];
            for (int Y = 0; Y < Currentbuilding.buildingType. Shape.GetLength(0); Y++)
            {
                for (int X = 0; X < Currentbuilding.buildingType.Shape.GetLength(1); X++)
                {

                    Vector3Int CurrentPos = GetPositionForSquare(new Vector3Int(Currentbuilding.OriginPos[0], Currentbuilding.OriginPos[1],0), Currentbuilding.buildingType.Shape, X, Y, Currentbuilding.buildingType.Origin);
                    if(CurrentPos == MousePos)
                    {
            //            Debug.Log("Item found at" + CurrentPos);
                        return i;
                    }
                }
            }
        }
        return -1;
    }
    void CheckForMouseHover()
    {
        Vector3Int MouseHoverPosition =GameMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
         //       Debug.Log("Hovering Over none grid square");
            }

        }
    }
    bool[,] GetSurroundingTiles(Vector3Int Origin)
    {
        bool[,] SurroundingTiles = new bool[3, 3]{
        {false,false,false },
        {false,false,false},
        {false,false,false}};

        // top left
        if (GameGrid[Origin.x - 1, Origin.y - 1].Contains == 1)
            SurroundingTiles[0, 0] = true;
        //top middle
        if (GameGrid[Origin.x , Origin.y -1].Contains == 1)
            SurroundingTiles[0,1] = true;
        //top right
        if (GameGrid[Origin.x +1, Origin.y -1].Contains == 1)
            SurroundingTiles[0, 2] = true;

        //middle left
        if (GameGrid[Origin.x -1 , Origin.y ].Contains == 1)
            SurroundingTiles[1, 0] = true;
        //Pure middle
        if (GameGrid[Origin.x , Origin.y ].Contains == 1)
            SurroundingTiles[1, 1] = true;
        //middle right
        if (GameGrid[Origin.x +1, Origin.y ].Contains == 1)
            SurroundingTiles[1, 2] = true;

        //top left
        if (GameGrid[Origin.x - 1, Origin.y + 1].Contains == 1)
            SurroundingTiles[2, 0] = true;
        //top middle
        if (GameGrid[Origin.x, Origin.y+1].Contains == 1)
            SurroundingTiles[2, 1] = true;
        //top right
        if (GameGrid[Origin.x + 1, Origin.y+1].Contains == 1)
            SurroundingTiles[2, 2] = true;


        return SurroundingTiles;
    }
    void UpdateRoadsAroundEdit(Vector3Int EditPosition)
    {
        bool[,] SurroundingTiles = GetSurroundingTiles(EditPosition);
        bool Check=true;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (!SurroundingTiles[x, y])
                {
                    Check=false; break;
                }
                    
            }
        }
        if (Check)
        {
            //road tile with no pavement, road all around 
            //roads at all sides should be set to have no pavement connecting 
        }
        else
        {
            if (SurroundingTiles[0,1] && SurroundingTiles[1,0]&& SurroundingTiles[1, 2] && SurroundingTiles[2, 1])
            {
                //road tile with pavement on each corner but open roads all 4 directions
                // 4 adjacent road tiles have open road leading into tile
            }
            else if (SurroundingTiles[0, 1] && SurroundingTiles[1, 0] && SurroundingTiles[1, 2] && !SurroundingTiles[2,1])
            {
                //Road tile with pavement in top corners and on the bottom
                //road tiles to left, right and top should have road leading in
            }
            else if (SurroundingTiles[0, 1] && SurroundingTiles[1, 0] && !SurroundingTiles[1, 2] && !SurroundingTiles[2, 1])
            {
                //Road tile with pavement in top left corner,on the right and on the bottom
                //road tiles to left and top should have road leading in
            }
            else if (SurroundingTiles[0, 1] && !SurroundingTiles[1, 0] && !SurroundingTiles[1, 2] && !SurroundingTiles[2, 1])
            {
                //Road tile with pavement in all directions other than ome
                //road tiles to top should have road leading in
            }
            else if (SurroundingTiles[0, 1] && SurroundingTiles[1, 0] && !SurroundingTiles[1, 2] && SurroundingTiles[2, 1])
            {
                //Road tile with pavement to the right
                //road tiles in all directions other than up should have road leading in
            }
            else if (SurroundingTiles[0, 1] && !SurroundingTiles[1, 0] && !SurroundingTiles[1, 2] && SurroundingTiles[2, 1])
            {
                //Road tile with pavement to the right and left
                //road tiles above and below should have road leading in
            }
            else if (!SurroundingTiles[0, 1] && !SurroundingTiles[1, 0] && !SurroundingTiles[1, 2] && SurroundingTiles[2, 1])
            {
                //Road tile with pavement up, right and left
                //road tile below should have road leading in
            }
        }


    }
    void PlaceBuilding(Vector3Int CellClickedPos)
    {
        //Place building
        GameObject NewSprite = new GameObject();
        Building CurrentlySelected = BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected];
        if(!GameStatusScript.CheckIfPurchaseAffordable(CurrentlySelected.CostToBuild))
        {
            uiHandler.ShowAlertPopUp("Not enough money to build");
        }
        else
        {
            if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
            {
                if (CheckIfBuildingCanBeplaced(CellClickedPos.x, CellClickedPos.y, CurrentlySelected))
                {
                    //    Debug.Log("Shape Y: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(0));
                    //  Debug.Log("Shape X: " + BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape.GetLength(1));
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
                                        NewSprite = Instantiate(MediumHousePreFab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 2)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(1, 0.5f, 0);
                                        NewSprite = Instantiate(ShopPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 3)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(0, 0, 0);
                                        NewSprite = Instantiate(HospitalPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 0)
                                {
                                    Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.5f, 0.5f, 0);
                                    NewSprite = Instantiate(SmallHousePreFab, AdjustedStartPos, Quaternion.identity);

                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 4)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                        NewSprite = Instantiate(TownHallPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 5)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                        NewSprite = Instantiate(PowerPlantPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 6)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                        NewSprite = Instantiate(WindFarmPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 7)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                        NewSprite = Instantiate(ShoppingCenterPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 8)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                        NewSprite = Instantiate(TrainStationPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }

                            }

                        }
                    }
                    RevertPreviousBuildingHightlight();
                    PlacedBuilding New = new PlacedBuilding(BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].GetInstance(), new int[] { CellClickedPos.x, CellClickedPos.y }, NewSprite);
                    New.SetWarningSprite(Instantiate(NoPowerForBuildingWarning,GameMap.CellToWorld( CellClickedPos),Quaternion.identity) );
                    New.SetBuildingPos(CellClickedPos);
                    PlacedBuildings.Add(New);
                    //     Debug.Log("New buildings count"+PlacedBuildings.Count);
                    

                    if (New.GetType().GetIfIsHome())
                    {
                        npcHandler.SetHomes();
                    }
                    if (MainMenu.GetCurrentGameMode() != 0)
                    {
                        GameStatusScript.DoPlaceBuildingCosts(BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected]);
                    }
                }
            }
            
        }

        UpdateStatusOfBuildingsInRangeOfPower();
        RevertPreviousBuildingHightlight();

        BuildingsListManager.BuildingCurrentlySelected = -1;

    }
    public  bool GetIfBusStopExists()
    {
        if (NumberOfBusStops > 0)
        {
            return true;
        }
        return false;
       
    }
    void PlaceTiles(Vector3Int CellClickedPos)
    {
        //Place tiles 
        try
        {
            if (UIHandlerScript.BusStopEditorOn)
            {
                if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 1)
                {
                    GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 5;
                    GameMap.SetTile(CellClickedPos, BusStopTile);
                    NumberOfBusStops++;
                    Debug.Log("Placing Bus stop");
                }
                else if (GameGrid[CellClickedPos.x,CellClickedPos.y].Contains==5)
                {
                    GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                    GameMap.SetTile(CellClickedPos, RoadTile);
                    NumberOfBusStops--;
                }
            }
            else
            {
                if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                {
                    GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                    GameMap.SetTile(CellClickedPos, RoadTile);
                    NumberOfRoads++;
                    RoadPositions.Add(CellClickedPos);
                }
                else if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 4)
                {
                    Debug.Log("Railway clicked");
                    if (TransportPlacementScript.CheckIfRouteExistsUsingTrack(CellClickedPos))
                    {
                        Debug.Log("Route uses track");
                    }
                    else
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                        GameMap.SetTile(CellClickedPos, GameTile);
                    }

                }
                else
                {
                    if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 1)
                    {
                        NumberOfRoads--;
                    }
                    GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                    GameMap.SetTile(CellClickedPos, GameTile);
                    RoadPositions.Remove(CellClickedPos);

                }
            }
            
            UpdateRoadsAroundEdit(CellClickedPos);
        }
        catch
        {
//            Debug.Log("Click not in grid square");

        }

    }
    void RemoveBuildings(Vector3Int CellClickedPos)
    {
       // Debug.Log("Removing Building");
        //building removing check
        if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 2)
        {
            int BuildingPos = GetBuildingClicked(CellClickedPos);
           // Debug.Log("Building found at poaition");
            if (BuildingPos != -1)
            {
                RemoveSelectedBuilding(PlacedBuildings[BuildingPos].buildingType,
                    new Vector3Int(PlacedBuildings[BuildingPos].OriginPos[0], PlacedBuildings[BuildingPos].OriginPos[1], 0));
                if (PlacedBuildings[BuildingPos] != null)
                {
                    Destroy(PlacedBuildings[BuildingPos].Sprite);
                }
                PlacedBuildings[BuildingPos].DestroyWarning();
                npcHandler.RemoveAllNPCsFromBuilding(PlacedBuildings[BuildingPos].GetNPCsInBuilding());
                List<int> Indexes = PlacedBuildings[BuildingPos].GetInhabitants();
                PlacedBuildings.RemoveAt(BuildingPos);
                npcHandler.UpdateHomesForNPCsAfterBuildingRemoval(Indexes);

         
        

            }
        }
        UpdateStatusOfBuildingsInRangeOfPower();
        DisplayPowerAvailabilityOnBuilding();
    }
    void DisplayBuildingInfo(int BuildingIndex)
    {
        Debug.Log("Displaying building info");
        Debug.Log("Displaying:" + PlacedBuildings[BuildingIndex].buildingType);
        uiHandler.DisplayBuildingInfo(PlacedBuildings[BuildingIndex]);

    }
    public void OnPowerWarningClicked()
    {
        Debug.Log("Power warning clicked");
    }
    void CheckForStationClicked(Vector3Int CellClickedPos)
    {
        int BuildingPos = GetBuildingClicked(CellClickedPos);
        if (BuildingPos != -1)
        {
            if (PlacedBuildings[BuildingPos].GetIfTrainStation())
            {
                Debug.Log("Train station clicked");
                uiHandler.OnTrainStationClicked(CellClickedPos,BuildingPos);
            }
        }
    }
    void CheckForBusStopClicked(Vector3Int CellClickedPos)
    {
        if (GridCreator.GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 5)
        {
            uiHandler.OnBusStopClicked(CellClickedPos);
        }
    }

    void CheckForMouseClicK() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int CellClickedPos = GameMap.WorldToCell(ClickPos);
             Debug.Log("Click at: " + ClickPos);
              Debug.Log("Click at: " + CellClickedPos);
            if (uiHandler.SelectingRouteLocation)
            {
                if (uiHandler.RouteIsForBus)
                {
                    CheckForBusStopClicked(CellClickedPos);
                }
                else
                {
                    CheckForStationClicked(CellClickedPos);
                }
                    
            }
            else
            {
                if (UIHandlerScript.TileEditorOn == true)
                {
                    PlaceTiles(CellClickedPos);
                }
                else if (BuildingsListManager.BuildingCurrentlySelected != -1)
                {
                    PlaceBuilding(CellClickedPos);
                }
                else if (UIHandlerScript.BuildingRemoverOn)
                {
                    RemoveBuildings(CellClickedPos);
                }
                else if (UIHandlerScript.TransportPlacementOn)
                {
                    if (BuildingsListManager.BuildingCurrentlySelected == 8)
                    {
                        PlaceBuilding(CellClickedPos);
                    }
                    else
                    {
                        TransportHandler.DoTransportPlacement(CellClickedPos);
                    }

                }
                else
                {
                    int BuildingPos = GetBuildingClicked(CellClickedPos);
                    if (BuildingPos != -1)
                    {
                        DisplayBuildingInfo(BuildingPos);
                    }

                }
            }
            

        }
    }
    public static Vector3 GetRandomRoadCoorindates()
    {
        if (GetIfRoadExists())
        {
            int RandomValue = UnityEngine.Random.Range(0, RoadPositions.Count);
            return RoadPositions[RandomValue];
        }
        else return new Vector3(-1, -1, -1);
    }
    public static int GetNumberOfEntertainment()
    {
        int Number = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetType().GetIfEntertainment())
            {
                Number++;
            }
        }
        return Number;
    }
    public int EnterBuildingForNPC(Vector3 Pos, int NPCIndex)
    {
        Vector3Int cell = GameMap.WorldToCell(Pos);
        int BuildingPos = -1;
        for (int i = 0; PlacedBuildings.Count > i; i++)
        {
            PlacedBuilding Currentbuilding = PlacedBuildings[i];
            for (int Y = 0; Y < Currentbuilding.buildingType.Shape.GetLength(0); Y++)
            {
                for (int X = 0; X < Currentbuilding.buildingType.Shape.GetLength(1); X++)
                {

                    Vector3Int CurrentPos = GetPositionForSquare(new Vector3Int(Currentbuilding.OriginPos[0], Currentbuilding.OriginPos[1], 0), Currentbuilding.buildingType.Shape, X, Y, Currentbuilding.buildingType.Origin);
                    if (CurrentPos == cell)
                    {
                        //            Debug.Log("Item found at" + CurrentPos);
                        PlacedBuildings[i].AddNPCIndex(NPCIndex);
                        return i;
                    }
                }
            }
        }

        return -1;
        

    }
    public static int NearestPowerPlantIndex = -1;
    public static Vector3 GetPosOfNearestPowerPlant(Vector3 CurrentPos)
    {
        Vector3 CurrentClosest = new Vector3(-1, -1, -1);
        bool PowerPlantFound = false;
        int CurrentMinDistance = 100000;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsPowerPlant())
            {
                int Distance = GetDistanceBetweenPostions(GameMap.WorldToCell(PlacedBuildings[i].GetBuildingPos()), GameMap.WorldToCell(CurrentPos));
                if (Distance < CurrentMinDistance)
                {
                    NearestPowerPlantIndex = i;
                    RecentlySelectedBuilding = PlacedBuildings[i].buildingType;
                    PowerPlantFound = true;
                    CurrentMinDistance = Distance;
                    CurrentClosest = PlacedBuildings[i].GetBuildingPos();
                }
            }
        }
        return CurrentClosest;
    }

    void UpdateStatusOfBuildingsInRangeOfPower()
    {
        for (int i = 0; PlacedBuildings.Count > i; i++) {
            PlacedBuildings[i].SetInRangeOfPowerPlant(false);
            Vector3 NearestPowerPlant = GetPosOfNearestPowerPlant(PlacedBuildings[i].GetBuildingPos());
            if (NearestPowerPlant.x != -1)
            {
                int DistanceToPowerPlant = GetDistanceBetweenPostions( GameMap.WorldToCell( NearestPowerPlant), GameMap.WorldToCell( PlacedBuildings[i].GetBuildingPos()));
                if (PlacedBuildings[NearestPowerPlantIndex].buildingType is PowerPlant powerplant)
                {
                    if (DistanceToPowerPlant <= powerplant.GetRange())
                    {
                        PlacedBuildings[i].SetInRangeOfPowerPlant(true);
                    }
                }
                
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        CheckForMouseHover();
        CheckForMouseClicK();

        DisplayPowerAvailabilityOnBuilding();
    }
    void CreateStartingArea()
    {
        Vector3Int MapCenter=new Vector3Int(WIDTH/2, HEIGHT/2,0);
        for(int i = -5; i < 5; i++)
        {
            Vector3Int Position1 = new Vector3Int(MapCenter.x + i, MapCenter.y, 0);
            Vector3Int Position2 = new Vector3Int(MapCenter.x + i, MapCenter.y + 1, 0);

            GameGrid[MapCenter.x+i, MapCenter.y].Contains = 1;
            GameMap.SetTile(Position1, RoadTile);
            RoadPositions.Add(Position1);

            GameGrid[MapCenter.x + i, MapCenter.y+1].Contains = 1;
            GameMap.SetTile(Position2, RoadTile);
            RoadPositions.Add(Position2);
            NumberOfRoads += 2;


        }
        Vector3Int BuildingStart = new Vector3Int( (int)MapCenter.x,MapCenter.y+2,0);
        BuildingsListManager.BuildingCurrentlySelected = 4;

        GameObject NewSprite = new GameObject();
        PlaceBuilding(BuildingStart);
   
        //Vector3 AdjustedStartPos = MapCenter + new Vector3(-0.5f, -0.5f, 0);
        //NewSprite = Instantiate( TownHallPrefab, AdjustedStartPos, Quaternion.identity);
        //PlacedBuildings.Add(new PlacedBuilding(BuildingsListManager.Buildings[4]
        //    , new int[] { (int)AdjustedStartPos.x,(int)AdjustedStartPos.y },
          //  NewSprite));
        //for(int x = 0; x < 3; x++)
       // {
           // for (int y = 0; y < 3; y++) {
         //       GameGrid[(int)AdjustedStartPos.x+ x, (int)AdjustedStartPos.y + y+3].Contains = 2;
          //  }
        //}
        npcHandler.SetHomes();

    }
    GameObject GetSriteForBuilding(SaveBuildingModel Save)
    {
     //   Debug.Log("GetSprite function");
        // create sprite by instantiating then return so it can be saved
        GameObject NewSprite = new GameObject();
        Vector3Int NewPos= new Vector3Int(Save.OriginX,Save.OriginY,0);
    //    Debug.Log("New Pos:"+NewPos.ToString());
        bool SpriteMade = false;
        for (int Y = 0; Y < BuildingsListManager.Buildings[Save.TypeIndex].Shape.GetLength(0); Y++)
        {
            for (int X = 0; X < BuildingsListManager.Buildings[Save.TypeIndex].Shape.GetLength(1); X++)
            {
      //          Debug.Log("Shape"+Y+","+X+" :" + BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X]);
                if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] != -1)
                {
                    Vector3Int CurrentPos = GetPositionForSquare(NewPos, BuildingsListManager.Buildings[Save.TypeIndex].Shape, X, Y, BuildingsListManager.Buildings[Save.TypeIndex].Origin);
                    GameGrid[CurrentPos.x, CurrentPos.y].Contains = 2;
                    if (!SpriteMade)
                    {
                        if (Save.TypeIndex == 1)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(1, 0.5f, 0);
                                NewSprite = Instantiate(MediumHousePreFab, AdjustedStartPos, Quaternion.identity);
                                SpriteMade = true;
                            }
                        }
                        else if (Save.TypeIndex == 2)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(1, 0.5f, 0);
                                NewSprite = Instantiate(ShopPrefab, AdjustedStartPos, Quaternion.identity);
                                SpriteMade = true;
                            }
                        }
                        else if (Save.TypeIndex == 3)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0, 0, 0);
                                NewSprite = Instantiate(HospitalPrefab, AdjustedStartPos, Quaternion.identity);
                                SpriteMade = true;
                            }
                        }
                        else if (Save.TypeIndex == 0)
                        {
                            Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.5f, 0.5f, 0); 
                            NewSprite = Instantiate(SmallHousePreFab, AdjustedStartPos, Quaternion.identity);
                            SpriteMade = true;
                        }
                        else if(Save.TypeIndex == 4)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
         //                       Debug.Log("Instantiating town hall");
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
  
                                NewSprite = Instantiate(TownHallPrefab, AdjustedStartPos, Quaternion.identity);
                                SpriteMade = true;
                            }
                            
                        }
                        else if (Save.TypeIndex == 5)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                NewSprite = Instantiate(PowerPlantPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }
                        else if (Save.TypeIndex == 6)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                NewSprite = Instantiate(WindFarmPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }
                        else if (Save.TypeIndex == 7)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                NewSprite = Instantiate(ShoppingCenterPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }
                        else if (Save.TypeIndex == 8)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                NewSprite = Instantiate(TrainStationPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }

                    }
                   

                 }
               // RevertPreviousBuildingHightlight();
              //  PlacedBuilding New = new PlacedBuilding(BuildingsListManager.Buildings[Save.TypeIndex].GetInstance(), new int[] { NewPos.x, NewPos.y }, NewSprite);
             //   New.SetBuildingPos(NewPos);
              //  PlacedBuildings.Add(New);
                //     Debug.Log("New buildings count"+PlacedBuildings.Count);
             //   if (New.GetType().GetIfIsHome())
               // {
                 //   npcHandler.SetHomes();
                //}
            }
        }
        PowerIcons.Add(null);
        return NewSprite;

    }
    void CreateGrid()
    {
        try
        {
            if (MainMenu.NewFileCreated == true)
            {
                //Create new
           //     Debug.Log("Creating new");
                
                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        Vector3Int CurrentPosition = new Vector3Int(x, y, 0);
                        GameMap.SetTile(CurrentPosition, GameTile);
                        GameGrid[x, y] = new Square(0);

                    }
                }
                DBManager.AddNewMapToDB(MainMenu.GetCurrentSaveID(), WIDTH, HEIGHT, GameGrid);
                CreateStartingArea();
                
            }
            else
            {
             //   Debug.Log("Loading map from db");
            //    Debug.Log("SaveID" + MainMenu.GetCurrentSaveID());
                //Get from db and set up
                SaveMapModel CurrentSaveMap = DBManager.GetSpecificMap(MainMenu.GetCurrentSaveID());
                List<SaveBuildingModel> BuildingsFromDb=DBManager.GetAllBuildingsForSave(MainMenu.GetCurrentSaveID());
           //     Debug.Log("CurrentID:" + CurrentSaveMap.AssociatedSaveID);
           //     Debug.Log("Map size" + CurrentSaveMap.GridData.Length);
                byte[] UnconvertedMap = CurrentSaveMap.GridData;
                for(int i=0;i< UnconvertedMap.Length; i++)
                {
             //       Debug.Log(UnconvertedMap[i]);
                }
                for (int x = 0; x < CurrentSaveMap.GridWidth; x++)
                {
                    for (int y = 0; y < CurrentSaveMap.GridHeight; y++)
                    {
                    
                        GameGrid[x, y] =new Square(  UnconvertedMap[x + y * CurrentSaveMap.GridWidth]);
                        Vector3Int CurrentPosition = new Vector3Int(x, y, 0);
                        if (GameGrid[x, y].Contains == 0){
                            GameMap.SetTile(CurrentPosition, GameTile);
                            
                        }
                        if (GameGrid[x, y].Contains == 1)
                        {
                            GameMap.SetTile(CurrentPosition, RoadTile);
                     //       Debug.Log("Placing road tile");
                            RoadPositions.Add(CurrentPosition);
                            NumberOfRoads++;
                        }
                        if (GameGrid[x, y].Contains == 2)
                        {
                            GameMap.SetTile(CurrentPosition, GameTile);

                            //   GameMap.SetTile(CurrentPosition, GameTile);
                            //  TransportPlacementScript.PlaceRail(CurrentPosition);
                        }
                        if (GameGrid[x,y].Contains == 4)
                        {
                            //place rail 
                            GameMap.SetTile(CurrentPosition, GameTile);
                            TransportHandler.PlaceRailOnSaveLoad(CurrentPosition);
                        }
                        if (GameGrid[x, y].Contains == 5)
                        {
                            //place bus stop
                            GameMap.SetTile(CurrentPosition, BusStopTile);
                            //       Debug.Log("Placing road tile");
                            RoadPositions.Add(CurrentPosition);
                            NumberOfRoads++;
                            NumberOfBusStops++;

                        }


                    }
                }
                Building[] BaseBuildingTypes = BuildingsListManager.GetBuildings();
            //    Debug.Log("Number of buildings from db:"+BuildingsFromDb.Count);
                for (int i = 0; i < BuildingsFromDb.Count; i++) {
                    PlacedBuilding New = new PlacedBuilding(BaseBuildingTypes[BuildingsFromDb[i].TypeIndex], new int[] { BuildingsFromDb[i].OriginX, BuildingsFromDb[i].OriginY, }, GetSriteForBuilding(BuildingsFromDb[i]));
                    New.SetBuildingPos(new Vector3(BuildingsFromDb[i].Xpos, BuildingsFromDb[i].Ypos, 0));
                    New.SetWarningSprite(NoPowerForBuildingWarning);
                    New.SetWarningSprite(Instantiate(NoPowerForBuildingWarning, new Vector3(BuildingsFromDb[i].Xpos, BuildingsFromDb[i].Ypos, 0), Quaternion.identity));
                    PlacedBuildings.Add(New);
                    



                }
                npcHandler.SetHomes();
                UpdateStatusOfBuildingsInRangeOfPower();
                DisplayPowerAvailabilityOnBuilding();

                TransportPlacementScript.SetupRoutesFromSave(MainMenu.GameSaveID,this,TransportHandler);
                
            }
        }
        catch (Exception e){
            Debug.LogError(e);
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    Vector3Int CurrentPosition = new Vector3Int(x, y, 0);
                    GameMap.SetTile(CurrentPosition, GameTile);
                    GameGrid[x, y] = new Square(0);

                }
            }
            if (MainMenu.GetIfNewFileCreated())
            {
                //Generate starting map
                CreateStartingArea();
            }
            else
            {
                //Generate from save 
            }
        }

        
    }
    private void OnDestroy()
    {
        PlacedBuildings.Clear();
        HomesPlaced.Clear();
        RoadPositions.Clear();
        GameMap = null;
        GameGrid = new Square[WIDTH, HEIGHT];
        RecentlySelectedBuilding = null;
        NearestPowerPlantIndex = -1;

        Sprites.Clear();
        PreviousBuildingHighlight.Clear();
        PowerIcons.Clear();
    }
}

