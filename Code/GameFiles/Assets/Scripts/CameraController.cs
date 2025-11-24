using System.Collections;
using System.Reflection;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    private Vector2 MoveInput;
    public float Speed = 5f;
    bool Moved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   //     StartCoroutine(WaitOneFrame());
        
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
        transform.position += new Vector3(MoveInput.x, MoveInput.y, 0) * Speed* Time.deltaTime;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }
}
