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





    public static bool NewFileCreated = false;
    public static int CurrentSaveID = -1;
    int CurrentScene = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectableSavesCanvas.SetActive(false);
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
    public void OnStartClicked()
    {
        StartButton.SetActive(false);
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
    public void OnNewFileCreateButtonClicked()
    {
        Debug.Log("Creating new save");
        string FileName = FileNameInput.text;
        string GameModeSelected = GameModeSelection.options[GameModeSelection.value].text;
        Debug.Log("FileName:" +FileName);
        Debug.Log("Game mode:" +GameModeSelected);

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
            DBManager.CreateNewFile("New File","",false);
     
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


}
