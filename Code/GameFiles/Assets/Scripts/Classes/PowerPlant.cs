using UnityEngine;

public class PowerPlant : Building
{
    public int PowerGeneration;
    public int Range;
 

    public PowerPlant(string name,int Cost,int Tax, string description, int[,] shape, int[] origin, bool Shop, 
        int LB, int UB, bool hospital, int typeIndex, int Usage,int EF,bool Entertainment,int EV,
        bool isStation,bool isGreenery, int AirPoll, int WaterPoll, int AreaPoll, int WasteCre, int PowerGen,int range)
        : base(name,Cost,Tax, description, shape, origin, false, LB, UB, hospital, typeIndex,Usage,EF,Entertainment,EV,isStation,isGreenery, AirPoll, WaterPoll,  AreaPoll, WasteCre)
    {
        PowerGeneration =PowerGen;
        IsPowerPlant = true;
        Range = range;
    }
    public int GetPowerGeneration()
    {
        return PowerGeneration;
    }
    public int GetRange()
    {
        return Range;
    }
    public override Building GetInstance()
    {
        return new PowerPlant(Name,CostToBuild,TaxGeneration, Description, Shape, Origin, IsShop, LowerTimeInBuilding, UpperTimeInBuilding,
            IsHospital, TypeIndex, PowerUsage,EnviromentalEffect,IsEntertaiment,EntertainmentValue,IsTrainStation,IsGreenery,AirPollution,
            WaterPollution, AreaPollution, WasteCreated, PowerGeneration,Range);
    }
}
