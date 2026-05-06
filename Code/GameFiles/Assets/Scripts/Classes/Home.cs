using System.Data;
using UnityEngine;

public class Home : Building
{
    public int MaximumNumberOfReisdents;
    public int CurrentResidents=0;
    
    // constructor inherits from building
    public Home(string name, int Cost, int Tax, string description, int[,] shape, int[] origin, bool Shop, int LB,
        int UB,bool hospital,int typeIndex,int Usage,int EF, bool Entertainment, int EV,bool IsStation,bool isGreenery,
        int AirPoll, int WaterPoll, int AreaPoll, int WasteCre, int MaxResidents)
        : base(name, Cost, Tax, description, shape, origin, false, LB, UB,hospital,typeIndex,Usage,EF, Entertainment, EV,IsStation,isGreenery, AirPoll, WaterPoll, AreaPoll, WasteCre)
    {
        MaximumNumberOfReisdents = MaxResidents;
        IsHome = true;
    }
    // return true if building full
    public bool GetIfFull()
    {
        if (CurrentResidents >= MaximumNumberOfReisdents)
        {
            return true;
        }
        return false;
    }
    // return the maximum residents at any one time in the building
    public int GetMaximumResidents()
    {
        return MaximumNumberOfReisdents;
    }
    // increase/ decrease residents in the home and return whether there is space in the home 
    public bool AdjustResidents(int change)
    {
        if((CurrentResidents+change)>-1 && (CurrentResidents + change) <= MaximumNumberOfReisdents){ 
            CurrentResidents += change;
            return true;
        }
        return false;
    }
    // get instance of class to prevent errors when assigning to buildings
    public override Building GetInstance()
    {
        return new Home(Name,CostToBuild,TaxGeneration , Description, Shape, Origin, IsShop, LowerTimeInBuilding,
            UpperTimeInBuilding,IsHospital,TypeIndex,PowerUsage,EnviromentalEffect,IsEntertaiment,EntertainmentValue,
            IsTrainStation,IsGreenery, AirPollution, WaterPollution, AreaPollution, WasteCreated, MaximumNumberOfReisdents);
    }
}
