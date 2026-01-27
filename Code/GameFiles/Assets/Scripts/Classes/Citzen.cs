using UnityEngine;

public class Citzen
{
    int CurrentAction=-1;
    //-1 No action 
    // 0 Moving
    Vector3 MovementTarget=new Vector3(0,0,0);
    Vector3 Position;
    public bool UpdateNeeded;
    public Citzen(Vector3 Pos)
    {
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
    
}
