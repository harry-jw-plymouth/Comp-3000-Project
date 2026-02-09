using UnityEngine;

public class Home : Building
{
    public int MaximumNumberOfReisdents;
    public int CurrentResidents=0;

    public Home(string name, int Cost, int Tax, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB,bool hospital,int typeIndex,int Usage,int EF, bool Entertainment, int EV, int MaxResidents)
        : base(name, Cost, Tax, description, shape, origin, false, LB, UB,hospital,typeIndex,Usage,EF, Entertainment, EV)
    {
        MaximumNumberOfReisdents = MaxResidents;
        IsHome = true;
    }
    public bool GetIfFull()
    {
        if (CurrentResidents >= MaximumNumberOfReisdents)
        {
            return true;
        }
        return false;
    }
    public int GetMaximumResidents()
    {
        return MaximumNumberOfReisdents;
    }
    public bool AdjustResidents(int change)
    {
        // if (!((CurrentResidents + change) < 0 || (CurrentResidents + change) > MaximumNumberOfReisdents))
        //{
        Debug.Log("Adjust residents function");
        if((CurrentResidents+change)>-1 && (CurrentResidents + change) <= MaximumNumberOfReisdents){ 
            Debug.Log("Residetns after adjustemnt"+CurrentResidents);
            CurrentResidents += change;
            return true;
        }
        return false;
    }
    public override Building GetInstance()
    {
        return new Home(Name,CostToBuild,TaxGeneration , Description, Shape, Origin, IsShop, LowerTimeInBuilding, UpperTimeInBuilding,IsHospital,TypeIndex,PowerUsage,EnviromentalEffect,IsEntertaiment,EntertainmentValue, MaximumNumberOfReisdents);
    }
}
