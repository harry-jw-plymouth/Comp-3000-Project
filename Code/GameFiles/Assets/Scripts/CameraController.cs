using System.Collections;
using System.Reflection;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UIElements.Experimental;

public class CameraController : MonoBehaviour
{
    private Vector2 MoveInput;
    public float Speed = 5f;
    bool Moved = false;

    private Camera GameCamera;

    Vector3 GridMinimum;
    Vector3 GridMaximum;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   //     StartCoroutine(WaitOneFrame());
        GameCamera=GetComponent<Camera>();
        SetBounds();

    }
    public void SetBounds()
    {

        Vector3 BottomLeftPos = GridCreator.GameMap.CellToWorld(new Vector3Int(0, 0, 0));
        Vector3 TopRightPos= GridCreator.GameMap.CellToWorld(new Vector3Int(GridCreator.WIDTH - 1, GridCreator.HEIGHT - 1, 0));

        float CameraHeight = GameCamera.orthographicSize;
        float CameraWidth = CameraHeight * GameCamera.aspect;

        GridMinimum= new Vector3( BottomLeftPos.x+CameraWidth,BottomLeftPos.y+CameraHeight,0);
        GridMaximum = new Vector3(TopRightPos.x - CameraWidth, TopRightPos.y - CameraHeight, 0);
        
    }
    public void CenterCamera()
    {
       
       // Vector3 Center = GridCreator.GameMap.transform.TransformPoint(GridCreator.GameMap.localBounds.center);
        //transform.position = new Vector3(Center.x, Center.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Moved)
        {
            CenterCamera();
            Moved=true;
        }
        MoveCamera();
        
    }
    IEnumerator WaitOneFrame()
    {
        yield return null;
        CenterCamera() ; 
    }
    
    void MoveCamera()
    {
        Vector3 NewPos=transform.position + new Vector3(MoveInput.x, MoveInput.y, 0) * Speed * Time.deltaTime;

        if(NewPos.x > GridMaximum.x)
        {
            NewPos.x = GridMaximum.x;
        }else if (NewPos.x < GridMinimum.x)
        {
            NewPos.x=GridMinimum.x;
        }
        if (NewPos.y > GridMaximum.y)
        {
            NewPos.y = GridMaximum.y;
        }
        else if (NewPos.y < GridMinimum.y)
        {
            NewPos.y = GridMinimum.y;
        }
        transform.position = NewPos;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
}
