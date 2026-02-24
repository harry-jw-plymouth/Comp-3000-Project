using UnityEngine;

public class Train
{
    public Vector3 CurrentPosition;
    public Vector3Int TargetStation;
   // public GameObject TrainSpritePrefab;
    public GameObject CreatedSprite;
    public bool CurrentlyAscendingRoute = true;
    public int CurrentlyTargetting;
    public Vector3 CurrentTarget;
    public bool XCurrentlyIncreasing, YCurrentlyIncreasing;
    public bool XSame, YSame;
    public Train(Vector3Int StartTile,GameObject Sprite)
    {
        CreatedSprite = Sprite;
        CreatedSprite.GetComponent<SpriteRenderer>().enabled = true;
        CurrentPosition = CreatedSprite.transform.position;
      //  InstantiateSprite();
        //TrainSpritePrefab = Sprite;

    }
    //  public void InstantiateSprite(GameObject Prefab)
    //  {
    //    .Instantiate()
    //}
    public bool GetIfTargetReached()
    {
        return Vector3.Distance(CurrentPosition, CurrentTarget) < 0.05f;
    }
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
        CurrentPosition += position;
    }
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }
}
