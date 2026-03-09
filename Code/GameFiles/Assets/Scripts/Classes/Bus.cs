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

    public Bus(Vector3Int StartTile, GameObject Sprite)
    {
        CreatedSprite = Sprite;
        CreatedSprite.GetComponent<SpriteRenderer>().enabled = true;
        CurrentPosition = CreatedSprite.transform.position;
    }
    public void SetLastStop(Vector3Int Stop)
    {
        CurrentOrPreviousStop = Stop;
    }
    public Vector3 GetCurrentStopPos()
    {
        return CurrentOrPreviousStop;
    }
    public void SetIDsOnBus(List<int> NPCIds)
    {
        NPCIdsOnBus= NPCIds;
       // Debug.Log("Train picked up " + NPCIds.Count + " people");
    }
    public void ResetIDsOnBus()
    {
        NPCIdsOnBus.Clear();
    }
    public List<int> GetNPCIDsOnBus()
    {
        return NPCIdsOnBus;
    }
    public bool GetIfTargetReached()
    {
        return Vector3.Distance(CurrentPosition, CurrentTarget) < 0.05f;
    }
    public bool GetIfCurrentlyMoving()
    {
        return isCurrentlyMoving;
    }
    public void SetIsCurrentlyMoving(bool New)
    {
        isCurrentlyMoving = New;
    }
    public void SetDirections(bool x, bool y)
    {
        XCurrentlyIncreasing = x; YCurrentlyIncreasing = y;
    }
    public void SetNewTarget(Vector3 Position)
    {
        CurrentTarget = Position;
    }
    public void AdjustPosition(Vector3 position)
    {
        CurrentPosition = position;
    }
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }

}
