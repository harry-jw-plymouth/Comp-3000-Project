using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class NPChandler : MonoBehaviour
{
    List<Citzen> NPCList=new List<Citzen>();
    int NumberOfNpcs;
    public GameObject NPCPrefab;

    Vector3Int MapCenter = new Vector3Int(GridCreator.WIDTH / 2, GridCreator.HEIGHT / 2, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NumberOfNpcs = GetNumberOfNPCs();
        LoadNPCs();
    }
    int GetNumberOfNPCs()
    {
        return 5;   
    }
    void LoadNPCs()
    {
        Vector3 worldPosition = GridCreator.GameMap.GetCellCenterWorld(MapCenter);
        for (int i = 0; i < NumberOfNpcs; i++) {
            worldPosition.x++; ;
            Instantiate(NPCPrefab,worldPosition,Quaternion.identity );
            NPCList.Add(new Citzen(worldPosition));
            Debug.Log("placinng NPC");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNPCs();
    }
    bool CheckIfOnRoad(Vector3 Position)
    {
        Vector3Int GridPosition = GridCreator.GameMap.WorldToCell(Position);
        if (GridCreator.GameGrid[GridPosition.x, GridPosition.y].Contains != 0)
        {
            return false;
        }
        return true;
    }
    void SelectNewAction(int NPCIndex)
    {
        if (CheckIfOnRoad(NPCList[NPCIndex].GetPosition())){
            if (GridCreator.GetIfRoadExists())
            {
                //Go to nearest path
                Vector3 RoadPos= GridCreator.GetPosOfNearestRoad(NPCList[NPCIndex].GetPosition());
                NPCList[NPCIndex].SetMovementTarget(RoadPos);
            }
        }
    }
    void UpdateNPCs()
    {
        // check for updates to each NPC
        for (int i = 0; i < NumberOfNpcs; i++) {
            if (NPCList[i].GetCurrentAction() == -1)
            {
                SelectNewAction(i);
            }
        }

    }

}
