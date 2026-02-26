using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Citzen
{
    float MovementSpeed = 0.05f;
    int MoveCounter=0;
    int CurrentAction=-1;
    //-1 No action 
    // 0 Moving
    //1 InBuilding
    //2 at home
    //3 in hospital
    int InBuilding = 0;
    bool TargetIsbuilding=false;
    Building BuildingCurrentlyTargetting;
    Vector3 MovementTarget=new Vector3(0,0,0);
    Vector3 Position;
    public bool UpdateNeeded;
    GameObject NPCSprite;
    bool IsHomeLess = true;
    public bool JustEnteredBuilding = false;
    public bool JustLeftBuilding=false;
    Vector3 BuildingInsidePos = new Vector3(-1, -1, -1);
    public int buldingInsideIndex = -1;

    Building Home;
    int HomeIndex = -1;
    Vector3 HomePosition=new Vector3(-1,-1,-1);


    int TiredNess = 0;
    int Sickness = 0;
    int Boredom = 0;
    //happiness rated out of 100
    int Happiness = 0;

    int PositionOnRoute;
    int NexPositionOnRoute;
    List<Vector3> RoutePositions=new List<Vector3>();

    public Citzen(Vector3 Pos,GameObject sprite)
    {
        NPCSprite = sprite;
        Position= Pos;
        UpdateNeeded = true;
    }
    public void RemoveNPCSprite()
    {
        Object.Destroy(NPCSprite);
    }
    public void SetHomeIndex(int Index)
    {
        HomeIndex = Index;
    }
    public int GetHomeIndex()
    {
        return HomeIndex;
    }
    public bool GetIfInBuilding()
    {
        if (InBuilding >= 0)
        {
            return true;
        }
        return false;
    }
    public int CalculateHappiness()
    {
        int Happiness = 100;
        Happiness -= TiredNess / 100;
        Happiness -= Sickness / 100;
        Happiness-=Boredom / 100;

        if (Happiness < 0)
        { 
            Happiness = 0;
        }

        if (IsHomeLess)
        {
            Happiness = Happiness / 2;
        }
        return Happiness;
    }
        
    public int GetHappiness()
    {
        return Happiness;
    }
    public void AdjustBoredom(int Adjustment)
    {
        Boredom += Adjustment;
    }
    public int GetBoredom()
    {
        return Boredom;
    } 
    public void IncreaseBoredom(int Max)
    {
        Boredom += (int)Random.Range(0, Max);
    }
    public bool GetIfJusteEnteredBuilding()
    {
        return JustEnteredBuilding;
    }
    public Vector3 GetPosOfBuildingToEnter()
    {
        return BuildingInsidePos;
    }
    public void IncreaseSickness(int Max)
    {
        Sickness += Random.Range(0, Max);
    }
    public int GetSickness()
    {
        return Sickness;
    }
    public void IncreaseTiredNess()
    {
        TiredNess += (int)Random.Range(0, 2);
    }
    public void SetHome(Building home)
    {
        Home =home;
    }
    public Building GetHome()
    {
        return Home;
    }
    public Vector3 GetHomePos()
    {
        return HomePosition;
    }
    public void SetHomePos(Vector3 Pos)
    {
        HomePosition = Pos;
    }
    public void UpdateHomeStatus(bool NewStatus)
    {
        IsHomeLess = NewStatus;
    }
    public void RemoveHomeData()
    {
        Home = null;
        HomePosition = new Vector3(-1, -1, -1);
    }
    public bool GetIfHomeless()
    {
        return IsHomeLess;
    }
    public void SetTargetBuilding(Building target)
    {
        BuildingCurrentlyTargetting = target;
        
    }
    public List<Vector3Int> GetRailsTouchingStation(Vector3Int CurrentPos)
    {
        List<Vector3Int> Positions = new List<Vector3Int>();

        if (GridCreator.GameGrid[CurrentPos.x + 1, CurrentPos.y].Contains == 4)
        {
            Positions.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x - 1, CurrentPos.y].Contains == 4)
        {
            Positions.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y + 1].Contains == 4)
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
        }
        if (GridCreator.GameGrid[CurrentPos.x, CurrentPos.y - 1].Contains == 4)
        {
            Positions.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
        }
        return Positions;
    }
    public bool SetRoute(Vector3Int Target, Square[,]Grid, Tilemap GameMap)
    {
        Queue<Vector3Int> ToCheck = new Queue<Vector3Int>();
        HashSet<Vector3Int> AlreadyVisited = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> CameFrom = new Dictionary<Vector3Int, Vector3Int>();
        List<Vector3Int> TilesAroundStart = GetRailsTouchingStation(GameMap.WorldToCell(Position));
        for (int i = 0; i < TilesAroundStart.Count; i++)
        {
            ToCheck.Enqueue(TilesAroundStart[i]);
            AlreadyVisited.Add(TilesAroundStart[i]);
            CameFrom[TilesAroundStart[i]] = TilesAroundStart[i];
        }

        while (ToCheck.Count > 0)
        {
            Vector3Int Current = ToCheck.Dequeue();

            if (Current==Target)
            {
                Debug.Log("Route set");
                RoutePositions = new List<Vector3>();
                Vector3Int CurrentRoutePos = Current;
                while (CameFrom[CurrentRoutePos] != CurrentRoutePos)
                {
                    RoutePositions.Add(CurrentRoutePos);
                    CurrentRoutePos = CameFrom[CurrentRoutePos];
                }
                RoutePositions.Add(CurrentRoutePos);
                RoutePositions.Reverse();
                NexPositionOnRoute = 1;
                if (NexPositionOnRoute > RoutePositions.Count)
                {
                    
                    NexPositionOnRoute=RoutePositions.Count-1;
                }

                return true;


            }

            Vector3Int New = new Vector3Int();
            List<Vector3Int> NewChecks = new List<Vector3Int>();

            //add surrounding tiles
            if (GridCreator.GameGrid[Current.x + 1, Current.y].Contains != 4 && GridCreator.GameGrid[Current.x + 1, Current.y].Contains != 2) 
            {
                NewChecks.Add(new Vector3Int(Current.x + 1, Current.y, 0));
                // PositionsToCheck.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x + 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x - 1, Current.y].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x - 1, Current.y, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x - 1, CurrentPos.y, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y + 1].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y + 1, 0));
                // AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y + 1, 0));
            }
            if (GridCreator.GameGrid[Current.x, Current.y - 1].Contains == 4)
            {
                NewChecks.Add(new Vector3Int(Current.x, Current.y - 1, 0));
                //  AlreadyAdded.Add(new Vector3Int(CurrentPos.x, CurrentPos.y - 1, 0));
            }
            for (int i = 0; i < NewChecks.Count; i++)
            {
                if (!AlreadyVisited.Contains(NewChecks[i]))
                {
                    ToCheck.Enqueue(NewChecks[i]);
                    AlreadyVisited.Add(NewChecks[i]);
                    CameFrom[NewChecks[i]] = Current;
                }
            }


        }
        Debug.Log("Route could not be set");
        return false;
    }
    public Vector3 GetPosition()
    {
        return Position;
    }
    public int GetCurrentAction()
    {
        return CurrentAction;
    }
    public void SetCurrentAction(int NewAction)
    {
        CurrentAction = NewAction;
    }
    public void SetMovementTarget(Vector3 Target)
    {
        MovementTarget = Target;
    }
    public int GetMoveCounter()
    {
        return MoveCounter;
    }
    public void UpdateCounter()
    {
        MoveCounter++;
    }
    public void ResetCounter()
    {
        MoveCounter = 0;
    }
    public void SetIfTargetIsBuilding(bool Target)
    {
        TargetIsbuilding = Target;
    }
    public bool GetIfTargetIsBuilding()
    {
        return TargetIsbuilding;
    }
    public void ForceLeaveBuidlingOnBuildingRemoval()
    {
        NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
        BuildingCurrentlyTargetting = null;
        SetCurrentAction(-1);
        InBuilding = 0;
        buldingInsideIndex = -1;
        JustLeftBuilding = false;
        JustEnteredBuilding = false;

     //   ResetBuildingData();
    }
    public void SpendTImeInBuilding()
    {
        IncreaseSickness(3);
        InBuilding--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            JustLeftBuilding = true;

        }
    }
    public void ResetBuildingData()
    {
        buldingInsideIndex = -1;
        JustLeftBuilding = false;
        BuildingInsidePos = new Vector3(-1, -1, -1);

    }
    public void AdjustTiredness(int Change)
    {
        TiredNess += Change;
    }
    public int GetTiredNess()
    {
        return TiredNess;
    }
    public void SpendTImeAtHome()
    {
        IncreaseBoredom(1);
        Sickness--;
       // Debug.Log("NPC at home");
        InBuilding--;
        TiredNess--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            if (TiredNess < 0)
            {
                TiredNess = 0;
            }

        }
    }
    public void SpendTImeAtHospital()
    {
        IncreaseBoredom(2);
        Sickness-=2;
        // Debug.Log("NPC at home");
        InBuilding--;
        TiredNess--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            if (TiredNess < 0)
            {
                TiredNess = 0;
            }
            if (Sickness < 0)
            {
                Sickness = 0;
            }

        }
    }
    public void PartakeInEnterainment()
    {
      //  Debug.Log("Partaking in entertainment");
        AdjustBoredom(-1);
        IncreaseSickness(3);
        IncreaseTiredNess();

        // Debug.Log("NPC at home");
        InBuilding--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);
            if (Boredom < 0)
            {
                Boredom = 0;
            }
            if (Boredom < 0)
            {
                Boredom = 0;
            }

        }
    }
    public int GetTimeInBuilding(int LowerBound,int UpperBound)
    {
        return UnityEngine.Random.Range(LowerBound, UpperBound); 
    }
    public void MoveTowardsTargetOnRoute()
    {
        IncreaseBoredom(1);
        IncreaseSickness(2);
        IncreaseTiredNess();

        Debug.Log("Nex position:" + NexPositionOnRoute);
        Debug.Log("Route length" + RoutePositions.Count);
        if(Position.y > RoutePositions[NexPositionOnRoute].y)
        {
            Position.y = Mathf.Max(Position.y - MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        else
        {
            Position.y = Mathf.Min(Position.y + MovementSpeed, RoutePositions[NexPositionOnRoute].y);
        }
        if (Position.x > RoutePositions[NexPositionOnRoute].x)
        {
            Position.x = Mathf.Max(Position.x - MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        else
        {
            Position.x = Mathf.Min(Position.x + MovementSpeed, RoutePositions[NexPositionOnRoute].x);
        }
        NPCSprite.transform.position = Position;
        if (RoutePositions[NexPositionOnRoute].x == Position.x && RoutePositions[NexPositionOnRoute].y == Position.y)
        {
            if(NexPositionOnRoute == RoutePositions.Count - 1)
            {
                //Final target reached
                RoutePositions = new List<Vector3>();
                MovementTarget = new Vector3();
                if (TargetIsbuilding)
                {
                    
                    JustEnteredBuilding = true;
                    SetCurrentAction(1);
                    TargetIsbuilding = false;
                    InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(), BuildingCurrentlyTargetting.GetUpperBound());
                    NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                    if (BuildingCurrentlyTargetting.IsHome)
                    {
                        SetCurrentAction(2);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfIsHospital())
                    {
                        SetCurrentAction(3);
                    }
                    else if (BuildingCurrentlyTargetting.GetIfEntertainment())
                    {
                        SetCurrentAction(4);
                    }
                }
                else
                {
                    SetCurrentAction(-1);
                }

            }
            else
            {
                //Next target
                NexPositionOnRoute++;
            }
        }
    }
    public void MovetowardsTarget()
    {
        IncreaseBoredom(1);
        IncreaseSickness(2);
        IncreaseTiredNess();
     //   Debug.Log("Moving");
        if (Position.y > MovementTarget.y)
        {
            Position.y = Mathf.Max(Position.y - MovementSpeed, MovementTarget.y);
        }
        else
        {
            Position.y = Mathf.Min(Position.y + MovementSpeed, MovementTarget.y);
        }
        if (Position.x > MovementTarget.x)
        {
            Position.x = Mathf.Max(Position.x- MovementSpeed,MovementTarget.x);
        }
        else
        {
            Position.x = Mathf.Min(Position.x+ MovementSpeed, MovementTarget.x);
        }
        NPCSprite.transform.position = Position;
        if(MovementTarget.x==Position.x  && MovementTarget.y == Position.y)
        {
         //   Debug.Log("Arrived at target");
            MovementTarget = new Vector3();
            if (TargetIsbuilding)
            {
                JustEnteredBuilding = true;
                SetCurrentAction(1);
                TargetIsbuilding = false;
                InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(), BuildingCurrentlyTargetting.GetUpperBound());
                NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
                if (BuildingCurrentlyTargetting.IsHome)
                {
                    SetCurrentAction(2);
                }
                else if (BuildingCurrentlyTargetting.GetIfIsHospital())
                {
                    SetCurrentAction(3);
                }
                else if (BuildingCurrentlyTargetting.GetIfEntertainment())
                {
                    SetCurrentAction(4);
                }
            }
            else
            {
                SetCurrentAction(-1);
            }
               

        }
    }
    
}
