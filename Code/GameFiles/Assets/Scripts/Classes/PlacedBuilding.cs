using UnityEngine;

public class PlacedBuilding
{
   public  Building buildingType;
    public int[] OriginPos;
    public GameObject Sprite;
    Vector3 BuildingPos;
    public PlacedBuilding(Building buildingType, int[] originPos, GameObject sprite)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
        Sprite = sprite;
        //BuildingPos = new Vector3( originPos[0],originPos[1],0);
    }
    public void SetBuildingPos(Vector3 NewPos)
    {
        BuildingPos = NewPos;
    }
    public Vector3 GetBuildingPos()
    {
        return BuildingPos;
    }
    public Building GetType()
    {
        return buildingType;
    }
    

    public bool GetIfIsShop()
    {
        return buildingType.GetIfBuildingIsAShop();
    }
}
