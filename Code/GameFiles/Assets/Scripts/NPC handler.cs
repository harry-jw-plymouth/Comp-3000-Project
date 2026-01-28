using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class NPChandler : MonoBehaviour
{
    int MovementCounter = 0; int frameToMoveOn = 10;
    int BuildingFrame = 20 ;
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
        return 1;   
    }
    void LoadNPCs()
    {
        Vector3 worldPosition = GridCreator.GameMap.GetCellCenterWorld(MapCenter);
        for (int i = 0; i < NumberOfNpcs; i++) {
            worldPosition.x++; 
            GameObject Current= Instantiate(NPCPrefab,worldPosition,Quaternion.identity );
            NPCList.Add(new Citzen(worldPosition,Current));
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
        if (GridCreator.GameGrid[GridPosition.x, GridPosition.y].Contains != 1)
        {
            return false;
        }
        return true;
    }
    void SelectNewAction(int NPCIndex)
    {
        int RandomValue = UnityEngine.Random.Range(0, 100);
        if (!CheckIfOnRoad(NPCList[NPCIndex].GetPosition())){
            //Debug.Log("Not on road");
            if (GridCreator.GetIfRoadExists())
            {
                Debug.Log("RoadFound");
                NPCList[NPCIndex].SetCurrentAction(0) ;
                //Go to nearest path
                Vector3 RoadPos= GridCreator.GetPosOfNearestRoad(NPCList[NPCIndex].GetPosition());
                RoadPos.y += 0.5f;
                RoadPos.x += 0.5f;
                NPCList[NPCIndex].SetMovementTarget(RoadPos);
            }
        }
        else
        {
           // Debug.Log("Already on road");
            if (RandomValue < 10)
            {
                Vector3 ShopPos = GridCreator.GetPosOfNearestShop(NPCList[NPCIndex].GetPosition());
                if (ShopPos.x != -1)
                {
                    //Go to nearest shop
                    NPCList[NPCIndex].SetCurrentAction(0);

                    ShopPos.x += 0.5f; ShopPos.y += 0.5f;
                    NPCList[NPCIndex].SetMovementTarget(ShopPos);
                    NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                    NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                    
                }
                else
                {
                    //no shop found
                }

                
            }
            else
            {
                //Wander
            }
        }
    }
    void UpdateNPCs()
    {
        MovementCounter++;
        // check for updates to each NPC
        for (int i = 0; i < NumberOfNpcs; i++) {
            //New action
            if (NPCList[i].GetCurrentAction() == -1)
            {
               // Debug.Log("Selecting new action");
                SelectNewAction(i);
            }
            //NPc moving
            else if (NPCList[i].GetCurrentAction() == 0 )
            {
                if (NPCList[i].GetMoveCounter() == frameToMoveOn)
                {
                    NPCList[i].ResetCounter();
                    Debug.Log("Moving towards target");
                    NPCList[i].MovetowardsTarget();
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }          
            }
            else if (NPCList[i].GetCurrentAction() == 1)
            {
                if (NPCList[i].GetMoveCounter() == BuildingFrame)
                {
                    NPCList[i].ResetCounter();
                    NPCList[i].SpendTImeInBuilding();
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            
        }

    }

}
