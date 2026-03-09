using System.Collections.Generic;
using UnityEngine;

public class Bus
{
    public Vector3 CurrentOrPreviousStop;
    public Vector3 CurrentPosition;
    public Vector3Int TargetStop;
    // public GameObject TrainSpritePrefab;
    public GameObject CreatedSprite;
    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;
    public List<int> NPCIdsOnBus = new List<int>();
    public bool isCurrentlyMoving = true;

}
