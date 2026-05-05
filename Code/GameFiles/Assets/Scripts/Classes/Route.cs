using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class Route
{
    List<Vector3Int> StopsPositions=new List<Vector3Int>();
    List<Vector3Int> RoutePositions=new List<Vector3Int>();
    public PlacedBuilding StartStation, EndStation;
    public bool HasBeenActivated = false;
    List<Train> TrainsOnRoute = new List<Train>();
    GameObject SpriteForTrains;

    public int CostToRun = 100;
    public int FareCost = 20;

    public bool IsCancelled = false;
    public bool Ended = false;
    public Route(PlacedBuilding Start,PlacedBuilding End)
    {
        StartStation= Start;EndStation= End;
    }
    // Return the cost each time a train on the route runs
    public int GetCostToRun()
    {
        return CostToRun;
    }
    // return the money made per NPC riding the train
    public int GetFareCost()
    {
        return FareCost;
    }
    // return if the train has been cancelled and reached the final destination on its journey 
    public bool GetIfEnded()
    {
        return Ended;   
    } 
    // Set the sprite used to show trains travelling on the route
    public void SetSpriteForTrainOnRoute(GameObject Sprite)
    {
        SpriteForTrains= Sprite;
    }
    // return if the train has begun operation after initial creation
    public bool GetIfActivated()
    {
        return HasBeenActivated;
    } 
    // Set the rotue to being cancelled, this will stop the train running again when it reaches its next stop
    public void SetCancelled()
    {
        IsCancelled = true;
    }
    // return if the route is cancelled
    public bool GetIfCancelled()
    {
        return IsCancelled;
    }
    //return a list of all NPCs currently travelling on this route
    public List<int> GetNPCIDs()
    {
        List<int> IDs = new List<int>();
        for (int i = 0; i < TrainsOnRoute.Count; i++)
        {
            List<int> Current = TrainsOnRoute[i].GetNPCIDsOnTrain();
            for (int e = 0; e < Current.Count; e++)
            {
                IDs.Add(Current[e]);
            }
        }
        return IDs;
    }
    // return the length of the route
    public int GetRouteLength()
    {
        return RoutePositions.Count;
    } 
    // activate a train if it was just created. Create the sprite, set direction information and any information needed for moving
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
    // check all tiles next to the station and return a list of positions that are rail tiles
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

    // destroy all trains on route then clear the list, this prevents errors on shutdown and route removal
    public void DestroyRoute()
    {
        if (Application.isPlaying)
        {
            for (int i = 0; i < TrainsOnRoute.Count; i++)
            {
                Object.Destroy(TrainsOnRoute[i].CreatedSprite);
            }
            TrainsOnRoute.Clear();
        }

    }
    // loop through trains on route, if the train is stopped at a station, check if it is cancelled, if not then set it to continue travelling back down the route
    // during this process, calculate momney spent/gained on fare fees and running cost and return total
    public int ReactivateTrains(NPChandler NpcHandler)
    {
        int totalReactivateCost = 0;

        for (int i = 0; i < TrainsOnRoute.Count; i++)
        {
            if (!TrainsOnRoute[i].GetIfCurrentlyMoving())
            {
                if (IsCancelled)
                {
                    Ended = true;
                }
                else
                {
                    if (TrainsOnRoute[i].GetIfTrainCanBeReactivated())
                    {
                        totalReactivateCost += CostToRun;

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
                        totalReactivateCost-= IDs.Count * FareCost;
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
        return totalReactivateCost;
    } 
    // for each train on route that isnt stopped, check if it has reached its destination, if so set it to stop moving, take NPCs of the train at the new station, and then reverse the direction of the train
    // if it is not at its destination, keep moving it along the route positions on its route
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

                            Vector3 OldTarget = TrainsOnRoute[i].CurrentTarget;
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
                    //target not reached
                    //Move
                    float MoveSpeed = 0.1f;
                    
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
                    TrainsOnRoute[i].AdjustPosition(CurrentPosition);


                    TrainsOnRoute[i].CreatedSprite.transform.position = TrainsOnRoute[i].GetPosition()+new Vector3(0.0f,0.25f,0.0f);

                    if (CurrentTarget.x == CurrentPosition.x && CurrentPosition.y == CurrentTarget.y)
                    {
                        if (TrainsOnRoute[i].CurrentlyTargetting == 0 || TrainsOnRoute[i].CurrentlyTargetting == RoutePositions.Count - 1)
                        {

                        }
                        else
                        {
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
        
    }
    // check the target station, and return true if any of its squares match the current position
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
    // check all 4 directions to see if the target station is next to the current position
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
    // get if sqaure on route is already checked in Breadth first search
    public bool GetIfAlreadyadded(Vector3Int Current, List<Vector3Int> Checked)
    {
        if (Checked.Contains(Current))
        {
            return true;
        }
        return false;
    }
    // return list of route positions on current route
    public List<Vector3Int> GetCurrentRoute()
    {
        return RoutePositions;
    }
    // Breadth first search with railway tiles, when target reached re traverse route to identify how target was reached 
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
}
