using UnityEngine;

public class Home : Building
{
    public int MaximumNumberOfReisdents;
    public Home(string name, string description, int[,] shape, int[] origin, bool Shop, int LB, int UB,int MaxResidents) 
        :base (name,description,shape,origin,false,LB,UB)
    {
        MaximumNumberOfReisdents = MaxResidents;
    }
    public int GetMaximumResidents()
    {
        return MaximumNumberOfReisdents;
    }
    
}
