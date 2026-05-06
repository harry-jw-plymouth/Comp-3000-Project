using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Citzen
{
    int CitzenID;
    float MovementSpeed = 0.05f;
    int MoveCounter=0;
    int CurrentAction=-1;
    //-1 No action 
    // 0 Moving
    //1 InBuilding
    //2 at home
    //3 in hospital
    //4 enterainment 
    //5 Leaving
    // 6 waiting for train
    // 7 waiting for bus 
    int InBuilding = 0;
    bool TargetIsbuilding=false;
    Building BuildingCurrentlyTargetting;
    Vector3 MovementTarget=new Vector3(0,0,0);
    Vector3 Position;
    Vector3 LastFramePos;
    public bool UpdateNeeded;
    GameObject NPCSprite;
    bool IsHomeLess = true;
    public bool JustEnteredBuilding = false;
    public bool JustLeftBuilding=false;
    Vector3 BuildingInsidePos = new Vector3(-1, -1, -1);
    public int buildingInsideIndex = -1;

    public bool NeedsUpdateAfterTravel = false;
    public bool ReadyToUpdateAfterTravel = false;

    Building Home;
    int HomeIndex = -1;
    Vector3 HomePosition=new Vector3(-1,-1,-1);
    int StuckCount = 0;



    int TiredNess = 0;
    int Sickness = 0;
    int Boredom = 0;
    //happiness rated out of 100
    int Happiness = 0;

    int PositionOnRoute;
    int NexPositionOnRoute;
    List<Vector3> RoutePositions=new List<Vector3>();
    List<Vector3>TrainStationPositionsOnRoute= new List<Vector3>();

    public PlacedBuilding CurrentStation;
    public PlacedBuilding TargetStation;

    public Vector3Int CurrentBusStop;
    public Vector3Int TargetBusStop;

    
    // constructor
    public Citzen(Vector3 Pos,int ID,GameObject sprite)
    {
        NPCSprite = sprite;
        Position= Pos;
        CitzenID = ID;
        UpdateNeeded = true;
       
        CurrentBusStop = new Vector3Int(-1, -1, -1);
        TargetBusStop = new Vector3Int(-1, -1, -1);
    }
    //stop displaying NPC sprite
    public void HideSprite()
    {
        NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
    }
    // return NPC route positions
    public List<Vector3> GetRoutePositions()
    {
        return RoutePositions; 
    }
    // return the citzen ID
    public int GetCitzenID()
    {
        return CitzenID;
    }
    // destroy NPC sprite to prevent errors when NPC removed
    public void RemoveNPCSprite()
    {
        Object.Destroy(NPCSprite);
    }
    // set index of the NPC home position
    public void SetHomeIndex(int Index)
    {
        HomeIndex = Index;
    }
    // do handling for when an NPC gets off a train
    public void GetOffTrain()
    {

        Position = TargetStation.GetBuildingPos();
        SetCurrentAction(0);
        CurrentStation = null;
        TargetStation= null;
        NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
        NPCSprite.transform.position = Position;

        if (NeedsUpdateAfterTravel)
        {
            ReadyToUpdateAfterTravel = true;
            NeedsUpdateAfterTravel = false;
        }
    }
    // do handling for when an NPC gets off a bus
    public void GetOffBus()
    {
       // Debug.Log("Get off bus called");

        //Debug.Log("Npc moved to target stop at " + TargetBusStop);
        //Debug.Log("Start stop was:" + CurrentBusStop);
        Position = GridCreator.GameMap.CellToWorld( TargetBusStop);

        SetCurrentAction(0);
        CurrentBusStop= new Vector3Int(-1, -1, -1);
        TargetBusStop = new Vector3Int(-1,-1,-1);
        NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
        NPCSprite.transform.position = Position;

        if (NeedsUpdateAfterTravel)
        {
            ReadyToUpdateAfterTravel = true;
            NeedsUpdateAfterTravel = false;
        }

    } 
    // return index for NPC home positiom
    public int GetHomeIndex()
    {
        return HomeIndex;
    }
    // return true if NPC in a building
    public bool GetIfInBuilding()
    {
        if (InBuilding > 0)
        {
            return true;
        }
        return false;
    }
    // calculate NPC happiness based on various factors
    public int CalculateHappiness()
    {
        int Happiness = 100;
        Happiness -= TiredNess / 100;
        Happiness -= Sickness / 100;
        Happiness-=Boredom / 100;

        if (Happiness < 0)
        { 
            Happiness = 0;
        }

        if (IsHomeLess)
        {
            Happiness = Happiness / 2;
        }
        return Happiness;
    }
    // get NPC happiness 
    public int GetHappiness()
    {
        return Happiness;
    }
    // Set NPC boredom to new value
    public void AdjustBoredom(int Adjustment)
    {
        Boredom += Adjustment;
    }
    // get NPC current boredom
    public int GetBoredom()
    {
        return Boredom;
    } 
    // increase NPC by a random amount
    public void IncreaseBoredom(int Max)
    {
        Boredom += (int)Random.Range(0, Max);
    }
    // return whether an NPC just entered a building
    public bool GetIfJusteEnteredBuilding()
    {
        return JustEnteredBuilding;
    }
    // return the poositon of the building the NPC wants to go inside
    public Vector3 GetPosOfBuildingToEnter()
    {
        return BuildingInsidePos;
    }
    // increase NPC sickness to a random amount
    public void IncreaseSickness(int Max,int AirQaulity)
    {
        
        Sickness += Random.Range(0, Max);
        if (AirQaulity < 80)
        {
            Sickness += 1;
        }
        if (AirQaulity >= 80&& AirQaulity<50)
        {
            Sickness += 2;
        }
        if (AirQaulity >= 50 && AirQaulity < 30)
        {
            Sickness += 3;
        }
        if (AirQaulity >= 30 && AirQaulity < 10)
        {
            Sickness += 4;
        }
        if (AirQaulity >= 10 && AirQaulity < 0)
        {
            Sickness += 5;
        }
    }
    // get NPC sickness
    public int GetSickness()
    {
        return Sickness;
    }
    // increase NPC tiredness by a somewhat random amount
    public void IncreaseTiredNess()
    {
        TiredNess += (int)Random.Range(0, 2);
    }
    // set NPC home position
    public void SetHome(Building home)
    {
        Home =home;
    }
    // Get NPC Home 
    public Building GetHome()
    {
        return Home;
    }
    // Get NPC Home position
    public Vector3 GetHomePos()
    {
        return HomePosition;
    }
    // Set home positon for NPC
    public void SetHomePos(Vector3 Pos)
    {
        HomePosition = Pos;
    }
    // update status of whether NPC is homeless
    public void UpdateHomeStatus(bool NewStatus)
    {
        IsHomeLess = NewStatus;
    }
    // remove data for NPC Home 
    public void RemoveHomeData()
    {
        Home = null;
        HomePosition = new Vector3(-1, -1, -1);
    }
    // get if NPC is homeless
    public bool GetIfHomeless()
    {
        return IsHomeLess;
    }
    //  set the building currently targetted by NPC to enter
    public void SetTargetBuilding(Building target)
    {
        BuildingCurrentlyTargetting = target;       
    }
    // get tiles surrounding a certain tile
    public List<Vector3Int> GetSurroundingTiles(Vector3Int CurrentPos)
    {
        List<Vector3Int> Positions = new List<Vector3Int>();

        if (GetIfInBounds(CurrentPos.x+1,CurrentPos.y)&& GetIfSquareValid(new Vector3Int(CurrentPos.x+1,CurrentPos.y)))
        {
            Positions.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
        }
        if (GetIfInBounds(CurrentPos.x-1, CurrentPos.y) && GetIfSquareValid(new Vector3Int(CurrentPos.x-1,CurrentPos.y,0 ))) { 
            Positions.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
        }
        if (GetIfInBounds(CurrentPos.x, CurrentPos.y+1) && GetIfSquareValid(new Vector3Int(CurrentPos.x,CurrentPos.y+1)))
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
        }
        if (GetIfInBounds(CurrentPos.x, CurrentPos.y-1) && GetIfSquareValid(new Vector3Int(CurrentPos.x,CurrentPos.y-1)))
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
        }
        return Positions;
    }
    // check if position is a building and return true if it is also a train station
    public List<Vector3Int> CheckForTrainStation(int xChange,int yChange,GridCreator GridHandler,Vector3Int Current)
    {
        List<Vector3Int> newChecks = new List<Vector3Int>();

        int buildingIndex = GridHandler.GetBuildingClicked(new Vector3Int(Current.x + xChange, Current.y + yChange, 0));

        if (buildingIndex == -1)
            return newChecks;

        if (!GridCreator.PlacedBuildings[buildingIndex].GetIfTrainStation())
        {
            return newChecks;
        }
            

        newChecks.Add(new Vector3Int(Current.x + xChange, Current.y + yChange, 0));

        return newChecks;
    }
    // debugging code for displaying a route
    public void ShowRoute()
    {
        for (int i = 0; i < RoutePositions.Count; i++)
        {
            Vector3Int Temp=GridCreator.GameMap.WorldToCell(RoutePositions[i]);
            if (GridCreator.GameGrid[Temp.x, Temp.y].Contains == 5)
            {
            //    Debug.Log(" (Bus stop)Route Pos " + i + ":" + RoutePositions[i]);
            }
            else
            {
              //  Debug.Log(" Route Pos " + i + ":" + RoutePositions[i]);
            }          
        }
    }
    // reactivate sprite for NPC
    public void ReDisplaySprite()
    {
        NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
    }
    // return if square is valid for NPC BFS traversal. blocks water, train tracks and buildings
    public bool GetIfSquareValid(Vector3Int Current)
    {
        return GridCreator.GameGrid[Current.x, Current.y].Contains != 4 &&
                GridCreator.GameGrid[Current.x, Current.y].Contains != 2 &&
                GridCreator.GameGrid[Current.x , Current.y].Contains != 3;
    }
    // Temporary version of set route BFS code used for debugging
    public bool SetRouteNew(Vector3Int Target, Square[,] Grid, Tilemap GameMap,GridCreator GridHandler)
    {
        Debug.Log("Setting new route");
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> TilesAroundStart = GetSurroundingTiles(GameMap.WorldToCell(Position));
        for (int i = 0; i < TilesAroundStart.Count; i++)
        {
            ToCheck.Enqueue(TilesAroundStart[i]);
            AlreadyVisited.Add(TilesAroundStart[i]);
            CameFrom[TilesAroundStart[i]] = TilesAroundStart[i];
        }

        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (Current == Target)
            {
                RoutePositions = new List<Vector3>();
                Vector3Int CurrentRoutePos = Current;
                while (CameFrom[CurrentRoutePos] != CurrentRoutePos)
                {
                    RoutePositions.Add(CurrentRoutePos);
                    CurrentRoutePos = CameFrom[CurrentRoutePos];
                }
                RoutePositions.Add(CurrentRoutePos);
                RoutePositions.Reverse();

                Debug.Log("Route found");
                return true;


            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles

            if (GetIfInBounds(Current.x + 1, Current.y)&&  GetIfSquareValid(new Vector3Int(Current.x + 1, Current.y, 0)))
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
            }
            if (GetIfInBounds(Current.x - 1, Current.y) && GetIfSquareValid(new Vector3Int(Current.x - 1, Current.y, 0)))
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
            }
            if (GetIfInBounds(Current.x, Current.y + 1) && GetIfSquareValid(new Vector3Int(Current.x, Current.y + 1, 0)))
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
            }
            if (GetIfInBounds(Current.x, Current.y - 1) && GetIfSquareValid(new Vector3Int(Current.x, Current.y - 1, 0)))
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
                //  AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
            }
            for (int i = 0; i < NewChecks.Count; i++)
            {
                if (!AlreadyVisited.Contains(NewChecks[i]))
                {
                    ToCheck.Enqueue(NewChecks[i]);
                    AlreadyVisited.Add(NewChecks[i]);
                    CameFrom[NewChecks[i]] = Current;
                }
            }


        }
        Debug.Log("Route not found");
        return false;
    }
    // Breadth first search code which constructs a route for NPC to a target
    // contains commented out debugging code
    // return true if route succesful, false if no route could be made 
    // when setting route, travel routes seen as fast travel, with train stations and bus stops adding the tiles around the corresponding bus stops and train stations
    public bool SetRoute(Vector3Int Target, Square[,]Grid, Tilemap GameMap,GridCreator GridHandler)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();

        Vector3Int StartPos = GameMap.WorldToCell(Position);
        if (GetIfInBounds(StartPos.x, StartPos.y))
        {
            ToCheck.Enqueue(StartPos);
            AlreadyVisited.Add(StartPos);
            CameFrom[StartPos] = StartPos;
        }

        List<Vector3Int> TilesAroundStart = GetSurroundingTiles(GameMap.WorldToCell(Position));
        for (int i = 0; i < TilesAroundStart.Count; i++)
        {
            if (!AlreadyVisited.Contains(TilesAroundStart[i]))
            {
                ToCheck.Enqueue(TilesAroundStart[i]);
                AlreadyVisited.Add(TilesAroundStart[i]);
                CameFrom[TilesAroundStart[i]] = StartPos;
            }               
        }
        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            int currentBuildingIndex = GridHandler.GetBuildingClicked(Current);

            if (currentBuildingIndex != -1)
            {
                PlacedBuilding currentBuilding =GridCreator.PlacedBuildings[currentBuildingIndex];

                if (currentBuilding.GetIfTrainStation())
                {
                    List<Route> RoutesForStation =TransportPlacementScript.GetAllTrainRoutesForStation(currentBuilding);
                    for (int i = 0; i < RoutesForStation.Count; i++)
                    {
                        PlacedBuilding ConnectingStation;

                        if (RoutesForStation[i].StartStation == currentBuilding)
                        {
                            ConnectingStation = RoutesForStation[i].EndStation;
                        }
                        else
                        {
                            ConnectingStation = RoutesForStation[i].StartStation;
                        }

                        Vector3Int ConnectingStationCellPos = new Vector3Int((int)ConnectingStation.GetBuildingPos().x,(int)ConnectingStation.GetBuildingPos().y,0);

                        if (!AlreadyVisited.Contains(ConnectingStationCellPos))
                        {
                            ToCheck.Enqueue(ConnectingStationCellPos);
                            AlreadyVisited.Add(ConnectingStationCellPos);
                            CameFrom[ConnectingStationCellPos] = Current;
                        }
                    }
                }
            }
            else
            {
                if (Grid[Current.x, Current.y].Contains == 5)
                {
                    List<BusRoute> RoutesForStop = TransportPlacementScript.GetAllBusRoutesForStop(Current);
                    for (int i = 0; i < RoutesForStop.Count; i++)
                    {
                        Vector3Int ConnectingStop;
                        if (RoutesForStop[i].StartStop == Current)
                        {
                            ConnectingStop = RoutesForStop[i].EndStop;
                        }
                        else
                        {
                            ConnectingStop = RoutesForStop[i].StartStop;
                        }
                        if(!AlreadyVisited.Contains(ConnectingStop))
                        {
                            ToCheck.Enqueue(ConnectingStop);
                            AlreadyVisited.Add(ConnectingStop);
                            CameFrom[ConnectingStop] = Current;
                        }
                    }
                }


            }

            if (Current == Target)
            {
             //   Debug.Log("Route set for " + CitzenID);
                /*
                RoutePositions = new List<Vector3>();
                Vector3Int CurrentRoutePos = Current;
                while (CameFrom[CurrentRoutePos] != CurrentRoutePos)
                {
                    RoutePositions.Add(CurrentRoutePos);
                    
                    CurrentRoutePos = CameFrom[CurrentRoutePos];
                }
                RoutePositions.Add(GridCreator.GameMap.CellToWorld( CurrentRoutePos));
                RoutePositions.Reverse();

                */
                RoutePositions = new List<Vector3>();

                Vector3Int currentRoutePos = Current;

                while (CameFrom[currentRoutePos] != currentRoutePos)
                {
                    RoutePositions.Add(
                        GridCreator.GameMap.CellToWorld(currentRoutePos)
                    );

                    currentRoutePos = CameFrom[currentRoutePos];
                }

                // add the start node
                RoutePositions.Add(
                    GridCreator.GameMap.CellToWorld(currentRoutePos)
                );

                RoutePositions.Reverse();


                NexPositionOnRoute = 0;
                if (NexPositionOnRoute > RoutePositions.Count)
                {

                    NexPositionOnRoute = RoutePositions.Count - 1;
                }

                // handling for failed route
                if (RoutePositions.Count==0)
                {
                    return false;
                }
                return true;
            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();
            
            //add surrounding tiles
            // check right tile
            if(GetIfInBounds(Current.x + 1, Current.y))
            {
                if (GetIfSquareValid(new Vector3Int(Current.x + 1, Current.y,0)))
                {
                    NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                }
                else if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 2)
                {
                    List<Vector3Int> TrainTiles = CheckForTrainStation(1, 0, GridHandler, Current);
                    for (int i = 0; i < TrainTiles.Count; i++)
                    {
                        NewChecks.Add(TrainTiles[i]);
                    }

                    if (Target.x == Current.x + 1 && Target.y == Current.y)
                    {
                        NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                    }
                }
                
            }
          
            //check left tile
            if (GetIfInBounds(Current.x - 1, Current.y))
            {
                if (GetIfSquareValid(new Vector3Int(Current.x - 1, Current.y,0)))
                {
                    NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                }
                else if(GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 2)
                {
                    List<Vector3Int> TrainTiles = CheckForTrainStation(-1, 0, GridHandler, Current);
                    for (int i = 0; i < TrainTiles.Count; i++)
                    {
                        NewChecks.Add(TrainTiles[i]);
                    }

                    if (Target.x == Current.x - 1 && Target.y == Current.y)
                    {
                        NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                    }

                }
            }
            
            //check above tile
            if (GetIfInBounds(Current.x , Current.y+1))
            {
                if (GetIfSquareValid(new Vector3Int(Current.x,Current.y+1,0)))
                {
                    NewChecks.Add(new Vector3Int(Current.x , Current.y+1, 0));
                }
                else if (GridCreator.GameGrid[Current.x , Current.y+1].Contains == 2)
                {
                    List<Vector3Int> TrainTiles = CheckForTrainStation(0, 1, GridHandler, Current);
                    for (int i = 0; i < TrainTiles.Count; i++)
                    {
                        NewChecks.Add(TrainTiles[i]);
                    }

                    if (Target.x == Current.x && Target.y == Current.y + 1)
                    {
                        NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                    }
                }
            }
           
            //check below tile
            if (GetIfInBounds(Current.x, Current.y - 1))
            {
                if (GetIfSquareValid(new Vector3Int(Current.x,Current.y-1,0)))
                {
                    NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
                }
                else if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 2)
                {
                    List<Vector3Int> TrainTiles = CheckForTrainStation(0, -1, GridHandler, Current);
                    for (int i = 0; i < TrainTiles.Count; i++)
                    {
                        NewChecks.Add(TrainTiles[i]);
                    }

                    if (Target.x == Current.x && Target.y == Current.y-1 - 1)
                    {
                        NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
                    }

                }
            }
           
            // check for new tiles and add them to list to be checked
            for (int i = 0; i < NewChecks.Count; i++)
            {
                if (!AlreadyVisited.Contains(NewChecks[i]))
                {
                    ToCheck.Enqueue(NewChecks[i]);
                    AlreadyVisited.Add(NewChecks[i]);
                    CameFrom[NewChecks[i]] = Current;
                }
            }
        }
      //  Debug.Log("Route could not be set for NPC"+ CitzenID);
        return false;
    }
    // return the bus stop the NPC is cuurrently at/moving too
    public Vector3Int GetCurrentBusStop()
    {
        return CurrentBusStop;
    }
    // return bus stop the NPC is currently targetting
    public Vector3Int GetTargetBusStop()
    {
        return TargetBusStop;
    }
    // return current world positon of NPC
    public Vector3 GetPosition()
    {
        return Position;
    }
    // get current action for NPC
    public int GetCurrentAction()
    {
        return CurrentAction;
    }
    // set current action for NPC
    public void SetCurrentAction(int NewAction)
    {
        CurrentAction = NewAction;
    }
    // set target for movement for NPC
    public void SetMovementTarget(Vector3 Target)
    {
        MovementTarget = Target;
    }
    // Get counter for checking when/frequency of NPC movement 
    public int GetMoveCounter()
    {
        return MoveCounter;
    }
    // update counter for checking when/frequency of NPC movement 
    public void UpdateCounter()
    {
        MoveCounter++;
    }
    // reset counter for checking when/frequency of NPC movement 
    public void ResetCounter()
    {
        MoveCounter = 0;
    }
    // set whether the target position is a building
    public void SetIfTargetIsBuilding(bool Target)
    {
        TargetIsbuilding = Target;
    }
    // return whether the target position is a building
    public bool GetIfTargetIsBuilding()
    {
        return TargetIsbuilding;
    }
    // when building removed, handle NPC to ensure NPC is not in a building that doesnt exist
    public void ForceLeaveBuidlingOnBuildingRemoval()
    {
        NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
        BuildingCurrentlyTargetting = null;
        SetCurrentAction(-1);
        InBuilding = 0;
        buildingInsideIndex = -1;
        JustLeftBuilding = false;
        JustEnteredBuilding = false;

     //   ResetBuildingData();
    }
    // spend time in Building, when time in building finished update NPC action
    public void SpendTImeInBuilding(int AirQaulity)
    {
        IncreaseSickness(2,AirQaulity);
        InBuilding--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            JustLeftBuilding = true;
            buildingInsideIndex=-1;

        }
    }
    // set requires data when NPC leaves a building 
    public void ResetBuildingData()
    {
        buildingInsideIndex = -1;
        JustLeftBuilding = false;
        BuildingInsidePos = new Vector3(-1, -1, -1);
    }
    // adjust how tired the NPC is
    public void AdjustTiredness(int Change)
    {
        TiredNess += Change;
    }
    // return how tired the NPC is
    public int GetTiredNess()
    {
        return TiredNess;
    }
    // adjust NPC Stats for being at home, When being at home long enough, update action so NPC continues
    public void SpendTImeAtHome()
    {
        IncreaseBoredom(1);
        Sickness--;
       // Debug.Log("NPC at home");
        InBuilding--;
        TiredNess--;
        Debug.Log("Time left ay home : " + InBuilding);
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            buildingInsideIndex = -1;
            if (TiredNess < 0)
            {
                TiredNess = 0;
            }

        }
    }
    // adjust NPC Stats for being in hospital, When being in hospital long enough update action so NPC continues
    public void SpendTImeAtHospital()
    {
        IncreaseBoredom(2);
        Sickness-=2;
        // Debug.Log("NPC at home");
        InBuilding--;
        TiredNess--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            buildingInsideIndex = -1;
            SetCurrentAction(-1);
            if (TiredNess < 0)
            {
                TiredNess = 0;
            }
            if (Sickness < 0)
            {
                Sickness = 0;
            }
        }
    }
    // adjust NPC Stats for being in entertainment, When being in entertainment long enough update action so NPC continues
    public void PartakeInEnterainment(int AirQualityRating)
    {
      //  Debug.Log("Partaking in entertainment");
        AdjustBoredom(-1);
        IncreaseSickness(2,AirQualityRating);
        IncreaseTiredNess();

        InBuilding--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            buildingInsideIndex = -1;
            if (Boredom < 0)
            {
                Boredom = 0;
            }
            if (Boredom < 0)
            {
                Boredom = 0;
            }
        }
    }
    // randomly determine time an NPC will spend in a building based on the buildings upper and lower time bound values
    public int GetTimeInBuilding(int LowerBound,int UpperBound)
    {
        return UnityEngine.Random.Range(LowerBound, UpperBound); 
    }
    // return if a square is within the bounds of the map to ensure out of bounds errors do not occur
    public bool GetIfInBounds(int XPos,int YPos)
    {
        return XPos>=0 && XPos<GridCreator.WIDTH && YPos>=0 && YPos<GridCreator.HEIGHT;
    }
    // Move NPC towards target on route 
    // each time it is called, move NPC closer to the next position on route, when a route position met, set movement target to next position on route
    // when final target reached, set NPC actions accordingly
    // contains debugging code commented out 
    public void MoveTowardsTargetOnRoute(GridCreator GridHandler,int AirQaulity)
    {

        //Debug.Log("NPC " + CitzenID + " is moving towards target on route");

        IncreaseBoredom(1);
        IncreaseSickness(1,AirQaulity);
        IncreaseTiredNess();
        if (RoutePositions == null || RoutePositions.Count == 0)
        {
            SetCurrentAction(-1);
            return;
        }    

     //   Debug.Log("Nex position:" + NexPositionOnRoute);
      //  Debug.Log("Route length" + RoutePositions.Count);
        if(Position.y > RoutePositions[NexPositionOnRoute].y)
        {
            // move down
            Position.y = Mathf.Max(Position.y - MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        else
        {
            // move up
            Position.y = Mathf.Min(Position.y + MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        if (Position.x > RoutePositions[NexPositionOnRoute].x)
        {
            //move left 
            NPCSprite.GetComponent<SpriteRenderer>().flipX = true;
            Position.x = Mathf.Max(Position.x - MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        else
        {
            //move right
            NPCSprite.GetComponent<SpriteRenderer>().flipX = false;
            Position.x = Mathf.Min(Position.x + MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        NPCSprite.transform.position = Position;
        if (Vector3.Distance(Position, RoutePositions[NexPositionOnRoute]) < 0.01f)
        {
            if(NexPositionOnRoute == RoutePositions.Count - 1)
            {
                Vector3Int cell = GridCreator.GameMap.WorldToCell(Position);
                if (GridCreator.GameGrid[cell.x, cell.y].Contains == 5 &&CurrentBusStop.x != -1 &&TargetBusStop.x != -1)
                {
              //      Debug.Log("NPC " + CitzenID + " reached bus stop and is now waiting for bus");
                    SetCurrentAction(7); // waiting for bus
                    return;
                }
                //Final target reached
                //Debug.Log("NPC With ID: " + CitzenID + " Finished route at " + RoutePositions[RoutePositions.Count-1]);
                CurrentBusStop = new Vector3Int(-1, -1, -1);
                TargetBusStop=new Vector3Int(-1, -1, -1);
                
                RoutePositions = new List<Vector3>();
                MovementTarget = new Vector3();
                if (TargetIsbuilding)
                {
                    
                    JustEnteredBuilding = true;
                    SetCurrentAction(1);
                    TargetIsbuilding = false;
                    InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(), BuildingCurrentlyTargetting.GetUpperBound());
                    NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                    if (BuildingCurrentlyTargetting.IsHome)
                    {
                 //       Debug.Log("NPC " + CitzenID + " is now home");
                        SetCurrentAction(2);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfIsHospital())
                    {
                    //    Debug.Log("NPC " + CitzenID + " is now partaling in hospital");
                        SetCurrentAction(3);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfEntertainment())
                    {
                 //       Debug.Log("NPC " + CitzenID + " is now partaling in entertainment");
                        SetCurrentAction(4);
                    }
                }
                else
                {
             //       Debug.Log("NPC " + CitzenID + " is now selecting a new route");
                    SetCurrentAction(-1);
                }
            }
            else
            {
                //Next target
                int TempCurrentIndex = NexPositionOnRoute;
                if (TempCurrentIndex == 0)
                {
          //          Debug.Log("NPC " + CitzenID + "Has reached the next point on their route \n Previous position: N/A \n" +
         //           " position just arrived at: " + RoutePositions[TempCurrentIndex] +
          //          "\n Next position : " + RoutePositions[TempCurrentIndex + 1]);

            //        Debug.Log("NexPositiom on route for "+ CitzenID+": "+NexPositionOnRoute);

              //      Debug.Log("Current route for npc " + CitzenID);
                    ShowRoute();
                }

                else if (TempCurrentIndex == RoutePositions.Count - 1)
                {

          //          Debug.Log("NPC " + CitzenID + "Has reached the next point on their route\n Previous position:" + RoutePositions[TempCurrentIndex - 1] + " \n" +
            //        " position just arrived at: " + RoutePositions[TempCurrentIndex] +
              //      "\n Next position : N/A");
                }
                else
                {
       //             Debug.Log("NPC " + CitzenID + "Has reached the next point on their route\n Previous position:" + RoutePositions[TempCurrentIndex - 1] + " \n" +
         //           " position just arrived at: " + RoutePositions[TempCurrentIndex] +
           //         "\n Next position : " + RoutePositions[TempCurrentIndex + 1]);
                }


                    // check for train station
                int BuildingCheckIndex = GridHandler.GetBuildingClicked(GridCreator.GameMap.WorldToCell((RoutePositions[TempCurrentIndex])));
                if (BuildingCheckIndex != -1)
                {
                    PlacedBuilding Station = GridCreator.PlacedBuildings[BuildingCheckIndex];
                    if (Station.GetIfTrainStation())
                    {
                        if (TempCurrentIndex + 1 < RoutePositions.Count)
                        {
                            // Debug.Log("NPC in train station");
                            // NPC is in train station
                            int NextBuildingCheckIndex = GridHandler.GetBuildingClicked(GridCreator.GameMap.WorldToCell((RoutePositions[NexPositionOnRoute + 1])));
                            if (NextBuildingCheckIndex != -1)
                            {
                                //Debug.Log("Building found at next postition");
                                PlacedBuilding NextStation = GridCreator.PlacedBuildings[NextBuildingCheckIndex];
                                if (NextStation.GetIfTrainStation())
                                {
                                    //Debug.Log("Building is train station");
                                    if (TransportPlacementScript.CheckIfRouteBetweenStations(Station.GetBuildingPosAsInt(), NextStation.GetBuildingPosAsInt()))
                                    {
                                        // Debug.Log("Id: "+CitzenID+" Waiting for train at "+Station.GetBuildingPosAsInt());
                                        //Debug.Log("Id: " + CitzenID + " Waiting to go too " + NextStation.GetBuildingPosAsInt());
                                        CurrentStation = Station;
                                        TargetStation = NextStation;
                                        SetCurrentAction(6);
                                        NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                                        NexPositionOnRoute++;
                                        return;
                                        //NexPositionOnRoute++;
                                    }
                                    else
                                    {
                                        //      NexPositionOnRoute++; 
                                    }
                                }
                                else
                                {
                                    //NexPositionOnRoute++;
                                }
                            }
                            else
                            {
                                // NexPositionOnRoute++;
                            }
                        }                      
                    }
                    else
                    {
                       // NexPositionOnRoute++;
                    }
                }

                if (TargetBusStop.x != -1)
                {

                }
                Vector3Int CurrentPosOnRoute =GridCreator.GameMap.WorldToCell(RoutePositions[TempCurrentIndex]);
                if (GridCreator.GameGrid[CurrentPosOnRoute.x, CurrentPosOnRoute.y].Contains == 5)
                {
                    if (TempCurrentIndex + 1 < RoutePositions.Count)
                    {
                        Vector3Int NextPosOnRoute = GridCreator.GameMap.WorldToCell(RoutePositions[TempCurrentIndex + 1]);
                        if (GridCreator.GameGrid[NextPosOnRoute.x, NextPosOnRoute.y].Contains == 5)
                        {
                            if (TransportPlacementScript.CheckIfRouteBetweenBusStops(CurrentPosOnRoute, NextPosOnRoute))
                            {
                                CurrentBusStop = CurrentPosOnRoute;
                                TargetBusStop = NextPosOnRoute;
                                SetCurrentAction(7);
                                NexPositionOnRoute++;
                                return;
                            }
                            else
                            {
                                //       NexPositionOnRoute++;
                            }
                        }
                        else
                        {
                            //    NexPositionOnRoute++;
                        }
                    }                   
                }
                NexPositionOnRoute++;                    
            }
        }
    }
    // temporary code used for debugging
    public void MoveTowardsTargetOnRouteNew(GridCreator GridHandler, int AirQaulity)
    {
        //Debug.Log("NPC " + CitzenID + " is moving towards target on route");

        IncreaseBoredom(1);
        IncreaseSickness(1,AirQaulity);
        IncreaseTiredNess();
        if (Position.y > RoutePositions[NexPositionOnRoute].y)
        {
          //  Debug.Log("NPC " + CitzenID + " is moving down");
            Position.y = Mathf.Max(Position.y - MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        else
        {
          //  Debug.Log("NPC " + CitzenID + " is moving up");
            Position.y = Mathf.Min(Position.y + MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        if (Position.x > RoutePositions[NexPositionOnRoute].x)
        {
        //    Debug.Log("NPC " + CitzenID + " is moving left");
            Position.x = Mathf.Max(Position.x - MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        else
        {
          //  Debug.Log("NPC " + CitzenID + " is moving right");
            Position.x = Mathf.Min(Position.x + MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        NPCSprite.transform.position = Position;
        if (Vector3.Distance(Position, RoutePositions[NexPositionOnRoute]) < 0.01f)
        {
            if (NexPositionOnRoute == RoutePositions.Count - 1)
            {
                //Final target reached
                MovementTarget = new Vector3();
                if (TargetIsbuilding)
                {
                    JustEnteredBuilding = true;
                    SetCurrentAction(1);
                    TargetIsbuilding = false;
                    InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(), BuildingCurrentlyTargetting.GetUpperBound());
                    NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                    if (BuildingCurrentlyTargetting.IsHome)
                    {
                        //Debug.Log("NPC " + CitzenID + " is now home");
                        SetCurrentAction(2);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfIsHospital())
                    {
                        //Debug.Log("NPC " + CitzenID + " is now partaling in hospital");
                        SetCurrentAction(3);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfEntertainment())
                    {
                        //Debug.Log("NPC " + CitzenID + " is now partaling in entertainment");
                        SetCurrentAction(4);
                    }
                }
                else
                {
                    //Debug.Log("NPC " + CitzenID + " is now selecting a new action");
                    SetCurrentAction(-1);
                }

            }
            else
            {
                //Next target
                NexPositionOnRoute++;
            }
        }
    }
    // old movement code, used before BFS routing was implemented
    public void MovetowardsTarget(int AirQaulity)
    {
        IncreaseBoredom(1);
        IncreaseSickness(1,AirQaulity);
        IncreaseTiredNess();
        // move up
        if (Position.y > MovementTarget.y)
        {
            Position.y = Mathf.Max(Position.y - MovementSpeed, MovementTarget.y);
        }
        else
        {
            Position.y = Mathf.Min(Position.y + MovementSpeed, MovementTarget.y);
        }
        if (Position.x > MovementTarget.x)
        {
            //move left 
            NPCSprite.GetComponent<SpriteRenderer>().flipX = true;
            Position.x = Mathf.Max(Position.x- MovementSpeed,MovementTarget.x);
        }
        else
        {
            //move right
            NPCSprite.GetComponent<SpriteRenderer>().flipX = false;
            Position.x = Mathf.Min(Position.x+ MovementSpeed, MovementTarget.x);
        }
        NPCSprite.transform.position = Position;
        if(MovementTarget.x==Position.x  && MovementTarget.y == Position.y)
        {
         //   Debug.Log("Arrived at target");
            MovementTarget = new Vector3();
            if (TargetIsbuilding)
            {
                JustEnteredBuilding = true;
                SetCurrentAction(1);
                TargetIsbuilding = false;
                InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(), BuildingCurrentlyTargetting.GetUpperBound());
                NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                if (BuildingCurrentlyTargetting.IsHome)
                {
                    SetCurrentAction(2);
                }
                else if (BuildingCurrentlyTargetting.GetIfIsHospital())
                {
                    SetCurrentAction(3);
                }
                else if (BuildingCurrentlyTargetting.GetIfEntertainment())
                {
                    SetCurrentAction(4);
                }
            }
            else
            {
                SetCurrentAction(-1);
            }
        }
    }
}