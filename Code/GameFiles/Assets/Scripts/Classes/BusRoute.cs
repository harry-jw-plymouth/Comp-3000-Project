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




    public BusRoute(Vector3Int Start, Vector3Int End)
    {
        StartStop = Start; EndStop = End;
    }
    public void SetSpriteForBusOnRoute(GameObject Sprite)
    {
        SpriteForBuses = Sprite;
    }
    public bool GetIfActivated()
    {
        return HasBeenActivated;
    }
}
