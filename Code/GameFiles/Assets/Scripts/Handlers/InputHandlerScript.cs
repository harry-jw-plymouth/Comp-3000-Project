using UnityEngine;

public class InputHandlerScript : MonoBehaviour
{
    public UIHandlerScript UiHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInputs();
    }
    void CheckForInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key press");
            UiHandler.OpenPauseMenu();
        }
    }
}
