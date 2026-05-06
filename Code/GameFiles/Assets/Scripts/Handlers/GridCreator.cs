using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
public class GridCreator : MonoBehaviour
{
    [SerializeField] NPChandler npcHandler;
    [SerializeField] TransportPlacementScript TransportHandler;
    [SerializeField] GameStatusScript GameStatusHandler;
    public static Square[,]GameGrid = new Square[100, 100];
    public static int  WIDTH = 100;
    public static int HEIGHT = 100;
    [SerializeField] private Tilemap GameMapReference;
    public static Tilemap GameMap;
    public UIHandlerScript uiHandler;
    public SoundManagerScript SoundManager;
    public int NumberOfRoads = 0;

    public CameraController MainCameraController;
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
    public RuleTile GreeneryTile;
    public RuleTile WaterTile;

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
    public GameObject NatureAreaPrefab;
    public GameObject WastageCenterPrefab;
    public GameObject RecyclingCenterPrefab;


    public GameStatusScript GameStatusScript;

    public List<Route> RoutesHighlighted = new List<Route>();
    public List<BusRoute> BusRoutesHighlighted = new List<BusRoute>();

    public bool DisplayingBuilding = true;
    public int CurrentlyDisplayedBuilding = -1;

    public GameObject NoPowerForBuildingWarning;
    public int NumberOfBusStops = 0;

    public static List<Vector3> RoadPositions=new List<Vector3>();
    public List<GameObject> PowerIcons=new List<GameObject>();

    public static List<Vector3Int> GreeneryPositions=new List<Vector3Int>();
    public static int NumberOfGreenery = 0;

    public static List<Vector3Int> WaterPositions = new List<Vector3Int>();
    public static int NumberOfWater = 0;

