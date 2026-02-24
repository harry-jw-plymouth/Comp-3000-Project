using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Route
{
    List<Vector3Int> StopsPositions=new List<Vector3Int>();
    List<Vector3Int> RoutePositions=new List<Vector3Int>();

    public Route(PlacedBuilding Start,PlacedBuilding End)
    {
        SetRoute(Start,End);
    }
    public void SetRoute(PlacedBuilding Start,PlacedBuilding End)
    {

    }
  //  public void SetRoute()List
  //  {

//    }
}
