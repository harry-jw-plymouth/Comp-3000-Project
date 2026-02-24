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
        GameObject Obj = Object.Instantiate(SpriteForTrains, (Vector3)RoutePositions[0], Quaternion.identity);
        Train New = new Train(RoutePositions[0], Obj);
        TrainsOnRoute.Add(New);

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
    public bool GetIfIsNextToTargetStation(Vector3Int pos, PlacedBuilding target)
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
