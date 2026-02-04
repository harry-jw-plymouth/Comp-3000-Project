using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

public class Building
{
    public string Name;
    public string Description;
    public int[,] Shape;
    public int[] Origin;
    public int PowerUsage;
    public bool IsShop;
    public bool IsHome=false;
    public bool IsHospital = false;
    public int LowerTimeInBuilding,UpperTimeInBuilding;
    public int TypeIndex;
    public int EnviromentalEffect;
    public bool IsPowerPlant = false;

    public Building(string name, string description, int[,] shape, int[] origin, bool Shop ,int LB,int UB,bool hospital,int typeIndex,int Usage,int EF)
    {
        Name = name;
        Description = description;
        Shape = shape;
        // 2d array to show shape of building in grid
        // -1 is empty square
        // 0 is origin square(Where the building is placed in correlation to the location selected by the player)
        // 1 is square
        Origin = origin;
        IsShop= Shop;
        LowerTimeInBuilding = LB;
        UpperTimeInBuilding = UB;
        IsHospital= hospital;
        TypeIndex = typeIndex;
        PowerUsage = Usage;
        EnviromentalEffect = EF;
    }
    public int[,] GetShape()
    {
        return Shape;
    }
    public int GetTypeIndex()
    {
        return TypeIndex;
    }
    public bool GetIfIsHospital()
    {
        return IsHospital;
    }
    public bool GetIfBuildingIsAShop()
    {
        return IsShop;
    }
    public bool GetIfIsHome()
    {
        return IsHome; 
    }
    public bool GetIfIsPowerPlant()
    {
        return IsPowerPlant;
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
        return new Building(Name, Description, Shape, Origin,IsShop,LowerTimeInBuilding,UpperTimeInBuilding,IsHospital,TypeIndex,PowerUsage);
    }

}
