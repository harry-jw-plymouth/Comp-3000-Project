using UnityEngine;

public class TransportPlacementScript : MonoBehaviour
{
    public int TransportModeSelected = -1;
    public RuleTile TrackTile;
    //-1 is not active, 0 is rail
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    public void CancelSelection()
    {
        //Reset value so check is not done for placement
        TransportModeSelected = -1;
    }
    public void OnRailButtonClicked()
    {
        //Set transport selection to rail
        Debug.Log("Rail selected");
        TransportModeSelected = 0; 
    }
    public static void PlaceRail(int x,int y)
    {
        Debug.Log("Attempting to place rail at "+ x+", "+y);
    }
    void CheckForMouseClick()
    {
        if(Input.GetMouseButtonDown(0) && TransportModeSelected!=-1)
        {
            if (TransportModeSelected != -1)
            {
                if (TransportModeSelected == 0)
                {
                    Debug.Log("Attempting to place rail");
                }
            }
            else
            {
                Debug.Log("No Mode of transport selected");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckForMouseClick();
    }

}
