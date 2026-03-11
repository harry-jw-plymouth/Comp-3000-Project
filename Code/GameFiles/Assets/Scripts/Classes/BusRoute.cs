using System.Collections.Generic;
using UnityEngine;

public class BusRoute
{
    List<Vector3Int> StopsPositions = new List<Vector3Int>();
    List<Vector3Int> RoutePositions = new List<Vector3Int>();
    public Vector3Int StartStop, EndStop;
    public bool HasBeenActivated = false;
    List<Bus> BusesOnRoute = new List<Bus>();
    GameObject SpriteForBuses;
    GameObject SideSprite;




    public BusRoute(Vector3Int Start, Vector3Int End)
    {
        StartStop = Start; EndStop = End;
    }
    public void SetSpritesForBusOnRoute(GameObject Sprite, GameObject Side)
    {
        SpriteForBuses = Sprite;
        SideSprite = Side;
    }
    public bool GetIfActivated()
    {
        return HasBeenActivated;
    }
    public void Activate()
    {

        HasBeenActivated = true;
        GameObject SpriteToSet = Object.Instantiate(SpriteForBuses, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0), Quaternion.identity);
        GameObject SideSpriteToSet = Object.Instantiate(SideSprite, (Vector3)(RoutePositions[0]) + new Vector3(0.25f, 0.75f, 0), Quaternion.identity);
        Bus New = new Bus(RoutePositions[0], SpriteToSet,SideSpriteToSet);
        BusesOnRoute.Add(New);
        New.CurrentlyTargetting = 1;
        New.SetNewTarget(RoutePositions[1]);

        Vector3 start = RoutePositions[0];
        Vector3 target = RoutePositions[1];

        New.SetDirections(target.x > start.x, target.y > start.y);
    }
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
    public void ReactivateBusesOnRoute(NPChandler NpcHandler)
    {
        for (int i = 0; i < BusesOnRoute.Count; i++)
        {
            if (!BusesOnRoute[i].GetIfCurrentlyMoving())
            {
                Bus CurrentBus = BusesOnRoute[i];
                List<int> IDs = new List<int>();

                Vector3Int CurrentStopCell = GridCreator.GameMap.WorldToCell(CurrentBus.GetCurrentStopPos());;

                if (BusesOnRoute[i].CurrentlyAscendingRoute)
                {
                    IDs = NpcHandler.GetNPCsIdWaitingForBus(StartStop, EndStop);
                }
                else
                {
                    IDs = NpcHandler.GetNPCsIdWaitingForBus(EndStop, StartStop);
                }
                BusesOnRoute[i].SetIDsOnBus(IDs);
                BusesOnRoute[i].SetIsCurrentlyMoving(true);
            }
        }
    }
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
                            //     Debug.Log("Reached new postions on route");


                            Vector3 OldTarget = BusesOnRoute[i].CurrentTarget;
                            //       Debug.Log("Reached : " + OldTarget);
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
                            //   TrainsOnRoute[i].CurrentlyTargetting--;
                            //         Debug.Log("Reached new postions on route");

                            Vector3 OldTarget = BusesOnRoute[i].CurrentTarget;
                            //           Debug.Log("Reached : " + OldTarget);
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
                    ;
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



                    //                Debug.Log("Current target:" + TrainsOnRoute[i].CurrentTarget);

                    //              Debug.Log("Train moving, positon before move:" + TrainsOnRoute[i].CurrentPosition) ;
                    BusesOnRoute[i].AdjustPosition(CurrentPosition);
                    BusesOnRoute[i].MoveSprite();
                    //            Debug.Log("Train moved, positon after move:" + TrainsOnRoute[i].CurrentPosition);
               //     BusesOnRoute[i].CreatedSprite.transform.position = BusesOnRoute[i].GetPosition();

                    if (CurrentTarget.x == CurrentPosition.x && CurrentPosition.y == CurrentTarget.y)
                    {
                        //              Debug.Log("Target reached, setting new");
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
    public bool GetIfSquareIsTargetStop(Vector3Int Current, Vector3Int Target)
    {
        if (Current == Target)
        {
            return true;
        }
        return false;
    }
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
    public bool GetIfAlreadyadded(Vector3Int Current, List<Vector3Int> Checked)
    {
        if (Checked.Contains(Current))
        {
            return true;
        }
        return false;
    }
    public List<Vector3Int> GetCurrentRoute()
    {
        return RoutePositions;
    }
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
                // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 1 || GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 1 || GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 5)
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
                }
            }
        }
        return false;

    }
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
                RoutePositions.Reverse();

                return;
            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 1|| GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 1|| GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 1|| GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 5)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 1|| GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 5)
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

    }
}
