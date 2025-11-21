using UnityEngine;

public class PlacedBuilding
{
    Building buildingType;
    int[] OriginPos;
    public PlacedBuilding(Building buildingType, int[] originPos)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
    }
}
