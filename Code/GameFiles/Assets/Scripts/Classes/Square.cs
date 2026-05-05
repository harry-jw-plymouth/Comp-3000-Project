using UnityEngine;
public class Square
{
    public int Contains;
    // 0 is grass, walkable 
    // 1 is road ,walkable 
    // 2 is building , not walkable
    // 3 is water, not walkable
    // 4 is train track, not walkable
    // 5 is bus stop tile, walkable
    // 6 is Greenery, walkable

    public Square(int contains)
    {
        Contains = contains;
    }
    
}
