using UnityEngine;

public class Citzen
{
    Vector3 Position;
    public bool UpdateNeeded;
    public Citzen(Vector3 Pos)
    {
        Position= Pos;
        UpdateNeeded = true;
    }
    
}
