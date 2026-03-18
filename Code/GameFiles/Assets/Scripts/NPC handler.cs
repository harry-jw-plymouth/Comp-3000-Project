using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class NPChandler : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
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
        Debug.Log("Number of npcs from save:" + Save.NumberOfNPCs);
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
            CheckForNewNPCs();
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
        if (RandomLeaveChance > Happiness && NumberOfNpcs >1) {
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
                    Debug.Log("NPC getting on train");
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
        
        int ShopChance = 5;
        int HomeChance = 15 + ((NPCList[NPCIndex].GetTiredNess()/50));
        int WanderChance = 80;
        int EntertainmentChance = 5 + ((NPCList[NPCIndex].GetBoredom() / 50));
        int HospitalChance = 0 + (NPCList[NPCIndex].GetSickness() / 100);
        int RandomValue = UnityEngine.Random.Range(0, ShopChance+HomeChance+WanderChance);


        if (NPCList[NPCIndex].GetCurrentAction() == 7)
        {
            return; // already waiting for bus
        }
            
                    //   Debug.Log("TiredNess:" + NPCList[NPCIndex].GetTiredNess());
        if (!CheckIfOnRoad(NPCList[NPCIndex].GetPosition())){
            //Debug.Log("Not on road");
            if (GridCreator.GetIfRoadExists())
            {
              //  Debug.Log("RoadFound");
              //  NPCList[NPCIndex].SetCurrentAction(0) ;
                //Go to nearest path
                Vector3 RoadPos= GridCreator.GetPosOfNearestRoad(NPCList[NPCIndex].GetPosition());
                RoadPos.y += 0.5f;
                RoadPos.x += 0.5f;
              //  NPCList[NPCIndex].SetMovementTarget(RoadPos);
                if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(RoadPos), GridCreator.GameGrid, GridCreator.GameMap,gridCreator))
                {
                    Debug.Log("Set action to 0");
                    NPCList[NPCIndex].SetCurrentAction(0);
                }
            }
        }
        else
        {
           // Debug.Log("Already on road");
            if (RandomValue < ShopChance)
            {
                // go to the shop
                Vector3 ShopPos = GridCreator.GetPosOfNearestShop(NPCList[NPCIndex].GetPosition());
                if (ShopPos.x != -1)
                {
                    //Go to nearest shop
                   

                    ShopPos.x += 0.5f; ShopPos.y += 0.5f;
                  //  NPCList[NPCIndex].SetMovementTarget(ShopPos);
                   if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell( ShopPos), GridCreator.GameGrid, GridCreator.GameMap,gridCreator))
                    {
                        Debug.Log("Set action to 0");
                        NPCList[NPCIndex].SetCurrentAction(0);
                        NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                    }
                    

                    
                }
                else
                {
                    //no shop found
                    //NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap,gridCreator))
                    {
                        Debug.Log("Set action to 0");
                        NPCList[NPCIndex].SetCurrentAction(0);
                    }
                   
                }

                
            }
            else if(RandomValue>=ShopChance && RandomValue < ShopChance+HomeChance)
            {
                //go home
                Vector3 HomePos = NPCList[NPCIndex].GetHomePos();
                if (HomePos.x != -1) {
                    if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HomePos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                    {
                        Debug.Log("Set action to 0");
                        NPCList[NPCIndex].SetCurrentAction(0);
                        //  NPCList[NPCIndex].SetMovementTarget(HomePos);

                        NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        NPCList[NPCIndex].SetTargetBuilding(NPCList[NPCIndex].GetHome());
                    }     
                }
                
            }
            else if(RandomValue >= ShopChance+HomeChance && RandomValue < ShopChance + HomeChance + HospitalChance)
            {
                //go to the hospital 
                if (GridCreator.GetNumberOfHospitals() != 0)// check hopsital exists
                {
                    Vector3 HospitalPos = GridCreator.GetPosOfNearestHospital(NPCList[NPCIndex].GetPosition());
                    if (HospitalPos.x != -1)
                    {
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HospitalPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                        {
                            Debug.Log("Set action to 0");
                            NPCList[NPCIndex].SetCurrentAction(0);
                            //   NPCList[NPCIndex].SetMovementTarget(HospitalPos);

                            NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                            NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                        }
                        
                    }

                   
                }
            }
            else if (RandomValue >= ShopChance + HomeChance+HospitalChance && RandomValue < ShopChance + HomeChance + HospitalChance+EntertainmentChance)
            {
                //go to some kind of entertainment
                if (GridCreator.GetNumberOfEntertainment()!=0)//check hospitals exist
                {
                    Vector3 EntertainmentPos = GridCreator.GetPosOfNearestEntertainment(NPCList[NPCIndex].GetPosition());
                    if(EntertainmentPos.x != -1)
                    {
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(EntertainmentPos), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                        {
                            Debug.Log("Set action to 0");
                            NPCList[NPCIndex].SetCurrentAction(0);
                            //  NPCList[NPCIndex].SetMovementTarget(EntertainmentPos);

                            NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                            NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());

                        }
                       
                    }
                }
               
            }
            else
            {
                //Wander
                if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap, gridCreator))
                {
                    Debug.Log("Set action to 0");
                    //  NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    NPCList[NPCIndex].SetCurrentAction(0);
                    // NPCList[NPCIndex].SetCurrentAction(0);
                }


            }
        }
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
            Debug.Log("Npc left");
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
               // Debug.Log("Selecting new action");
                SelectNewAction(i);
            }
            //NPc moving
            else if (NPCList[i].GetCurrentAction() == 0 )
            {
                //moving
                if (NPCList[i].GetMoveCounter() == frameToMoveOn)
                {
                    NPCList[i].ResetCounter();
                    //      Debug.Log("Moving towards target");
                    // NPCList[i].MovetowardsTarget();
                    NPCList[i].MoveTowardsTargetOnRoute(gridCreator);
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
                    NPCList[i].SpendTImeInBuilding();
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
                    NPCList[i].PartakeInEnterainment();
                }
                else
                {
                    NPCList[i].UpdateCounter();
                }
            }
            else if (NPCList[i].GetCurrentAction() == 5)
            {
                //Leave the city
                NPCsToRemove.Add(i);

            }
            else if (NPCList[i].GetCurrentAction() == 6)
            {
                //Waiting for train
            }
            else if (NPCList[i].GetCurrentAction() == 7)
            {
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
