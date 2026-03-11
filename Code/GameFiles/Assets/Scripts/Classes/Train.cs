using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Train
{
    public Vector3 CurrentOrPreviousStation;
    public Vector3 CurrentPosition;
    public Vector3Int TargetStation;
   // public GameObject TrainSpritePrefab;
    public GameObject CreatedSprite;
    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;

    public List<int> NPCIdsOnTrain = new List<int>();

    public bool isCurrentlyMoving = true;
    public Train(Vector3Int StartTile,GameObject Sprite)
    {
        CreatedSprite = Sprite;
        CreatedSprite.GetComponent<SpriteRenderer>().enabled = true;
        CurrentPosition = CreatedSprite.transform.position+ new Vector3(0, 0.25f, 0);
      //  InstantiateSprite();
        //TrainSpritePrefab = Sprite;

    }
    //  public void InstantiateSprite(GameObject Prefab)
    //  {
    //    .Instantiate()
    //}
    public void SetLastStation(Vector3Int Station)
    {
        CurrentOrPreviousStation = Station;
    }
    public Vector3 GetCurrentStationPos()
    {
        return CurrentOrPreviousStation;
    }
    public void SetIDsOnTrain(List<int> NPCIds)
    {
        NPCIdsOnTrain= NPCIds;
        Debug.Log("Train picked up " + NPCIds.Count + " people");
    }
    public void ResetIDsOnTrain()
    {
        NPCIdsOnTrain.Clear();
    }
    public List<int> GetNPCIDsOnTrain()
    {
        return NPCIdsOnTrain;
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
        isCurrentlyMoving=New;
    }
    // :O
//    public bool GetIfTargetReached()
  //  {
    //    if (XCurrentlyIncreasing)
      //  {
        //    if (CurrentPosition.x >= CurrentTarget.x) {
          //      return false;
           // }
       // }
       // else
       // {
         //   if (CurrentPosition.x >= CurrentTarget.x)
           // {
             //   return false;
            //}
       // }

    //    if (YCurrentlyIncreasing)
      //  {
        //    if (CurrentPosition.y <= CurrentTarget.y)
          //  {
            //    return false;
            //}
     //   }//
     //   else
       // {
         //   if (CurrentPosition.y >= CurrentTarget.y)
           // {
             //   return false;
            //}
       // }

//        return true;

  //  }
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
