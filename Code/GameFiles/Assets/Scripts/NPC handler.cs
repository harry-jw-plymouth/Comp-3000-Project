using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPChandler : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    int MovementCounter = 0; int frameToMoveOn = 10;
    int AmountCheckCounter = 0;int CheckFrame = 1000;
    int BuildingFrame = 20 ;
   [SerializeField]  List<Citzen> NPCList=new List<Citzen>();
    [SerializeField] int NumberOfNpcs;
    public GameObject NPCPrefab;
    public TextMeshProUGUI PopulationCountDisplay;
    


    Vector3Int MapCenter = new Vector3Int(GridCreator.WIDTH / 2, GridCreator.HEIGHT / 2, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    void LoadNPCs()
    {
        Vector3 worldPosition = GridCreator.GameMap.GetCellCenterWorld(MapCenter);
        for (int i = 0; i < NumberOfNpcs; i++) {
            worldPosition.x++; 
            GameObject Current= Instantiate(NPCPrefab,worldPosition,Quaternion.identity );
            NPCList.Add(new Citzen(worldPosition,Current));
          //  Debug.Log("placinng NPC");
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
        }
    //    Debug.Log("Number of NPCS:" + NumberOfNpcs);

    }
    void CheckForLeavingNPCs()
    {
        int RandomNpcIndex = Random.Range(0, NPCList.Count-1);
        int Happiness = 50;
        try
        {
            Happiness = NPCList[RandomNpcIndex].CalculateHappiness();
        }
        catch {
            Happiness = NPCList[RandomNpcIndex/2].CalculateHappiness();
        }
        ;

        
        int RandomLeaveChance = Random.Range(0, 100);
        if (RandomLeaveChance > Happiness) {
            if (NPCList[RandomNpcIndex].GetIfInBuilding())
            {
                NPCList[RandomNpcIndex].ForceLeaveBuidlingOnBuildingRemoval();
                
            }
            NPCList[RandomNpcIndex].SetCurrentAction(5);
        }
    }
    void CheckForNewNPCs()
    {
        float value = Random.Range(30, 200);
        if (value < GameStatusScript.GetRating())
        {
            Vector3 worldPosition = new Vector3(Random.Range(0,GridCreator.WIDTH),Random.Range(0,GridCreator.HEIGHT),0);
            NumberOfNpcs++;
            GameObject Current = Instantiate(NPCPrefab, worldPosition, Quaternion.identity);
            NPCList.Add(new Citzen(worldPosition, Current));
        }
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
                if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(RoadPos), GridCreator.GameGrid, GridCreator.GameMap))
                {
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
                   if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell( ShopPos), GridCreator.GameGrid, GridCreator.GameMap))
                    {
                        NPCList[NPCIndex].SetCurrentAction(0);
                        NPCList[NPCIndex].SetIfTargetIsBuilding(true);
                        NPCList[NPCIndex].SetTargetBuilding(GridCreator.GetSelectedBuilding());
                    }
                    

                    
                }
                else
                {
                    //no shop found
                    //NPCList[NPCIndex].SetMovementTarget(GetWanderTarget());
                    if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap))
                    {
                        NPCList[NPCIndex].SetCurrentAction(0);
                    }
                   
                }

                
            }
            else if(RandomValue>=ShopChance && RandomValue < ShopChance+HomeChance)
            {
                //go home
                Vector3 HomePos = NPCList[NPCIndex].GetHomePos();
                if (HomePos.x != -1) {
                    if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HomePos), GridCreator.GameGrid, GridCreator.GameMap))
                    {
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
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(HospitalPos), GridCreator.GameGrid, GridCreator.GameMap))
                        {
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
                        if(NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(EntertainmentPos), GridCreator.GameGrid, GridCreator.GameMap))
                        {
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
                if( NPCList[NPCIndex].SetRoute(GridCreator.GameMap.WorldToCell(GetWanderTarget()), GridCreator.GameGrid, GridCreator.GameMap))
                {
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
                    NPCList[i].buldingInsideIndex=BuildingIndex; 
                }
                NPCList[i].JustEnteredBuilding = false;

            }
            if (NPCList[i].JustLeftBuilding && NPCList[i].buldingInsideIndex != -1)
            {
                if (NPCList[i].buldingInsideIndex < GridCreator.PlacedBuildings.Count) {
                    GridCreator.PlacedBuildings[NPCList[i].buldingInsideIndex].RemoveSpecificIndex(i);
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
                    NPCList[i].MoveTowardsTargetOnRoute();
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
