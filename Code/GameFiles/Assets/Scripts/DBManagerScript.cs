using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class DBManager : MonoBehaviour
{
    private SQLiteConnection db;
    string SaveFileTableName = "SaveFile.db";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialiseDb();
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
    public void CreateNewFile(string FileName)
    {
        SaveFileModel Save = new SaveFileModel { Name=FileName};
        db.Insert(Save);
    }
    public void ShowSaveFiles()
    {
        var SaveFiles=db.Table<SaveFileModel>().ToList();
        foreach (var SaveFile in SaveFiles) {
            Debug.Log("SaveID : " + SaveFile.Id);
            Debug.Log("SaveName: " + SaveFile.Name);
        }
    }
}
