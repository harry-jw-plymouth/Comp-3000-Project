using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
public class GridCreator : MonoBehaviour
{
    [SerializeField] NPChandler npcHandler;
    public static Square[,]GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    public const int WIDTH = 100;
    public const int HEIGHT = 100;
    [SerializeField] private Tilemap GameMapReference;
    public static Tilemap GameMap;
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

    public RuleTile SmallHouseTile;
    public RuleTile MediumHouseTile;

    public GameObject SmallHousePreFab;
    public GameObject MediumHousePreFab;
    public GameObject HospitalPrefab;
    public GameObject ShopPrefab;
    public GameObject TownHallPrefab;

    public static List<Vector3> RoadPositions=new List<Vector3>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        CreateGrid();
        CenterCamera();
    }
    private void Awake()
    {

        GameMap = GameMapReference;
    }
    void CenterCamera()
    {
        Debug.Log("Camera centered");
        Vector3 CenterPos = GameMap.CellToWorld(new Vector3Int(WIDTH / 2, HEIGHT / 2, 0));
        MainCamera.transform.position = new Vector3(CenterPos.x, CenterPos.y, MainCamera.transform.position.z);
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
    public int GetNumberOfRoads()
    {
        return NumberOfRoads;
    }
    public static int GetNumberOfHospitals()
    {
        int Number = 0;
        for (int i = 0; i < PlacedBuildings.Count; i++)
        {
            if (PlacedBuildings[i].GetIfIsShop())
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
    public static bool GetIfRoadExists()
    {
        for(int Y = 0; Y < HEIGHT; Y++)
        {
            for(int X = 0; X < WIDTH; X++)
            {
               // Debug.Log("COntains:"+ GameGrid[X, Y].Contains);
                if (GameGrid[X, Y].Contains == 1)
                {
                    return true;
                }
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
                if (GameGrid[X, Y].Contains == 1)
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
                if (RemovedBuilding.Shape[X, Y] != -1)
                {
                    Vector3Int CurrentPos = GetPositionForSquare(Origin, RemovedBuilding.Shape, X, Y, RemovedBuilding.Origin);
                    GameMap.SetColor(CurrentPos, new Color(1f, 1f, 1f, 0.5f));
                    GameMap.SetTile(CurrentPos, GameTile);
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
    int GetBuildingClicked(Vector3Int MousePos)
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
                        Debug.Log("Item found at" + CurrentPos);
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
        if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
        {
            if (CheckIfBuildingCanBeplaced(CellClickedPos.x, CellClickedPos.y, BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected]))
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
                                Vector3 AdjustedStartPos = CurrentPos + new Vector3(0, 0, 0);
                                NewSprite = Instantiate(SmallHousePreFab, AdjustedStartPos, Quaternion.identity);

                            }

                        }

                    }
                }
                RevertPreviousBuildingHightlight();
                PlacedBuilding New = new PlacedBuilding(BuildingsListManager.Buildings[BuildingsListManager.BuildingCurrentlySelected].GetInstance(), new int[] { CellClickedPos.x, CellClickedPos.y }, NewSprite);
                New.SetBuildingPos(CellClickedPos);
                PlacedBuildings.Add(New);
           //     Debug.Log("New buildings count"+PlacedBuildings.Count);
                if (New.GetType().GetIfIsHome())
                {
                    npcHandler.SetHomes();
                }
            }
        }
        RevertPreviousBuildingHightlight();

        BuildingsListManager.BuildingCurrentlySelected = -1;

    }
    void PlaceTiles(Vector3Int CellClickedPos)
    {
        //Place tiles 
        try
        {
            if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
            {
                GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 1;
                GameMap.SetTile(CellClickedPos, RoadTile);
                NumberOfRoads++;
                RoadPositions.Add(CellClickedPos);
            }
            else
            {
                if(GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 1)
                {
                    NumberOfRoads--;
                }
                GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 0;
                GameMap.SetTile(CellClickedPos, GameTile);
                RoadPositions.Remove(CellClickedPos);

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
        Debug.Log("Removing Building");
        //building removing check
        if (GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 2)
        {
            int BuildingPos = GetBuildingClicked(CellClickedPos);
            Debug.Log("Building found at poaition");
            if (BuildingPos != -1)
            {
                Debug.Log("Removing building " + BuildingPos);
                RemoveSelectedBuilding(PlacedBuildings[BuildingPos].buildingType,
                    new Vector3Int(PlacedBuildings[BuildingPos].OriginPos[0], PlacedBuildings[BuildingPos].OriginPos[1], 0));
                if (PlacedBuildings[BuildingPos] != null)
                {
                    Destroy(PlacedBuildings[BuildingPos].Sprite);
                }
                PlacedBuildings.RemoveAt(BuildingPos);

                //  if (Sprites[BuildingPos] != null)
                //  {

                // }

            }
        }
    }
    void CheckForMouseClicK() {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 ClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int CellClickedPos = GameMap.WorldToCell(ClickPos);
            // Debug.Log("Click at: " + ClickPos);
          //  Debug.Log("Click at: " + CellClickedPos);
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
                TransportPlacementScript.PlaceRail(CellClickedPos);
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



    // Update is called once per frame
    void Update()
    {
        CheckForMouseHover();
        CheckForMouseClicK();
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
        Vector3 BuildingStart = GameMap.CellToWorld(MapCenter);
        GameObject NewSprite = new GameObject();
        GameGrid[WIDTH/2,HEIGHT/2].Contains = 2;
        Vector3 AdjustedStartPos = MapCenter + new Vector3(-0.5f, 3.5f, 0);
        NewSprite = Instantiate( TownHallPrefab, AdjustedStartPos, Quaternion.identity);
        PlacedBuildings.Add(new PlacedBuilding(BuildingsListManager.Buildings[4]
            , new int[] { MapCenter.x,MapCenter.y },
            NewSprite));
        for(int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++) {
                GameGrid[MapCenter.x+ x,MapCenter.y+ y+3].Contains = 2;
            }
        }
        npcHandler.SetHomes();

    }
    void CreateGrid()
    {
        try
        {
            if (MainMenu.NewFileCreated == false)
            {
                //Create new
                SaveMapModel CurrentMap = DBManager.GetSpecificMap(MainMenu.GetCurrentSaveID());
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
                //Get from db and set up
                SaveMapModel CurrentSaveMap = DBManager.GetSpecificMap(MainMenu.GetCurrentSaveID());
                byte[] UnconvertedMap = CurrentSaveMap.GridData;
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
                        }
                        if (GameGrid[x, y].Contains == 2)
                        {
                            GameMap.SetTile(CurrentPosition, GameTile);
                            TransportPlacementScript.PlaceRail(CurrentPosition);
                        }
                        

                    }
                }
            }
        }
        catch {
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
}
