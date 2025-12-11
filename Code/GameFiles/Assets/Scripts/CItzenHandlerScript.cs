using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CItzenHandlerScript : MonoBehaviour
{
    int NumberOfCitzens;
    List<Citzen> Citzens = new List<Citzen>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateCitzens();

    }
    void CreateCitzens()
    {
        if (true)//check db to see if citzens already created
        {
            NumberOfCitzens = 5;
        }
        else
        {
            //Get from db
        }
        System.Random rnd = new System.Random();

        for (int i = 0; i < NumberOfCitzens; i++) {
            bool IsValidSquare = false;

            int x = rnd.Next(GridCreator.WIDTH);
            int y = rnd.Next(GridCreator.HEIGHT);
            while (GridCreator.GameGrid[x, y].Contains !=0)
            {
                x = rnd.Next(GridCreator.WIDTH);
                y = rnd.Next(GridCreator.HEIGHT);
            }
            Citzens.Add(new Citzen(GridCreator.GameMap.CellToWorld(new Vector3Int(x, y, 0))));

        }


    }
    // Update is called once per frame
    void Update()
    {
        DrawAllCitzens();
    }
    void DrawAllCitzens()
    {
        for(int i = 0;i < Citzens.Count; i++)
        {

        }
    }
}
