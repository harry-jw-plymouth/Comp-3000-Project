using UnityEngine;

public class Home : Building
{
    public int MaximumNumberOfReisdents;
    public int CurrentResidents=0;

    public Home(string name, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB, int MaxResidents)
        : base(name, description, shape, origin, false, LB, UB)
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
        if (!((CurrentResidents + change) < 0 || (CurrentResidents + change) > MaximumNumberOfReisdents))
        {
            MaximumNumberOfReisdents += change;
            return true;
        }
        return false;
    }
}
