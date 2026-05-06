using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TransportPlacementScript : MonoBehaviour
{
    int AmountCheckCounter = 0; int CheckFrame = 10;
    public int TransportModeSelected = -1;
    public static  RuleTile TrackTile;
    public GridCreator GridHandler;
    public UIHandlerScript uiHandler;
    public GameStatusScript GameHandler;
    public SoundManagerScript SoundManager;

    public NPChandler NPChandler;
    [SerializeField] private RuleTile TrackTileReference;

    
    public static List<Route> TrainRoutes=new List<Route>();
    public static List<BusRoute> BusRoutes=new List<BusRoute>();

    public GameObject RedModernTrainFront;

    public GameObject RedModernBusFront;
    public GameObject RedModernBusLeft;
    public GameObject RedModernBusRight;
    public GameObject RedModernBusBack;


    public List<Route> RoutesCurrentlyDisplayed;

    public static int RailMode = -1;
    //0 is rail, 1 is rail
    
    public int TransportOption = -1;
    //-1 is not active, 0 is rail, 1 is underground 2 i bus
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    //add new train route
    public static void AddRoute(Route New)
    {
        TrainRoutes.Add(New);
    }
    // add new bus route
    public static void AddBusRoute(BusRoute New)
    {
        BusRoutes.Add(New);
    }
    private void Awake()
    {
        TrackTile = TrackTileReference;
    }
    // return all routes using a certain piece of track
    public static List<Route> GetAllRoutesUsingTrack(Vector3Int TrackPos)
    {
        List<Route> routes = new List<Route>();
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if (TrainRoutes[i].GetCurrentRoute().Contains(TrackPos))
            {
                routes.Add(TrainRoutes[i]);
            }
        }
        return routes;
    }
    // return all routes containing a road position
    public static List<BusRoute> GetAllRoutesUsingRoad(Vector3Int RoadPos)
    {
        List<BusRoute> routes = new List<BusRoute>();
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (BusRoutes[i].GetCurrentRoute().Contains(RoadPos))
            {
                routes.Add(BusRoutes[i]);
            }
        }
        return routes;
    }
    // if loading save, load all routes for save ID and set up
    public static void SetupRoutesFromSave(int ID,GridCreator GridHandler,TransportPlacementScript TransportHandler)
    {
        List<TrainRouteModel>RoutesInDB=DBManager.GetAllTrainRoutesForID(ID);
        for(int i = 0; i < RoutesInDB.Count; i++)
        {
            TrainRouteModel Current = RoutesInDB[i];
            int StartStationIndex= GridHandler.GetBuildingClicked(new Vector3Int(Current.StartXpos, Current.StartYpos, 0));
            int EndStationIndex= GridHandler.GetBuildingClicked(new Vector3Int(Current.EndXpos, Current.EndYpos, 0));

            if(StartStationIndex==-1 || EndStationIndex==-1)
            {
                Debug.Log("Error loading route, station not found");
                continue;
            }
            if (GridCreator.PlacedBuildings[StartStationIndex].GetIfTrainStation()&& GridCreator.PlacedBuildings[EndStationIndex].GetIfTrainStation())
            {
                if (TransportHandler.GetIfLinkBetweenStations(StartStationIndex, EndStationIndex))
                {
                    Route New=new Route(GridCreator.PlacedBuildings[StartStationIndex],GridCreator.PlacedBuildings[EndStationIndex]);
                    New.SetRoute(GridCreator.GameGrid);
                    AddRoute(New);
                }
            }
        }

        List<BusRouteModel> BusRoutesInDB = DBManager.GetAllBusRoutesForID(ID);
        for (int i = 0; i < BusRoutesInDB.Count; i++)
        {
            Debug.Log("Loading Bus route " + i);
            BusRouteModel Current = BusRoutesInDB[i];
            Vector3Int StartStop = new Vector3Int(Current.StartXpos, Current.StartYpos, 0);
            Vector3Int EndStop = new Vector3Int(Current.EndXpos, Current.EndYpos, 0);
            if (StartStop.x < 0 || StartStop.x >= GridCreator.WIDTH || StartStop.y < 0 || StartStop.y >= GridCreator.HEIGHT ||
           EndStop.x < 0 || EndStop.x >= GridCreator.WIDTH || EndStop.y < 0 || EndStop.y >= GridCreator.HEIGHT)
            {
                Debug.Log("Error loading bus route");
                continue;
            }
            if (GridCreator.GameGrid[StartStop.x, StartStop.y].Contains == 5 &&
                GridCreator.GameGrid[EndStop.x, EndStop.y].Contains == 5)
            {
                BusRoute newRoute = new BusRoute(StartStop, EndStop);
                if (newRoute.GetIfPathBetweenBusStops(StartStop, EndStop))
                {
                    newRoute.SetRoute(GridCreator.GameGrid);
                    AddBusRoute(newRoute);
                }
            }
        }
    }
    // loop through all positions occupied by station and return true if any of them match the currebt target position
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
    // check surounding squares and return true if any surrounding squares is the target station
    bool getIfIsNextToTargetStation(Vector3Int pos, PlacedBuilding target)
    {
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.right, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.left, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.up, target)) return true;
        if (GetIfSquareIsPartOfTargetStation(pos + Vector3Int.down, target)) return true;

        return false;
    }
    // return true if a sqaure aready checked in BFS
    public bool GetIfAlreadyadded(Vector3Int Current, List<Vector3Int> Checked)
    {
        if (Checked.Contains(Current))
        {
            return true;
        }
        return false;
    }
    // BFS navigating rail tiles returning true if a valid route can be found between stations 
    public bool GetIfLinkBetweenStations(int StartBuilding,int EndBuilding)
    {
        List<Vector3Int>PositionsToCheck = new List<Vector3Int>();
        List<Vector3Int> AlreadyAdded = new List<Vector3Int>();
        List<Vector3Int>ToCheck= new List<Vector3Int>();
        if (StartBuilding == -1 || EndBuilding == -1)
        {
            return false;
        }

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
                    }
                    if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
                    }
                    if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y + 1].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
                    }
                    if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y - 1].Contains == 4)
                    {
                        ToCheck.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
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
    //Reset value so check is not done for placement
    public void CancelSelection()
    {
        
        TransportModeSelected = -1;
    }
    //Set transport selection to rail
    public void OnRailButtonClicked()
    {
        Debug.Log("Rail selected");
        TransportModeSelected = 0;
    }
    // Set tile editing to placing rail
    public void OnRailPlacementButtonClicked()
    {
        RailMode = 0;
        TransportModeSelected = 0;
    }
    // set placement settings to place train stations
    public void OnStationPlacementButtonClicked()
    {
        Debug.Log("Rail mode set to 1");
        RailMode = 1;
        BuildingsListManager.BuildingCurrentlySelected = 8;
    }
    // unused: would have been for building underground routes
    public void OnUnderGroundButtonClicked()
    {
        //Set transport selection to Underground
        TransportModeSelected = 1; 
    }
    //Set transport selection to Bus
    public void OnbusButtonClicked()
    { 
        Debug.Log("Bus selected");
        TransportModeSelected = 2;
    }
    // do handling for placing rail
    public void DoTransportPlacement(Vector3Int CellClickedPos)
    {
        
        Debug.Log("attempting to build mode " + TransportModeSelected);
        
        if (TransportModeSelected == 0)
        {
            PlaceRail(CellClickedPos);
        }
        else if (TransportModeSelected == 1)
        {
        }
        else if (TransportModeSelected == 2) {
        }
        else
        {
            Debug.Log("No transport mode selected");
        }
    }
    // return tiles surrounding another tikes
    public List<Vector3Int>GetSurroundingTiles(Vector3Int CellClickedPos)
    {
        List<Vector3Int> Tiles = new List<Vector3Int> ();
        Tiles.Add(new Vector3Int(CellClickedPos.x+1, CellClickedPos.y , 0));
        Tiles.Add(new Vector3Int(CellClickedPos.x-1, CellClickedPos.y , 0));
        Tiles.Add(new Vector3Int(CellClickedPos.x, CellClickedPos.y + 1, 0));
        Tiles.Add (new Vector3Int(CellClickedPos.x, CellClickedPos.y-1,0));
        return Tiles;


    }
    // return true if rail can be placed at position, checking if surrounding tiles contain rail
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
    // place all rail from save on game load
    public void PlaceRailOnSaveLoad(Vector3Int CurrentPos)
    {
        GridCreator.GameMap.SetTile(CurrentPos, TrackTile);
    }
    // attempt to place rail at specified positon
    public void PlaceRail(Vector3Int CellClickedPos)
    {
        // debugging information
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
                        uiHandler.OpenNewPopUp("Cant place rail", "Invalid position");
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
            }
        }
        else
        {
            Debug.Log("No rail option selected");
        }
        
    }
    // loop though all train routes, if any of them have just been addded then begin their movement
    void CheckForNewRoutes()
    {
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if (!TrainRoutes[i].GetIfActivated())
            {
                GameObject NewSprite = RedModernTrainFront;
                TrainRoutes[i].SetSpriteForTrainOnRoute(NewSprite);
                TrainRoutes[i].Activate();
                NewSprite.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }
    // loop though all bus routes, if any of them have just been addded then begin their movement
    void CheckForNewBusRoutes()
    {
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (!BusRoutes[i].GetIfActivated())
            {
                GameObject FrontSprite = RedModernBusFront;
                GameObject LeftSprite = RedModernBusLeft;
                GameObject RightSprite = RedModernBusRight;
                GameObject BackSprite = RedModernBusBack;
                BusRoutes[i].SetSpritesForBusOnRoute(FrontSprite,LeftSprite,RightSprite,BackSprite);
                BusRoutes[i].Activate();
            }
        }
    }
    // loop through train routes to allow them to handle movement on the route
    void DoMovement()
    {
        for (int i = 0; i < TrainRoutes.Count; i++) {
            if (TrainRoutes[i].GetIfActivated())
            {
                TrainRoutes[i].DoMovement(NPChandler);
            }
        }
    }
    // loop through bus routes to allow them to handle movement on the route
    void DoBusMovement()
    {
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (BusRoutes[i].GetIfActivated())
            {
                BusRoutes[i].DoMovement(NPChandler);
            }
        }
    }
    // return all train routes
    public static List<Route> GetAllTrainRoutes()
    {
        return TrainRoutes;
    }
    // check all train routes and return true if any of them contain both station position parameters
    public static bool CheckIfRouteBetweenStations(Vector3Int StartPos,Vector3Int EndPos) 
    {
        for(int i = 0;i < TrainRoutes.Count; i++)
        {
            if (TrainRoutes[i].StartStation.GetBuildingPosAsInt()==StartPos && TrainRoutes[i].EndStation.GetBuildingPosAsInt() == EndPos
                || TrainRoutes[i].StartStation.GetBuildingPosAsInt() == EndPos && TrainRoutes[i].EndStation.GetBuildingPosAsInt() == StartPos)
            {
                return true;
            }
        }
        return false;
    }
    // check all bus routes and return true if any of them contain a certain road position
    public static bool CheckIfRouteExistsUsingRoad(Vector3Int RoadPosition)
    {
        for (int i = 0; i <BusRoutes.Count; i++)
        {
            if (BusRoutes[i].GetCurrentRoute().Contains(RoadPosition))
            {
                return true;
            }
        }
        return false;
    }
    // check all train route and return true if any of them contain the track position passed
    public static bool CheckIfRouteExistsUsingTrack(Vector3Int TrackPosition)
    {
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if (TrainRoutes[i].GetCurrentRoute().Contains(TrackPosition))
            {
                return true;
            }
        }
        return false ;
    } 
    // check all bus routes and return true if any of them start/end at the positions passed as parameters 
    public static bool CheckIfRouteBetweenBusStops(Vector3Int StartPos, Vector3Int EndPos)
    {
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (BusRoutes[i].StartStop == StartPos && BusRoutes[i].EndStop == EndPos
                || BusRoutes[i].StartStop == EndPos && BusRoutes[i].EndStop == StartPos)
            {
                return true;
            }
        }
        return false;
    }
    // check all train routes and return all routes using a certain train station
    public static List<Route> GetAllTrainRoutesForStation(PlacedBuilding Current)
    {
        List<Route> routes = new List<Route>();
        for (int i=0;i< TrainRoutes.Count; i++)
        {
            if (TrainRoutes[i].StartStation.GetBuildingPosAsInt() == Current.GetBuildingPosAsInt() ||
                TrainRoutes[i].EndStation.GetBuildingPosAsInt() == Current.GetBuildingPosAsInt())
            {
                routes.Add(TrainRoutes[i]);
            }
        }
        return routes;
    }
    // check all bus routes and return all routes using a certain bus stop
    public static List<BusRoute> GetAllBusRoutesForStop(Vector3Int CurrentStop)
    {
        List<BusRoute> Busroutes = new List<BusRoute>();
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (BusRoutes[i].StartStop == CurrentStop||
                BusRoutes[i].EndStop == CurrentStop)
            {
                Busroutes.Add(BusRoutes[i]);
            }
        }
        return Busroutes;
    }
    // check all train routes and reactivate any trains that have been stopped long enough at a station  
    void CheckForReactivation()
    {
        int TotalCost = 0;
        for (int i = 0; i < TrainRoutes.Count; i++)
        {
            TotalCost+= TrainRoutes[i].ReactivateTrains(NPChandler);
        }
        GameHandler.AdjustMoney(-TotalCost);

        if (TotalCost != 0)
        {
            SoundManager.PlayStartTrain();
        }

    }
    // check all bus routes and reactivate any buses that have been stopped long enough at a bus stop  
    void CheckForBusReactivation()
    {
        int TotalCost= 0;
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            TotalCost+=BusRoutes[i].ReactivateBusesOnRoute(NPChandler);
        }
        GameHandler.AdjustMoney(-TotalCost);
        if (TotalCost != 0)
        {
            SoundManager.PlayStartBus();
        }
    }
    // loop through train routes and remove any cancelled routes that have concluded movement
    public void CheckForcancelledTrains()
    {
        List<Route>DeletedRoutes=new List<Route>();
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if (TrainRoutes[i].GetIfEnded())
            {
                TrainRoutes[i].DestroyRoute();
                TrainRoutes.RemoveAt(i); 
            }
        }
        for (int i = 0; i < DeletedRoutes.Count; i++) { 
            TrainRoutes.Remove(DeletedRoutes[i]);
        }
        NPChandler.UpdateNPCRoutesAfterRoutesRemoval(DeletedRoutes);

    }
    //loop through bus routes and return index of route using the road position, return -1 if not found
    public static int CheckIfRoadIsInUseForRoute(Vector3Int RoadPos)
    {
        for(int i = 0; i < BusRoutes.Count; i++)
        {
            if(BusRoutes[i].GetCurrentRoute().Contains(RoadPos))
            {
                return i;
            }
        }
        return -1;
    }
    // check possible routes and return false if the specified pos would make the bus route impossible if removed
    public static bool GetIfReRoutePossible(Vector3Int RoadPos,int RouteIndex)
    {
        return BusRoutes[RouteIndex].CheckIfRoutePossibleWithEdit(GridCreator.GameGrid, RoadPos);
    }
    // Update bus route after tile on route edited
    public static void UpdateBusRoute(int RouteIndex )
    {
          BusRoutes[RouteIndex].SetRoute(GridCreator.GameGrid);
    }
    // loop through buses and remove any buses routes that are cancelled and have concluded moving
    void CheckForCancelledBuses()
    {
        List<BusRoute> DeletedRoutes = new List<BusRoute>();
        for (int i = 0; i < BusRoutes.Count; i++)
        {
            if (BusRoutes[i].GetIfEnded())
            {
                BusRoutes[i].DestroyRoute();
                BusRoutes.RemoveAt(i);
            }
        }
        for (int i = 0; i < DeletedRoutes.Count; i++)
        {
            BusRoutes.Remove(DeletedRoutes[i]);
        }
        NPChandler.UpdateNPCRoutesAfterBusRoutesRemoval(DeletedRoutes);
    } 
    public static bool CheclIfTrainStationInUse(PlacedBuilding Station)
    {
        for(int i = 0; i < TrainRoutes.Count; i++)
        {
            if ( TrainRoutes[i].StartStation.GetBuildingPosAsInt() == Station.GetBuildingPosAsInt() ||
                TrainRoutes[i].EndStation.GetBuildingPosAsInt() == Station.GetBuildingPosAsInt())
            {
                return true;
            }
        }
        return false;
    }
    // loop through bus routes and return true if any use the bus stop 
    public static bool CheckIfBusStopInUse(Vector3Int BusStopPos)
    {
        for(int i = 0; i < BusRoutes.Count; i++)
        {
            if ( BusRoutes[i].StartStop == BusStopPos || 
                BusRoutes[i].EndStop==BusStopPos ||
                BusRoutes[i].GetCurrentRoute().Contains(BusStopPos))
            {
                return true;
            }

        }
        return false;
    } 
    // do handling for train routes
    void DoTrainRoutes()
    {
        CheckForNewRoutes();
        CheckForReactivation();
        DoMovement();
        CheckForcancelledTrains();
    }
    // hide NPCs sprites who are on a bus
    void CheckForJustReactivatedBuses()
    {
        for (int i = 0; i < BusRoutes.Count; i++) {
            if (BusRoutes[i].GetIfJustActivated())
            {
                NPChandler.HideNPCSBoardingBus(BusRoutes[i].GetNPCIDs());
                BusRoutes[i].SetJustActivated(false);
            }
        }
    }
    // do all handling for bus routes
    void DoBusRoutes()
    {
        CheckForNewBusRoutes();
        CheckForBusReactivation();
        DoBusMovement();
        CheckForCancelledBuses();
        CheckForJustReactivatedBuses();
    }
    // Update is called once per frame
    void Update()
    {
        //check trains and buses every once in a while using as a counter to stop it occuring to often every single frame
        AmountCheckCounter++;
        if (AmountCheckCounter >= CheckFrame)
        {
            AmountCheckCounter = 0;
            DoTrainRoutes();
            DoBusRoutes();
        }
    }
    // clear all train routes to prevent errors when game closed
    public void ClearTrainRoutes()
    {
        if (TrainRoutes != null)
        {
            for (int i = 0; i < TrainRoutes.Count; i++)
            {
                if (TrainRoutes[i] != null)
                {
                    TrainRoutes[i].DestroyRoute();
                }
            }
            TrainRoutes.Clear();
        }
    }
    // clear all bus routes to prevent errors when game closed
    public void ClearBusRoutes()
    {
        if (BusRoutes != null)
        {
            for (int i = 0; i < BusRoutes.Count; i++)
            {
                if (BusRoutes[i] != null)
                {
                    BusRoutes[i].DestroyRoute();
                }
            }
            BusRoutes.Clear();
        }
    }
    // destroy train route information to ensure errors do not occur upon removal
    private void OnDestroy()
    {
        ClearTrainRoutes();
        ClearBusRoutes();
    }

}
