using NUnit.Framework;
using UnityEngine;

public class Building
{
    public string Name;
    public string Description;
    public int[,] Shape;
    public int[] Origin;
    public bool IsShop;
    public bool IsHome=false;
    public int LowerTimeInBuilding,UpperTimeInBuilding; 


    public Building(string name, string description, int[,] shape, int[] origin, bool Shop ,int LB,int UB)
    {
        Name = name;
        Description = description;
        Shape = shape;
        Origin = origin;
        IsShop= Shop;
        LowerTimeInBuilding = LB;
        UpperTimeInBuilding = UB;
        // 2d array to show shape of building in grid
        // -1 is empty square
        // 0 is origin square(Where the building is placed in correlation to the location selected by the player)
        // 1 is square
    }
    public bool GetIfBuildingIsAShop()
    {
        return IsShop;
    }
    public bool GetIfIsHome()
    {
        return IsHome; 
    }
    public int GetLowerBound()
    {
        return LowerTimeInBuilding;
    }
    public int GetUpperBound()
    {
        return UpperTimeInBuilding;
    }
    public virtual Building GetInstance()
    {
        return new Building(Name, Description, Shape, Origin,IsShop,LowerTimeInBuilding,UpperTimeInBuilding);
    }

}
