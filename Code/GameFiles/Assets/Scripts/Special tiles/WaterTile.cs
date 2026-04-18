using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/WaterTile")] 
public class WaterTile:RuleTile
{
    public Sprite[] Frames;
    public float AnimationSpeed = 1.5f;
    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        if (Frames != null && Frames.Length > 0)
        {
            tileAnimationData.animatedSprites = Frames;
            tileAnimationData.animationSpeed = AnimationSpeed;
            tileAnimationData.animationStartTime = Random.value;

            return true;
        }

        return false;
    }



}
