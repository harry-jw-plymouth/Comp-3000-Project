using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject SavesCanvas; //scene 1
    public GameObject StartButton; // scene 0
    public GameObject CreditsCanvas;
    public GameObject TutorialCanvas;
    public GameObject SelectableSavesCanvas; //Scene 2
    public static int GameSaveID;

    public GameObject SaveObject1;
    public GameObject SaveObject2;
    public GameObject SaveObject3;

    public GameObject SaveObjectButton1;
    public GameObject SaveObjectButton2;
    public GameObject SaveObjectButton3;


    public GameObject NewObjectButton;
    public GameObject ClearSavesButton;

    public TMP_Dropdown GameModeSelection;
    public TMP_Dropdown GameSizeSelection;
    public TMP_InputField FileNameInput;
    public TMP_Text NewFilePrompt;

    public TMP_Text SaveText1;
    public TMP_Text SaveText2;
    public TMP_Text SaveText3;
    public TMP_Text CreditsText;
    public TMP_Text TutorialTitleText;
    public TMP_Text TutorialText;

    public MainMenuSoundHandler SoundHandler;
    public DBManager dbmanager;


    List<string> CreditsPageContents = new List<string>();
    int CurrentCreditsPage = 0;

  

    List<string> TutorialPageContents = new List<string>();
    List<string > TutorialPageTitles = new List<string>();
    int CurrentTutorialPage = 0;


    public static bool NewFileCreated = false;
    public static int CurrentSaveID = -1;
    public static int CurrentGameMode = -1;
    public static int WorldType = 0;
    //0 is Sand box 
    //1 is Simulation
    int CurrentScene = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectableSavesCanvas.SetActive(false);
        SetupCreditsContents();
        SetupTutorialContents();
    }
    // set the values of the list for each page of the credits 
    void SetupCreditsContents()
    {
        CreditsPageContents.Add("  UI\\r\\n\r\nBinIcon https://karsiori.itch.io/free-pixel-art-trash-pack-animated Money ICON: https://xflomasterx.itch.io/coins-free\\r\\nCity background for main menu: https://free-game-assets.itch.io/free-city-backgrounds-pixel-art \\r\\nButtons and most UI elements :\\r\\nhttps://crusenho.itch.io/complete-ui-essential-pack \\r\\nGame Font : https://www.1001fonts.com/arcadeclassic-font.html  \\r\\nTiles\\r\\nGrass tiles: https://cardinalzebra.itch.io/grass-road-tiles \\r\\nRoad tiles (edited to have pathway): https://kubigames.itch.io/road-tiles/download/eyJleHBpcmVzIjoxNzczMjY1NTE3LCJpZCI6MzkwMjY1fQ%3d%3d.WJny6AaDSPHNmF3wyioaGVCdeuk%3d \\r\\nWater tile/animation (grass overlay was added by me):https://zrodfects.itch.io/16x16-water-tiles-animated-tileset-1-starter-pack \\r\\n\r\n");
        CreditsPageContents.Add("  Buildings\\r\r\nShop sprite,wastage center and recycling center(edited to fit style of game slightly) : https://avkov.itch.io/city-tilemap-32x32?download \\nHospital sprite:https://dai420.itch.io/hospital \\r\\nTown hall sprite https://avkov.itch.io/city-tilemap-32x32  \\r\\nShop sprite(edited to fit style of game slightly) : https://avkov.itch.io/city-tilemap-32x32?download \\r\\nAssorted sprites/items\\r\\nPlants: https://shubibubi.itch.io/nature-things \\r\\nTrees :https://karsiori.itch.io/free-pixel-art-tree-pack \\r\\nNPCs https://free-game-assets.itch.io/free-townspeople-cyberpunk-pixel-art \\r\\nPackages/tools\\r\\nColour blind mode package :https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/colorblind-effect-76360  \\r\\nSQL for Database operations: https://github.com/robertohuertasm/SQLite4Unity3d\\r\\n ");
        CreditsPageContents.Add("Music \r\nGameplay background Music : https://www.zapsplat.com/music/game-day-rhythmic-upbeat-electronic-sports-game-instrumental-with-piano-chords-and-synths/ \r\nMain menu music: https://www.epidemicsound.com/music/tracks/e8438738-fb07-479c-97c2-09d60eb04539/ \r\nSound effects\r\nUi Click: https://www.epidemicsound.com/saved/27887854/ \r\nShop place : https://www.epidemicsound.com/sound-effects/tracks/6caad476-d3e4-44ad-afa1-1e9187c46bd0/ \r\nBuilding remove: https://www.epidemicsound.com/sound-effects/search?term=buildinf%20damage \r\nBuilding place: https://www.epidemicsound.com/sound-effects/search?term=Hammer \r\nBus running : https://www.epidemicsound.com/sound-effects/tracks/1d2a910c-11ae-4bb6-b7a1-72519ee4cdc5/ \r\nTrain running: https://www.epidemicsound.com/sound-effects/tracks/fe9bbc35-9158-4743-bdc8-188082a917dd/ \r\nCity AMbience: https://www.epidemicsound.com/sound-effects/tracks/8d637c99-8043-4cbe-a50c-ae2eac8cefaa/ \r\nTile editing : https://www.epidemicsound.com/sound-effects/tracks/4dc122e1-f2c4-4547-a4a7-549008b5eaa3/  \r\n");
        CreditsPageContents.Add("  Sprite splitter : https://ezgif.com/split\\r\\nWebsite for developing sprites : https://www.piskelapp.com/\\r\\nWebsite  for getting sound assets: https://www.epidemicsound.com/ \r\n\r\nHand made assets - Made By Harry Watton \\r\\nUI \\r\\nMain menu game logo, Save slot Block, Power icon, Population icon \\r\\nTiles\\r\\nTrain track tiles ,Bus stop tile (Modified from road mentioned previously)\\r\\nBuildings \\r\\nShopping center, Small house, Medium house, Power plant, Wind farm,Train station\\r\\nOther:\\r\\nBus sprites, Train sprites\\r\\n\\r\\nDeveloper- Harry Watton\\r\\n\\r\\nDeveloped using the Unity 6 engine \\r\\n\r\n");
    }
    // set the titles and page contents from each page of the tutorial
    void SetupTutorialContents()
    {
        TutorialPageTitles.Add("Tutorial part1: Background");
        TutorialPageContents.Add("\r\nWelcome  to Green city Builder! \r\n\r\nYou  are the newest city planner and now you get the opportunity to build a city however you would like \r\n\r\nYou will build buildings, set up public transport, terraform you environment and more to  build a  city fit for purpose. ");
        TutorialPageTitles.Add("Tutorial part 2: Getting started");
        TutorialPageContents.Add("Once you have selected your game mode, your world will be generated, starting  out your city will be a simple road with only greenery and some rivers nearby.\r\n\r\nFrom here you can begin planning and developing your city\r\n\r\n");
        TutorialPageTitles.Add("Tutorial part 3: Game objectives");
        TutorialPageContents.Add("In  simple  terms, your objective  is to build a city  that pleases its citzens while  ensuring your'e  still taking care of the environment\r\nAlmost every aspect of the city is measure and compiled into  a rating  that increases or decreases  as you play and build. You can improve your city by building homes, building infastrcture and transportation, and keeping enviornmental impact minimal\r\nBy clicking the rating  icon at the top left you can see this in more  detail for your city");
        TutorialPageTitles.Add("Tutorial part 4: Tile editing");
        TutorialPageContents.Add("The world around you can be reshaped to fit your cities needs\r\nBy clicking the tile  editor button while playing the game, the tile  editor will be open. From here you can select a tile type to edit. Once a tile is selected, click the tile you want to  edit, selecting a grass tile will replace it with the selected tile, selecting a tile of that type again will turn it back into grass. In standard mode this will cost money");
        TutorialPageTitles.Add("Tutorial part 5: Buildings");
        TutorialPageContents.Add("Various buildings can be placed from the buildings menu, these cost money to place in standard mode. \r\nBuildings each have their own use so you have to consider what you build carefully. Homes provide housing for your people, wastage centers ensure your city stays clean, power stations keep your buildings running, shops hospitals and entertainment venues keep your citzens happy. ");
        TutorialPageTitles.Add("Tutorial part 6: Citizens");
        TutorialPageContents.Add("Citizens are a key part of your city. \r\nNPCs will make a variety of decisions depending on how happy, tired, sick or bored they are. Make sure they have the correct facilities available and they should remain happy. \r\n\r\nNpcs who are unhappy for long peroids of time are likely  to  leave the city, meanwhile having lots of NPCs may attract new people to move to your city  allowing  for  expansion ");
        TutorialPageTitles.Add("Tutorial part 7: Public transport");
        TutorialPageContents.Add("To help your citzens get around, public transport can be built in the form of trains and buses\r\nThese can be set up by using the set route menu on the transport section. Here you can  select the route points (bus stops for buses,train stations for trains). As  long as these are connected by the relevant tiles(road for buses, track for  trains),  transport will be able to run along   this route. \r\nIn  standard mode routes cost money to run and set up");
        TutorialPageTitles.Add("Tutorial part 8: Trading");
        TutorialPageContents.Add("If you ever run low on money or power, trading is an option. \r\nSimply open the trade menu from the top right, select whether you would like to buy or sell power then choose your amount to trade. This can be used when you're running  low   on something an need a top up fast!");
        TutorialPageTitles.Add("Tutorial part 9: environmental impact");
        TutorialPageContents.Add("Managing the enviornment of your city is very important and failing to do so will affect your city,its rating and even  the people living there\r\nMaking sure your city has enough green spaces, focuses on green energy, and makes use of recycling where possible are the key ways this can be done");
        TutorialPageTitles.Add("Tutorial part 10: Power");
        TutorialPageContents.Add("Power is an important part of managing your city, each month every building will use a certain amount of power\r\nTo keep your buildings running, make sure to build power facilities. If power runs out your city may stop functioning. \r\nAlso ensure you buildings are all placed in good proximity  to power stations, power reach is not unlimited ");
        TutorialPageTitles.Add("Tutorial part 11: Game modes");
        TutorialPageContents.Add("There are two game modes to choose from, sandbox and standard\r\nSandbox mode allows you to build your city without worrying about money or power, this is a great mode for just building and experimenting\r\nStandard mode is more challenging, you have to manage your money and power while building your city, this is a great mode for players looking for a challenge and a more realistic experience");
        TutorialPageTitles.Add("Tutorial part 12: UI  and controls");
        TutorialPageContents.Add("By clicking the escape key, the pause menu is opened where you can save, manage settings and exit to main main menu\r\nPressing WASD will move the view around the map \r\nClicking tiles/buildings in relation to the UI allows for world editing ");
        TutorialPageTitles.Add("Tutorial part 13: Money");
        TutorialPageContents.Add("Money is a key part of standard mode, it is used to build and maintain your city\r\nMake sure to keep an eye on your money and manage it well, if you run out of money you may find yourself in a difficult position\r\nMoney can be earned by building shops and entertainment venues, these attract NPCs who will spend money there. You can also earn money by selling power using tradinf");
        TutorialPageTitles.Add("Tutorial part 14: UI elements");
        TutorialPageContents.Add("The top of the screen while playing shows various icons/ numbers.\r\nThe person icon is next to the display of the cities population\r\nThe coins icon is for the current money\r\nThe  Lightning icon is for the players power reserves\r\nThe Bin icon is for the number of waste the city has built up");

    }
    // Return the ID of the current save slot selected (0,1 or 2)
    public static int GetCurrentSaveID()
    {
        return CurrentSaveID;
    }
    // return true if a new file was created when opening the current game
    public static bool GetIfNewFileCreated()
    {
        return NewFileCreated;
    }
    // reopen title page
    public void OnFirstPageBackButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        SavesCanvas.SetActive(false);
        StartButton.SetActive(true);
    }
    // on start clicked open main menu selection
    public void OnStartClicked()
    {
        StartButton.SetActive(false);
        SavesCanvas.SetActive(true);
        SoundHandler.PlayButtonClickSound();
    }
    // on exit button clicked close the game
    public void OnExitClicked()
    {
        SoundHandler.PlayButtonClickSound();
        Application.Quit();
    }
    // display the contents of the credits list at the index passed to the credits page
    public void DisplayCredits(int Page)
    {
        CreditsText.text = CreditsPageContents[Page];
        CurrentCreditsPage = Page;

    }
    // display the contents of the next position of the credits list to the credits page
    public void OnNextCreditsClicked()
    {
        SoundHandler.PlayButtonClickSound();
        if (CurrentCreditsPage < CreditsPageContents.Count - 1)
        {
            DisplayCredits(CurrentCreditsPage + 1);
        }
    }
    // display the previous page of credits 
    public void OnPreviousCreditsClicked()
    {
        SoundHandler.PlayButtonClickSound();
        if (CurrentCreditsPage > 0)
        {
            DisplayCredits(CurrentCreditsPage - 1);
        }
    }
    // display tutorial content of specific page to UI
    public void DisplayTutorial(int Page)
    {
        TutorialTitleText.text = TutorialPageTitles[Page];
        TutorialText.text = TutorialPageContents[Page];
        CurrentTutorialPage = Page;

    }
    //  display contnets of next tutorial page
    public void OnNextTutorialClicked()
    {
        SoundHandler.PlayButtonClickSound();
        if (CurrentTutorialPage < TutorialPageContents.Count - 1)
        {
            DisplayTutorial(CurrentTutorialPage + 1);
        }
    }
    // display contents of previous tutorial page to UI
    public void OnPreviousTutorialClicked()
    {
        SoundHandler.PlayButtonClickSound();
        if (CurrentTutorialPage > 0)
        {
            DisplayTutorial(CurrentTutorialPage - 1);
        }
    }
    // open tutorial page and call display tutorial to display the first page
    public void OnTutorialButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        TutorialCanvas.SetActive(true);
        SavesCanvas.SetActive(false);
        DisplayTutorial(0);
    }
    // close tutorial page
    public void OnTutorialBackButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        TutorialCanvas.SetActive(false);
        SavesCanvas.SetActive(true);
    }
    // open credits page and call display credits to display the first page
    public void OnCreditsClicked()
    {
        SoundHandler.PlayButtonClickSound();
        CreditsCanvas.SetActive(true);
        SavesCanvas.SetActive(false);
        DisplayCredits(0);
    }
    // close credits page
    public void OnCreditsBackButtonClicked()
    {
        CreditsCanvas.SetActive(false);
        SavesCanvas.SetActive(true);
    }
    // Display Save slots menu UI
    public void OnSelectSaveClicked()
    {
        NewFilePrompt.text = "";
        SoundHandler.PlayButtonClickSound();
        SavesCanvas.SetActive(false);
        SelectableSavesCanvas.SetActive(true);
        ShowSaves();
    }
    // load existing save file attached to the save index passed
    public void LoadSaveFile(int Save)
    {
        NewFileCreated = false;
        CurrentSaveID = Save;
        CurrentGameMode=dbmanager.GetSaveTypeForID(Save);
        SceneManager.LoadScene("GameScene");
    }
    // Open save file in save slot 1
    public void OnSave1Click()
    {
        LoadSaveFile(0);
    }
    // Open save file in save slot 2
    public void OnSave2Click()
    {
        LoadSaveFile(1);
    }
    // Open save file in save slot 3
    public void OnSave3Click()
    {
        LoadSaveFile(2);
    }
    //display saves to save slot UI
    public void ShowSaves()
    {
        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);

        SaveObjectButton1.SetActive(false);
        SaveObjectButton2.SetActive(false);
        SaveObjectButton3.SetActive(false);

        ClearSavesButton.SetActive(true);
        NewObjectButton.SetActive(false);

        List<SaveFileModel> Saves = DBManager.GetSaveFiles();
        if (!Saves[0].IsEmpty)
        {
            SaveObject1.SetActive(true);
            SaveText1.text = "Save slot 1: " + Saves[0].Name+"\n" +
                "File type: "+Saves[0].Type;
        }
        else
        {
            SaveObjectButton1.SetActive(true);
        }
        if (!Saves[1].IsEmpty)
        {
            SaveObject2.SetActive(true);
            SaveText2.text = "Save slot 2: " + Saves[1].Name + "\n" +
                "File type: " + Saves[1].Type;
        }
        else
        {
            SaveObjectButton2.SetActive(true);
        }
        if (!Saves[2].IsEmpty)
        {
            SaveObject3.SetActive(true);
            SaveText3.text = "Save slot 3: " + Saves[2].Name + "\n" +
                "File type: " + Saves[2].Type;
        }
        else
        {
            SaveObjectButton3.SetActive(true);
        }
    }
    // return game mode selected,0 is sand box, 1 is standard
    public static int GetCurrentGameMode()
    {
        return CurrentGameMode;
    }
    // return world size,0 is small,1 is medium,2 is large
    public static int GetCurrentWorldSize()
    {
        return WorldType;
    }
    // create a new save file then load into the game if creating save was succesful
    public void OnNewFileCreateButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        Debug.Log("Creating new save");
        string FileName = FileNameInput.text;
        string GameModeSelected = GameModeSelection.options[GameModeSelection.value].text;
        int GameSizeSelected = GameSizeSelection.value;
        WorldType = GameSizeSelected;

        // debugging code
        Debug.Log("FileName:" +FileName);
        Debug.Log("Game mode:" +GameModeSelected);
        if (GameSizeSelected == 0)
        {
            Debug.Log("GameSize small");

        }
        if (GameSizeSelected == 1)
        {
            Debug.Log("GameSize medium");
        }
        if (GameSizeSelected == 2)
        {
            Debug.Log("GameSize large");
        }


        CurrentGameMode = GameModeSelection.value;
        if (FileName == "")
        {
            Debug.Log("Empty field");
            NewFilePrompt.text = "Please  enter  a file name";
        }
        else
        {
            int SaveID = DBManager.AttemptToCreateNewFile(FileName, GameModeSelected);
            if (SaveID!=-1)
            {
                NewFileCreated = true;
                CurrentSaveID = SaveID;
                SceneManager.LoadScene("GameScene");
            }
        }
    }
    //  clear and reset all save slots
    public void OnClearButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        DBManager.ResetSaves();
        ShowSaves();
    }
    // display UI for creating a new save file
    public void OnCreateNewButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);

        SaveObjectButton1.SetActive(false);
        SaveObjectButton2.SetActive(false);
        SaveObjectButton3.SetActive(false);

        ClearSavesButton.SetActive(false);

        NewObjectButton.SetActive(true);
    }
    // close save selection and go to menu page
    public void OnBackButtonClicked()
    {
        SoundHandler.PlayButtonClickSound();
        SavesCanvas.SetActive(true);
        SelectableSavesCanvas.SetActive(false);

        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);
        SaveObjectButton1.SetActive(false);
    }
    // destroy data of all train routes
    public void ClearTrainRoutes()
    {
        if (TransportPlacementScript.TrainRoutes != null)
        {
            for (int i =0;i<TransportPlacementScript.TrainRoutes.Count; i++)
            {
                if (TransportPlacementScript.TrainRoutes[i] != null)
                {
                    TransportPlacementScript.TrainRoutes[i].DestroyRoute();
                }
            }
            TransportPlacementScript.TrainRoutes.Clear();
        }
    }
    // destroy data of all bus routes
    public void ClearBusRoutes()
    {
        if (TransportPlacementScript.BusRoutes != null)
        {
            for (int i = 0; i < TransportPlacementScript.BusRoutes.Count; i++)
            {
                if (TransportPlacementScript.BusRoutes[i] != null)
                {
                    TransportPlacementScript.BusRoutes[i].DestroyRoute();
                }
            }
            TransportPlacementScript.BusRoutes.Clear();
        }
    }
    // make sure old data is removed before opening a new game to prevent errors 
    public void CleanupBeforeOpeningSave()
    {
        ClearTrainRoutes();
        ClearBusRoutes();
    }
}
