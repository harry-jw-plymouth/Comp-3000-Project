using UnityEngine;

public class Train
{
    public Vector3 CurrentPosition;
    public Vector3Int TargetStation;
   // public GameObject TrainSpritePrefab;
    public GameObject CreatedSprite;
    public Train(Vector3Int StartTile,GameObject Sprite)
    {
        CreatedSprite = Sprite;
        CurrentPosition = StartTile;
      //  InstantiateSprite();
        //TrainSpritePrefab = Sprite;

    }
  //  public void InstantiateSprite(GameObject Prefab)
  //  {
    //    .Instantiate()
    //}

    public void AdjustPosition(Vector3 position)
    {
        CurrentPosition += position;
    }
    public Vector3 GetPosition()
    {
        return CurrentPosition;
    }
}
