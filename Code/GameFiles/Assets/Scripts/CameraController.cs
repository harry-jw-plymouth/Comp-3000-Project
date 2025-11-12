using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.InputSystem;
using Unity.Hierarchy;

public class CameraController : MonoBehaviour
{
    private Vector2 MoveInput;
    public float Speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
        
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
