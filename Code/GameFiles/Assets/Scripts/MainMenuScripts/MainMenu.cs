using Mono.Cecil;
using NUnit.Framework;
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
    public TMP_InputField FileNameInput;

    public TMP_Text SaveText1;
    public TMP_Text SaveText2;
    public TMP_Text SaveText3;
    public TMP_Text CreditsText;

    public DBManager dbmanager;


    List<string> CreditsPageContents = new List<string>();
    int CurrentCreditsPage = 0;



    public static bool NewFileCreated = false;
    public static int CurrentSaveID = -1;
    public static int CurrentGameMode = -1;
    //0 is Sand box 
    //1 is Simulation
    int CurrentScene = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectableSavesCanvas.SetActive(false);
        SetupCreditsContents();
    }
    void SetupCreditsContents()
    {
        CreditsPageContents.Add("Music\r\nGame background Music : https://www.zapsplat.com/music/game-day-rhythmic-upbeat-electronic-sports-game-instrumental-with-piano-chords-and-synths/ \r\nUI\r\nMoney ICON: https://xflomasterx.itch.io/coins-free\r\nCity background for main menu: https://free-game-assets.itch.io/free-city-backgrounds-pixel-art \r\nButtons and most UI elements :\r\nhttps://crusenho.itch.io/complete-ui-essential-pack \r\nGame Font : https://www.1001fonts.com/arcadeclassic-font.html  \r\nTiles\r\nGrass tiles: https://cardinalzebra.itch.io/grass-road-tiles \r\nRoad tiles (edited to have pathway): https://kubigames.itch.io/road-tiles/download/eyJleHBpcmVzIjoxNzczMjY1NTE3LCJpZCI6MzkwMjY1fQ%3d%3d.WJny6AaDSPHNmF3wyioaGVCdeuk%3d \r\nWater tile/animation (grass overlay was added by me):https://zrodfects.itch.io/16x16-water-tiles-animated-tileset-1-starter-pack \r\n");
        CreditsPageContents.Add("Buildings\r\nHospital sprite:https://dai420.itch.io/hospital \r\nTown hall sprite https://avkov.itch.io/city-tilemap-32x32  \r\nShop sprite(edited to fit style of game slightly) : https://avkov.itch.io/city-tilemap-32x32?download \r\nAssorted sprites/items\r\nPlants: https://shubibubi.itch.io/nature-things \r\nTrees :https://karsiori.itch.io/free-pixel-art-tree-pack \r\nNPCs https://free-game-assets.itch.io/free-townspeople-cyberpunk-pixel-art \r\nPackages/tools\r\nColour blind mode package :https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/colorblind-effect-76360  \r\nSQL for Database operations: https://github.com/robertohuertasm/SQLite4Unity3d\r\nSprite splitter : https://ezgif.com/split\r\nWebsite for developing sprites : https://www.piskelapp.com/\r\n");
        CreditsPageContents.Add("Hand made assets - Made By Harry Watton \r\nUI \r\nMain menu game logo, Save slot Block, Power icon, Population icon \r\nTiles\r\nTrain track tiles ,Bus stop tile (Modified from road mentioned previously)\r\nBuildings \r\nShopping center, Small house, Medium house, Power plant, Wind farm,Train station\r\nOther:\r\nBus sprites, Train sprites\r\n\r\nDeveloper- Harry Watton\r\n\r\nDeveloped using the Unity 6 engine \r\n");
    }
//    void PopulateSaveView()
  //  {
    //    List<SaveFileModel> SaveFiles=DBManager.GetSaveFiles();

      //  Debug.Log("Amount of files to display:" + SaveFiles.Count);
     //   for (int i = 0;  i < SaveFiles.Count; i++)
       // {
         //   SaveItemScript ScrollItem = Instantiate(ScrollPrefab, ScrollContent);
           // //ScrollItem.GetComponentInChildren<TMP_Text>().text = SaveFiles[i].Name;
     //       ScrollItem.Setup(SaveFiles[i].Name);
       // }

    //}
    public static int GetCurrentSaveID()
    {
        return CurrentSaveID;
    }
    public static bool GetIfNewFileCreated()
    {
        return NewFileCreated;
    }
    public void OnFirstPageBackButtonClicked()
    {
        SavesCanvas.SetActive(false);
        StartButton.SetActive(true);
    }
    public void OnStartClicked()
    {
        StartButton.SetActive(false);
        SavesCanvas.SetActive(true);
    }
    public void OnExitClicked()
    {
        Application.Quit();
    }
    public void DisplayCredits(int Page)
    {
        CreditsText.text = CreditsPageContents[Page];
        CurrentCreditsPage = Page;

    }
    public void OnNextCreditsClicked()
    {
        if (CurrentCreditsPage < CreditsPageContents.Count - 1)
        {
            DisplayCredits(CurrentCreditsPage + 1);
        }
    }
    public void OnPreviousCreditsClicked()
    {
        if (CurrentCreditsPage > 0)
        {
            DisplayCredits(CurrentCreditsPage - 1);
        }
    }
    public void OnCreditsClicked()
    {
        CreditsCanvas.SetActive(true);
        SavesCanvas.SetActive(false);
        DisplayCredits(0);
    }
    public void OnCreditsBackButtonClicked()
    {
        CreditsCanvas.SetActive(false);
        SavesCanvas.SetActive(true);
    }
    public void OnSelectSaveClicked()
    {
        SavesCanvas.SetActive(false);
        SelectableSavesCanvas.SetActive(true);
        ShowSaves();
    }
    public void LoadSaveFile(int Save)
    {
        NewFileCreated = false;
        CurrentSaveID = Save;
        CurrentGameMode=dbmanager.GetSaveTypeForID(Save);
        SceneManager.LoadScene("GameScene");
    }
    public void OnSave1Click()
    {
        LoadSaveFile(0);
    }
    public void OnSave2Click()
    {
        LoadSaveFile(1);
    }
    public void OnSave3Click()
    {
        LoadSaveFile(2);
    }
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
    public static int GetCurrentGameMode()
    {
        return CurrentGameMode;
    }
    public void OnNewFileCreateButtonClicked()
    {
        Debug.Log("Creating new save");
        string FileName = FileNameInput.text;
        string GameModeSelected = GameModeSelection.options[GameModeSelection.value].text;
        Debug.Log("FileName:" +FileName);
        Debug.Log("Game mode:" +GameModeSelected);

        CurrentGameMode = GameModeSelection.value;
        if (FileName == "")
        {
            Debug.Log("Empty field");
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
    public void OnClearButtonClicked()
    {
        DBManager.ResetSaves();
        ShowSaves();
    }
    public void OnCreateNewButtonClicked()
    {
        Debug.Log("Create new clicked");
        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);

        SaveObjectButton1.SetActive(false);
        SaveObjectButton2.SetActive(false);
        SaveObjectButton3.SetActive(false);

        ClearSavesButton.SetActive(false);

        NewObjectButton.SetActive(true);

    }

    public void OnGenerateNewClicked()
    {
        //var Parmeters=new LoadSceneParameters(LoadSceneMode.Single)
        GameSaveID = -1;

        if (GameSaveID == -1)
        {
            DBManager.CreateNewFile("New File","",false,10,10000);
     
        }
        SceneManager.LoadScene("GameScene");
        
    }
    public void OnBackButtonClicked()
    {
        SavesCanvas.SetActive(true);
        SelectableSavesCanvas.SetActive(false);

        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);
        SaveObjectButton1.SetActive(false);
    }

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
    public void CleanupBeforeOpeningSave()
    {
        ClearTrainRoutes();
        ClearBusRoutes();
    }
}
