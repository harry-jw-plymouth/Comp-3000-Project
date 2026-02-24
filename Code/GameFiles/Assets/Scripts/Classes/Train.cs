using UnityEngine;

public class Train
{
    public Vector3 CurrentPosition;
    public Vector3Int TargetStation;
    public GameObject TrainSprite;
    public Train(Vector3Int StartTile, GameObject Sprite)
    {
        CurrentPosition = StartTile; 
        TrainSprite = Sprite;
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
