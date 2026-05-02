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

        NumberOfNpcs = GetNumberOfNPCs();
        LoadNPCs();
        SetHomes();
        UpdatePopulationDisplay();

    }
    void UpdatePopulationDisplay()
    {
        // Display number of NPCs to UI
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
        //for each NPC that was assigned to the home just removed, set them to homeless
        for (int i = 0; i < Indexes.Count; i++) {
            NPCList[Indexes[i]].SetHomeIndex(-1);
            NPCList[Indexes[i]].UpdateHomeStatus(true);
            NPCList[Indexes[i]].RemoveHomeData();
        }

        //then update the NPC homeless status
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
        //else get NPC amount from save file
        SaveFileModel Save= DBManager.GetSaveFiles()[MainMenu.GetCurrentSaveID()];
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
        //calculate percentage from total and return 
        float percent = (float)total / NPCList.Count * 100f;
        return percent;
    }
    public void SetHomes()
    {
        int TotalSet=0; int TotalNotSet = 0;
        int HomeIndex = 0;
        bool HomeFound=false;

        // loop through each NPC
        for (int i = 0; i < NPCList.Count; i++) {
            HomeFound = false;
            if (NPCList[i].GetIfHomeless())
            {
                HomeIndex = 0;
                foreach (PlacedBuilding building in GridCreator.PlacedBuildings)
                {
                    HomeIndex++;
                    if (building.buildingType is Home home)
                    {
                        // check if home has space to assign NPC
                        if (!home.GetIfFull())
                        {
                            if (home.AdjustResidents(1))
                            {
                                // if NPC was able to be added, set home found to true so building loop can break and set required data for NPC living at the specified home
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
                    // if home not available set the NPC as homeless 
                    NPCList[i].UpdateHomeStatus(true);
                    NPCList[i].SetHomePos(new Vector3(-1, -1, -1));
                    TotalNotSet++;
                }
            }
            else
            {
                // if already assigned a home, add to counter
                TotalSet++;
            }
        }
    }
    public int GetRandomNPCSpriteIndex()
    {
        // return a random NPC sprite from the list defined earlier
        return Random.Range(0,NPCSprites.Count);
    }
    void LoadNPCs()
    {
        // for each NPC, set a random start position on a road and assign a random sprite
        for (int i = 0; i < NumberOfNpcs; i++) {
            Vector3 StartPos=  GridCreator.RoadPositions[ Random.Range(0, GridCreator.RoadPositions.Count)];

            GameObject Current = Instantiate(NPCSprites[GetRandomNPCSpriteIndex()], GridCreator.GameMap.CellToWorld(new Vector3Int((int)StartPos.x,(int)StartPos.y,0)), Quaternion.identity);
            NPCList.Add(new Citzen(StartPos,CurrentID++,Current));
        }
    }
    void CheckForNPCsToUpdateAfterTrainRouteRemoval()
    {
        // for each NPC, check if theyve just got off a train that had its route removed
        // if this is the case, recaculate their route to avoid errors
        for (int i = 0; i < NumberOfNpcs; i++)
        {
            if (NPCList[i].ReadyToUpdateAfterTravel)
            {
                Vector3 TargetPos = (NPCList[i].GetRoutePositions()[NPCList[i].GetRoutePositions().Count - 1]);
                NPCList[i].SetRoute(GridCreator.GameMap.WorldToCell(TargetPos), GridCreator.GameGrid, GridCreator.GameMap, GridHandler);
                NPCList[i].ReadyToUpdateAfterTravel = false;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateNPCs();
        
        // counter implemented to ensure code doesnt go off every frame(would be too frequent)
        AmountCheckCounter++;
        if (AmountCheckCounter >= CheckFrame)
        {
            AmountCheckCounter = 0;
            CheckForNewNPCs();
            CheckForLeavingNPCs();
            UpdatePopulationDisplay();
            CheckForNPCsToUpdateAfterTrainRouteRemoval();
       
        }

    }
    void CheckForLeavingNPCs()        
    {
        // stop NPC leaving if it is the final one
        if (NPCList.Count == 0)
        {
            return;
        }
        int RandomNpcIndex = Random.Range(0, NPCList.Count);
        int Happiness = NPCList[RandomNpcIndex].CalculateHappiness();
        

        // Generate a random and use this to determine if an NPC is deciding to leave
        int RandomLeaveChance = Random.Range(0, 100);
        if (RandomLeaveChance > Happiness*1.8f  && NumberOfNpcs >1) {
            if (NPCList[RandomNpcIndex].GetIfInBuilding())
            {
                NPCList[RandomNpcIndex].ForceLeaveBuidlingOnBuildingRemoval();
                
            }
            // on next update NPCs call, NPC will be removed
            NPCList[RandomNpcIndex].SetCurrentAction(5);
        }
    }
    public List<int> GetNPCsIdWaitingForTrain(PlacedBuilding Start,PlacedBuilding End)
    {
        List<int> Ids = new List<int>();
        // for each NPC, check if the NPC route information matches the train route information
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
        // do a random to check for new NPCs and then generate a new NPC based on the result
        float value = Random.Range(30, 400);
        if (value < GameStatusScript.GetRating())
        {
            Vector3 StartPos = GridCreator.RoadPositions[Random.Range(0, GridCreator.RoadPositions.Count)];
            NumberOfNpcs++;
            GameObject Current = Instantiate(NPCSprites[GetRandomNPCSpriteIndex()],
                GridCreator.GameMap.CellToWorld(new Vector3Int((int)StartPos.x, (int)StartPos.y, 0)),
                Quaternion.identity);
            NPCList.Add(new Citzen(StartPos,CurrentID++, Current));
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
    // for each NPC, get them to continue on traversal route after getting off a train
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
    // for each NPC, get them to continue on traversal route after getting off a bus
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
    // Get random coordinates for NPC to travel to, go to a road if possible
    public Vector3 GetWanderTarget()
    {   
        Vector3 RoadTarget = GridCreator.GetRandomRoadCoorindates();
        if (RoadTarget.x == -1)
        {
            //No road
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
    // Select new action for NPC
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


        if (!CheckIfOnRoad(NPCList[NPCIndex].GetPosition())){
            Debug.Log("Not on road,checking for road");
            if (GridCreator.GetIfRoadExists())
            {
                Debug.Log("RoadFound");
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
                }
                else
                {
                    Debug.Log("no shop found, attempting to wander");
                    //no shop found
                    if (NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                    {
                        Debug.Log("Set action to 0, couldnt find shop, wandering");
                        NPCList[NPCIndex].SetCurrentAction(0);
                    }
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

                    }
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
            }

        }
        Debug.Log("End of selecting action");
    } 
    // for each NPC index in the list, remove their data from anywhere else in code that would cause issues then remove the NPC
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

        }
    } 
    // After Travel route removal,find any NPC that is using route and adjust accordingly
    // if NPC walking but will use the route, reroute 
    // if NPC Waiting for bus/train stop them waiting and reroute 
    // if NPC currently on route, mark them as needing update for next NPC update code
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
                            }
                        }
                    }
                    else if (NPCList[x].GetCurrentAction() == 7)
                    {
                        // check if NPC got off of any buses. If yes then 
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
                            }
                        }
                    }
                }
            }
        }

    }

   // for each NPC, update what they are currently doing
   // For movement, NPCs  only move on certain frames to ensure they dont move too often 
    void UpdateNPCs()
    {
        List<int> NPCsToRemove=new List<int>();
        MovementCounter++;
        // check for updates to each NPC
        for (int i = 0; i < NPCList.Count; i++) {

            if (NPCList[i].GetIfJusteEnteredBuilding())
            {
                int BuildingIndex=gridCreator.EnterBuildingForNPC(NPCList[i].GetPosition(),i);
                if (BuildingIndex != -1)
                {
                    NPCList[i].buildingInsideIndex=BuildingIndex; 
                }
                NPCList[i].JustEnteredBuilding = false;

            }
            //if NPCs just left building then update building data accordingly 
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
                // No action selected ,select new action
                SelectNewAction(i);
            }
            //NPc moving
            else if (NPCList[i].GetCurrentAction() == 0 )
            {
                //moving
                if (NPCList[i].GetMoveCounter() == frameToMoveOn)
                {
                    NPCList[i].ResetCounter();
                     NPCList[i].MoveTowardsTargetOnRoute(gridCreator,GameFeaturesHandler.GetAirQaulityRating());
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }          
            }
            else if (NPCList[i].GetCurrentAction() == 1)
            {
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
                //Add to the list of NPCs to be removed when NPCs next updated
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
            // After NPC updates done, remove any leaving NPCs and update game accordingly
            RemoveNPCs(NPCsToRemove);
            UpdatePopulationDisplay();
            NPCsToRemove.Clear();
            SetHomes();
            
        }
        
    }

}
