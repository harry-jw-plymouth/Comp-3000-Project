using UnityEngine;

public class PlacedBuilding
{
   public  Building buildingType;
    public int[] OriginPos;
    public GameObject Sprite;
    public PlacedBuilding(Building buildingType, int[] originPos, GameObject sprite)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
        Sprite = sprite;
    }
}
