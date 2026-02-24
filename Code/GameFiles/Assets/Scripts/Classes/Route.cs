using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Route
{
    List<Vector3Int> StopsPositions=new List<Vector3Int>();
    List<Vector3Int> RoutePositions=new List<Vector3Int>();

    public Route(Vector3Int Start,Vector3Int End)
    {
        SetRoute(Start,End);
    }
    public void SetRoute(Vector3Int Start,Vector3Int End)
    {

    }
  //  public void SetRoute()List
  //  {

//    }
}
