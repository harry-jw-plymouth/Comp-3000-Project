using UnityEngine;

public class PowerPlant : Building
{
    public int PowerGeneration;
 

    public PowerPlant(string name, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB, bool hospital, int typeIndex, int Usage,int EF,bool Entertainment,int EV , int PowerGen)
        : base(name, description, shape, origin, false, LB, UB, hospital, typeIndex,Usage,EF,Entertainment,EV)
    {
        PowerGeneration =PowerGen;
        IsPowerPlant = true;
    }
    public int GetPowerGeneration()
    {
        return PowerGeneration;
    }
    public override Building GetInstance()
    {
        return new PowerPlant(Name, Description, Shape, Origin, IsShop, LowerTimeInBuilding, UpperTimeInBuilding, IsHospital, TypeIndex, PowerUsage,EnviromentalEffect,IsEntertaiment,EntertainmentValue,PowerGeneration);
    }
}
