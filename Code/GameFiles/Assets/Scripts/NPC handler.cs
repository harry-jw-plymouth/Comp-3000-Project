using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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
        SetHomes();
    }
    int GetNumberOfNPCs()
    {
        return 10;   
    }
    public void SetHomes()
    {
        int TotalSet=0; int TotalNotSet = 0;
        Debug.Log("Setting homes");
        Debug.Log("Number of buildings in NPC handler:" + GridCreator.PlacedBuildings.Count);
        bool HomeFound=false;
        for (int i = 0; i < NumberOfNpcs; i++) {
            HomeFound = false;
            if (NPCList[i].GetIfHomeless())
            {
                Debug.Log("Npc:" + i);
                
                foreach (PlacedBuilding building in GridCreator.PlacedBuildings)
                {
                    Debug.Log("Building type: " + building.buildingType.GetType());
                    if (building.buildingType is Home home)
                    {
                        if (!home.GetIfFull())
                        {
                            if (home.AdjustResidents(1))
                            {
                                HomeFound = true;

                                NPCList[i].UpdateHomeStatus(false);
                                NPCList[i].SetHomePos(building.GetBuildingPos());
                                TotalSet++;
                            }
                            
                        }
                    }
                    if (HomeFound)
                    {
                        break;
                    }
                }
                if (!HomeFound)
                {
                    NPCList[i].UpdateHomeStatus(true);
                    NPCList[i].SetHomePos(new Vector3(-1, -1, -1));
                    TotalNotSet++;
                }
            }
            else
            {
                TotalSet++;
            }
        }
        Debug.Log("NPC homes set: " + TotalSet + "\n Total not set: " + TotalNotSet);
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
    public Vector3 GetWanderTarget()
    {

        Vector3 RoadTarget = GridCreator.GetRandomRoadCoorindates();
        if (RoadTarget.x == -1)
        {
            //No road
            Debug.Log("No road found");
            int RandomX = UnityEngine.Random.Range(0,GridCreator.WIDTH);
            int RandomY = UnityEngine.Random.Range(0, GridCreator.HEIGHT);
            Vector3 FinalPos= GridCreator.GameMap.CellToWorld(new Vector3Int(RandomX, RandomY, 0));
            FinalPos.x += 0.5f;FinalPos.y += 0.5f;
            return FinalPos;
        }
        else
        {
            RoadTarget.x += 0.5f;RoadTarget.y += 0.5f;
            return RoadTarget;
        }
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
            if (RandomValue < 5)
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
                    NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    NPCList[NPCIndex].SetCurrentAction(0);
                }

                
            }
            else
            {
                //Wander
                NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                NPCList[NPCIndex].SetCurrentAction(0);
                // NPCList[NPCIndex].SetCurrentAction(0);

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