    public static int NearestPowerPlantIndex = -1;

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
    // set camera position to center of map when world generated
    void CenterCamera()
    {
        Vector3 CenterPos = GameMap.CellToWorld(new Vector3Int(WIDTH / 2, HEIGHT / 2, 0));
        MainCamera.transform.position = new Vector3(CenterPos.x, CenterPos.y, MainCamera.transform.position.z);
    }
    // return all placed buildings
    public static List<PlacedBuilding> GetAllBuildings()
    {
        return PlacedBuildings;
    }
    // dehighlight all positions on routes previously highlighted
    public void DeHighlightAllRoutes()
    {
        for (int i = 0; i < RoutesHighlighted.Count; i++) {
            List<Vector3Int> Current = RoutesHighlighted[i].GetCurrentRoute();
            for(int e=0; e<Current.Count; e++)
            {
                GameMap.SetTileFlags(Current[e], TileFlags.None);
                GameMap.SetColor(Current[e], Color.white);
            }
        }
        RoutesHighlighted.Clear();
    }
    // dehighlight all positions on bus routes previously highlighted
    public void DeHighlightAllBusRoutes()
    {
        for (int i = 0; i < BusRoutesHighlighted.Count; i++)
        {
            List<Vector3Int> Current = BusRoutesHighlighted[i].GetCurrentRoute();
            for (int e = 0; e < Current.Count; e++)
            {
                GameMap.SetTileFlags(Current[e], TileFlags.None);
                GameMap.SetColor(Current[e], Color.white);
            }
        }
        BusRoutesHighlighted.Clear();
    }
    //check all sqaures that would be taken up by a placed building, if all are unoccupied return true
    bool CheckIfBuildingCanBeplaced(int x, int y,Building building)
    {
        //Debug.Log("New Y: " + (y + building.Shape.GetLength(0)));
        if (x + building.Shape.GetLength(1) > WIDTH )
        {
            uiHandler.OpenNewPopUp("Cant place building", "Space already occupied");
          //  Debug.Log("Building cant be placed"); 
            return false;
        }
        else if (y + building.Shape.GetLength(0) >= HEIGHT+1)
        {
            uiHandler.OpenNewPopUp("Cant place building", "Space already occupied");
             // Debug.Log("Building cant be placed, final Y pos:"+ (y + building.Shape.GetLength(0)));
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
                    uiHandler.OpenNewPopUp("Cant place building", "Space already occupied");
                 //   Debug.Log(" Building cant be placed, building already occupying square: " + CurrentPos.x + " , " + CurrentPos.y);
                    return false;
                }
            }
        }
        return true;
    } 
    // highlight area where buiilding would be placed
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
    // return number of placed buildings
    public int GetNumberOfBuildings()
    {
        return PlacedBuildings.Count;
    }
    // return number of greenery tiles
    public static int GetNumberOfGreenery()
    {
        return NumberOfGreenery;
    }
    // return number of buildings marked with greenery
    public static int GetNumberOfGreenBuildings()
    {
        int total = 0;
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetType().GetIfIsGreenery())
            {
                total++;
            }
        }
        return total;
    }
    // calculate and return total waste from buildings placed
    public static int GetWasteFromBuildings()
    {
        int Waste = 0;
        List<PlacedBuilding> Buildings =GetAllBuildings();
        for (int i = 0; i < Buildings.Count; i++)
        {
            Waste += Buildings[i].GetType().GetBuildingWaste();
        }
        return Waste;
    }
    // return number of wastage management buildings
    public static int GetNumberOfWastageFacilities()
    {
        int Amount = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetType().GetIfIsWastageCenter())
            {
                Amount++;
            }
        }
        return Amount;
    }
    // total and return water pollution from placed buildings
    public static int GetWaterPollution()
    {
        int Amount = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            Amount += PlacedBuildings[i].GetType().GetWaterPollution();
        }
        return Amount;
    }
    // total and return power generation from placed buildings
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
            if (PlacedBuildings[i].GetIfIsShop()) 
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
    // find and return postion of nearest road, return 0 vector if no road found
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
    // find and return postion of nearest shop, return -1 vector if no shop found
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
    // calculate and return total environmental effect from placed buildings
    public static int GetTotalEnviormentalEffects()
    {
        int EnviromentalEffects = 0;
        for(int i = 0; i < PlacedBuildings.Count; i++)
        {
            EnviromentalEffects += PlacedBuildings[i].GetEnviromentalValue();
        }
        return EnviromentalEffects;
    }
    // find and return postion of nearest entertainment, return -1 vector if no entertainment found
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
    // find and return postion of nearest hospital, return -1 vector if no hospital found
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
    // return building type selected in building selection UI
    public static Building GetSelectedBuilding()
    {
        return RecentlySelectedBuilding;
    }
    // remove building and set the tiles it occupies to grass tiles
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
    public bool GetIfHighlightedForRoute(Vector3Int CurrentPos)
    {
        for(int i = 0; i < RoutesHighlighted.Count; i++)
        {
            if (RoutesHighlighted[i].GetCurrentRoute().Contains(CurrentPos))
            {
                return true;
            }
            
        }
        return false;
    }
    public bool GetIfHighlightedForBusRoute(Vector3Int CurrentPos)
    {
        for (int i = 0; i < BusRoutesHighlighted.Count; i++)
        {
            if (BusRoutesHighlighted[i].GetCurrentRoute().Contains(CurrentPos))
            {
                return true;
            }

        }
        return false;
    }
    void DisplayBuildingInfoPopUpAboveBuilding(int BuildingIndex,Vector3Int Pos)
    {
        uiHandler.DisplayBuildingInfo(PlacedBuildings[BuildingIndex]);
    }
    void CheckForBuildingInfoPopUp(Vector3Int CellHoverPos)
    {
        if(GameGrid[CellHoverPos.x, CellHoverPos.y].Contains == 2)
        {
            int BuildingPos = GetBuildingClicked(CellHoverPos);
            if (BuildingPos != -1)
            {
                uiHandler.DisplayBuildingInfoAtSpecificPos(PlacedBuildings[BuildingPos], CellHoverPos);
            }
        }
        else
        {
             uiHandler.HideBuildingInfo();
        }
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
                    bool isHighlightedTrain = GetIfHighlightedForRoute(PreviousMousePosition);
                    bool isHighlightedBus = GetIfHighlightedForBusRoute(PreviousMousePosition);

                    if (!isHighlightedTrain && !isHighlightedBus)
                    {
                        GameMap.SetTileFlags(PreviousMousePosition, TileFlags.None);
                        GameMap.SetColor(PreviousMousePosition, Color.white);
                    }
                      
                }
                GameMap.SetTileFlags(MouseHoverPosition, TileFlags.None);
                GameMap.SetColor(MouseHoverPosition, new Color(1f, 1f, 1f, 0.5f));
                if (BuildingsListManager.BuildingCurrentlySelected != -1)
                {
                    RevertPreviousBuildingHightlight();
                    DrawSelectedBuilding(MouseHoverPosition);
                }
                PreviousMousePosition = MouseHoverPosition;

                CheckForBuildingInfoPopUp(MouseHoverPosition);


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

    void PlaceBuilding(Vector3Int CellClickedPos)
    {
        //Place building
        GameObject NewSprite = new GameObject();
        Building CurrentlySelected = BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected];
        if(!GameStatusScript.CheckIfPurchaseAffordable(CurrentlySelected.CostToBuild))
        {
            uiHandler.ShowAlertPopUp("Not enough money to build");
            uiHandler.OpenNewPopUp("Cant place building", "Not enough money to build");
         //    Debug.Log("Not enough money to build");
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
                                        SoundManager.PlayShopSoundEffect();
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
                                        SoundManager.PlayShopSoundEffect();
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
                                else if (BuildingsListManager.BuildingCurrentlySelected == 9)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                        NewSprite = Instantiate(NatureAreaPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 10)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                        NewSprite = Instantiate(WastageCenterPrefab, AdjustedStartPos, Quaternion.identity);
                                    }
                                }
                                else if (BuildingsListManager.BuildingCurrentlySelected == 11)
                                {
                                    if (BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].Shape[Y, X] == 0)
                                    {
                                        Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                        NewSprite = Instantiate(RecyclingCenterPrefab, AdjustedStartPos, Quaternion.identity);
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

                    SoundManager.PlayPlaceBuilding();

                    if (New.GetType().GetIfIsHome())
                    {
                        npcHandler.SetHomes();
                    }
                    if (MainMenu.GetCurrentGameMode() != 0)
                    {
                        GameStatusScript.DoPlaceBuildingCosts(BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected]);
                    }
                    GameStatusHandler.DisplayMoneyChange();
                    GameStatusHandler.DisplayPowerChange();
                    GameStatusHandler.AdjustMoney(- BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].GetCostToBuild());
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
                    SoundManager.PlayEditTile();
                }
                else if (GameGrid[CellClickedPos.x,CellClickedPos.y].Contains==5)
                {
                    if (TransportPlacementScript.CheckIfBusStopInUse(CellClickedPos)){
                        uiHandler.OpenNewPopUp("Cant remove stop", "stop still in use, route must be cancelled before stop is removed");
                    }
                    else
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                        GameMap.SetTile(CellClickedPos, RoadTile);
                        NumberOfBusStops--;
                        SoundManager.PlayEditTile();
                    }
                        
                }
            }
            else
            {
                if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                {
                    if (UIHandlerScript.GreeneryEditorOn)
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 6;
                        GameMap.SetTile(CellClickedPos, GreeneryTile);
                        NumberOfGreenery++;
                        GreeneryPositions.Add(CellClickedPos);
                        SoundManager.PlayEditTile();

                    }
                    else if (UIHandlerScript.WaterEditorOn)
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 3;
                        GameMap.SetTile(CellClickedPos, WaterTile);
                        NumberOfWater++;
                        WaterPositions.Add(CellClickedPos);
                        SoundManager.PlayEditTile();
                    }
                    else
                    {
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                        GameMap.SetTile(CellClickedPos, RoadTile);
                        NumberOfRoads++;
                        RoadPositions.Add(CellClickedPos);
                        SoundManager.PlayEditTile();
                    }                
                }
                else if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 4)
                {
                    if (TransportPlacementScript.CheckIfRouteExistsUsingTrack(CellClickedPos))
                    {
                        uiHandler.OpenNewPopUp("Cant remove track", "Route exists using route");
                    }
                    else
                    {
                        SoundManager.PlayEditTile();
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                        GameMap.SetTile(CellClickedPos, GameTile);
                    }

                }
                else
                {
                    if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 1)
                    {
                        int RouteIndex = TransportPlacementScript.CheckIfRoadIsInUseForRoute(CellClickedPos);
                        if (RouteIndex!=-1)
                        {
                            //route found using road
                            if(TransportPlacementScript.GetIfReRoutePossible(CellClickedPos, RouteIndex))
                            {
                                //reroute possible, remove road and update route
                                SoundManager.PlayEditTile();
                                NumberOfRoads--;
                                GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                                GameMap.SetTile(CellClickedPos, GameTile);
                                RoadPositions.Remove(CellClickedPos);

                                TransportPlacementScript.UpdateBusRoute(RouteIndex);

                            }
                            else
                            {
                                // reroute not possible, show pop up saying cannot remove road
                                uiHandler.OpenNewPopUp("Cant remove Road", "removing this road will disrupt a bus route");
                            }

                        }
                        else
                        {
                            //no route found using road, remove road
                            NumberOfRoads--;
                            GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                            GameMap.SetTile(CellClickedPos, GameTile);
                            RoadPositions.Remove(CellClickedPos);
                        }
                        
                    }
                    else if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 5)
                    {
                        if (TransportPlacementScript.CheckIfBusStopInUse(CellClickedPos))
                        {
                            uiHandler.OpenNewPopUp("Cant remove stop", "stop still in use, route must be cancelled before stop is removed");
                        }
                        else {
                            SoundManager.PlayEditTile();
                            NumberOfBusStops--;
                            GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                            GameMap.SetTile(CellClickedPos, GameTile);
                            RoadPositions.Remove(CellClickedPos);
                        }
                    }
                    else if(GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 6)
                    {
                        if (UIHandlerScript.GreeneryEditorOn)
                        {
                            SoundManager.PlayEditTile();
                            NumberOfGreenery--;
                            GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                            GameMap.SetTile(CellClickedPos, GameTile);
                            GreeneryPositions.Remove(CellClickedPos);
                        }
                        
                    }
                    else
                    {
                        SoundManager.PlayEditTile();
                        GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                        GameMap.SetTile(CellClickedPos, GameTile);
                        RoadPositions.Remove(CellClickedPos);
                        NumberOfRoads = RoadPositions.Count;
                    }
                  
                }
                GameStatusHandler.AdjustMoney(-30);
            }
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
                PlacedBuilding Selected= PlacedBuildings[BuildingPos];
                if (Selected.GetIfTrainStation())
                {
                    if(TransportPlacementScript.CheclIfTrainStationInUse(Selected))
                    {
                        uiHandler.ShowAlertPopUp("Train station in use on route, cannot remove");
                        uiHandler.OpenNewPopUp("Cant remove station", "station still in use, route must be cancelled before station is removed");
                        return;
                    }
                }

                RemoveSelectedBuilding(PlacedBuildings[BuildingPos].buildingType,
                    new Vector3Int(PlacedBuildings[BuildingPos].OriginPos[0], PlacedBuildings[BuildingPos].OriginPos[1], 0));
                if (PlacedBuildings[BuildingPos] != null)
                {
                    Destroy(PlacedBuildings[BuildingPos].Sprite);
                }
                SoundManager.PlayBuildingRemove();
                PlacedBuildings[BuildingPos].DestroyWarning();
                npcHandler.RemoveAllNPCsFromBuilding(PlacedBuildings[BuildingPos].GetNPCsInBuilding());
                List<int> Indexes = PlacedBuildings[BuildingPos].GetInhabitants();
                PlacedBuildings.RemoveAt(BuildingPos);
                npcHandler.UpdateHomesForNPCsAfterBuildingRemoval(Indexes);

                GameStatusHandler.DisplayMoneyChange();
                GameStatusHandler.DisplayPowerChange();




            }
        }
        UpdateStatusOfBuildingsInRangeOfPower();
        DisplayPowerAvailabilityOnBuilding();
    }

    void DisplayBuildingInfo(int BuildingIndex)
    {
  //      Debug.Log("Displaying building info");
    //    Debug.Log("Displaying:" + PlacedBuildings[BuildingIndex].buildingType);
        uiHandler.DisplayBuildingInfo(PlacedBuildings[BuildingIndex]);

    }
    public void OnPowerWarningClicked()
    {
     //   Debug.Log("Power warning clicked");
    }
    void CheckForStationClicked(Vector3Int CellClickedPos)
    {
        int BuildingPos = GetBuildingClicked(CellClickedPos);
        if (BuildingPos != -1)
        {
            if (PlacedBuildings[BuildingPos].GetIfTrainStation())
            {
       //         Debug.Log("Train station clicked");
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
    void DoClickForRouteManager(Vector3Int ClickPos)
    {
        
        if (uiHandler.TrainRouteViewerOn)
        {
            //check square is for rail
            if (GameGrid[ClickPos.x, ClickPos.y].Contains == 4)
            {
                if (TransportPlacementScript.CheckIfRouteExistsUsingTrack(ClickPos))
                {
                    List<Route> RoutesForPos = TransportPlacementScript.GetAllRoutesUsingTrack(ClickPos);
                    uiHandler.DisplayRoutes(RoutesForPos);
                    for (int i = 0; i < RoutesForPos.Count; i++)
                    {
                        List<Vector3Int> Current = RoutesForPos[i].GetCurrentRoute();
                        for (int e = 0; e < RoutesForPos[i].GetCurrentRoute().Count; e++)
                        {
                            GameMap.SetColor(Current[e], new Color(1f, 1f, 1f, 0.5f));

                        }
                        RoutesHighlighted.Add(RoutesForPos[i]);
                    }

                }
            }
        }
        if (uiHandler.BusRouteViewerOn)
        {
            if (GameGrid[ClickPos.x, ClickPos.y].Contains == 1 || GameGrid[ClickPos.x, ClickPos.y].Contains == 5)
            {
          //      Debug.Log("Road/bus stop clicked");
                if (TransportPlacementScript.CheckIfRouteExistsUsingRoad(ClickPos))
                {
            //        Debug.Log("Route uses road");
                    List<BusRoute> BusRoutesForPos = TransportPlacementScript.GetAllRoutesUsingRoad(ClickPos);
                    uiHandler.DisplayBusRoutes(BusRoutesForPos);
                    for (int i = 0; i < BusRoutesForPos.Count; i++)
                    {
                        List<Vector3Int> Current = BusRoutesForPos[i].GetCurrentRoute();
                        for (int e = 0; e < BusRoutesForPos[i].GetCurrentRoute().Count; e++)
                        {
                            GameMap.SetColor(Current[e], new Color(1f, 1f, 1f, 0.5f));

                        }
                        BusRoutesHighlighted.Add(BusRoutesForPos[i]);
                    }
                }
            }
        }

    }


    void CheckForMouseClicK() {
        if (Input.GetMouseButtonDown(0))
        {
           // uiHandler.OpenNewPopUp("Test", "Test");
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
           // uiHandler.OpenNewPopUp("Test", "Test");

            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int CellClickedPos = GameMap.WorldToCell(ClickPos);
       //      Debug.Log("Click at: " + ClickPos);
         //     Debug.Log("Click at: " + CellClickedPos);
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
            else if (uiHandler.TrainRouteViewerOn|| uiHandler.BusRouteViewerOn)
            {
                DoClickForRouteManager(CellClickedPos);
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
    public static bool GetIfWaterExists()
    {
        return NumberOfWater > 0;
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
    // return posisiton of nearest power plant
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
    // set buildings within range of power plants as being powered
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
    // create road area for NPCs to start on when new save made
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
      //  PlaceBuilding(BuildingStart);

        npcHandler.SetHomes();

    } 
    // Place buildings function but for when laoding map from save
    GameObject GetSriteForBuilding(SaveBuildingModel Save)
    {
        GameObject NewSprite = new GameObject();
        Vector3Int NewPos= new Vector3Int(Save.OriginX,Save.OriginY,0);
        bool SpriteMade = false;
        for (int Y = 0; Y < BuildingsListManager.Buildings[Save.TypeIndex].Shape.GetLength(0); Y++)
        {
            for (int X = 0; X < BuildingsListManager.Buildings[Save.TypeIndex].Shape.GetLength(1); X++)
            {
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
                        else if (Save.TypeIndex == 9)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(-0.5f, -0.5f, 0);
                                NewSprite = Instantiate(NatureAreaPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }else if (Save.TypeIndex == 10)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                NewSprite = Instantiate(WastageCenterPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }
                        else if (Save.TypeIndex == 11)
                        {
                            if (BuildingsListManager.Buildings[Save.TypeIndex].Shape[Y, X] == 0)
                            {
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0.0f, 0.0f, 0);
                                NewSprite = Instantiate(RecyclingCenterPrefab, AdjustedStartPos, Quaternion.identity);
                            }
                        }
                    }
                }
            }
        }
        PowerIcons.Add(null);
        return NewSprite;
    }
    // check if position within bounds of the map
    public bool GetIfInBounds(int XPos, int YPos)
    {
        return XPos >= 0 && XPos < WIDTH && YPos >= 0 && YPos < HEIGHT;
    }
    // get lowest value neighbour from perlin noise map for river generation
    Vector3Int GetLowestNeigbour(int x, int y, float[,]HeightMap)
    {
        Vector3Int LowestNeighbour= new Vector3Int(x, y, 0);
        float CurrentLowestHeight=HeightMap[x, y];

        for (int xOffset= -1; xOffset < 2; xOffset++)
        {
            for (int yOffset = -1; yOffset < 2; yOffset++)
            {
                int NewX= x + xOffset;
                int NewY= y + yOffset;
                if(GetIfInBounds(NewX, NewY))
                {
                    if (HeightMap[NewX, NewY] < CurrentLowestHeight)
                    {
                        CurrentLowestHeight = HeightMap[NewX, NewY];
                        LowestNeighbour = new Vector3Int(NewX, NewY, 0);
                    }
                }
            }
        }
        return LowestNeighbour;
    }
    // generate start positions for rivers, ensuring they all start at squares with atleast a minimum height for the start position
    public List<Vector3Int> GetRiverStartPositions(int Min, int Max, float MinimumHeight, float[,]HeightMap)
    {
        int NumberOfRivers = UnityEngine.Random.Range(Min, Max);
        NumberOfRivers += MainMenu.GetCurrentWorldSize()*20;
        List<Vector3Int> StartPositions = new List<Vector3Int>();
        for (int i = 0; i < NumberOfRivers; i++)
        {
            int x = UnityEngine.Random.Range(0, WIDTH);
            int y = UnityEngine.Random.Range(0, HEIGHT);
            if (HeightMap[x, y] > MinimumHeight)
            {
                if (GameGrid[x, y].Contains == 0)
                {
                    StartPositions.Add(new Vector3Int(x, y, 0));
                }
            }
            else
            {
                while (HeightMap[x, y] <= MinimumHeight && GameGrid[x, y].Contains == 0)
                {
                    x = UnityEngine.Random.Range(0, WIDTH);
                    y = UnityEngine.Random.Range(0, HEIGHT);
                    if (HeightMap[x, y] > MinimumHeight)
                    {
                        if (GameGrid[x, y].Contains == 0)
                        {
                            StartPositions.Add(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
        }
        return StartPositions;
    }
    // check 2 tiles and return true if they are not parralel
    bool CheckIfDiagonal(Vector3Int Next, Vector3Int Current)
    {
        if (Next.x != Current.x && Next.y != Current.y)
        {
            return true;
        }
        return false;
    }
    // generate rivers around the map upon loading save
    void GenerateRivers()
    {
        float[,] HeightMap=new float[WIDTH, HEIGHT];

        float MinimumHeight = 0.5f;
        
        int CurrentWorldSize=MainMenu.GetCurrentWorldSize();
        float Scalevalue=0.3f;
        if (CurrentWorldSize == 0)
        {
            Scalevalue = 0.3f;
        }
        else if (CurrentWorldSize == 1)
        {
            Scalevalue = 0.5f;
        }
        else
        {
            Scalevalue=0.6f;
        }
         // generate perlin noise map to simulate a hill 
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                HeightMap[x, y] = Mathf.PerlinNoise(x * 0.1f, y *0.1f);
             }
        }
        List<Vector3Int> RiverStartPositions = GetRiverStartPositions(5, 8, MinimumHeight, HeightMap);
        List<Vector3Int> RiverPositions = new List<Vector3Int>();

        List<List<Vector3Int>> Rivers = new List<List<Vector3Int>>();

        // loop through river start positions, simulate water flowing down perlin noise hill
        for (int i = 0; i < RiverStartPositions.Count;i++)
        {
            List<Vector3Int> CurrentRiver = new List<Vector3Int>();

            Vector3Int CurrentPos=RiverStartPositions[i];
            int RiverLength = 0;
            bool BasinFound= false;
            while (!BasinFound)
            {
                CurrentRiver.Add(CurrentPos);
                Vector3Int Next=GetLowestNeigbour(CurrentPos.x, CurrentPos.y,HeightMap);
                if(Next==CurrentPos)
                {
                    BasinFound = true;
                }
                else
                {
                    // if water flowing diagonally, add connection points so water is not disconnected
                    if (CheckIfDiagonal(Next, CurrentPos)){
                        Vector3Int Connection1 = new Vector3Int(
                            CurrentPos.x+ (Next.x - CurrentPos.x),
                            CurrentPos.y,
                            0);
                        Vector3Int Connection2 = new Vector3Int(
                            CurrentPos.x ,
                            CurrentPos.y + (Next.y - CurrentPos.y),
                            0);

                        if (HeightMap[Connection1.x,Connection1.y]<HeightMap[Connection2.x, Connection2.y])
                        {
                            CurrentRiver.Add(Connection1);
                        }
                        else
                        {
                            CurrentRiver.Add(Connection2);
                        }
                    }
                    CurrentPos = Next;

                }
            }
            Rivers.Add(CurrentRiver);
        }
        // loop through generated rivers, make them different widths
        for (int i = 0; i < Rivers.Count; i++)
        {
            int Width = UnityEngine.Random.Range(1, 4);

            for (int Pos=0; Pos < Rivers[i].Count; Pos++)
            {
                Vector3Int CurrentPos = new Vector3Int(Rivers[i][Pos].x, Rivers[i][Pos].y, 0);
                if (Width == 1)
                {
                    if(GameGrid[CurrentPos.x, CurrentPos.y].Contains != 3)
                    {
                        GameGrid[CurrentPos.x, CurrentPos.y].Contains = 3;
                        GameMap.SetTile(CurrentPos, WaterTile);
                    }
                          
                }
                else if (Width == 2)
                {
                    if (GameGrid[CurrentPos.x, CurrentPos.y].Contains != 3)
                    {
                        GameGrid[CurrentPos.x, CurrentPos.y].Contains = 3;
                        GameMap.SetTile(CurrentPos, WaterTile);
                    }
                    int XPlace= UnityEngine.Random.Range(0, 2);
                    if (XPlace == 0)
                    {
                        if (GetIfInBounds(CurrentPos.x + 1, CurrentPos.y))
                        {
                            if (GameGrid[CurrentPos.x,+ CurrentPos.y].Contains != 3)
                            {
                                GameGrid[CurrentPos.x+1, CurrentPos.y].Contains = 3;
                                GameMap.SetTile(CurrentPos, WaterTile);
                            }
                        }
                            
                    }
                    else
                    {
                        if(GetIfInBounds(CurrentPos.x-1, CurrentPos.y))
                        {
                            if (GameGrid[CurrentPos.x-1, CurrentPos.y].Contains != 3)
                            {
                                GameGrid[CurrentPos.x-1, CurrentPos.y].Contains = 3;
                                GameMap.SetTile(CurrentPos, WaterTile);
                            }
                        }
                            
                    }

                    int YPlace = UnityEngine.Random.Range(0, 2);
                    if (YPlace == 0)
                    {
                        if(GetIfInBounds(CurrentPos.x, CurrentPos.y + 1))
                        {
                            if (GameGrid[CurrentPos.x, CurrentPos.y+1].Contains != 3)
                            {
                                GameGrid[CurrentPos.x, CurrentPos.y+1].Contains = 3;
                                GameMap.SetTile(CurrentPos, WaterTile);
                            }
                        }
                            
                    }
                    else
                    {
                        if(GetIfInBounds(CurrentPos.x, CurrentPos.y - 1))
                        {
                            if (GameGrid[CurrentPos.x, CurrentPos.y-1].Contains != 3)
                            {
                                GameGrid[CurrentPos.x, CurrentPos.y-1].Contains = 3;
                                GameMap.SetTile(CurrentPos, WaterTile);
                            }
                        }
                            
                    }
                }
                else
                {
                    for (int XOffset = -1; XOffset <= 1; XOffset++)
                    {
                        for (int YOffset = -1; YOffset <= 1; YOffset++)
                        {
                            if (GetIfInBounds(CurrentPos.x + XOffset, CurrentPos.y + YOffset))
                            {
                                if (GameGrid[CurrentPos.x + XOffset, CurrentPos.y + YOffset].Contains != 3)
                                {
                                    GameGrid[CurrentPos.x + XOffset, CurrentPos.y + YOffset].Contains = 3;
                                    GameMap.SetTile(new Vector3Int(CurrentPos.x + XOffset, CurrentPos.y + YOffset, 0), WaterTile);
                                }
                            }
                        }
                    }
                }   
            }  
        }
    }
    // select random postions across the map and distribute greenery
    void ScatterGreenery()
    {
        int Mode = MainMenu.GetCurrentWorldSize();
        for (int i = 0; i < 150+(50*Mode); i++)
        {
            int x = UnityEngine.Random.Range(0, WIDTH);
            int y = UnityEngine.Random.Range(0, HEIGHT);
            if (GameGrid[x, y].Contains == 0)
            {
                GameGrid[x, y].Contains = 6;
                GameMap.SetTile(new Vector3Int(x, y, 0), GreeneryTile);
                NumberOfGreenery++;
                GreeneryPositions.Add(new Vector3Int(x, y, 0));
            }
            int Up=UnityEngine.Random.Range(0, 2);
            if (Up==0)
            {
                if(GetIfInBounds(x, y + 1))
                {
                    if (GameGrid[x, y + 1].Contains == 0)
                    {
                        GameGrid[x, y + 1].Contains = 6;
                        GameMap.SetTile(new Vector3Int(x, y + 1, 0), GreeneryTile);
                        NumberOfGreenery++;
                        GreeneryPositions.Add(new Vector3Int(x, y + 1, 0));
                    }
                }
                
            }
            int Down = UnityEngine.Random.Range(0, 2);
            if (Down == 0)
            {
                if(GetIfInBounds(x, y - 1))
                {
                    if (GameGrid[x, y - 1].Contains == 0)
                    {
                        GameGrid[x, y - 1].Contains = 6;
                        GameMap.SetTile(new Vector3Int(x, y - 1, 0), GreeneryTile);
                        NumberOfGreenery++;
                        GreeneryPositions.Add(new Vector3Int(x, y - 1, 0));
                    }
                }                    
            }
            int Left = UnityEngine.Random.Range(0, 2);
            if (Left == 0)
            {
                if(GetIfInBounds(x+1, y))
                {
                    if (GameGrid[x + 1, y].Contains == 0)
                    {
                        GameGrid[x + 1, y].Contains = 6;
                        GameMap.SetTile(new Vector3Int(x + 1, y, 0), GreeneryTile);
                        NumberOfGreenery++;
                        GreeneryPositions.Add(new Vector3Int(x + 1, y, 0));
                    }
                }
                    
            }
            int Right = UnityEngine.Random.Range(0, 2);
            if (Left == 0)
            {
                if (GetIfInBounds(x - 1, y))
                {
                    if (GameGrid[x - 1, y].Contains == 0)
                    {
                        GameGrid[x - 1, y].Contains = 6;
                        GameMap.SetTile(new Vector3Int(x - 1, y, 0), GreeneryTile);
                        NumberOfGreenery++;
                        GreeneryPositions.Add(new Vector3Int(x - 1, y, 0));
                    }
                }
                    
            }
        }
    }   
    // Load Game map from data base and set up if loading a save file, or generate a map for new save file
    void CreateGrid()
    {
        try
        {
            if (MainMenu.NewFileCreated == true)
            {
                //Create new
                //     Debug.Log("Creating new");


                int WorldType= MainMenu.GetCurrentWorldSize();

                if (WorldType == 0)
                {
                    // Small world
                    WIDTH = 50;
                    HEIGHT = 50;
                }
                else if (WorldType == 1)
                {
                    //medium world
                    WIDTH = 100;
                    HEIGHT = 100;
                }
                else if (WorldType == 2) {
                    //large world
                    WIDTH = 150;
                    HEIGHT = 150;
                }
               // MainCameraController.SetBounds();

                    GameGrid = new Square[WIDTH,HEIGHT];
                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT; y++)
                    {
                        Vector3Int CurrentPosition = new Vector3Int(x, y, 0);
                        GameMap.SetTile(CurrentPosition, GameTile);
                        GameGrid[x, y] = new Square(0);

                    }
                }
                GenerateRivers();
                ScatterGreenery();
                
                CreateStartingArea();
                DBManager.AddNewMapToDB(MainMenu.GetCurrentSaveID(), WIDTH, HEIGHT, GameGrid);
            }
            else
            {
                SaveMapModel CurrentSaveMap = DBManager.GetSpecificMap(MainMenu.GetCurrentSaveID());
                List<SaveBuildingModel> BuildingsFromDb=DBManager.GetAllBuildingsForSave(MainMenu.GetCurrentSaveID());

                byte[] UnconvertedMap = CurrentSaveMap.GridData;

                WIDTH = CurrentSaveMap.GridWidth; HEIGHT=CurrentSaveMap.GridHeight; 
                GameGrid = new Square[WIDTH, HEIGHT];

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
                        if (GameGrid[x,y].Contains == 3){
                            GameMap.SetTile(CurrentPosition, WaterTile);
                            NumberOfWater++;
                            WaterPositions.Add(CurrentPosition);
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
                        if (GameGrid[x, y].Contains == 6)
                        {
                            //place greenery
                            GameMap.SetTile(CurrentPosition, GreeneryTile);
                            NumberOfGreenery++;
                            GreeneryPositions.Add(CurrentPosition);
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

                TransportPlacementScript.SetupRoutesFromSave(MainMenu.CurrentSaveID,this,TransportHandler);
                
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
    // destroy and clear all data that could cause issues on reload
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