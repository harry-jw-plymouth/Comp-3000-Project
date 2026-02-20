using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TransportPlacementScript : MonoBehaviour
{
    public int TransportModeSelected = -1;
    public static  RuleTile TrackTile;
    [SerializeField] private RuleTile TrackTileReference;

    public int RailOption = -1;
    //-1 is not active, 0 is rail, 1 is underground 2 i bus
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        TrackTile = TrackTileReference;
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
    public void OnUnderGroundButtonClicked()
    {
        //Set transport selection to Underground
        Debug.Log("Underground selected");
        TransportModeSelected = 1; 
    }
    public void OnbusButtonClicked()
    {
        //Set transport selection to Bus
        Debug.Log("Bus selected");
        TransportModeSelected = 2;
    }
    public void DoBusClickHandling(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place bus route");
    }
    public void DoUnderGroundClickHandling(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place underground");
    }
    public void DoTransportPlacement(Vector3Int CellClickedPos)
    {
        Debug.Log("attempting to build mode " + TransportModeSelected);
        if (TransportModeSelected == -1)
        {
            Debug.Log("No transport mode selected");
        }
        else if (TransportModeSelected == 0)
        {
            PlaceRail(CellClickedPos);
        }
        else if (TransportModeSelected == 1)
        {
            DoUnderGroundClickHandling(CellClickedPos);
        }
        else if (TransportModeSelected == 2) {
            DoBusClickHandling(CellClickedPos);
        }
    }
    public static void PlaceRail(Vector3Int CellClickedPos)
    {
        Debug.Log("Attempting to place rail at "+ CellClickedPos.x +", "+CellClickedPos.y);

        //Place rail tiles 
        try
        {
            if (GridCreator.GameGrid[CellClickedPos.x, CellClickedPos.y].Contains == 0)
            {
                GridCreator.GameGrid[CellClickedPos.x, CellClickedPos.y].Contains = 2;
                GridCreator.GameMap.SetTile(CellClickedPos, TrackTile);
            }
        }
        catch
        {
            Debug.Log("Click not in grid square");

        }
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
      //  CheckForMouseClick();
    }

}
