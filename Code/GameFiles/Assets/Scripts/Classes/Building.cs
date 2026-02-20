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
    public bool IsEntertaiment = false;
    public int EntertainmentValue;
    public int CostToBuild;
    public int TaxGeneration;
    public bool IsTrainStation = false;

    public Building(string name,int Cost,int Tax, string description, int[,] shape, int[] origin, bool Shop ,int LB,int UB,bool hospital,int typeIndex,int Usage,int EF,bool Entertainment, int EV)
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
        IsEntertaiment = Entertainment;
        EntertainmentValue = EV;
        CostToBuild = Cost;
        TaxGeneration= Tax;
    }
    public void SetIsTrainStation(bool New)
    {
        IsTrainStation= New;
    }
    public bool GetIfIsTrainStation()
    {
        return IsTrainStation;
    }
    public int GetEnviromentalValue()
    {
        return EnviromentalEffect;
    }
    public int GetCostToBuild()
    {
        return CostToBuild;
    }
    public int GetTaxGeneration()
    {
        return TaxGeneration;
    }
    public int GetEntertainmentValue()
    {
        return EntertainmentValue;
    }
    public bool GetIfEntertainment()
    {
        return IsEntertaiment;
    }
    public bool GetIfPowerPlant()
    {
        return IsPowerPlant;
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
        return new Building(Name,CostToBuild,TaxGeneration, Description, Shape, Origin,IsShop,LowerTimeInBuilding,UpperTimeInBuilding,IsHospital,TypeIndex,PowerUsage,EnviromentalEffect,IsEntertaiment,EntertainmentValue);
    }

}
