using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class BusRoute
{
    List<Vector3Int> StopsPositions = new List<Vector3Int>();
    List<Vector3Int> RoutePositions = new List<Vector3Int>();
    public Vector3Int StartStop, EndStop;
    public bool HasBeenActivated = false;
    List<Bus> BusesOnRoute = new List<Bus>();

    bool RouteJustActivated = false;
    bool RouteJustFinished = false;

    GameObject FrontSprite;
    GameObject LeftSprite;
    GameObject RightSprite;
    GameObject BackSprite;

    public bool IsCancelled = false;
    public bool Ended= false;

    public int CostToRun = 100;
    public int FareCost= 10;


    public BusRoute(Vector3Int Start, Vector3Int End)
    {
        StartStop = Start; EndStop = End;
    }
    // return cost each time bus runs
    public int GetCostToRun()
    {
        return CostToRun;
    }
    // return cost to NPCs to use bus
    public int GetFareCost()
    {
        return FareCost;
    }
    // return true if route has ended
    public bool GetIfEnded()
    {
        return Ended;
    }
    // return true if route has been cancelled
    public void SetCancelled()
    {
        IsCancelled = true;
    }
    // return if route has been cancelled
    public bool GetIfCancelled()
    {
        return IsCancelled;
    }
    // destory all sprites on route to ensure errors do not occur when removed
    public void DestroyRoute()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < BusesOnRoute.Count; i++)
            {
                BusesOnRoute[i].DestroySprite();
            }
            BusesOnRoute.Clear();
        }     
    }
    // set sprites accordingly for route
    public void SetSpritesForBusOnRoute(GameObject Front, GameObject Left,GameObject Right,GameObject Back)
    {
        FrontSprite = Front;
        LeftSprite = Left;
        RightSprite = Right;
        BackSprite = Back;
    }
    // return true if route activated after being created
    public bool GetIfActivated()
    {
        return HasBeenActivated;
    }
    // create sprites for route and set route targets and routing info upon route activation
    public void Activate()
    { 
        HasBeenActivated = true;
        GameObject FrontSpriteToSet = Object.Instantiate(FrontSprite, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0), Quaternion.identity);
        GameObject LeftSpriteToSet = Object.Instantiate(LeftSprite, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0), Quaternion.identity);
        GameObject RightSideSpriteToSet = Object.Instantiate(RightSprite, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0),Quaternion.identity);
        GameObject BackSideSpriteToSet = Object.Instantiate(BackSprite, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0), Quaternion.identity);
        Bus New = new Bus(RoutePositions[0], FrontSpriteToSet,LeftSpriteToSet,RightSideSpriteToSet,BackSideSpriteToSet);
        BusesOnRoute.Add(New);
        New.CurrentlyTargetting = 1;
        New.SetNewTarget(RoutePositions[1]);

        Vector3 start = RoutePositions[0];
        Vector3 target = RoutePositions[1];

        New.SetDirections(target.x > start.x, target.y > start.y);
    }
    // return list of roads surrounding a tile 
    public List<Vector3Int> GetRoadsTouchingStop(Vector3Int CurrentPos)
    {
        List<Vector3Int> Positions = new List<Vector3Int>();
        if (GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 1|| GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 5)
        {
            Positions.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 1 || GridCreator.GameGrid[CurrentPos.x -1, CurrentPos.y].Contains == 5)
        {
            Positions.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y + 1].Contains == 1 || GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y+1].Contains == 5)
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y - 1].Contains == 1 || GridCreator.GameGrid[CurrentPos.x, CurrentPos.y-1].Contains == 5)
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
        }
        return Positions;
    }
    //loop through buses on route, for each bus on route,if it has been stopped for a certain amount of time, begin its movement again
    public int ReactivateBusesOnRoute(NPChandler NpcHandler)
    {
        int ReactivateCost= 0;
        for (int i = 0; i < BusesOnRoute.Count; i++)
        {
            if (!BusesOnRoute[i].GetIfCurrentlyMoving())
            {
                if (IsCancelled)
                {
                    Ended = true;
                }
                else
                {
                    if (BusesOnRoute[i].GetIfBusCanBeReactivated())
                    {
                        ReactivateCost += CostToRun; ;
                        Bus CurrentBus = BusesOnRoute[i];
                        List<int> IDs = new List<int>();

                        Vector3Int CurrentStopCell = GridCreator.GameMap.WorldToCell(CurrentBus.GetCurrentStopPos()); ;

                        if (BusesOnRoute[i].CurrentlyAscendingRoute)
                        {
                            IDs = NpcHandler.GetNPCsIdWaitingForBus(StartStop, EndStop);
                        }
                        else
                        {
                            IDs = NpcHandler.GetNPCsIdWaitingForBus(EndStop, StartStop);
                        }
                        ReactivateCost -=FareCost * IDs.Count;
                        BusesOnRoute[i].SetIDsOnBus(IDs);
                        BusesOnRoute[i].SetIsCurrentlyMoving(true);
                        BusesOnRoute[i].ResetReactivateCount();
                        RouteJustActivated = true;
                    }
                    else
                    {
                        BusesOnRoute[i].IncrementReactivateCount();
                    }
                }    
            }
        }
        return ReactivateCost;
    }
    // return true if the route was just created
    public bool GetIfJustActivated()
    {
        return RouteJustActivated;
    }
    // set whether train was just activated
    public void SetJustActivated(bool New)
    {
        RouteJustActivated= New;
    }
    // return true if the train just finished 
    public bool GetIfJustFinished()
    {
        return RouteJustFinished;
    }
    // set whether the bus had just finished its route
    public void SetJustFinished(bool New)
    {
        RouteJustFinished = New;
    }
    // Movement handling for buses on route 
    // for each bus, check if the bus is currently stopped, if so check if it can reactivate and handle accordingly 
    // if moving, move position of bus and check if its reached its target, when this happens set target to next position on route
    // do this until arrived at final position on route 
    public void DoMovement(NPChandler NpcHandler)
    {
        for (int i = 0; i < BusesOnRoute.Count; i++)
        {
            if (BusesOnRoute[i].GetIfCurrentlyMoving())
            {
                if (BusesOnRoute[i].GetIfTargetReached())
                {
                    int CurrentPos = BusesOnRoute[i].CurrentlyTargetting;

                    if (CurrentPos == RoutePositions.Count - 1 || CurrentPos == 0)
                    {
                        // Drop off NPCs at the stop we just arrived at
                        if (BusesOnRoute[i].CurrentlyAscendingRoute)
                        {
                            NpcHandler.UpdateNPCsAfterBusJourney(BusesOnRoute[i].GetNPCIDsOnBus());
                        }
                        else
                        {
                            NpcHandler.UpdateNPCsAfterBusJourney(BusesOnRoute[i].GetNPCIDsOnBus());
                        }
                        BusesOnRoute[i].ResetIDsOnBus();

                        // Change direction
                        BusesOnRoute[i].CurrentlyAscendingRoute = !BusesOnRoute[i].CurrentlyAscendingRoute;

                        if (BusesOnRoute[i].CurrentlyAscendingRoute)
                        {
                            BusesOnRoute[i].CurrentlyTargetting++;
                        }
                        else
                        {
                            BusesOnRoute[i].CurrentlyTargetting--;
                        }

                        BusesOnRoute[i].SetIsCurrentlyMoving(false);
                    }
                    else
                    {
                        if (BusesOnRoute[i].CurrentlyAscendingRoute)
                        {
                            Vector3 OldTarget = BusesOnRoute[i].CurrentTarget;

                            BusesOnRoute[i].CurrentlyTargetting++;

                            BusesOnRoute[i].SetNewTarget(RoutePositions[BusesOnRoute[i].CurrentlyTargetting]);
                            Vector3 NewTarget = BusesOnRoute[i].CurrentTarget;
                            bool x = false; bool y = false;
                            if (NewTarget.x > OldTarget.x)
                            {
                                x = true;
                            }
                            if (NewTarget.y > OldTarget.y)
                            {
                                y = true;
                            }
                            BusesOnRoute[i].SetDirections(x, y);
                            BusesOnRoute[i].UpdateSprite(NewTarget,OldTarget);
                        }
                        else
                        {

                            Vector3 OldTarget = BusesOnRoute[i].CurrentTarget;
                            BusesOnRoute[i].CurrentlyTargetting--;

                            BusesOnRoute[i].SetNewTarget(RoutePositions[BusesOnRoute[i].CurrentlyTargetting]);
                            Vector3 NewTarget = BusesOnRoute[i].CurrentTarget;
                            bool x = false; bool y = false;
                            if (NewTarget.x > OldTarget.x)
                            {
                                x = true;
                            }
                            if (NewTarget.y > OldTarget.y)
                            {
                                y = true;
                            }
                            BusesOnRoute[i].SetDirections(x, y);
                            BusesOnRoute[i].UpdateSprite(NewTarget, OldTarget);
                        }
                    }
                }
                else
                {
                    //Move
                    float MoveSpeed = 0.1f;
                    
                    Vector3 Movement = new Vector3(0.0f, 0.0f, 0.0f);
                    Movement *= Time.deltaTime;
                    Vector3 CurrentPosition = BusesOnRoute[i].GetPosition();
                    Vector3 CurrentTarget = RoutePositions[BusesOnRoute[i].CurrentlyTargetting];

                    float XChange = CurrentTarget.x - CurrentPosition.y;
                    float YChange = CurrentTarget.y - CurrentPosition.y;

                    if (CurrentPosition.y > CurrentTarget.y)
                    {
                        CurrentPosition.y = Mathf.Max(CurrentPosition.y - MoveSpeed, CurrentTarget.y);
                    }
                    else
                    {
                        CurrentPosition.y = Mathf.Min(CurrentPosition.y + MoveSpeed, CurrentTarget.y);
                    }
                    if (CurrentPosition.x > CurrentTarget.x)
                    {
                        CurrentPosition.x = Mathf.Max(CurrentPosition.x - MoveSpeed, CurrentTarget.x);
                    }
                    else
                    {
                        CurrentPosition.x = Mathf.Min(CurrentPosition.x + MoveSpeed, CurrentTarget.x);
                    }
                    BusesOnRoute[i].AdjustPosition(CurrentPosition);
                    BusesOnRoute[i].MoveSprite();

                    if (BusesOnRoute[i].CurrentlyTargetting == 0 || BusesOnRoute[i].CurrentlyTargetting == RoutePositions.Count - 1)
                    {

                    }
                    else
                    {
                        if (CurrentTarget.x == CurrentPosition.x && CurrentPosition.y == CurrentTarget.y)
                        {
                            //Target reached
                            if (BusesOnRoute[i].CurrentlyAscendingRoute)
                            {
                                BusesOnRoute[i].CurrentlyTargetting++;
                            }
                            else
                            {
                                BusesOnRoute[i].CurrentlyTargetting--;
                            }

                            BusesOnRoute[i].SetNewTarget(RoutePositions[BusesOnRoute[i].CurrentlyTargetting]);
                        }
                    }

                    
                }
            }
        }

    }
    // return true if current position matches target position
    public bool GetIfSquareIsTargetStop(Vector3Int Current, Vector3Int Target)
    {
        if (Current == Target)
        {
            return true;
        }
        return false;
    }
    // check surrounding tiles and return true if any of them are the target
    public bool GetIfIsNextToTargetStop(Vector3Int Current, Vector3Int target)
    {
        if (GetIfSquareIsTargetStop(new Vector3Int(Current.x + 1, Current.y, 0), target))
        {
            return true;
        }
        if (GetIfSquareIsTargetStop(new Vector3Int(Current.x - 1, Current.y, 0), target))
        {
            return true;
        }
        if (GetIfSquareIsTargetStop(new Vector3Int(Current.x, Current.y + 1, 0), target))
        {
            return true;
        }
        if (GetIfSquareIsTargetStop(new Vector3Int(Current.x, Current.y - 1, 0), target))
        {
            return true;
        }

        return false;
    }
    // return true if position already checked in BFS search
    public bool GetIfAlreadyadded(Vector3Int Current, List<Vector3Int> Checked)
    {
        if (Checked.Contains(Current))
        {
            return true;
        }
        return false;
    }
    // return list of route positions
    public List<Vector3Int> GetCurrentRoute()
    {
        return RoutePositions;
    }
    // Bfs search, traversing route between bus stops, returning true if route exists and saving route positions to a list
    public bool GetIfPathBetweenBusStops(Vector3Int StartStop, Vector3Int TargetStop)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();
        List<Vector3Int> RoadAroundStop = GetRoadsTouchingStop(StartStop);
        for (int i = 0; i < RoadAroundStop.Count; i++)
        {
            ToCheck.Enqueue(RoadAroundStop[i]);
            AlreadyVisited.Add(RoadAroundStop[i]);
        }

        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (GetIfIsNextToTargetStop(Current, EndStop))
            {
                return true;
            }
            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 1 || GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 1 || GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
            }
            for (int i = 0; i < NewChecks.Count; i++)
            {
                if (!AlreadyVisited.Contains(NewChecks[i]))
                {
                    ToCheck.Enqueue(NewChecks[i]);
                    AlreadyVisited.Add(NewChecks[i]);
                }
            }
        }
        return false;

    }
    // return NPC IDS on any buses on route
    public List<int> GetNPCIDs()
    {
        List<int> IDs = new List<int>();
        for (int i = 0; i < BusesOnRoute.Count; i++)
        {
            List<int> Current = BusesOnRoute[i].GetNPCIDsOnBus();
            for (int e = 0; e < Current.Count; e++)
            {
                IDs.Add(Current[e]);
            }
        }
        return IDs;
    }
    // BFS search returning true if a valid route can still be found after a tile on route is editied 
    public bool CheckIfRoutePossibleWithEdit(Square[,] GameGrid, Vector3Int EditedPos)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> RoadAroundStop = GetRoadsTouchingStop(StartStop);
        for (int i = 0; i < RoadAroundStop.Count; i++)
        {
            if (RoadAroundStop[i] != EditedPos)
            {
                ToCheck.Enqueue(RoadAroundStop[i]);
                AlreadyVisited.Add(RoadAroundStop[i]);
                CameFrom[RoadAroundStop[i]] = RoadAroundStop[i];
            }
            
        }

        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (GetIfIsNextToTargetStop(Current, EndStop))
            { 
                //route possible
                return true;
            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 1 || GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 1 || GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
            }
            for (int i = 0; i < NewChecks.Count; i++)
            {
                if (!AlreadyVisited.Contains(NewChecks[i]) && NewChecks[i]!=EditedPos)
                {
                    ToCheck.Enqueue(NewChecks[i]);
                    AlreadyVisited.Add(NewChecks[i]);
                    CameFrom[NewChecks[i]] = Current;
                }
            }
        }
        return false;
    }
    // BFS traversal setting route between bus stops using only roads and bus stops
    public void SetRoute(Square[,] GameGrid)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> RoadAroundStop = GetRoadsTouchingStop(StartStop);
        for (int i = 0; i < RoadAroundStop.Count; i++)
        {
            ToCheck.Enqueue(RoadAroundStop[i]);
            AlreadyVisited.Add(RoadAroundStop[i]);
            CameFrom[RoadAroundStop[i]] =RoadAroundStop[i];
        }
        // repeat until check tiles list empty
        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (GetIfIsNextToTargetStop(Current,EndStop ))
            {
                RoutePositions = new List<Vector3Int>();
                Vector3Int CurrentRoutePos = Current;
                while (CameFrom[CurrentRoutePos] != CurrentRoutePos)
                {
                    RoutePositions.Add(CurrentRoutePos);
                    CurrentRoutePos = CameFrom[CurrentRoutePos];
                }
                RoutePositions.Add(CurrentRoutePos);
                RoutePositions.Add(StartStop);
                RoutePositions.Reverse();
                RoutePositions.Add(EndStop);

                return;
            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles fpr checking
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 1|| GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 1|| GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 1|| GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 1|| GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
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

    }
}
