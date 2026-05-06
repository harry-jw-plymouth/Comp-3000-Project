using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Train
{

    public Vector3 CurrentOrPreviousStation;
    public Vector3 CurrentPosition;
    public Vector3Int TargetStation;
    public GameObject CreatedSprite;
    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;

    public int ReactivateCount = 0;
    public int ReactivateTime = 100;

    public List<int> NPCIdsOnTrain = new List<int>();

    public bool IsCancelled = false;

    public bool isCurrentlyMoving = true;
    public Train(Vector3Int StartTile,GameObject Sprite)
    {
        CreatedSprite = Sprite;
        CreatedSprite.GetComponent<SpriteRenderer>().enabled = true;
        CurrentPosition = CreatedSprite.transform.position+ new Vector3(0, 0.25f, 0);
    }
    // return counter for checking if train can begin moving from station again
    public int GetReactivateCount()
    {
        return ReactivateCount;
    }
    // set reactivation counter to 0 
    public void ResetReactivateCount()
    {
        ReactivateCount = 0;
    }
    // destroy sprite info to stop errors when the train is removed
    public void DestroySprite()
    {
        Object.Destroy(CreatedSprite);
    }
    // increcment count for train waiting at station
    public void IncrementReactivateCount()
    {
        ReactivateCount++;
    }
    // return true if the train has waited long enough at the station to be reactivated and begin moving again
    public bool GetIfTrainCanBeReactivated()
    {
        return ReactivateCount >= ReactivateTime;
    }
    // update the position of the last station a train is at for when a new station is reached
    public void SetLastStation(Vector3Int Station)
    {
        CurrentOrPreviousStation = Station;
    }
    //return the positon of the last station the train was at 
    public Vector3 GetCurrentStationPos()
    {
        return CurrentOrPreviousStation;
    }
    // set list of NPC IDs on train to new list
    public void SetIDsOnTrain(List<int> NPCIds)
    {
        NPCIdsOnTrain= NPCIds;
    }
    // clear list of NPC Ids on train
    public void ResetIDsOnTrain()
    {
        NPCIdsOnTrain.Clear();
    }
    // return list of NPC IDs on the train
    public List<int> GetNPCIDsOnTrain()
    {
        return NPCIdsOnTrain;
    }
    // return whether the target has been reached by checking distance to the target
    public bool GetIfTargetReached()
    {
        return Vector3.Distance(CurrentPosition, CurrentTarget) < 0.05f;
    }
    // return whether the train is currently moving
    public bool GetIfCurrentlyMoving()
    {
        return isCurrentlyMoving;
    }
    // set whether the train is currently moving
    public void SetIsCurrentlyMoving(bool New)
    {
        isCurrentlyMoving=New;
    }
    // set what direction the train is currently moving (left/right,up/down)
    public void SetDirections(bool x, bool y)
    {
        XCurrentlyIncreasing = x; YCurrentlyIncreasing = y;
    }
    // set the trains movement target to a new position
    public void SetNewTarget(Vector3 Position)
    {
        CurrentTarget = Position;
    }
    // set the position of the train to  a new value
    public void AdjustPosition(Vector3 position)
    {
        CurrentPosition = position;
    }
    // return current position of the train
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }
}
