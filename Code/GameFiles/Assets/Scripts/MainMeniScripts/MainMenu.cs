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
    }
    public void OnGenerateNewClicked()
    {
        //var Parmeters=new LoadSceneParameters(LoadSceneMode.Single)
        GameSaveID = -1;

        if (GameSaveID == -1)
        {
            DBManager.CreateNewFile("New File");
     
        }
        SceneManager.LoadScene("GameScene");
    }
    public void OnBackButtonClicked()
    {
        SavesCanvas.SetActive(true);
        SelectableSavesCanvas.SetActive(false);
    }


}
