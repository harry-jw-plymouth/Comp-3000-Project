using JetBrains.Annotations;
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
    public bool IsGreenery = false;
    public int AirPollution;
    public int WaterPollution;
    public int AreaPollution;
    public int WasteCreated;
    public bool IsWastageCenter = false;

    public Building(string name,int Cost,int Tax, string description, int[,] shape, int[] origin, bool Shop ,
        int LB,int UB,bool hospital,int typeIndex,int Usage,int EF,bool Entertainment, int EV,bool 
        isStation,bool isGreenery, int AirPoll,int WaterPoll,int AreaPoll,int WasteCre)
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
        IsTrainStation = isStation;
        IsGreenery = isGreenery;
        AirPollution= AirPoll;
        WaterPollution= WaterPoll;
        AreaPollution= AreaPoll;
        WasteCreated= WasteCre;
}
    //return waste amount for building
    public int GetBuildingWaste()
    {
        return WasteCreated;
    }
    //return true if building is a wastage center
    public bool GetIfIsWastageCenter()
    {
        return IsWastageCenter;
    }
    //set whether the building is a train station or not 
    public void SetIsTrainStation(bool New)
    {
        IsTrainStation= New;
    }
    //set whether the building is a wastage center or not 
    public void SetIsWatageCenter(bool New)
    {
        IsWastageCenter = New;
    }
    //return true if building is a train station
    public bool GetIfIsTrainStation()
    {
        return IsTrainStation;
    }
    // get value denoting how much the building would effect the environment
    public int GetEnviromentalValue()
    {
        return EnviromentalEffect;
    }
    // return cost to place building
    public int GetCostToBuild()
    {
        return CostToBuild;
    }
    // return how much the money generates or costs to run
    public int GetTaxGeneration()
    {
        return TaxGeneration;
    }
    // return how much entertainment the building provides
    public int GetEntertainmentValue()
    {
        return EntertainmentValue;
    }
    //return true if building is entertainment
    public bool GetIfEntertainment()
    {
        return IsEntertaiment;
    }
    //return true if building is a power plant
    public bool GetIfPowerPlant()
    {
        return IsPowerPlant;
    }
    // return the shape of the building
    public int[,] GetShape()
    {
        return Shape;
    }
    // return the index of the building type in the building list in building list manager
    public int GetTypeIndex()
    {
        return TypeIndex;
    }
    //return true if the building is a hopsital
    public bool GetIfIsHospital()
    {
        return IsHospital;
    }
    //return true if the building is greenery
    public bool GetIfIsGreenery()
    {
        return IsGreenery;
    }
    //return true if building is a shop
    public bool GetIfBuildingIsAShop()
    {
        return IsShop;
    }
    // return true if building is a home
    public bool GetIfIsHome()
    {
        return IsHome; 
    }
    //return true if building is a power plant
    public bool GetIfIsPowerPlant()
    {
        return IsPowerPlant;
    }
    // return lower amount for time an NPC could spend in the building
    public int GetLowerBound()
    {
        return LowerTimeInBuilding;
    }
    // return upper amount for time an NPC could spend in the building
    public int GetUpperBound()
    {
        return UpperTimeInBuilding;
    }
    // return water pollution
    public int GetWaterPollution()
    {
        return WaterPollution;
    }
    // return instance of building to not cause issues with the same instance in multiple buildings
    public virtual Building GetInstance()
    {
        Building building = new Building(Name, CostToBuild, TaxGeneration, Description, Shape, Origin, IsShop, LowerTimeInBuilding, UpperTimeInBuilding, IsHospital, TypeIndex, PowerUsage, EnviromentalEffect, IsEntertaiment, EntertainmentValue, IsTrainStation, IsGreenery, AirPollution, WaterPollution, AreaPollution, WasteCreated);
        building.SetIsTrainStation(IsTrainStation);
        building.SetIsWatageCenter(IsWastageCenter);
        return building;

    }

}
