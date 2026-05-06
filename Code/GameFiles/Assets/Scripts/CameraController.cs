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
        GameCamera=GetComponent<Camera>();
    }
    // set limits to where the camera can be moved to ensure it cannot go out of bounds
    public void SetBounds()
    {

        Vector3 BottomLeftPos = GridCreator.GameMap.CellToWorld(new Vector3Int(0, 0, 0));
        Vector3 TopRightPos= GridCreator.GameMap.CellToWorld(new Vector3Int(GridCreator.WIDTH - 1, GridCreator.HEIGHT - 1, 0));

        float CameraHeight = GameCamera.orthographicSize;
        float CameraWidth = CameraHeight * GameCamera.aspect;

        GridMinimum= new Vector3( BottomLeftPos.x+CameraWidth,BottomLeftPos.y+CameraHeight,0);
        GridMaximum = new Vector3(TopRightPos.x - CameraWidth, TopRightPos.y - CameraHeight, 0);
        
    }
    // Update is called once per frame
    // update camera position each frame
    void Update()
    {
        if (!Moved)
        {
            SetBounds();
            Moved=true;
        }
        MoveCamera();
        
    }
    // move camera in accordance with player inputs
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
    // get data from the player inputs
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
}
