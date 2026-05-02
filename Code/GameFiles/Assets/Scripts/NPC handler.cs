using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class NPChandler : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    [SerializeField] GameStatusScript GameFeaturesHandler;

    int MovementCounter = 0; int frameToMoveOn = 10;
    int AmountCheckCounter = 0;int CheckFrame = 1000;
    public int BuildingFrame = 20 ;
    int CurrentID = 0;
   [SerializeField]  List<Citzen> NPCList=new List<Citzen>();
    [SerializeField] int NumberOfNpcs;

    public List<GameObject> NPCSprites = new List<GameObject>();
    public GameObject NPC1Prefab;
    public GameObject NPC2Prefab;
    public GameObject NPC3Prefab;
    public GameObject NPC4Prefab;
    public GameObject NPC5Prefab;
    public GameObject NPC6Prefab;
    public GameObject NPC7Prefab;
    public GameObject NPC8Prefab;
    public GameObject NPC9Prefab;
    public GameObject NPC10Prefab;


    public TextMeshProUGUI PopulationCountDisplay;
    GridCreator GridHandler;
    

    Vector3Int MapCenter = new Vector3Int(GridCreator.WIDTH / 2, GridCreator.HEIGHT / 2, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        NPCSprites.Add(NPC1Prefab);
        NPCSprites.Add(NPC2Prefab);
        NPCSprites.Add(NPC3Prefab);
        NPCSprites.Add(NPC4Prefab);
        NPCSprites.Add(NPC5Prefab);
        NPCSprites.Add(NPC6Prefab);
        NPCSprites.Add(NPC7Prefab);
        NPCSprites.Add(NPC8Prefab);
        NPCSprites.Add(NPC9Prefab);
        NPCSprites.Add(NPC10Prefab);

        // NumberOfNpcs = GetNumberOfNPCs();
        NumberOfNpcs = 1;
        LoadNPCs();
        SetHomes();
        UpdatePopulationDisplay();

    }
    void UpdatePopulationDisplay()
    {
        PopulationCountDisplay.text = NumberOfNpcs.ToString();
    }
    public  void RemoveAllNPCsFromBuilding(List<int>Indexes)
    {
        for (int i = 0; i < Indexes.Count; i++) {
            NPCList[Indexes[i]].ForceLeaveBuidlingOnBuildingRemoval();
        }
    }
    public void UpdateHomesForNPCsAfterBuildingRemoval(List<int>Indexes)
    {
        for (int i = 0; i < Indexes.Count; i++) {
            NPCList[Indexes[i]].SetHomeIndex(-1);
            NPCList[Indexes[i]].UpdateHomeStatus(true);
            NPCList[Indexes[i]].RemoveHomeData();
        }
        SetHomes();

    }
    int GetNumberOfNPCs()
    {
        //New save
        // if new save ID create new npcs
        if (MainMenu.GetIfNewFileCreated() )
        {
            return 10;
        }
        //else get NPC amounf from save file
        SaveFileModel Save= DBManager.GetSaveFiles()[MainMenu.GetCurrentSaveID()];
      //  Debug.Log("Number of npcs from save:" + Save.NumberOfNPCs);
         return Save.NumberOfNPCs;   
    }
    public int GetCurrentNumberOfNPCs()
    {
        return NPCList.Count;
    }
    public  float GetHomeLessPercentage()
    {
        int total = 0;
        for(int i = 0; i < NPCList.Count; i++)
        {
            if (NPCList[i].GetIfHomeless())
            {
                total++;
            }
        }
     //   Debug.Log("Numebr homeless:" + total);
        float percent = (float)total / NPCList.Count * 100f;
       // Debug.Log("homesless %:" + percent);
        return percent;
    }
    public void SetHomes()
    {
        int TotalSet=0; int TotalNotSet = 0;
        int HomeIndex = 0;
   //     Debug.Log("Setting homes"
     //   Debug.Log("Number of buildings in NPC handler:" + GridCreator.PlacedBuildings.Count);
        bool HomeFound=false;
        for (int i = 0; i < NPCList.Count; i++) {
            HomeFound = false;
            if (NPCList[i].GetIfHomeless())
            {
                //         Debug.Log("Npc:" + i);
                HomeIndex = 0;
                foreach (PlacedBuilding building in GridCreator.PlacedBuildings)
                {
                    HomeIndex++;
         //           Debug.Log("Building type: " + building.buildingType.GetType());
                    if (building.buildingType is Home home)
                    {
                        if (!home.GetIfFull())
                        {
                            if (home.AdjustResidents(1))
                            {
                                building.AddInhabitantIndex(i);
                                HomeFound = true;

                                NPCList[i].SetHome(home);
                                NPCList[i].SetHomeIndex(HomeIndex--);
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
     //   Debug.Log("NPC homes set: " + TotalSet + "\n Total not set: " + TotalNotSet);
    }
    public int GetRandomNPCSpriteIndex()
    {
        return Random.Range(0,NPCSprites.Count);
    }
    void LoadNPCs()
    {
        Vector3 worldPosition = GridCreator.GameMap.GetCellCenterWorld(MapCenter);
        for (int i = 0; i < NumberOfNpcs; i++) {
            worldPosition.x++; 
            GameObject Current= Instantiate(NPCSprites[GetRandomNPCSpriteIndex()],worldPosition,Quaternion.identity );
            NPCList.Add(new Citzen(worldPosition,CurrentID++,Current));
          //  Debug.Log("placinng NPC");
        }
    }
    void CheckForNPCsToUpdateAfterTrainRouteRemoval()
    {
        for (int i = 0; i < NumberOfNpcs; i++)
        {
            if (NPCList[i].ReadyToUpdateAfterTravel)
            {
                Vector3 TargetPos = (NPCList[i].GetRoutePositions()[NPCList[i].GetRoutePositions().Count - 1]);
                NPCList[i].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                //NPCList[i].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                NPCList[i].ReadyToUpdateAfterTravel = false;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateNPCs();
        AmountCheckCounter++;
        if (AmountCheckCounter >= CheckFrame)
        {
            AmountCheckCounter = 0;
           // CheckForNewNPCs();
            CheckForLeavingNPCs();
            UpdatePopulationDisplay();
            CheckForNPCsToUpdateAfterTrainRouteRemoval();
       
        }
    //    Debug.Log("Number of NPCS:" + NumberOfNpcs);

    }
    void CheckForLeavingNPCs()        
    {
        if (NPCList.Count == 0)
        {
            return;
        }
        int RandomNpcIndex = Random.Range(0, NPCList.Count);
        int Happiness = NPCList[RandomNpcIndex].CalculateHappiness();
        


        int RandomLeaveChance = Random.Range(0, 100);
        if (RandomLeaveChance > Happiness*1.8f  && NumberOfNpcs >1) {
            if (NPCList[RandomNpcIndex].GetIfInBuilding())
            {
                NPCList[RandomNpcIndex].ForceLeaveBuidlingOnBuildingRemoval();
                
            }
            NPCList[RandomNpcIndex].SetCurrentAction(5);
        }
    }
    public List<int> GetNPCsIdWaitingForTrain(PlacedBuilding Start,PlacedBuilding End)
    {
        List<int> Ids = new List<int>();
        for(int i = 0; i < NPCList.Count; i++)
        {
            if (NPCList[i].CurrentStation == null || NPCList[i].TargetStation == null)
            {

            }
            else
            {
                if (NPCList[i].CurrentStation.GetBuildingPosAsInt() == Start.GetBuildingPosAsInt() &&
                NPCList[i].TargetStation.GetBuildingPosAsInt() == End.GetBuildingPosAsInt())
                {
                    Ids.Add(NPCList[i].GetCitzenID());
                  //  Debug.Log("NPC getting on train");
                }
            }          
        }
        return Ids;
    }
    public List<int> GetNPCsIdWaitingForBus(Vector3Int Start, Vector3Int End)
    {
        List<int> Ids = new List<int>();
        for (int i = 0; i < NPCList.Count; i++)
        {
            if (NPCList[i].GetCurrentBusStop().x==-1 || NPCList[i].GetTargetBusStop().x==-1)
            {

            }
            else
            {
                if (NPCList[i].GetCurrentBusStop() == Start &&
                NPCList[i].GetTargetBusStop() == End)
                {
                    Ids.Add(NPCList[i].GetCitzenID());
                    Debug.Log("NPC getting on bus");
                }
            }
        }
        return Ids;
    }
    void CheckForNewNPCs()
    {
        float value = Random.Range(30, 400);
        if (value < GameStatusScript.GetRating())
        {
            Vector3 worldPosition = new Vector3(Random.Range(0,GridCreator.WIDTH),Random.Range(0,GridCreator.HEIGHT),0);
            NumberOfNpcs++;
            GameObject Current = Instantiate(NPCSprites[GetRandomNPCSpriteIndex()], worldPosition, Quaternion.identity);
            NPCList.Add(new Citzen(worldPosition,CurrentID++, Current));
        }
    }
    bool CheckIfOnRoad(Vector3 Position)
    {
        Vector3Int GridPosition = GridCreator.GameMap.WorldToCell(Position);
        if (GridCreator.GameGrid[GridPosition.x, GridPosition.y].Contains != 1&& GridCreator.GameGrid[GridPosition.x, GridPosition.y].Contains != 5)
        {
            return false;
        }
        return true;
    }
    public void UpdateNPCsAfterTrainJourney(List<int> NPCIDs)
    {
        for (int i = 0; i < NPCList.Count; i++)
        {
            if (NPCIDs.Contains(NPCList[i].GetCitzenID())){
                //npc on train
                NPCList[i].GetOffTrain();
            }
        }
    }
    public void UpdateNPCsAfterBusJourney(List<int> NPCIDs)
    {
        for (int i = 0; i < NPCList.Count; i++)
        {
            if (NPCIDs.Contains(NPCList[i].GetCitzenID()))
            {
                //npc on train
                NPCList[i].GetOffBus();
            }
        }
    }

    public Vector3 GetWanderTarget()
    {

        Vector3 RoadTarget = GridCreator.GetRandomRoadCoorindates();
        if (RoadTarget.x == -1)
        {
            //No road
          //  Debug.Log("No road found");
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
        Debug.Log("Start of select action");
        int ShopChance = 5;
        int HomeChance = 15 + ((NPCList[NPCIndex].GetTiredNess()/50));
        int WanderChance = 80;
        int EntertainmentChance = 5 + ((NPCList[NPCIndex].GetBoredom() / 50));
        int HospitalChance = 0 + (NPCList[NPCIndex].GetSickness() / 100);
        int RandomValue = UnityEngine.Random.Range(0, ShopChance+HomeChance+WanderChance);
        Vector3Int TargetTile=new Vector3Int(-1,-1,-1);

   //     if (NPCList[NPCIndex].GetCurrentAction() == 7)
    //    {
     //       return; // already waiting for bus
     //   }
            
                    //   Debug.Log("TiredNess:" + NPCList[NPCIndex].GetTiredNess());
        if (!CheckIfOnRoad(NPCList[NPCIndex].GetPosition())){
            Debug.Log("Not on road,checking for road");
            if (GridCreator.GetIfRoadExists())
            {
                Debug.Log("RoadFound");
              //  NPCList[NPCIndex].SetCurrentAction(0) ;
                //Go to nearest path
                Vector3 RoadPos= GridCreator.GetPosOfNearestRoad(NPCList[NPCIndex].GetPosition());
                RoadPos.y += 0.5f;
                RoadPos.x += 0.5f;
              //  NPCList[NPCIndex].SetMovementTarget(RoadPos);
                if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(RoadPos), GridCreator.GameGrid, GridCreator.GameMap,gridCreator))
                {
                    Debug.Log("Set action to 0, moving onto nearest road");
                    NPCList[NPCIndex].SetCurrentAction(0);
                }
                else
                {
                    Debug.Log("NPC could not move to nearest road");
                }

                //     if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(RoadPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                //    {
                //      Debug.Log("Set action to 0");
                //     NPCList[NPCIndex].SetCurrentAction(0);
                //     NPCList[NPCIndex].SetMovementTarget(RoadPos);
                ////}

            }
            else
            {
                Debug.Log("No road found");
            }
        }
        else
        {
            Debug.Log("NPC Already on road");
            if (RandomValue < ShopChance)
            {
                Debug.Log("NPC wants to go to a shop");
                // go to the shop
                Vector3 ShopPos = GridCreator.GetPosOfNearestShop(NPCList[NPCIndex].GetPosition());
                if (ShopPos.x != -1)
                {
                    //Go to nearest shop
                    Debug.Log("Shop found");

                    ShopPos.x += 0.5f; ShopPos.y += 0.5f;
                  //  NPCList[NPCIndex].SetMovementTarget(ShopPos);
                   if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell( ShopPos), GridCreator.GameGrid, GridCreator.GameMap,gridCreator))
                   {
                        Debug.Log("Set action to 0, moving to nearest shop");
                        NPCList[NPCIndex].SetCurrentAction(0);
                        NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                   }
                    else
                    {
                        Debug.Log("Route to nearest shop not found, route not set");
                    }
                   // if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(ShopPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                  //  {
                  //        Debug.Log("Set action to 0");
                  //      NPCList[NPCIndex].SetCurrentAction(0);
                  //      NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                  //      NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                    //}



                }
                else
                {
                    Debug.Log("no shop found, attempting to wander");
                    //no shop found
                    //NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    if (NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                    {
                        Debug.Log("Set action to 0, couldnt find shop, wandering");
                        NPCList[NPCIndex].SetCurrentAction(0);
                    }
                    else
                    {
                        Debug.Log("Coud not wander, route setting failed");
                    }
            //        if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
             //       {
               //         Debug.Log("Set action to 0");
                 //       NPCList[NPCIndex].SetCurrentAction(0);
                 //   }

                }
                

                
            }
            else if(RandomValue>=ShopChance && RandomValue < ShopChance+HomeChance)
            {
                //go home
                Debug.Log("NPC wants to go home");
                Vector3 HomePos = NPCList[NPCIndex].GetHomePos();
                if (HomePos.x != -1) {
                    Debug.Log("Home found,attempting to set route");
                    if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HomePos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                    {
                        Debug.Log("Set action to 0 going home");
                        NPCList[NPCIndex].SetCurrentAction(0);
                        NPCList[NPCIndex].SetMovementTarget(HomePos);

                        NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        NPCList[NPCIndex].SetTargetBuilding(NPCList[NPCIndex].GetHome());
                    }
                    else
                    {
                        Debug.Log("Route to home not found, no route set");
                    }

                }
                else
                {
                    Debug.Log("No home found");
                }
                
            }
            else if(RandomValue >= ShopChance+HomeChance && RandomValue < ShopChance + HomeChance + HospitalChance)
            {
                Debug.Log("NPC wants to go to the hospital");
                //go to the hospital 
                if (GridCreator.GetNumberOfHospitals() != 0)// check hopsital exists
                {
                    Debug.Log("Hospital found");
                    Vector3 HospitalPos = GridCreator.GetPosOfNearestHospital(NPCList[NPCIndex].GetPosition());
                    if (HospitalPos.x != -1)
                    {
                        Debug.Log("Hospital position found, attempting to set route");
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HospitalPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                        {
                            Debug.Log("Set action to 0, Going to hospital");
                            NPCList[NPCIndex].SetCurrentAction(0);
                            NPCList[NPCIndex].SetMovementTarget(HospitalPos);

                           NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                           NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                        }
                        else
                        {
                            Debug.Log("Route to hospital failed ot be set, no route set");
                        }

                      //  if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(HospitalPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                      //  {
                      //      Debug.Log("Set action to 0");
                      //      NPCList[NPCIndex].SetCurrentAction(0);
                       //        NPCList[NPCIndex].SetMovementTarget(HospitalPos);

//                            NPCList[NPCIndex].SetIfTargetIsBuilding(true);
  //                          NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
    //                    }

                    }
                    else
                    {
                        Debug.Log("Nearest hospital pos could not be identified, no route set");
                    }

                   
                }
                else
                {
                    Debug.Log("No hospitals found,no route set");
                }
            }
            else if (RandomValue >= ShopChance + HomeChance+HospitalChance && RandomValue < ShopChance + HomeChance + HospitalChance+EntertainmentChance)
            {
                //go to some kind of entertainment
                Debug.Log("NPC wants to partake in entertainment");
                if (GridCreator.GetNumberOfEntertainment()!=0)//check entertainment exists
                {
                    Debug.Log("Entertainment found to exit");
                    Vector3 EntertainmentPos = GridCreator.GetPosOfNearestEntertainment(NPCList[NPCIndex].GetPosition());
                    if(EntertainmentPos.x != -1)
                    {
                        Debug.Log("Position of entertainment identified, attempting to set route");
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(EntertainmentPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                        {
                            Debug.Log("Set action to 0 but going to partake in entertainment");
                            NPCList[NPCIndex].SetCurrentAction(0);
                              NPCList[NPCIndex].SetMovementTarget(EntertainmentPos);

                            NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                            NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());

                        }
                        else
                        {
                            Debug.Log("Failed to set route to entertainment, no route set");
                        }

                        //       if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(EntertainmentPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                        //      {
                        //         Debug.Log("Set action to 0");
                        //        NPCList[NPCIndex].SetCurrentAction(0);
                        //  NPCList[NPCIndex].SetMovementTarget(EntertainmentPos);

                        //         NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        //       NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());

                        //                        }

                    }
                    else
                    {
                        Debug.Log("Position of nearest entertainment could not be identified, no route set");
                    }
                }
                else
                {
                    Debug.Log("No entertainment identified, no route set");
                }
               
            }
            else
            {
                Debug.Log("NPC wants to wander");
               // Wander
                if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                {
                    Debug.Log("Set action to 0, wandering");
                    NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    NPCList[NPCIndex].SetCurrentAction(0);
                    NPCList[NPCIndex].SetCurrentAction(0);
                }
                else
                {
                    Debug.Log("No route identified for wandering");
                }
               // if (NPCList[NPCIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
               //{
                 //     Debug.Log("Set action to 0");
                 //   //  NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                  //  NPCList[NPCIndex].SetCurrentAction(0);
                    // NPCList[NPCIndex].SetCurrentAction(0);
                //}


            }

        }
        Debug.Log("End of selecting action");
    }
    void RemoveNPCs(List<int> Indexes)
    {
        for(int i = Indexes.Count-1; i >= 0; i--)
        {
            if (NPCList[Indexes[i]].GetHomeIndex() != -1)
            {
                if (GridCreator.PlacedBuildings[NPCList[Indexes[i]].GetHomeIndex()].buildingType is Home home)
                {
                    home.AdjustResidents(-1);
                }
                
            }
            NPCList[Indexes[i]].RemoveNPCSprite();
            NPCList.RemoveAt(Indexes[i]);
            NumberOfNpcs--;
          //  Debug.Log("Npc left");
        }
    }
    public void UpdateNPCRoutesAfterRoutesRemoval(List<Route> DeletedRoutes)
    {
        for (int i = 0; i < DeletedRoutes.Count; i++)
        {
            for (int x = 0; x < NPCList.Count; x++)
            {
                if (NPCList[x].CurrentStation == DeletedRoutes[i].StartStation || NPCList[x].CurrentStation == DeletedRoutes[i].EndStation||
                    NPCList[x].TargetStation == DeletedRoutes[i].StartStation || NPCList[x].TargetStation == DeletedRoutes[i].EndStation)
                {
                    if (NPCList[x].GetCurrentAction() == 0)
                    {
                        Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                        NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                      //  NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                    }
                    else if (NPCList[x].GetCurrentAction() == 6)
                    {
                        for(int e=0; e < TransportPlacementScript.TrainRoutes.Count; e++)
                        {
                            Route Current = TransportPlacementScript.TrainRoutes[e];
                            if (Current.GetNPCIDs().Contains(NPCList[x].GetCitzenID()))
                            {
                                NPCList[x].NeedsUpdateAfterTravel = true;
                            }
                            else
                            {
                                NPCList[x].ReDisplaySprite();
                                NPCList[x].SetCurrentAction(0);
                                Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                                NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                               // NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            }
                        }
                    }
                    else if (NPCList[x].GetCurrentAction()== 7)
                    {
                        for (int e = 0; e < TransportPlacementScript.BusRoutes.Count; e++)
                        {
                            BusRoute Current = TransportPlacementScript.BusRoutes[e];
                            if (Current.GetNPCIDs().Contains(NPCList[x].GetCitzenID()))
                            {
                                NPCList[x].NeedsUpdateAfterTravel = true;
                            }
                            else 
                            {
                                NPCList[x].ReDisplaySprite();
                                NPCList[x].SetCurrentAction(0);
                                Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                                NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            //    NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            }
                        }
                    }
                }
            }
        }
        
    }
    public void UpdateNPCRoutesAfterBusRoutesRemoval(List<BusRoute> DeletedRoutes)
    {
        for (int i = 0; i < DeletedRoutes.Count; i++)
        {
            for (int x = 0; x < NPCList.Count; x++)
            {
                if (NPCList[x].CurrentBusStop == DeletedRoutes[i].StartStop || NPCList[x].CurrentBusStop == DeletedRoutes[i].EndStop ||
                    NPCList[x].TargetBusStop == DeletedRoutes[i].StartStop || NPCList[x].TargetBusStop== DeletedRoutes[i].EndStop)
                {
                    if (NPCList[x].GetCurrentAction() == 0)
                    {
                        Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                         NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                       // NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                    }
                    else if (NPCList[x].GetCurrentAction() == 7)
                    {
                        for (int e = 0; e < TransportPlacementScript.TrainRoutes.Count; e++)
                        {
                            Route Current = TransportPlacementScript.TrainRoutes[e];
                            if (Current.GetNPCIDs().Contains(NPCList[x].GetCitzenID()))
                            {
                                NPCList[x].NeedsUpdateAfterTravel = true;
                            }
                            else
                            {
                                NPCList[x].ReDisplaySprite();
                                NPCList[x].SetCurrentAction(0);
                                Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                                NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            //    NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            }
                        }
                    }
                    else if (NPCList[x].GetCurrentAction() == 7)
                    {
                        for (int e = 0; e < TransportPlacementScript.BusRoutes.Count; e++)
                        {
                            BusRoute Current = TransportPlacementScript.BusRoutes[e];
                            if (Current.GetNPCIDs().Contains(NPCList[x].GetCitzenID()))
                            {
                                NPCList[x].NeedsUpdateAfterTravel = true;
                            }
                            else
                            {
                                NPCList[x].ReDisplaySprite();
                                NPCList[x].SetCurrentAction(0);
                                Vector3 TargetPos = (NPCList[x].GetRoutePositions()[NPCList[x].GetRoutePositions().Count - 1]);
                                NPCList[x].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                          //      NPCList[x].SetRouteNew(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                            }
                        }
                    }
                }
            }
        }

    }
    void CheckIfStuck(int Index)
    {
        NPCList[Index].UpdateStuckCount(1);
        if (NPCList[Index].CheckIfStuck())
        {
            NPCList[Index].ResetStuckNPC();
        }

    }
    // Add this method to NPChandler
    void HandleStuckNPC(int npcIndex)
    {
        int action = NPCList[npcIndex].GetCurrentAction();
        // Do not handle stuck if waiting for train or bus
        if (action == 6 || action == 7)
        {
            return;
        }
           
        // Try to assign a wander route as a fallback
        Vector3 wanderTarget = GetWanderTarget();
        //  bool routeSet = NPCList[npcIndex].SetRouteNew(GridCreator.GameMap.WorldToCell(wanderTarget),GridCreator.GameGrid,GridCreator.GameMap, gridCreator);
        bool routeSet = NPCList[npcIndex].SetRoute(GridCreator.GameMap.WorldToCell(wanderTarget), GridCreator.GameGrid, GridCreator.GameMap, gridCreator);
        if (routeSet)
        {
            NPCList[npcIndex].SetCurrentAction(0);
            NPCList[npcIndex].ResetStuckCount();
        }
        else
        {
            // If even wandering fails, just reset and try again next frame
            NPCList[npcIndex].SetCurrentAction(-1);
            NPCList[npcIndex].ResetStuckCount();
        }
    }
    void UpdateNPCs()
    {
        List<int> NPCsToRemove=new List<int>();
        MovementCounter++;
        // check for updates to each NPC
        for (int i = 0; i < NPCList.Count; i++) {
            // CheckIfStuck(i);
            // Stuck detection and handling
         //   NPCList[i].UpdateStuckCount(1);
          //  if (NPCList[i].CheckIfStuck())
           // {
            //    HandleStuckNPC(i);
             //   continue; // Skip further processing for this NPC this frame
           // }

            if (NPCList[i].GetIfJusteEnteredBuilding())
            {
                int BuildingIndex=gridCreator.EnterBuildingForNPC(NPCList[i].GetPosition(),i);
                if (BuildingIndex != -1)
                {
                    NPCList[i].buildingInsideIndex=BuildingIndex; 
                }
                NPCList[i].JustEnteredBuilding = false;

            }
            if (NPCList[i].JustLeftBuilding && NPCList[i].buildingInsideIndex != -1)
            {
                if (NPCList[i].buildingInsideIndex < GridCreator.PlacedBuildings.Count) {
                    GridCreator.PlacedBuildings[NPCList[i].buildingInsideIndex].RemoveSpecificIndex(i);
                }
                
                NPCList[i].ResetBuildingData();
            }
            //New action
            if (NPCList[i].GetCurrentAction() == -1)
            {
                Debug.Log("NPC selecting new action");
                // No action selected ,select new action
               // Debug.Log("Selecting new action");
                SelectNewAction(i);
            }
            //NPc moving
            else if (NPCList[i].GetCurrentAction() == 0 )
            {
                Debug.Log("NPC Moving");
                //moving
                if (NPCList[i].GetMoveCounter() == frameToMoveOn)
                {
                    NPCList[i].ResetCounter();
                    //      Debug.Log("Moving towards target");
                    // NPCList[i].MovetowardsTarget();
                     NPCList[i].MoveTowardsTargetOnRoute(gridCreator,GameFeaturesHandler.GetAirQaulityRating());
                  //  NPCList[i].MoveTowardsTargetOnRouteNew(gridCreator,GameFeaturesHandler.GetAirQaulityRating());
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }          
            }
            else if (NPCList[i].GetCurrentAction() == 1)
            {
                Debug.Log("NPC in building");
                // In building
                if (NPCList[i].GetMoveCounter() == BuildingFrame)
                {
                    NPCList[i].ResetCounter();
                    NPCList[i].SpendTImeInBuilding(GameFeaturesHandler.GetAirQaulityRating());
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            else if (NPCList[i].GetCurrentAction() == 2)
            {
                Debug.Log("NPC at home");
                // at home
                if (NPCList[i].GetMoveCounter() == BuildingFrame)
                {
                    NPCList[i].ResetCounter();
                    NPCList[i].SpendTImeAtHome();
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            else if (NPCList[i].GetCurrentAction() == 3)
            {
                // in hospital
                Debug.Log("Npc at hospital");
                if (NPCList[i].GetMoveCounter() == BuildingFrame)
                {
                    NPCList[i].ResetCounter();
                    NPCList[i].SpendTImeAtHospital();
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            else if (NPCList[i].GetCurrentAction() == 4)
            {
                // partaking in entertainment
                Debug.Log("Npc partaking in entertainment");
                if(NPCList[i].GetMoveCounter() == BuildingFrame)
                {
                    NPCList[i].ResetCounter();
                    NPCList[i].PartakeInEnterainment(GameFeaturesHandler.GetAirQaulityRating());
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            else if (NPCList[i].GetCurrentAction() == 5)
            {
                Debug.Log("NPC leaving");
                //Leave the city
                NPCsToRemove.Add(i);

            }
            else if (NPCList[i].GetCurrentAction() == 6)
            {
                Debug.Log("NPC waiting for train");
                //Waiting for train
            }
            else if (NPCList[i].GetCurrentAction() == 7)
            {
                Debug.Log("NPC waiting for bus");
                //waiting for bus
            }
            
        }
        if(NPCList.Count!= 0)
        {
            RemoveNPCs(NPCsToRemove);
            UpdatePopulationDisplay();
            NPCsToRemove.Clear();
            SetHomes();
            
        }
        
    }

}
