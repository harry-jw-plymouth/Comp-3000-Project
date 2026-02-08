using UnityEngine;

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
    Vector3 HomePosition=new Vector3(-1,-1,-1);


    int TiredNess = 0;
    int Sickness = 0;
    int Boredom = 0;

    public Citzen(Vector3 Pos,GameObject sprite)
    {
        NPCSprite = sprite;
        Position= Pos;
        UpdateNeeded = true;
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
