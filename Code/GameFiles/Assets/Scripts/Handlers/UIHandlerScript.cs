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
    public TextMeshProUGUI TradeInfo;

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
    public Slider SFXVolumeSlider;
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

    double ElectricityPrice = 1.5;
    int PurchaseAmount = -1;

    int RouteStationPos = -1;
    //  public Square[,] GameGrid;
    private void Start()
    {
        TileEditorOn = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // Close UI for selecting what tile to place and set tile editing to off to stop tile editing from happening 
    public void CloseTilePlaceCanvas()
    {
        TilePlaceCanvasActive = false;
        TilePlaceCanvas.SetActive(false);
        TileEditorOn = false;
    }
    // on button click, stop the player clicks editing tiles
    public void OnCancelPlacementButtonClicked()
    {
        SoundManager.PlayButtonClick();
        TileEditorOn= false;
    }
    // display info pop up with a title and text content as parameters
    public void OpenNewPopUp(string Title, string Text)
    {
        NewPopUpCanvas.SetActive(true);
        PopUpTitle.text = Title;
        PopUpDescription.text = Text;
    }
    // close pop up when close button clicked
    public void OnNewPopUpClosed()
    {
        NewPopUpCanvas.SetActive(false);
    }
    // open tile place canvas if not open and close all other UI, otherwise close the tile canvas
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
    // Display route to UI
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
    // Display bus route to UO
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
    // on cancel route button clicked, delete that route and display info to user
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
    // on cancel bus route button clicked, delete that route and display info to user
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
   //when bus route confirm button clicked, if the route is valid and the player can afford it a new rotue is created
    public void OnBusRouteConfirmButtonClicked()
    {
        SoundManager.PlayButtonClick();
        if (StartBusStop != null && EndBusStop != null)
        {
            if (StartBusStop != EndBusStop)
            {

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
    // on settings button click, toggle settings menu on/off
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
            CloseAllTransportPopUps();
            TradeMenuActive = false;
            TradeCanvas.SetActive(false);

            SFXVolumeSlider.value = SoundManager.GetSFXVolume();
            MusicVolumeSlider.value = SoundManager.GetMusicVolume();
        }
    }
    // Adjust music volume 
    public void OnMusicVolumeSliderAdjusted()
    {
        float Value=MusicVolumeSlider.value;
        SoundHandler.ChangeMusicVolume(Value);
    } 
    // adjust SFX volume 
    public void OnSFXVolumeSliderAdjusted()
    {
        float Value =SFXVolumeSlider.value;
        SoundHandler.ChangeSFXVolume(Value);
    }
    // when colour blind mode changed in settings,call function to change view accordingly
    public void OnColourBlindModeDropdownChanged()
    {
        Debug.Log("Colour blind mode dropdown changed");
        int index = ColourBlindModeDropdown.value;
        ColourBlindHandler.SetMode(index);
    }
    // on settings button clicked, close other UI and open settings, or close settings and open core UI if already open
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
    // if route displaying UI open, close it and open core route ui, otherwise open route setting UI
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
    // On route confirm button clicked, if route speicifed is valid then create route. display information to user if not valid 
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
    // open canvas to select positions for route 
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
    // on route start button in train UI clicked, open UI for selecting route start position
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
    // on bus route start button in train UI clicked, open UI for selecting route start position
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
    // when bus route end selction button clicked, open UI for selecting bus stops
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
   // When setting train stations on route, display info for clicked train station
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
    // When selecting bus stop on route, set information for bus stop clicked
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
    // Open UI for selection end point for UI
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
    //Show route for building rail transport
    public void ShowTrainRouteUI()
    {
        RailMenuActive = true;
        RailCanvas.SetActive(true);
    }
    //show transport builder options
    public void ShowTransportUI()
    {
        TransportBuilderPopUp.SetActive(true);
    }
    // close train route UI and display core UI
    public void OnRouteMenuCloseButtonClicked()
    {
        SoundManager.PlayButtonClick();
        RouteSetCanvas.SetActive(false);
        RouteMenuActive = false ;

        ShowCoreUI();
        ShowTrainRouteUI();
        ShowTransportUI();
    }
    //open train route creation UI
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
    // open  canvas for creating routes
    public void OpenRouteCanvas()
    {
        RouteMenuActive= true ;
        RouteSetCanvas.SetActive(true);
    }
    // close canvad for bus related options
    public void CloseBusCanvas()
    {
        BusCanvasActive = false;
        BusCanvas.SetActive(false);
    } 
    // when bus stop editor clicked, set values accordingly so clicking tiles will build bus stops, or turn off if already on
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
    // turn settings for editing bus stops to off
    public void StopEditingBusStops()
    {
        TileEditorOn = false;
        BusStopEditorOn = false;
    }
    // hide Core UI and open bus route creation canvas 
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
    // close bus route creation canvas and reopen core UI
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
    // On bus button clicked, if already on then turn off, if not on, hide conflicting UI then open
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
        }
    } 
    // display info for building and display box for displaying info
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
   // display info for building hover at position of building
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
    }
    // close building info box
    public void HideBuildingInfo()
    {
        BuildingInfoShowing = false;
        BuildingInfoBox.SetActive(false);
        NewBuidingInfoBox.SetActive(false);

    }
    // open pause menu if not open or close if already open
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
    //  when value edited in trade UI, display the way this will effect players money reserves
    public void OnEditMadeToNumberToTrade()
    {      
       
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
    // display info for trade 
    public void SetTradeInfo()
    {
        string MaterialSelectedToBuy = TradeItems.options[TradeItems.value].text;
        //0=buy, 1=sell
        int action = Action.value;
        // set trade cost
        CostConversionText.text = "Money cost per " + MaterialSelectedToBuy + " : " + 1.5f;
    }
    // if trade info valid, complete trade
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
                    TradeInfo.text= "purhased " + FinalCost + " for " + PurchaseAmount + " electricity";
                    AmountToBuy.text = "";
                }
                else
                {
                    TradeInfo.text = "Not enough money for trade";
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
                    TradeInfo.text="sold " + FinalElectricityCost + " for " + PurchaseAmount + " money";
                }
                else
                {
                   TradeInfo.text="Not enough power";
                }
            }
        }
        else
        {
            TradeInfo.text="Please enter a value ";
        }
    }
    // close All UI associated with transport
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
    // Hide base UI for transport
    public void HideTransportCanvases()
    {
        CloseBusCanvas();
        CloseRailPopUp();
        CloseTransportPopup();
    }
    // hide core transport,layout and buidlings buttons
   public void HideCoreUI()
    {
        TransportCoreButton.SetActive(false); 
        LayoutCoreButton.SetActive(false); ;
        BuildingCoreButton.SetActive(false);
    }
    // show core transport,layout and buidlings buttons
    public void ShowCoreUI()
    {
        TransportCoreButton.SetActive(true);
        LayoutCoreButton.SetActive(true);
        BuildingCoreButton.SetActive(true);
    }
    // toggle trade UI on and off, hiding/opening other UI accordingly
    public void OnTradeButtonClicked()
    {
        SoundManager.PlayButtonClick();
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
    // show pop up (old)
    public  void ShowAlertPopUp(string Alert)
    {
        AlertpopUp.SetActive(true);
        AlertText.text = Alert;
    }
    // close popup (old)
    public void OnAlertPopupButtonClicked()
    {
        SoundManager.PlayButtonClick();
        AlertpopUp.SetActive(false);
    } 
    // save the game by updating all relevant info in the db
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
    // save the game then close to main menu
    public void OnSaveAndExitClicked()
    {
        SoundManager.PlayButtonClick();
        OnSaveButtonClicked();
        OnExitButtonClicked();
    }
    //Close scene and return to main menu
    public void OnExitButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    // set some core UI to hidden
    void SetUIInactive()
    {
        BuildingsMenuPopUp.SetActive(false);
        TransportBuilderPopUp.SetActive(false);
        BuildingsListManager.BuildingCurrentlySelected = -1;
        BuildingRemoveButton.SetActive(false);

        TileEditorOn = false;
        TransportPlacementOn = false;
        BuildingRemoverOn=false;

        CloseTilePlaceCanvas();
    }
    //display reports from rating information
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
    // hide reports from rating info
    public void OnReportClicked()
    {
        SoundManager.PlayButtonClick();
        UpdatesButton.SetActive(true);
        
        ReportDisplay.SetActive(false);
    }
    // on water greenery clicked, set tile editor to be editing water
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
    // on water button clicked, set tile editor to be editing greenery
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
    // on road button clicked, set tile editor to be editing road
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
    // open UI for selecting canvas type or close it if already open
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
    // open rail UI or close it if already open
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
    // close transport UI
    public void CloseTransportPopup()
    {
        TransportBuilderPopUp.SetActive(false);
        StartRouteStationBuilding = -1;
        EndRouteStationBuilding = -1;
        TransportPlacementOn = false;
        CloseRailPopUp();
    }
    // close UI for rail related buttons
    public void CloseRailPopUp()
    {
        RailCanvas.SetActive(false);
    }
    // open UI for placing buildings or close it if already open
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
    // set building remover to be active if not already or not active if already active 
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

