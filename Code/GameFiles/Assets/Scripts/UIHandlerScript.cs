using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandlerScript : MonoBehaviour
{
    public NPChandler NpcHandler;
    public GridCreator GridHandler;

    public TextMeshProUGUI ReportText;
    public GameObject ReportDisplay;
    public GameObject RatingDisplay;
    public GameObject BuildingsMenuPopUp;
    public GameObject BuildingRemoveButton;
    public GameObject TransportButton;
    public GameObject TransportBuilderPopUp;
    public GameObject PauseCanvas;
    public GameObject TradeCanvas;
    public GameObject MainUICanvas;
    public GameObject StationSelectCanvas;
    public GameObject UpdatesButton;
    public GameObject BusCanvas;
    public Button TradeButton;
    


    public TMP_Dropdown TradeItems;
    public TMP_Dropdown Action;
    public TextMeshProUGUI CostConversionText;
    public TextMeshProUGUI FinalCost;
    public TMP_InputField AmountToBuy;
    public Button TradeConfirm;

    public GameStatusScript gameHandler;
    public PowerHandlerScript powerHandler;
    public TransportPlacementScript TransportHandler;


    public GameObject AlertpopUp;
    public TextMeshProUGUI AlertText;
    
    public static bool TileEditorOn;
    public static bool BusStopEditorOn;
    public static bool TransportPlacementOn=false;
    public static bool BuildingRemoverOn = false;
    public bool TradeMenuActive=false;
    public bool PauseMenuActive = false;
    public bool RailMenuActive = false;
    public bool RouteMenuActive = false;

    public GameObject BuildingInfoBox;
    public TextMeshProUGUI BuildingTypeText;
    public TextMeshProUGUI BuildingMoneyText;
    public TextMeshProUGUI BuildingPowerText;
    public TextMeshProUGUI BuildingSpeificText;
    public TextMeshProUGUI BuildingIsPoweredText;
    public TextMeshProUGUI BuildingEnviromentalValue;
    public TextMeshProUGUI RouteStartInfo;
    public TextMeshProUGUI RouteEndInfo;

    public TextMeshProUGUI RouteTypeInfo;

    public TextMeshProUGUI StartingBusStopInfo;
    public TextMeshProUGUI EndBusStopInfo;
    Vector3Int StartBusStop;
    Vector3Int EndBusStop;

   // public Button PlaceRailButton;
    public GameObject RailCanvas;
    public GameObject RouteSetCanvas;
    public GameObject BusRouteSetCanvas;
    public bool BuildingInfoShowing = false;
    public bool SelectingRouteLocation = false;
    public bool BusCanvasActive = false;
    public bool BusRouteCanvasActive = false;

    public bool RouteIsForBus=false;

    public int StartRouteStationBuilding = -1;
    public int EndRouteStationBuilding = -1;

    int RouteStationPos = -1;
    //  public Square[,] GameGrid;
    private void Start()
    {
        TileEditorOn = false ;
    //  GameGrid = new Square[GridCreator.WIDTH, GridCreator.HEIGHT];
    //   SetGrid();
}
    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBusRouteConfirmButtonClicked()
    {
        if (StartBusStop != null && EndBusStop != null)
        {
            if (StartBusStop != EndBusStop)
            {
                Debug.Log("Making new bus route");
                BusRoute NewBusRoute = new BusRoute(StartBusStop,EndBusStop);
                NewBusRoute.SetRoute(GridCreator.GameGrid);
                TransportPlacementScript.AddBusRoute(NewBusRoute);
            }
        }
    }
    public void OnRouteConfirmButtonClicked()
    {
        if(StartRouteStationBuilding!=-1&& EndRouteStationBuilding != -1)
        {
            Debug.Log("Making new route");
            Route NewRoute = new Route(GridCreator.PlacedBuildings[StartRouteStationBuilding], GridCreator.PlacedBuildings[EndRouteStationBuilding]);
            NewRoute.SetRoute(GridCreator.GameGrid);
            TransportPlacementScript.AddRoute(NewRoute);
            //  List<Vector3Int> test = NewRoute.GetCurrentRoute();
            //  Debug.Log("Route made");
            // Debug.Log("Route length: " + test.Count);
            // for(int i=0; i<test.Count; i++)
            //{
            ///      Debug.Log("X:" + test[i].x + ", Y:"+ test[i].y);
            //  }


        }
        else
        {
            Debug.Log("Proper selection not made");
        }
    }
    public void OnStationSelectorBackButtonClicked() {
        MainUICanvas.SetActive(true);
        StationSelectCanvas.SetActive(false);
        SelectingRouteLocation=false;
        if (RouteIsForBus)
        {
            BusRouteSetCanvas.SetActive(true);
            BusRouteCanvasActive = true;
        }
        else
        {
            RouteSetCanvas.SetActive(true);
        }
       
    }
    public void OnRouteStartButtonClick()
    {
        Debug.Log("Selecting route start");
        if (GridCreator.GetIfTrainStationExists())
        {
            Debug.Log("Selecting start");
            RouteSetCanvas.SetActive(false);
            MainUICanvas.SetActive(false);
            RailCanvas.SetActive(false);
            StationSelectCanvas.SetActive(true);
            SelectingRouteLocation = true;
            RouteStationPos = 0;
            //0 = first pos of route
            RouteTypeInfo.text = "Select train station";

        }
        else
        {
            Debug.Log("No train station found");
            ShowAlertPopUp("No train stations for route");
       
        }
    }
    public void OnBusRouteStartButtonClicked()
    {
        Debug.Log("Selecting route start");
        if (GridHandler.GetIfBusStopExists())
        {
            BusRouteSetCanvas.SetActive(false);
            MainUICanvas.SetActive(false);
            StationSelectCanvas.SetActive(true);
            SelectingRouteLocation = true;
            RouteIsForBus=true;
            RouteStationPos=0;
            RouteTypeInfo.text = "Select bus stop";
        }

    }
    public void OnBusRouteEndButtonClicked()
    {
        Debug.Log("Selecting route start");
        if (GridHandler.GetIfBusStopExists())
        {
            BusRouteSetCanvas.SetActive(false);
            MainUICanvas.SetActive(false);
            StationSelectCanvas.SetActive(true);
            SelectingRouteLocation = true;
            RouteIsForBus = true;
            RouteStationPos = 1;
            RouteTypeInfo.text = "Select bus stop";
        }
    }
   
    public void OnTrainStationClicked(Vector3Int CellPos, int BuildingPos)
    {
        //Check where on the route this station is
        if (RouteStationPos == 0)
        {
            //re display UI
            OnStationSelectorBackButtonClicked();
            RouteStartInfo.text = "Station at " + CellPos.x + "," + CellPos.y;
            StartRouteStationBuilding = BuildingPos;
            Debug.Log("Start route:" + StartRouteStationBuilding);
        }
        else if (RouteStationPos == 1)
        {
            if (TransportHandler.GetIfLinkBetweenStations(StartRouteStationBuilding,BuildingPos)&&BuildingPos!=StartRouteStationBuilding)
            {
                // re display UI
                OnStationSelectorBackButtonClicked();
                RouteEndInfo.text = "Station at " + CellPos.x + "," + CellPos.y;
                EndRouteStationBuilding = BuildingPos;
                Debug.Log("End route:" + EndRouteStationBuilding);
            }
        }
    }
    public void OnBusStopClicked(Vector3Int CellPos)
    {
        OnStationSelectorBackButtonClicked();
        RouteIsForBus = false;
        if (RouteStationPos == 0)
        {
            StartingBusStopInfo.text="Bus stop at " + CellPos.x + "," + CellPos.y;
            StartBusStop = CellPos;

        }
        else if(RouteStationPos == 1)
        {
            EndBusStopInfo.text = "Bus stop at " + CellPos.x + "," + CellPos.y;
            EndBusStop = CellPos;
        }

    }
    public void OnRouteEndButtonClicked()
    {
        Debug.Log("Selecting route end");
        if (GridCreator.GetIfTrainStationExists())
        {
            Debug.Log("Selecting end");
            RouteSetCanvas.SetActive(false);
            MainUICanvas.SetActive(false);
            RailCanvas.SetActive(false);
            StationSelectCanvas.SetActive(true);
            SelectingRouteLocation = true;
            RouteStationPos = 1;
            //1 = end pos of route
            RouteTypeInfo.text = "Select train station";
        }
        else
        {
            ShowAlertPopUp("No train stations for route");
        }
    }

    public void OnRouteMenuCloseButtonClicked()
    {
        RouteSetCanvas.SetActive(false);
        RouteMenuActive = false ;
    }
    public void OnTrainRouteButtonClicked()
    {
        OpenRouteCanvas();
    }
    public void OpenRouteCanvas()
    {
        RouteMenuActive= true ;
        RouteSetCanvas.SetActive(true);
    }
    public void CloseBusCanvas()
    {
        BusCanvasActive = false;
        BusCanvas.SetActive(false);
    }
    public void OnBusStopButtonClicked()
    {
        if (BusStopEditorOn)
        {
            TileEditorOn = false;
            BusStopEditorOn = false; 
        }
        else
        {
            TileEditorOn= true;
            BusStopEditorOn=true;
        }

    }
    public void StopEditingBusStops()
    {
        TileEditorOn = false;
        BusStopEditorOn = false;
    }
    public void OnBusrouteCanvasButtonClicked()
    {
        BusRouteCanvasActive = true;
        BusRouteSetCanvas.SetActive(true);
        BusCanvas.SetActive(false);
        BusCanvasActive= false;
        StopEditingBusStops();

    }
    public void OnBusRouteCanvasCloseButtonClosed()
    {
        BusRouteCanvasActive = false;
        BusRouteSetCanvas.SetActive(false);
        BusCanvas.SetActive(true);
        BusCanvasActive = true;
    }
    public void OnBusButtonClicked()
    {
        if (BusCanvasActive)
        {
            CloseBusCanvas();
            TransportBuilderPopUp.SetActive(true);
            
        }
        else
        {
            CloseRailPopUp();
            BusCanvasActive = true;
            BusCanvas.SetActive(true);
            TransportBuilderPopUp.SetActive(false);
        }
    }
    public void DisplayBuildingInfo(PlacedBuilding BuildingToDisplay)
    {
        BuildingInfoBox.SetActive(true);
        BuildingInfoShowing = true;
        BuildingTypeText.text = BuildingToDisplay.buildingType.Name;
        BuildingMoneyText.text=BuildingToDisplay.buildingType.TaxGeneration.ToString()+" money per cycle";
        BuildingPowerText.text = BuildingToDisplay.buildingType.PowerUsage.ToString() + " power per cycle";
        BuildingEnviromentalValue.text="Environment value:"+BuildingToDisplay.GetEnviromentalValue().ToString();
        if (BuildingToDisplay.GetIfInRangeOfPowerPlant())
        {
            BuildingIsPoweredText.text = "Powered";
        }
        else
        {
            BuildingIsPoweredText.text = " Not powered";
        }
        if(BuildingToDisplay.buildingType is PowerPlant powerPlant)
        {
            BuildingSpeificText.text = "Generating " + powerPlant.GetPowerGeneration() + " power";
        }
        else if (BuildingToDisplay.buildingType is Home home)
        {
            BuildingSpeificText.text = "Home to " + home.CurrentResidents + " people";
        }
        else
        {
            BuildingSpeificText.text = " building is a " + BuildingToDisplay.GetType();
        }


    }
    public void HideBuildingInfo()
    {
        BuildingInfoShowing = false;
        BuildingInfoBox.SetActive(false);
        
    }

    public void OpenPauseMenu()
    {
        if (PauseMenuActive)
        {
            PauseCanvas.SetActive(false);
            PauseMenuActive = false;
        }
        else
        {
            PauseMenuActive = true;
            PauseCanvas.SetActive(true);
        }
    }
    double ElectricityPrice=1.5;
    int PurchaseAmount = -1;
    public void OnEditMadeToNumberToTrade()
    {      
        Debug.Log("Edit made");
       
        if (AmountToBuy.text != "")
        {
            int TradeType = Action.value;
            PurchaseAmount = int.Parse(AmountToBuy.text);
            if (TradeType == 0)
            {
                FinalCost.text = "Final cost:" + PurchaseAmount * ElectricityPrice;
            }
            else
            {
                FinalCost.text = "Money gained from selling: " + PurchaseAmount / ElectricityPrice;
            }
        }
       
       
    }
    public void SetTradeInfo()
    {
        string MaterialSelectedToBuy = TradeItems.options[TradeItems.value].text;
        //0=buy, 1=sell
        int action = Action.value;
        // set trade cost
        CostConversionText.text = "Money cost per " + MaterialSelectedToBuy + " : " + 1.5f;
        

    }
    public void OnConfirmTradeButtonClicked()
    {
        int action = Action.value;
        if (AmountToBuy.text != "")
        {
            PurchaseAmount = int.Parse(AmountToBuy.text);
            if (action == 0)
            {
                //buying 
                int FinalCost = (int)(PurchaseAmount * ElectricityPrice);
                if (gameHandler.CheckIfPurchaseAffordable(FinalCost)){
                    gameHandler.AdjustMoney(-(FinalCost));
                    powerHandler.AdjustPower(PurchaseAmount);
                    ShowAlertPopUp("purhased "+FinalCost+" for "+PurchaseAmount+" electricity");
                    AmountToBuy.text = "";
                }
                else
                {
                    ShowAlertPopUp("Not enough money");
                }
                
            }
            else
            {
                //selling
                int FinalElectricityCost = (int)(PurchaseAmount / ElectricityPrice);
                if (powerHandler.GetIfEnoughPowerForSell(FinalElectricityCost))
                {
                    powerHandler.AdjustPower(-(FinalElectricityCost));
                    gameHandler.AdjustMoney(PurchaseAmount);
                    AmountToBuy.text = "";
                    ShowAlertPopUp("sold " + FinalElectricityCost + " for " + PurchaseAmount + " money");
                }
                else
                {
                    ShowAlertPopUp("Not enough power");
                }
            }
        }
        else
        {
            ShowAlertPopUp("Please enter a value ");
        }
    }
    public void OnTradeButtonClicked()
    {
        Debug.Log("TradeButton clicked");
        if (TradeMenuActive)
        {
            TradeMenuActive = false;
            TradeCanvas.SetActive(false);
        }
        else
        {
            TradeMenuActive = true;
            TradeCanvas.SetActive(true);
            SetTradeInfo();
        }
    }
    public  void ShowAlertPopUp(string Alert)
    {
        AlertpopUp.SetActive(true);
        AlertText.text = Alert;
    }
    public void OnAlertPopupButtonClicked()
    {
        AlertpopUp.SetActive(false);
    }
    public void OnSaveButtonClicked()
    {
        Debug.Log("Save button clicked");
        DBManager.UpdateSave(NpcHandler.GetCurrentNumberOfNPCs(), MainMenu.CurrentSaveID);
        DBManager.UpdateMapSave(MainMenu.CurrentSaveID, GridCreator.WIDTH, GridCreator.HEIGHT, GridCreator.GameGrid);
        DBManager.AddAllBuildingsForSave(MainMenu.CurrentSaveID, GridCreator.PlacedBuildings);
        DBManager.UpdateTrainRoutesForSave(MainMenu.GameSaveID, TransportPlacementScript.TrainRoutes);
    }
    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked");
        SceneManager.LoadScene("MainMenu");
    }
    void SetUIInactive()
    {
      //  TransportButton.SetActive(false);
        BuildingsMenuPopUp.SetActive(false);
        TransportBuilderPopUp.SetActive(false);
        BuildingsListManager.BuildingCurrentlySelected = -1;
        BuildingRemoveButton.SetActive(false);

        TileEditorOn = false;
        TransportPlacementOn = false;
        BuildingRemoverOn=false;

    }
    public void OnRatingClicked()
    {
        Debug.Log(" rating section clicked");
        ReportDisplay.SetActive(true);
        UpdatesButton.SetActive(false);
        List<string> Updates = GameStatusScript.GetReport();
        string Info = "";
        Debug.Log("Number of updates:"+Updates.Count);
        for(int i = 0; i < Updates.Count; i++)
        {
            Debug.Log("Update[" + i + "]:" + Updates[i]);
            Info += Updates[i] + "\n";
        }
        ReportText.text = Info;

    }
    public void OnReportClicked()
    {
        UpdatesButton.SetActive(true);
        Debug.Log(" report section clicked");
        ReportDisplay.SetActive(false);
    }
    public void OnRoadButtonClicked()
    {
        BusStopEditorOn = false;
        Debug.Log("Road button clicked");
        SetUIInactive();
        if (TileEditorOn)
        {
            TileEditorOn = false;
        }
        else
        {
            CloseBusCanvas();
            CloseTransportPopup();
            TileEditorOn= true;
        }
    }
    public void OnTransportButtonClicked()
    {
        Debug.Log("Transport button clicked");
        SetUIInactive();
        RailCanvas.SetActive(false);
        if (TransportPlacementOn)
        {
            CloseTransportPopup();
            CloseBusCanvas();
        }
        else
        {
            CloseBusCanvas();
            TransportPlacementOn = true;
            TransportBuilderPopUp.SetActive(true);
        }
    }
    public void OnRailButtonClicked()
    {
        if (RailMenuActive)
        {
            RailMenuActive = false;
            RailCanvas.SetActive(false);
            TransportBuilderPopUp.SetActive(true);
        }
        else
        {
            CloseBusCanvas();
            RailMenuActive = true;
            RailCanvas.SetActive(true);
            TransportBuilderPopUp.SetActive(false);
        }
    }
    public void CloseTransportPopup()
    {
        TransportBuilderPopUp.SetActive(false);
        StartRouteStationBuilding = -1;
        EndRouteStationBuilding = -1;
        TransportPlacementOn = false;
        CloseRailPopUp();
    }
    public void CloseRailPopUp()
    {
        RailCanvas.SetActive(false);
    }
    public void OnBuildingsButtonClick()
    {
        SetUIInactive();
        TileEditorOn = false;
        Debug.Log("Building button clicked");
        if (BuildingsMenuPopUp.activeInHierarchy)
        {
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn= false;
        }
        else
        {
            CloseBusCanvas();
            CloseTransportPopup();
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoveButton.SetActive(true);
        }
            
    }
    public void OnBuildingRemoveButtonClick()
    {
        SetUIInactive();
        TransportBuilderPopUp.SetActive(false);

        if (BuildingRemoverOn)
        {
            Debug.Log("Building remover off");
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoverOn = false;

        }
        else
        {         
            Debug.Log("Building remover on");
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn = true;
        }

    }
}

