using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using Unity.VisualScripting;

public class TransportPlacementScript : MonoBehaviour
{
    public int TransportModeSelected = -1;
    public static  RuleTile TrackTile;
    public GridCreator GridHandler;
    public UIHandlerScript uiHandler;
    [SerializeField] private RuleTile TrackTileReference;

    public static List<Route> TrainRoutes=new List<Route>();

    public static int RailMode = -1;
    //0 is rail, 1 is rail
    
    public int TransportOption = -1;
    //-1 is not active, 0 is rail, 1 is underground 2 i bus
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public static void AddRoute(Route New)
    {
        TrainRoutes.Add(New);
    }
    private void Awake()
    {
        TrackTile = TrackTileReference;
    }

    public bool GetIfSquareIsPartOfTargetStation(Vector3Int Current, PlacedBuilding Target)
    {
        for (int y = 0; y < Target.GetShape().GetLength(0); y++)
        {
            for (int x = 0; x < Target.GetShape().GetLength(1); x++)
            {
                if (Current == Target.GetBuildingPosAsInt() + new Vector3Int(x, y, 0))
                {
                    return true;
                }
            }
                
        }
        return false;
    }
    bool getIfIsNextToTargetStation(Vector3Int pos, PlacedBuilding target)
    {
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.right, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.left, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.up, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.down, target)) return true;

        return false;
    }
    public bool GetIfAlreadyadded(Vector3Int Current, List<Vector3Int> Checked)
    {
        if (Checked.Contains(Current))
        {
            return true;
        }
        return false;
    }

    public bool GetIfLinkBetweenStations(int StartBuilding,int EndBuilding)
    {
        List<Vector3Int>PositionsToCheck = new List<Vector3Int>();
        List<Vector3Int> AlreadyAdded = new List<Vector3Int>();
        List<Vector3Int>ToCheck= new List<Vector3Int>();
        PlacedBuilding StartStation=GridCreator.PlacedBuildings[StartBuilding];
        PlacedBuilding EndStation = GridCreator.PlacedBuildings[EndBuilding];

        for(int y = 0; y < StartStation.GetShape().GetLength(0); y++)
        {
            for(int x = 0; x<StartStation.GetShape().GetLength(1); x++)
            {
                Vector3Int CurrentPos= StartStation.GetBuildingPosAsInt()+new Vector3Int(x,y,0);
                if (GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 4)
                {
                    PositionsToCheck.Add(new Vector3Int(CurrentPos.x+1,CurrentPos.y,0));
                    AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 4)
                {
                    PositionsToCheck.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
                    AlreadyAdded.Add(new Vector3Int(CurrentPos.x -1, CurrentPos.y, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x , CurrentPos.y+1].Contains == 4)
                {
                    PositionsToCheck.Add(new Vector3Int(CurrentPos.x, CurrentPos.y+1, 0));
                    AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x , CurrentPos.y-1].Contains == 4)
                {
                    PositionsToCheck.Add(new Vector3Int(CurrentPos.x , CurrentPos.y-1, 0));
                    AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));

                }
            }
        }
        bool found = false;
        while (!found && PositionsToCheck.Count > 0) {
            Vector3Int CurrentPos = PositionsToCheck[0];
           // Vector3Int New;
            PositionsToCheck.RemoveAt(0);
            if (getIfIsNextToTargetStation(CurrentPos,EndStation))
            {
                Debug.Log("Link found");
                found = true;
                //Connected station found
            }
            else
            {
                //check if rail
                if (GridCreator.GameGrid[CurrentPos.x, (int)CurrentPos.y].Contains == 4)
                {
                    Vector3Int New = new Vector3Int() ;
                    //add surrounding tiles
                    if (GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                        // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                        // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                    }
                    if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
                      // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
                    }
                    if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y + 1].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
                       // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
                    }
                    if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y - 1].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
                      //  AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
                    }
                    for (int i = 0; i < ToCheck.Count; i++) {
                        if (!GetIfAlreadyadded(ToCheck[i], AlreadyAdded)){
                            PositionsToCheck.Add(ToCheck[i]);
                            AlreadyAdded.Add(ToCheck[i]);
                        }
                    }
                    ToCheck=new List<Vector3Int>();
                }
            }
            
        }
        if (!found)
        {
            Debug.Log("No station connected");
        }
        return found;
    }
    public void CancelSelection()
    {
        //Reset value so check is not done for placement
        TransportModeSelected = -1;
    }
    public void OnRailButtonClicked()
    {
        //Set transport selection to rail
        Debug.Log("Rail selected");
        TransportModeSelected = 0;
    }
    public void OnRailPlacementButtonClicked()
    {
        Debug.Log("Rail mode set to 0");
        RailMode = 0;
        TransportModeSelected = 0;
    }
    public void OnStationPlacementButtonClicked()
    {
        Debug.Log("Rail mode set to 1");
        RailMode = 1;
        BuildingsListManager.BuildingCurrentlySelected = 8;
    }
    public void OnUnderGroundButtonClicked()
    {
        //Set transport selection to Underground
        Debug.Log("Underground selected");
        TransportModeSelected = 1; 
    }
    public void OnbusButtonClicked()
    {
        //Set transport selection to Bus
        Debug.Log("Bus selected");
        TransportModeSelected = 2;
    }
    public void DoBusClickHandling(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place bus route");
    }
    public void DoUnderGroundClickHandling(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place underground");
    }
    public void DoTransportPlacement(Vector3Int CellClickedPos)
    {
        
        Debug.Log("attempting to build mode " + TransportModeSelected);
        
        if (TransportModeSelected == 0)
        {
            PlaceRail(CellClickedPos);
        }
        else if (TransportModeSelected == 1)
        {
            DoUnderGroundClickHandling(CellClickedPos);
        }
        else if (TransportModeSelected == 2) {
            DoBusClickHandling(CellClickedPos);
        }
        else
        {
            Debug.Log("No transport mode selected");
        }
    }
    public void PlaceStation(Vector3Int CellClickedPos)
    {
        Debug.Log("PlacingStations");
        
        
        
    }
    public List<Vector3Int>GetSurroundingTiles(Vector3Int CellClickedPos)
    {
        List<Vector3Int> Tiles = new List<Vector3Int> ();
        Tiles.Add(new Vector3Int(CellClickedPos.x+1, CellClickedPos.y , 0));
        Tiles.Add(new Vector3Int(CellClickedPos.x-1, CellClickedPos.y , 0));
        Tiles.Add(new Vector3Int(CellClickedPos.x, CellClickedPos.y + 1, 0));
        Tiles.Add (new Vector3Int(CellClickedPos.x, CellClickedPos.y-1,0));
        return Tiles;


    }
    public bool GetIfRailCanBePlaced(Vector3Int CellClickedPos)
    {
        List<Vector3Int> Tiles = GetSurroundingTiles(CellClickedPos);
        for(int i = 0; i < Tiles.Count; i++)
        {
            if (GridCreator.GameGrid[Tiles[i].x,Tiles[i].y].Contains == 4)
            {
                return true;
            }else if (GridCreator.GameGrid[Tiles[i].x, Tiles[i].y].Contains == 2)
            {
                int BuildPos = GridHandler.GetBuildingClicked(Tiles[i]);
                if (BuildPos!=-1 ||GridCreator.PlacedBuildings[BuildPos].GetIfTrainStation())
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    public void PlaceRail(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place rail at "+ CellClickedPos.x +", "+CellClickedPos.y);
        Debug.Log("RailMode:" + RailMode);
        if (RailMode != -1)
        {
            if (RailMode == 0)
            {
                //place rail
                try
                {
                    if (GetIfRailCanBePlaced(CellClickedPos))
                    {
                        if (GridCreator.GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
                        {
                            GridCreator.GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 4;
                            GridCreator.GameMap.SetTile(CellClickedPos, TrackTile);
                        }
                    }
                    else
                    {
                        Debug.Log("Rail could not be placed");
                    }
                    
                }
                catch(System.Exception error)
                {

                    
                    Debug.Log("Click not in grid square: "+error);

                }

            }
            else if(RailMode == 1)
            {
                //place stations
                PlaceStation(CellClickedPos);
            }
        }
        else
        {
            Debug.Log("No rail option selected");
        }
        
    }
    void CheckForMouseClick()
    {
        if(Input.GetMouseButtonDown(0) && TransportModeSelected!=-1)
        {
            if (TransportModeSelected != -1)
            {
                if (TransportModeSelected == 0)
                {
                    Debug.Log("Attempting to place rail");
                }
            }
            else
            {
                Debug.Log("No Mode of transport selected");
            }
        }
    }
    void CheckForNewRoutes()
    {
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if (!TrainRoutes[i].GetIfActivated())
            {
                TrainRoutes[i].Activate();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckForNewRoutes();
      //  CheckForMouseClick();
    }

}
