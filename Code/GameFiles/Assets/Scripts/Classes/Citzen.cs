using UnityEngine;

public class Citzen
{
    float MovementSpeed = 0.05f;
    int MoveCounter=0;
    int CurrentAction=-1;
    //-1 No action 
    // 0 Moving
    Vector3 MovementTarget=new Vector3(0,0,0);
    Vector3 Position;
    public bool UpdateNeeded;
    GameObject NPCSprite;
    public Citzen(Vector3 Pos,GameObject sprite)
    {
        NPCSprite = sprite;
        Position= Pos;
        UpdateNeeded = true;
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
    public void MovetowardsTarget()
    {
        Debug.Log("Moving");
        if (Position.y > MovementTarget.y) {
            Position.y -= MovementSpeed;
        }
        else
        {
            Position.y+= MovementSpeed;
        }
        if (Position.x > MovementTarget.x)
        {
            Position.x -= MovementSpeed;
        }
        else
        {
            Position.x += MovementSpeed;
        }
        NPCSprite.transform.position = Position;
        
    }
    
}
