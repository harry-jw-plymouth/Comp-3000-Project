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

    public TMP_Dropdown GameModeSelection;
    public TMP_InputField FileNameInput;





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
    public void ShowSaves()
    {
        SaveObject1.SetActive(false);
        SaveObject2.SetActive(false);
        SaveObject3.SetActive(false);

        SaveObjectButton1.SetActive(false);
        SaveObjectButton2.SetActive(false);
        SaveObjectButton3.SetActive(false);

        NewObjectButton.SetActive(false);

        List<SaveFileModel> Saves = DBManager.GetSaveFiles();
        if (!Saves[0].IsEmpty)
        {
            SaveObject1.SetActive(true);
        }
        else
        {
            SaveObjectButton1.SetActive(true);
        }
        if (!Saves[1].IsEmpty)
        {
            SaveObject2.SetActive(true);
        }
        else
        {
            SaveObjectButton2.SetActive(true);
        }
        if (!Saves[2].IsEmpty)
        {
            SaveObject3.SetActive(true);
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
        Debug.Log("FileName:" +FileName);
        Debug.Log("Game mode:" + GameModeSelection.options[GameModeSelection.value].text);

        if (FileName == "")
        {
            Debug.Log("Empty field");
        }
        else
        {

        }
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
    }


}
