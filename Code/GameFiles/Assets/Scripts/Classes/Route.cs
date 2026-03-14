using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class Route
{
    List<Vector3Int> StopsPositions=new List<Vector3Int>();
    List<Vector3Int> RoutePositions=new List<Vector3Int>();
    public PlacedBuilding StartStation, EndStation;
    public bool HasBeenActivated = false;
    List<Train> TrainsOnRoute = new List<Train>();
    GameObject SpriteForTrains;

    


    public Route(PlacedBuilding Start,PlacedBuilding End)
    {
        StartStation= Start;EndStation= End;
    }
    public void SetSpriteForTrainOnRoute(GameObject Sprite)
    {
        SpriteForTrains= Sprite;
    }
    public bool GetIfActivated()
    {
        return HasBeenActivated;
    }
    public void Activate()
    {
    
        HasBeenActivated= true;
        GameObject SpriteToSet = Object.Instantiate(SpriteForTrains, (Vector3)(RoutePositions[0])+new Vector3(0.25f,0.75f,0), Quaternion.identity);
        Train New = new Train(RoutePositions[0], SpriteToSet);
        TrainsOnRoute.Add(New);
        New.CurrentlyTargetting = 1;
        New.SetNewTarget(RoutePositions[1]);

        Vector3 start = RoutePositions[0];
        Vector3 target = RoutePositions[1];

        New.SetDirections(target.x > start.x, target.y > start.y);

    }
    public List<Vector3Int> GetRailsTouchingStation(PlacedBuilding Current)
    {
        List<Vector3Int>Positions=new List<Vector3Int>();
        for (int y = 0; y < Current.GetShape().GetLength(0); y++)
        {
            for (int x = 0; x < Current.GetShape().GetLength(1); x++)
            {
                Vector3Int CurrentPos = Current.GetBuildingPosAsInt() + new Vector3Int(x, y, 0);
                if (GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 4)
                {
                    Positions.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 4)
                {
                    Positions.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y + 1].Contains == 4)
                {
                    Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
                }
                if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y - 1].Contains == 4)
                {
                    Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
                }
            }
        }
        return Positions;
    }
    /*
    public void ReactivateTrains(NPChandler NpcHandler)
    {
        for (int i = 0; i < TrainsOnRoute.Count; i++)
        {
            if (!TrainsOnRoute[i].GetIfCurrentlyMoving())
            {
                Train CurrentTrain = TrainsOnRoute[i];
                List<int> IDs=new List<int>();
                if (GridCreator.GameMap.WorldToCell( CurrentTrain.GetCurrentStationPos()) ==  StartStation.GetBuildingPos())
                {
                    // Train at start station
                    IDs= NpcHandler.GetNPCsIdWaitingForTrain(StartStation, EndStation);                         
                }
                else
                {
                    //train at end station
                    IDs = NpcHandler.GetNPCsIdWaitingForTrain(EndStation,StartStation);
                }
                if (IDs.Count > 0) {
                    Debug.Log("Picked up NPCs");
                }
                TrainsOnRoute[i].SetIDsOnTrain(IDs);
                TrainsOnRoute[i].SetIsCurrentlyMoving(true);
            }
        }
    }
    */
    public void ReactivateTrains(NPChandler NpcHandler)
    {
        for (int i = 0; i < TrainsOnRoute.Count; i++)
        {
            if (!TrainsOnRoute[i].GetIfCurrentlyMoving())
            {
                if (TrainsOnRoute[i].GetIfTrainCanBeReactivated())
                {
                    Train CurrentTrain = TrainsOnRoute[i];
                    List<int> IDs = new List<int>();

                    Vector3Int trainStationCell = GridCreator.GameMap.WorldToCell(CurrentTrain.GetCurrentStationPos());
                    Vector3Int startCell = GridCreator.GameMap.WorldToCell(StartStation.GetBuildingPos());
                    Vector3Int endCell = GridCreator.GameMap.WorldToCell(EndStation.GetBuildingPos());

                    if (TrainsOnRoute[i].CurrentlyAscendingRoute)
                    {
                        IDs = NpcHandler.GetNPCsIdWaitingForTrain(StartStation, EndStation);
                    }
                    else
                    {
                        IDs = NpcHandler.GetNPCsIdWaitingForTrain(EndStation, StartStation);
                    }
                    TrainsOnRoute[i].SetIDsOnTrain(IDs);
                    TrainsOnRoute[i].SetIsCurrentlyMoving(true);
                    TrainsOnRoute[i].ResetReactivateCount();

                }
                else
                {
                    TrainsOnRoute[i].IncrementReactivateCount();
                }
                
            }
        }
    }
    public void DoMovement(NPChandler NpcHandler)
    {
        for (int i = 0; i < TrainsOnRoute.Count; i++) 
        {
            if (TrainsOnRoute[i].GetIfCurrentlyMoving())
            {
                if (TrainsOnRoute[i].GetIfTargetReached())
                {
                    int CurrentPos = TrainsOnRoute[i].CurrentlyTargetting;

                    if (CurrentPos == RoutePositions.Count - 1 || CurrentPos == 0)
                    {
                        // Drop off NPCs at the station we just arrived at
                        if (TrainsOnRoute[i].CurrentlyAscendingRoute)
                        {
                            NpcHandler.UpdateNPCsAfterTrainJourney(TrainsOnRoute[i].GetNPCIDsOnTrain());
                        }
                        else
                        {
                            NpcHandler.UpdateNPCsAfterTrainJourney(TrainsOnRoute[i].GetNPCIDsOnTrain());
                        }
                        TrainsOnRoute[i].ResetIDsOnTrain();

                        // Change direction
                        TrainsOnRoute[i].CurrentlyAscendingRoute = !TrainsOnRoute[i].CurrentlyAscendingRoute;

                        if (TrainsOnRoute[i].CurrentlyAscendingRoute)
                        {
                            TrainsOnRoute[i].CurrentlyTargetting++;
                        }
                        else
                        {
                            TrainsOnRoute[i].CurrentlyTargetting--;
                        }

                        TrainsOnRoute[i].SetIsCurrentlyMoving(false);
                    }
                    else
                    {
                        if (TrainsOnRoute[i].CurrentlyAscendingRoute)
                        {
                            //     Debug.Log("Reached new postions on route");

                            Vector3 OldTarget = TrainsOnRoute[i].CurrentTarget;
                            //       Debug.Log("Reached : " + OldTarget);
                            TrainsOnRoute[i].CurrentlyTargetting++;

                            TrainsOnRoute[i].SetNewTarget(RoutePositions[TrainsOnRoute[i].CurrentlyTargetting]);
                            Vector3 NewTarget = TrainsOnRoute[i].CurrentTarget;
                            bool x = false; bool y = false;
                            if (NewTarget.x > OldTarget.x)
                            {
                                x = true;
                            }
                            if (NewTarget.y > OldTarget.y)
                            {
                                y = true;
                            }
                            TrainsOnRoute[i].SetDirections(x, y);
                        }
                        else
                        {
                            //   TrainsOnRoute[i].CurrentlyTargetting--;
                            //         Debug.Log("Reached new postions on route");

                            Vector3 OldTarget = TrainsOnRoute[i].CurrentTarget;
                            //           Debug.Log("Reached : " + OldTarget);
                            TrainsOnRoute[i].CurrentlyTargetting--;

                            TrainsOnRoute[i].SetNewTarget(RoutePositions[TrainsOnRoute[i].CurrentlyTargetting]);
                            Vector3 NewTarget = TrainsOnRoute[i].CurrentTarget;
                            bool x = false; bool y = false;
                            if (NewTarget.x > OldTarget.x)
                            {
                                x = true;
                            }
                            if (NewTarget.y > OldTarget.y)
                            {
                                y = true;
                            }
                            TrainsOnRoute[i].SetDirections(x, y);
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
                    Vector3 CurrentPosition = TrainsOnRoute[i].GetPosition();
                    Vector3 CurrentTarget = RoutePositions[TrainsOnRoute[i].CurrentlyTargetting];

                    float XChange = CurrentTarget.x - CurrentPosition.y;
                    float YChange = CurrentTarget.y - CurrentPosition.z;

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
                    TrainsOnRoute[i].AdjustPosition(CurrentPosition);
                    //            Debug.Log("Train moved, positon after move:" + TrainsOnRoute[i].CurrentPosition);
                    TrainsOnRoute[i].CreatedSprite.transform.position = TrainsOnRoute[i].GetPosition()+new Vector3(0.0f,0.25f,0.0f);

                    if (CurrentTarget.x == CurrentPosition.x && CurrentPosition.y == CurrentTarget.y)
                    {
                        //              Debug.Log("Target reached, setting new");
                        //Target reached
                        if (TrainsOnRoute[i].CurrentlyAscendingRoute)
                        {
                            TrainsOnRoute[i].CurrentlyTargetting++;
                        }
                        else
                        {
                            TrainsOnRoute[i].CurrentlyTargetting--;
                        }

                        TrainsOnRoute[i].SetNewTarget(RoutePositions[TrainsOnRoute[i].CurrentlyTargetting]);
                    }
                }
            }
        }
        
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
    public bool GetIfIsNextToTargetStation(Vector3Int Current, PlacedBuilding target)
    {
        if (GetIfSquareIsPartOfTargetStation(new Vector3Int(Current.x+1,Current.y,0), target))
        {
            return true;
        }
        if (GetIfSquareIsPartOfTargetStation(new Vector3Int(Current.x-1,Current.y,0), target)) {
            return true;
        }
        if (GetIfSquareIsPartOfTargetStation(new Vector3Int(Current.x,Current.y+1,0), target))
        {
            return true;
        }
        if (GetIfSquareIsPartOfTargetStation(new Vector3Int(Current.x,Current.y-1,0), target))
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
    public void SetRoute(Square[,]GameGrid)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int>AlreadyVisited=new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> RailsAroundStation=GetRailsTouchingStation(StartStation);
        for (int i = 0; i < RailsAroundStation.Count; i++) { 
            ToCheck.Enqueue(RailsAroundStation[i]);
            AlreadyVisited.Add(RailsAroundStation[i]);
            CameFrom[RailsAroundStation[i]] = RailsAroundStation[i];
        }
        
        while(ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (GetIfIsNextToTargetStation(Current, EndStation))
            {
                RoutePositions=new List<Vector3Int>();
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
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 4)
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
  //  public void SetRoute()List
  //  {

//    }
}
