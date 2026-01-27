using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TransportPlacementScript : MonoBehaviour
{
    public int TransportModeSelected = -1;
    public static  RuleTile TrackTile;
    [SerializeField] private RuleTile TrackTileReference;

    //-1 is not active, 0 is rail
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
        CheckForMouseClick();
    }

}
