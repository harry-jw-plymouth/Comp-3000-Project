using UnityEngine;

public class PowerPlant : Building
{
    public int PowerGeneration;
 

    public PowerPlant(string name, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB, bool hospital, int typeIndex, int Usage,int EF, int PowerGen)
        : base(name, description, shape, origin, false, LB, UB, hospital, typeIndex,Usage,EF)
    {
        PowerGeneration =PowerGen;
        IsPowerPlant = true;
    }
    public int GetPowerGeneration()
    {
        return PowerGeneration;
    }
}
