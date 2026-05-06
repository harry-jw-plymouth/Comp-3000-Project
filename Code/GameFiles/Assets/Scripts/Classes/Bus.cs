using System.Collections.Generic;
using UnityEngine;

public class Bus
{
    public Vector3 CurrentOrPreviousStop;
    public Vector3 CurrentPosition;
    public Vector3Int TargetStop;
    // public GameObject TrainSpritePrefab;
    public GameObject FrontSprite;
    public GameObject LeftSprite;
    public GameObject RightSprite;
    public GameObject BackSprite;

    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;
    public List<int> NPCIdsOnBus = new List<int>();
    public bool isCurrentlyMoving = true;

    public int ReactivateCount = 0;
    public int ReactivateTime = 100;

    public bool IsCancelled = false;

    public int CurrentDirection;
    //0 is down
    //1 is left
    //2 is right
    //3 is up


    public Bus(Vector3Int StartTile, GameObject Sprite, GameObject Left, GameObject Right, GameObject Back)
    {
        CurrentDirection= 0;
        LeftSprite = Left;
        FrontSprite = Sprite;
        RightSprite = Right;
        BackSprite = Back;

        FrontSprite.GetComponent<SpriteRenderer>().enabled = true;
        LeftSprite.GetComponent<SpriteRenderer>().enabled = false;
        RightSprite.GetComponent<SpriteRenderer>().enabled = false;
        BackSprite.GetComponent<SpriteRenderer>().enabled= false; 

        CurrentPosition = FrontSprite.transform.position;
    }
    // return counter for reactivation count
    public int GetReactivateCount()
    {
        return ReactivateCount;
    }
    // reset counter for reactivation checking
    public void ResetReactivateCount()
    {
        ReactivateCount = 0;
    }
    // increment counter for checking if bus has stopped long enough to be reactivated
    public void IncrementReactivateCount()
    {
        ReactivateCount++;
    }
    // return true if counter for checking how long bus has been stopped is high enough
    public bool GetIfBusCanBeReactivated()
    {
        return ReactivateCount >= ReactivateTime;
    }
    // destory all sprites
    public void DestroySprite()
    {
        Object.Destroy(FrontSprite);
        Object.Destroy(LeftSprite);
        Object.Destroy(RightSprite);
        Object.Destroy(BackSprite);
    }
    // update sprite positon
    public void MoveSprite()
    {
        FrontSprite.transform.position = GetPosition() +new Vector3(0.5f, 0.5f, 0);
        LeftSprite.transform.position = GetPosition() +new Vector3(0.5f, 0.5f, 0);
        RightSprite.transform.position = GetPosition() +new Vector3(0.5f, 0.5f, 0);
        BackSprite.transform.position = GetPosition() +new  Vector3(0.5f,0.5f,0) ;
    }
    // update the most recent stop a bus has been too
    public void SetLastStop(Vector3Int Stop)
    {
        CurrentOrPreviousStop = Stop;
    }
    // return the most recent stop visited by the bus
    public Vector3 GetCurrentStopPos()
    {
        return CurrentOrPreviousStop;
    }
    // set list of NPC IDs on the bus 
    public void SetIDsOnBus(List<int> NPCIds)
    {
        NPCIdsOnBus= NPCIds;
    }
    // clear list of NPC Ids on the bus
    public void ResetIDsOnBus()
    {
        NPCIdsOnBus.Clear();
    }
    // return list of NPC IDs on the bus
    public List<int> GetNPCIDsOnBus()
    {
        return NPCIdsOnBus;
    }
    // based on position, return true if target is reached
    public bool GetIfTargetReached()
    {
        return Vector3.Distance(CurrentPosition, CurrentTarget) < 0.05f;
    }
    // return if the bus is moving
    public bool GetIfCurrentlyMoving()
    {
        return isCurrentlyMoving;
    }
    // set whether the bus is moving
    public void SetIsCurrentlyMoving(bool New)
    {
        isCurrentlyMoving = New;
    }
    // set directions for bus, up/down, left/right
    public void SetDirections(bool x, bool y)
    {
        XCurrentlyIncreasing = x; YCurrentlyIncreasing = y;
    }
    // set new movement target for bus
    public void SetNewTarget(Vector3 Position)
    {
        CurrentTarget = Position;
    }
    // move sprite position 
    public void UpdateSprite(Vector3 New,Vector3 Old)
    {
        int NewDirection;
        float XDiff=0,YDiff=0;
        if (New.x > Old.x)
        {
            XDiff = New.x - Old.x;
        }
        else if (New.x < Old.x) { 
            XDiff = Old.x - New.x;
        }

        if (New.y > Old.y)
        {
            YDiff = New.y - Old.y;
        }
        else if (New.y < Old.y)
        {
            YDiff = Old.y - New.y;
        }

        if (XDiff > YDiff)
        {
            if (XCurrentlyIncreasing)
            {
                //Going right
                NewDirection = 2;
            }
            else
            {
                //GoingLeft
                NewDirection = 1;
            }
        }
        else
        {
            if (YCurrentlyIncreasing)
            {
                //Going down
                NewDirection = 0;
            }
            else
            {
                //Going up
                NewDirection = 3;
            }
        }
        ChangeSpriteDirection(NewDirection);

    }
    // set sprite to be facing direction the bus is going
    public void ChangeSpriteDirection(int NewDir)
    {
        if (NewDir != CurrentDirection)
        {
            CurrentDirection = NewDir;
            if (CurrentDirection == 0)
            {
                //GoDown
                FrontSprite.GetComponent<Renderer>().enabled = false;
                LeftSprite.GetComponent<Renderer>().enabled = false;
                BackSprite.GetComponent<Renderer>().enabled = true;
                RightSprite.GetComponent<Renderer>().enabled = false;
            }
            else if (CurrentDirection == 1)
            {
                //Go left
                FrontSprite.GetComponent<Renderer>().enabled = false;
                LeftSprite.GetComponent<Renderer>().enabled = true ;
                BackSprite.GetComponent<Renderer>().enabled = false;
                RightSprite.GetComponent<Renderer>().enabled = false;
            }
            else if (CurrentDirection == 2)
            {
                //Go right
                FrontSprite.GetComponent<Renderer>().enabled = false;
                LeftSprite.GetComponent<Renderer>().enabled = false;
                BackSprite.GetComponent<Renderer>().enabled = false;
                RightSprite.GetComponent<Renderer>().enabled = true;
            }
            else if (CurrentDirection == 3)
            {
                //Go up
                FrontSprite.GetComponent<Renderer>().enabled = true;
                LeftSprite.GetComponent<Renderer>().enabled = false ;
                BackSprite.GetComponent<Renderer>().enabled = false;
                RightSprite.GetComponent<Renderer>().enabled = false;
            }
        }
    }
    // set position of bus to new position
    public void AdjustPosition(Vector3 position)
    {
        CurrentPosition = position;
    }
    // return current position of bus
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }

}
