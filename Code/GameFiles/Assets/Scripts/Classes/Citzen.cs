using UnityEngine;

public class Citzen
{
    float MovementSpeed = 0.05f;
    int MoveCounter=0;
    int CurrentAction=-1;
    //-1 No action 
    // 0 Moving
    //1 InBuilding
    int InBuilding = 0;
    bool TargetIsbuilding=false;
    Building BuildingCurrentlyTargetting;
    Vector3 MovementTarget=new Vector3(0,0,0);
    Vector3 Position;
    public bool UpdateNeeded;
    GameObject NPCSprite;
    bool IsHomeLess = true;
    Vector3 HomePosition=new Vector3(-1,-1,-1);
    public Citzen(Vector3 Pos,GameObject sprite)
    {
        NPCSprite = sprite;
        Position= Pos;
        UpdateNeeded = true;

    }
    public void SetHomePos(Vector3 Pos)
    {
        HomePosition = Pos;
    }
    public void UpdateHomeStatus(bool NewStatus)
    {
        IsHomeLess = NewStatus;
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
    public void SpendTImeInBuilding()
    {
        InBuilding--;
        if (InBuilding == 0)
        {
            NPCSprite.GetComponent<SpriteRenderer>().enabled = true;
            BuildingCurrentlyTargetting = null;
            SetCurrentAction(-1);

        }
    }
    public int GetTimeInBuilding(int LowerBound,int UpperBound)
    {
        return UnityEngine.Random.Range(LowerBound, UpperBound); 
    }
    public void MovetowardsTarget()
    {
        Debug.Log("Moving");
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
            Debug.Log("Arrived at target");
            MovementTarget = new Vector3();
            if (TargetIsbuilding)
            {
                SetCurrentAction(1);
                TargetIsbuilding = false;
                InBuilding = GetTimeInBuilding(BuildingCurrentlyTargetting.GetLowerBound(),BuildingCurrentlyTargetting.GetUpperBound());
                NPCSprite.GetComponent<SpriteRenderer>().enabled = false;
            }
            else
            {
                SetCurrentAction(-1);
            }
               

        }
    }
    
}
