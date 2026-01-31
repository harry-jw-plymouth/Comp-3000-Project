using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class DBManager : MonoBehaviour
{
    //[SerializeField] private SQLiteConnection dbReference;
    public static SQLite4Unity3d.SQLiteConnection db;
    string SaveFileTableName = "SaveFile.db";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialiseDb();
      //  ResetSaves();
        DisplaySaveFiles();
    }
    private void Awake()
    {
        
    }
    void InitialiseDb()
    {
        string DBPath = Path.Combine(Application.persistentDataPath, SaveFileTableName);
        db = new SQLiteConnection(DBPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        db.CreateTable<SaveFileModel>();
        Debug.Log("Database loaded");
    }
    public static bool AttemptToCreateNewFile(string Name, string Mode)
    {
        List<SaveFileModel> SaveFiles = GetSaveFiles();
        if (SaveFiles[0].IsEmpty)
        {
            Debug.Log("Saving to save file 1");
            return UpdateForNewFile(Name, Mode, SaveFiles[0]);
        }
        else if (SaveFiles[1].IsEmpty)
        {
            Debug.Log("Saving to save file 2");
            return UpdateForNewFile(Name, Mode, SaveFiles[1]);
        }
        else if (SaveFiles[2].IsEmpty){
            Debug.Log("Saving to save file 3");

            return UpdateForNewFile(Name,Mode,SaveFiles[2]);
        }
        Debug.Log("Error, no save slot available");
        return false;
    }
    static bool UpdateForNewFile(string Name, string Mode, SaveFileModel Current) {
        var SaveFile=db.Table<SaveFileModel>().Where(x=>x.Id==Current.Id).FirstOrDefault();

        if (SaveFile == null) {
            Debug.Log("Error updating save file");
            return false;
        }
        SaveFile.Name = Name;
        SaveFile.Type= Mode;
        SaveFile.IsEmpty = false;

        db.Update(SaveFile);
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void ClearDB()
    {
        db.DeleteAll<SaveFileModel>();
    }
    public static void ResetSaves()
    {
        ClearDB();
        CreateNewFile("", "", true);
        CreateNewFile("", "", true);
        CreateNewFile("", "", true);

    }
    public static void CreateNewFile(string FileName,string FileType, bool FileIsEmpty)
    {
        SaveFileModel Save = new SaveFileModel { Name=FileName,Type=FileType,IsEmpty=FileIsEmpty};
        db.Insert(Save);
    }
    public void DisplaySaveFiles()
    {
        List<SaveFileModel> SaveFiles = db.Table<SaveFileModel>().ToList();
        Debug.Log("Number of files:" + SaveFiles.Count);
        foreach (SaveFileModel SaveFile in SaveFiles) {
            Debug.Log("SaveID : " + SaveFile.Id);
            Debug.Log("SaveName: " + SaveFile.Name);
            Debug.Log("Type:" + SaveFile.Type);
        }
    }
    public static List<SaveFileModel> GetSaveFiles()
    {
        return db.Table<SaveFileModel>().ToList();
    }
}
