using UnityEngine;

public class Home : Building
{
    public int MaximumNumberOfReisdents;
    public int CurrentResidents=0;

    public Home(string name, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB,bool hospital,int typeIndex, int MaxResidents)
        : base(name, description, shape, origin, false, LB, UB,hospital,typeIndex)
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
        return new Home(Name, Description, Shape, Origin, IsShop, LowerTimeInBuilding, UpperTimeInBuilding,IsHospital,TypeIndex, MaximumNumberOfReisdents);
    }
}
