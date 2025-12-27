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

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void CreateNewFile(string FileName)
    {
        SaveFileModel Save = new SaveFileModel { Name=FileName};
        db.Insert(Save);
    }
    public void DisplaySaveFiles()
    {
        List<SaveFileModel> SaveFiles = db.Table<SaveFileModel>().ToList();
        Debug.Log("Number of files:" + SaveFiles.Count);
        foreach (SaveFileModel SaveFile in SaveFiles) {
            Debug.Log("SaveID : " + SaveFile.Id);
            Debug.Log("SaveName: " + SaveFile.Name);
        }
    }
    public static List<SaveFileModel> GetSaveFiles()
    {
        return db.Table<SaveFileModel>().ToList();
    }
}
