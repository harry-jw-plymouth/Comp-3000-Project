using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandlerScript : MonoBehaviour
{
    public NPChandler NpcHandler;
    public GridCreator GridHandler;
    public UIHandlerScript uiHandler;
    public SoundManagerScript SoundManager;

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
    public GameObject SettingsCanvas;
    public Button TradeButton;

    public TextMeshProUGUI BusRouteInfoText;
    public TextMeshProUGUI TrainRouteInfoText;

    public GameObject BuildingCoreButton;
    public GameObject TransportCoreButton;
    public GameObject LayoutCoreButton;

    public GameObject NewPopUpCanvas;
    public TextMeshProUGUI PopUpTitle;
    public TextMeshProUGUI PopUpDescription;
    


    public TMP_Dropdown TradeItems;
    public TMP_Dropdown Action;
    public TextMeshProUGUI CostConversionText;
    public TextMeshProUGUI FinalCost;
    public TMP_InputField AmountToBuy;
    public Button TradeConfirm;

    public GameStatusScript gameHandler;
    public PowerHandlerScript powerHandler;
    public TransportPlacementScript TransportHandler;
    public SoundManagerScript SoundHandler;
    public ColourBlindCameraController ColourBlindHandler;


    public GameObject AlertpopUp;
    public TextMeshProUGUI AlertText;
    
    public static bool TileEditorOn;
    public static bool GreeneryEditorOn;
    public static bool WaterEditorOn=false;
    public static bool BusStopEditorOn;
    public static bool TransportPlacementOn=false;
    public static bool BuildingRemoverOn = false;
    public bool TradeMenuActive=false;
    public bool PauseMenuActive = false;
    public bool RailMenuActive = false;
    public bool RouteMenuActive = false;
    public bool SettingsMenuActive = false;

    public bool TrainRouteViewerOn = false;


    public GameObject NewBuidingInfoBox;
    public GameObject BuildingInfoBox;
    public TextMeshProUGUI BuildingTypeText;
    public TextMeshProUGUI BuildingMoneyText;
    public TextMeshProUGUI BuildingPowerText;
    public TextMeshProUGUI BuildingSpeificText;
    public TextMeshProUGUI BuildingIsPoweredText;
    public TextMeshProUGUI BuildingEnviromentalValue;
    public TextMeshProUGUI RouteStartInfo;
    public TextMeshProUGUI RouteEndInfo;

    public GameObject RouteSelectedInfo;
    public TextMeshProUGUI StartStationForRoute;
    public TextMeshProUGUI EndStationForRoute;

    public TextMeshProUGUI RouteTypeInfo;

    public TextMeshProUGUI StartingBusStopInfo;
    public TextMeshProUGUI EndBusStopInfo;

    public TextMeshProUGUI RouteDisplayStartingBusStopInfo;
    public TextMeshProUGUI RouteDisplayEndBusStopInfo;

    public Slider MusicVolumeSlider;
    public TMP_Dropdown ColourBlindModeDropdown;

    public GameObject TilePlaceCanvas;
    public bool TilePlaceCanvasActive = false; 




    Vector3Int StartBusStop;
    Vector3Int EndBusStop;

   // public Button PlaceRailButton;
    public GameObject RailCanvas;
    public GameObject RailDisplayCanvas;
    public GameObject RouteSetCanvas;
    public GameObject BusRouteSelectedInfo;

    public GameObject BusRouteDisplayCanvas;
    public GameObject BusRouteSetCanvas;
    public bool BuildingInfoShowing = false;
    public bool SelectingRouteLocation = false;
    public bool BusCanvasActive = false;
    public bool RailRouteDisplayCanvasActive = false;
    public bool BusRouteDisplayCanvasActive = false;
    public bool BusRouteCanvasActive = false;

    public bool BusRouteViewerOn = false;

    public bool RouteIsForBus=false;

    public int StartRouteStationBuilding = -1;
    public int EndRouteStationBuilding = -1;

    public List<Route> CurrentRoutes=new List<Route>();
    public List<BusRoute> CurrentBusRoutes = new List<BusRoute>();
    public int RouteDisplayIndex = -1;
    public int BusRouteDisplayIndex = -1;

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
    public void CloseTilePlaceCanvas()
    {
        TilePlaceCanvasActive = false;
        TilePlaceCanvas.SetActive(false);
        TileEditorOn = false;
    }
    public void OnCancelPlacementButtonClicked()
    {
        SoundManager.PlayButtonClick();
        TileEditorOn= false;
    }
    public void OpenNewPopUp(string Title, string Text)
    {
       // Debug.Log("Displaying Pop up");
        NewPopUpCanvas.SetActive(true);
        PopUpTitle.text = Title;
        PopUpDescription.text = Text;
    }
    public void OnNewPopUpClosed()
    {
        NewPopUpCanvas.SetActive(false);
    }
    public void OnLayoutButtonClick()
    {
        SoundManager.PlayButtonClick();
        if (TilePlaceCanvasActive)
        {
            CloseTilePlaceCanvas();
        }
        else
        {
            //SetUIInactive();
            TilePlaceCanvasActive = true;
            TilePlaceCanvas.SetActive(true);

            BuildingInfoBox.SetActive(false);
            BuildingInfoShowing = false;
            BuildingsMenuPopUp.SetActive(false);
            TransportBuilderPopUp.SetActive(false);
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoveButton.SetActive(false);

            TileEditorOn = false;
            TransportPlacementOn = false;
            BuildingRemoverOn = false;

            CloseTransportPopup();
            CloseBusCanvas();
            
        }

    }
    public void DisplayRoutes(List<Route> RoutesToDisplay)
    {
        CurrentRoutes = RoutesToDisplay;
        if (RoutesToDisplay.Count != 0)
        {
            RouteDisplayIndex = 0;
            StartStationForRoute.text = "Start: "+RoutesToDisplay[0].StartStation.GetBuildingPosAsInt();
            EndStationForRoute.text = "End:" + RoutesToDisplay[0].EndStation.GetBuildingPosAsInt();
        }
    }
    public void DisplayBusRoutes(List<BusRoute> RoutesToDisplay)
    {
        CurrentBusRoutes = RoutesToDisplay;
        if (RoutesToDisplay.Count != 0)
        {
            BusRouteDisplayIndex = 0;
            RouteDisplayStartingBusStopInfo.text = "Start: " + RoutesToDisplay[0].StartStop;
            RouteDisplayEndBusStopInfo.text = "End:" + RoutesToDisplay[0].EndStop;
        }
    }
    public void OnCancelRouteButtonClick()
    {
        SoundManager.PlayButtonClick();
        if (RouteDisplayIndex == -1) 
        {
            return;
        }
        GridHandler.DeHighlightAllRoutes();
        CurrentRoutes[RouteDisplayIndex].SetCancelled();
        ShowAlertPopUp("Route being cancelled. Cancellation will occur at the next station the train stops at");

        RouteDisplayIndex = -1;
    }
    public void OnCancelBusRouteButtonClick()
    {
        SoundManager.PlayButtonClick();
        if (BusRouteDisplayIndex==-1)
        {
            ShowAlertPopUp("No bus route selected");
            return;
        }

        GridHandler.DeHighlightAllBusRoutes();
        CurrentBusRoutes[BusRouteDisplayIndex].SetCancelled();
        ShowAlertPopUp("Route being cancelled. Cancellation will occur at the next stop the bus stops at");

        BusRouteDisplayIndex = -1;
    }
   
    public void OnBusRouteConfirmButtonClicked()
    {
        SoundManager.PlayButtonClick();
        if (StartBusStop != null && EndBusStop != null)
        {
            if (StartBusStop != EndBusStop)
            {
                Debug.Log("Making new bus route");
                BusRoute NewBusRoute = new BusRoute(StartBusStop,EndBusStop);
                if (NewBusRoute.GetIfPathBetweenBusStops(StartBusStop, EndBusStop))
                {
                    if(gameHandler.CheckIfPurchaseAffordable(50))
                    {
                        gameHandler.AdjustMoney(-50);
                        NewBusRoute.SetRoute(GridCreator.GameGrid);
                        TransportPlacementScript.AddBusRoute(NewBusRoute);
                        OpenNewPopUp("Bus route created", "New bus route created between stop at " + StartBusStop + " and stop at " + EndBusStop + ". ");
                        OnBusRouteCanvasCloseButtonClosed();
                    }
                    else
                    {
                        OpenNewPopUp("Not enough money to create bus route", "You need 50 money to create a bus route.");
                        return;
                    }
                    
                }
                else
                {
                    uiHandler.OpenNewPopUp("Cant create bus route", "no route between stops");
                    Debug.Log("Couldnt set bus route, no path");
                }
               
            }
        }
    }
    public void OnSettingsButtonClicked()
    {
        SoundManager.PlayButtonClick();
        if (SettingsMenuActive)
        {
            SettingsCanvas.SetActive(false);
            SettingsMenuActive = false;
            ShowCoreUI();
        }
        else
        {
            PauseCanvas.SetActive(false);
            SettingsCanvas.SetActive(true);
            SettingsMenuActive = true;
            SetUIInactive();
            HideCoreUI();
            CloseAllRegularPopUps();
            CloseAllTransportPopUps();
            TradeMenuActive = false;
            TradeCanvas.SetActive(false);


        }
    }
    public void OnMusicVolumeSliderAdjusted()
    {
        float Value=MusicVolumeSlider.value;
        SoundHandler.ChangeMusicVolume(Value);
    }
    public void OnColourBlindModeDropdownChanged()
    {
        Debug.Log("Colour blind mode dropdown changed");
        int index = ColourBlindModeDropdown.value;
        ColourBlindHandler.SetMode(index);
    }

    public void OnDisplayBusRoutesButtonClicked()
    {
        SoundManager.PlayButtonClick();
        BusRouteDisplayIndex = -1;
        if (BusRouteDisplayCanvasActive)
        {
            //close
            BusRouteDisplayCanvasActive = false;
            BusRouteDisplayCanvas.SetActive(false);
            MainUICanvas.SetActive(true);
            BusRouteSetCanvas.SetActive(true);
            BusRouteViewerOn = false;
            RouteSelectedInfo.SetActive(false);
            GridHandler.DeHighlightAllBusRoutes();
            

        }
        else
        {
            BusRouteDisplayCanvasActive = true;
            BusRouteDisplayCanvas.SetActive(true);
            MainUICanvas.SetActive(false);
            BusRouteSetCanvas.SetActive(false);
            BusCanvas.SetActive(false);
            BusRouteSelectedInfo.SetActive(true);
            BusRouteViewerOn = true;
            TransportBuilderPopUp.SetActive(false);
        }
    }
    public void OnDisplayRoutesButtonClicked()
    {
        SoundManager.PlayButtonClick();
        RouteDisplayIndex = -1;
        if (RailRouteDisplayCanvasActive)
        {
            //close
            RailRouteDisplayCanvasActive= false;
            RailDisplayCanvas.SetActive(false);
            MainUICanvas.SetActive(true);
            RouteSetCanvas.SetActive(true);
            TrainRouteViewerOn = false;
            RouteSelectedInfo.SetActive(false);
            GridHandler.DeHighlightAllRoutes();
            

        }
        else
        {
            RailRouteDisplayCanvasActive = true;
            RailDisplayCanvas.SetActive(true);
            MainUICanvas.SetActive(false);
            RouteSetCanvas.SetActive(false);
            RailCanvas.SetActive(false);
            RouteSelectedInfo.SetActive(true);
            TrainRouteViewerOn = true;
            TransportBuilderPopUp.SetActive(false);
        }
    }

    public void OnRouteConfirmButtonClicked()
    {
        SoundManager.PlayButtonClick();
        if (StartRouteStationBuilding!=-1&& EndRouteStationBuilding != -1)
        {
            Debug.Log("Making new route");
            if (gameHandler.CheckIfPurchaseAffordable(100))
            {
                Route NewRoute = new Route(GridCreator.PlacedBuildings[StartRouteStationBuilding], GridCreator.PlacedBuildings[EndRouteStationBuilding]);
                NewRoute.SetRoute(GridCreator.GameGrid);

                if (NewRoute.GetRouteLength() > 1)
                {
                    TransportPlacementScript.AddRoute(NewRoute);
                    OpenNewPopUp("Route created", "New route created between station at " + GridCreator.PlacedBuildings[StartRouteStationBuilding].GetBuildingPosAsInt() + " and station at " + GridCreator.PlacedBuildings[EndRouteStationBuilding].GetBuildingPosAsInt() + ". ");
                    gameHandler.AdjustMoney(-100);
                    OnRouteMenuCloseButtonClicked();
                }
                else
                {
                    TrainRouteInfoText.text="Could not create route, route too short";
                }
            }
            else
            {
                TrainRouteInfoText.text = "Could not create route, not enough money";
            }
        }
        else
        {
            TrainRouteInfoText.text = "Could not create route, please select a valid start and end point then try again";
        }
    }
    public void OnStationSelectorBackButtonClicked() {
        SoundManager.PlayButtonClick();
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
        SoundManager.PlayButtonClick();
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
            TrainRouteInfoText.text = "No stations found for route, build more stations then try again";
        }
    }
    public void OnBusRouteStartButtonClicked()
    {
        SoundManager.PlayButtonClick();
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
        else
        {
            BusRouteInfoText.text = "No stop exists\r\nPlease place bus stop before attempting to set route";
        }

    }
    public void OnBusRouteEndButtonClicked()
    {
        SoundManager.PlayButtonClick();
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
        SoundManager.PlayButtonClick();
        //Check where on the route this station is
        if (RouteStationPos == 0)
        {
            //re display UI
            OnStationSelectorBackButtonClicked();
            RouteStartInfo.text = "Station at " + CellPos.x + "," + CellPos.y;
            StartRouteStationBuilding = BuildingPos;
        }
        else if (RouteStationPos == 1)
        {
            if (TransportHandler.GetIfLinkBetweenStations(StartRouteStationBuilding,BuildingPos)&&BuildingPos!=StartRouteStationBuilding)
            {
                // re display UI
                OnStationSelectorBackButtonClicked();
                RouteEndInfo.text = "Station at " + CellPos.x + "," + CellPos.y;
                EndRouteStationBuilding = BuildingPos;
            }
        }
    }
    public void OnBusStopClicked(Vector3Int CellPos)
    {
        SoundManager.PlayButtonClick();
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
        SoundManager.PlayButtonClick();
        if (GridCreator.GetIfTrainStationExists())
        {
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
            TrainRouteInfoText.text = "No train stations found, build more before setting a route";
        }
    }
    public void ShowTrainRouteUI()
    {
        RailMenuActive = true;
        RailCanvas.SetActive(true);
    }
    public void ShowTransportUI()
    {
        TransportBuilderPopUp.SetActive(true);
    }

    public void OnRouteMenuCloseButtonClicked()
    {
        SoundManager.PlayButtonClick();
        RouteSetCanvas.SetActive(false);
        RouteMenuActive = false ;

        ShowCoreUI();
        ShowTrainRouteUI();
        ShowTransportUI();

    }
    public void OnTrainRouteButtonClicked()
    {
        SoundManager.PlayButtonClick();
        OpenRouteCanvas();
        HideCoreUI();
        SetUIInactive();
        HideBuildingInfo();
        RailMenuActive = false;
        RailCanvas.SetActive(false);

        TrainRouteInfoText.text = "Select the start and end station on the route and then click confirm. \r\nSetting up a route costs 10 coins";
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
        SoundManager.PlayButtonClick();
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
        SoundManager.PlayButtonClick();
        HideCoreUI() ;
        SetUIInactive();

        BusRouteCanvasActive = true;
        BusRouteSetCanvas.SetActive(true);
        BusCanvas.SetActive(false);
        BusCanvasActive= false;
        StopEditingBusStops();

        StartingBusStopInfo.text = "";
        EndBusStopInfo.text = "";
        StartBusStop = new Vector3Int(-1, -1, -1);
        EndBusStop = new Vector3Int(-1, -1, -1);
        BusRouteInfoText.text = "Set a start and end point for your route then confirm to create your route!";

    }
    public void OnBusRouteCanvasCloseButtonClosed()
    {
        SoundManager.PlayButtonClick();
        BusRouteCanvasActive = false;
        BusRouteSetCanvas.SetActive(false);
        BusCanvas.SetActive(true);
        BusCanvasActive = true;

        ShowCoreUI();
        ShowTransportUI();
        BusCanvas.SetActive(true);




    }
    public void OnBusButtonClicked()
    {
        SoundManager.PlayButtonClick();
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
          //  TransportBuilderPopUp.SetActive(false);
        }
    }
    public void DisplayBuildingInfo(PlacedBuilding BuildingToDisplay)
    {
        NewBuidingInfoBox.SetActive(true);

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
   
    public void DisplayBuildingInfoAtSpecificPos(PlacedBuilding BuildingToDisplay, Vector3Int PosToShow)
    {
        NewBuidingInfoBox.SetActive(true);

        BuildingInfoBox.SetActive(true);
        BuildingInfoShowing = true;
        BuildingTypeText.text = BuildingToDisplay.buildingType.Name;
        BuildingMoneyText.text = BuildingToDisplay.buildingType.TaxGeneration.ToString() + " money per cycle";
        BuildingPowerText.text = BuildingToDisplay.buildingType.PowerUsage.ToString() + " power per cycle";
        BuildingEnviromentalValue.text = "Environment value:" + BuildingToDisplay.GetEnviromentalValue().ToString();
        if (BuildingToDisplay.GetIfInRangeOfPowerPlant())
        {
            BuildingIsPoweredText.text = "Powered";
        }
        else
        {
            BuildingIsPoweredText.text = " Not powered";
        }
        if (BuildingToDisplay.buildingType is PowerPlant powerPlant)
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

        var NewInfoRect = NewBuidingInfoBox.GetComponent<RectTransform>();
        var infoRect = BuildingInfoBox.GetComponent<RectTransform>();
       // infoRect.position=GridCreator.GameMap.CellToWorld(PosToShow)+ new Vector3(0.5f, 0.5f, 0);
        Vector3 worldPos = GridCreator.GameMap.CellToWorld(PosToShow) + new Vector3(0.5f, 1.5f, 0);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

   


        //infoRect.position = screenPos;
        //NewInfoRect.position = screenPos;
    }

    public void HideBuildingInfo()
    {
        BuildingInfoShowing = false;
        BuildingInfoBox.SetActive(false);
        NewBuidingInfoBox.SetActive(false);

    }

    public void OpenPauseMenu()
    {
        if (PauseMenuActive)
        {
            PauseCanvas.SetActive(false);
            PauseMenuActive = false;
            ShowCoreUI();
        }
        else
        {
            TradeMenuActive = false;
            TradeCanvas.SetActive(false);
            PauseMenuActive = true;
            PauseCanvas.SetActive(true);
            SetUIInactive();
            CloseTransportPopup();
            HideCoreUI();
        }
    }
    double ElectricityPrice=1.5;
    int PurchaseAmount = -1;
    public void OnEditMadeToNumberToTrade()
    {      
       // Debug.Log("Edit made");
       
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
        SoundManager.PlayButtonClick();
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
    public void CloseAllRegularPopUps()
    {
        // Hide Buildings view


        // Hide transport View

        // Hide Tile editor view
    }
    public void CloseAllTransportPopUps()
    {
        // rail route viewer
        RailRouteDisplayCanvasActive = false;
        RailDisplayCanvas.SetActive(false);
        MainUICanvas.SetActive(true);
        TrainRouteViewerOn = false;
        RouteSelectedInfo.SetActive(false);
        GridHandler.DeHighlightAllRoutes();


        // train station selection for route
        StationSelectCanvas.SetActive(false);
        SelectingRouteLocation = false;

        // train route set canvas
        RouteSetCanvas.SetActive(false);
        RouteMenuActive = false;

        //rail buttons
        RailMenuActive = false;
        RailCanvas.SetActive(false);

        // close Bus canvas
        CloseBusCanvas();

        // cclose bus route canvas
        BusCanvas.SetActive(false);
        BusCanvasActive = false;
        StopEditingBusStops();

        StartingBusStopInfo.text = "";
        EndBusStopInfo.text = "";
        StartBusStop = new Vector3Int(-1, -1, -1);
        EndBusStop = new Vector3Int(-1, -1, -1);

        //close display but routes
        BusRouteDisplayCanvasActive = false;
        BusRouteDisplayCanvas.SetActive(false);
        BusRouteSetCanvas.SetActive(false);
        BusRouteViewerOn = false;
        RouteSelectedInfo.SetActive(false);
        GridHandler.DeHighlightAllBusRoutes();

    }
    public void HideTransportCanvases()
    {
        CloseBusCanvas();
        CloseRailPopUp();
        CloseTransportPopup();
    }

   public void HideCoreUI()
    {
        TransportCoreButton.SetActive(false); 
        LayoutCoreButton.SetActive(false); ;
        BuildingCoreButton.SetActive(false);
    }
    public void ShowCoreUI()
    {
        TransportCoreButton.SetActive(true);
        LayoutCoreButton.SetActive(true);
        BuildingCoreButton.SetActive(true);
    }
    public void OnTradeButtonClicked()
    {
        SoundManager.PlayButtonClick();
        //     Debug.Log("TradeButton clicked");
        if (TradeMenuActive)
        {
            
            TradeMenuActive = false;
            TradeCanvas.SetActive(false);
            ShowCoreUI();

        }
        else
        {
            SettingsCanvas.SetActive(false);
            SettingsMenuActive = false;
            HideCoreUI();
            SetUIInactive();
            CloseAllTransportPopUps();
            TradeMenuActive = true;
            TradeCanvas.SetActive(true);
            SetTradeInfo();
            HideBuildingInfo();
            PauseCanvas.SetActive(false);
            PauseMenuActive = false;
        }
    }
    public  void ShowAlertPopUp(string Alert)
    {
        AlertpopUp.SetActive(true);
        AlertText.text = Alert;
    }
    public void OnAlertPopupButtonClicked()
    {
        SoundManager.PlayButtonClick();
        AlertpopUp.SetActive(false);
    }
    public void OnSaveButtonClicked()
    {
        SoundManager.PlayButtonClick();
        Debug.Log("Save button clicked");
        DBManager.UpdateSave(NpcHandler.GetCurrentNumberOfNPCs(), MainMenu.CurrentSaveID, gameHandler.GetPlayerMoney(),powerHandler.GetPowerReserves(),gameHandler.GetPlayerWaste()) ;
        DBManager.UpdateMapSave(MainMenu.CurrentSaveID, GridCreator.WIDTH, GridCreator.HEIGHT, GridCreator.GameGrid);
        DBManager.AddAllBuildingsForSave(MainMenu.CurrentSaveID, GridCreator.PlacedBuildings);
        DBManager.UpdateTrainRoutesForSave(MainMenu.CurrentSaveID, TransportPlacementScript.TrainRoutes);
        DBManager.UpdateBusRoutesForSave(MainMenu.CurrentSaveID, TransportPlacementScript.BusRoutes);
    }
    public void OnSaveAndExitClicked()
    {
        SoundManager.PlayButtonClick();
        OnSaveButtonClicked();
        OnExitButtonClicked();
    }
    public void OnExitButtonClicked()
    {
    //    Debug.Log("Exit button clicked");
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

        CloseTilePlaceCanvas();

    }
    public void OnRatingClicked()
    {
        SoundManager.PlayButtonClick();

        ReportDisplay.SetActive(true);
        UpdatesButton.SetActive(false);
        List<string> Updates = GameStatusScript.GetReport();
        string Info = "";
        for(int i = 0; i < Updates.Count; i++)
        {
            Info += Updates[i] + "\n";
        }
        ReportText.text = Info;

    }
    public void OnReportClicked()
    {
        SoundManager.PlayButtonClick();
        UpdatesButton.SetActive(true);
        
        ReportDisplay.SetActive(false);
    }
    public void OnGreeneryButtonClicked()
    {
        SoundManager.PlayButtonClick();
        BusStopEditorOn = false;
        WaterEditorOn = false;
        SetUIInactive();
        if (TileEditorOn)
        {
            TileEditorOn = false;
            GreeneryEditorOn = false;
        }
        else
        {
            
            TilePlaceCanvas.SetActive(true);
            TilePlaceCanvasActive = true;
            CloseBusCanvas();
            CloseTransportPopup();
            TileEditorOn = true;
            GreeneryEditorOn = true;
        }
    }
    public void OnWaterButtonClicked()
    {
        SoundManager.PlayButtonClick();
        GreeneryEditorOn = false;
        WaterEditorOn = false;
        SetUIInactive();
        if (TileEditorOn)
        {
            TileEditorOn = false;
            WaterEditorOn = false;
        }
        else
        {

            TilePlaceCanvas.SetActive(true);
            TilePlaceCanvasActive = true;
            CloseBusCanvas();
            CloseTransportPopup();
            TileEditorOn = true;
            WaterEditorOn = true;
        }
    }

    public void OnRoadButtonClicked()
    {
        SoundManager.PlayButtonClick();
        BusStopEditorOn = false;
        SetUIInactive();
        if (TileEditorOn)
        {
            TileEditorOn = false;
        }
        else
        {
            TilePlaceCanvas.SetActive(true);
            TilePlaceCanvasActive = true;
            CloseBusCanvas();
            CloseTransportPopup();
            TileEditorOn= true;
            GreeneryEditorOn = false;
            WaterEditorOn = false;
        }
    }

    public void OnTransportButtonClicked()
    {
        SoundManager.PlayButtonClick();
        SetUIInactive();
        RailCanvas.SetActive(false);
        if (TransportPlacementOn)
        {
            CloseTransportPopup();
            CloseBusCanvas();
        }
        else
        {
            CloseTilePlaceCanvas();
            CloseBusCanvas();
            TransportPlacementOn = true;
            TransportBuilderPopUp.SetActive(true);
        }
    }
    public void OnRailButtonClicked()
    {
        SoundManager.PlayButtonClick();
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
        SoundManager.PlayButtonClick();

        SetUIInactive();
        TileEditorOn = false;
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
        SoundManager.PlayButtonClick();
        SetUIInactive();
        TransportBuilderPopUp.SetActive(false);

        if (BuildingRemoverOn)
        {
            BuildingsMenuPopUp.SetActive(true);
            BuildingRemoverOn = false;

        }
        else
        {         
            BuildingsListManager.BuildingCurrentlySelected = -1;
            BuildingRemoverOn = true;
        }

    }
}

