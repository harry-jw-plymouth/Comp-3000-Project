using NUnit.Framework;
using UnityEngine;

public class Building
{
    public string Name;
    public string Description;
    public int[,] Shape;
    public int[] Origin;

    public Building(string name, string description, int[,] shape, int[] origin)
    {
        Name = name;
        Description = description;
        Shape = shape;
        Origin = origin;
        // 2d array to show shape of building in grid
        // -1 is empty square
        // 0 is origin square(Where the building is placed in correlation to the location selected by the player)
        // 1 is square
    }

}
