using System.Collections.Generic;
using UnityEngine;

public class Bus
{
    public Vector3 CurrentOrPreviousStop;
    public Vector3 CurrentPosition;
    public Vector3Int TargetStop;
    // public GameObject TrainSpritePrefab;
    public GameObject CreatedSprite;
    public GameObject SideSprite;
    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;
    public List<int> NPCIdsOnBus = new List<int>();
    public bool isCurrentlyMoving = true;

    public int CurrentDirection;
    //0 is down
    //1 is left
    //2 is right
    //3 is up

    
    public Bus(Vector3Int StartTile, GameObject Sprite,GameObject Side)
    {
        CurrentDirection= 0;
        SideSprite = Side;
        CreatedSprite = Sprite;
        CreatedSprite.GetComponent<SpriteRenderer>().enabled = true;
        SideSprite.GetComponent<SpriteRenderer>().enabled = false;

        CurrentPosition = CreatedSprite.transform.position;
    }
    public void MoveSprite()
    {
        CreatedSprite.transform.position = GetPosition();
        SideSprite.transform.position = GetPosition();

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
    public void ChangeSpriteDirection(int NewDir)
    {
        if (NewDir != CurrentDirection)
        {
            CurrentDirection = NewDir;
            if (CurrentDirection == 0)
            {
                //GoDown
                CreatedSprite.GetComponent<Renderer>().enabled = true;
                SideSprite.GetComponent<Renderer>().enabled = false;
            }
            else if (CurrentDirection == 1)
            {
                //Go left
                CreatedSprite.GetComponent<Renderer>().enabled = false;
                SideSprite.GetComponent<Renderer>().enabled = true ;
            }
            else if (CurrentDirection == 2)
            {
                //Go right
                CreatedSprite.GetComponent<Renderer>().enabled = false;
                SideSprite.GetComponent<Renderer>().enabled = true;
            }
            else if (CurrentDirection == 3)
            {
                //Go up
                CreatedSprite.GetComponent<Renderer>().enabled = true;
                SideSprite.GetComponent<Renderer>().enabled = false ;
            }
        }
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
