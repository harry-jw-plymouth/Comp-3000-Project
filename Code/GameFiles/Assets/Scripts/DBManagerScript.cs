using NUnit.Framework;
using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEditor.Build.Content;
using UnityEngine;

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
        db.CreateTable<SaveMapModel>();
        Debug.Log("Database loaded");
    }
    public static byte[] GetMapForDB(Square[,] Grid, int Height, int Width)
    {
        byte[] TranslatedMap = new byte[Width * Height];
        for (int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                TranslatedMap[Width * y + x] = (byte)Grid[x, y].Contains;
            }
        }
        return TranslatedMap;
    }
    public static void AddNewMapToDB(int AssociatedID, int width, int Height, Square[,]Grid )
    {
        SaveMapModel Map = new SaveMapModel { AssociatedSaveID = AssociatedID, GridWidth=width, GridHeight=Height,GridData=GetMapForDB(Grid,Height,width) };
        db.Insert(Map);
    }
    public static List<SaveMapModel> GetMapsFromDB()
    {
        return db.Table<SaveMapModel>().ToList();
    }
    public static SaveMapModel GetSpecificMap(int AssociatedID)
    {
        SaveMapModel Map = db.Table<SaveMapModel>()
                     .FirstOrDefault(x => x.AssociatedSaveID == AssociatedID);
        return Map;
    }
    public static bool UpdateMapSave(int AssociatedID, int width, int Height, Square[,] Grid)
    {
        var SaveFile = db.Table<SaveMapModel>().Where(x => x.AssociatedSaveID == AssociatedID).FirstOrDefault();

        if (SaveFile == null)
        {
            Debug.Log("Error updating save file");
            return false;
        }
        SaveFile.GridWidth = width;
        SaveFile.GridHeight = Height;
        SaveFile.GridData = GetMapForDB(Grid,Height,width);

        db.Update(SaveFile);
        return true;

    }
  //  public static Square[,] GeB(int AssociatedId)
   // {
  //      SaveMapModel Map = GetSpecificMap(AssociatedId);
   //     return Map

//    }
    public static int AttemptToCreateNewFile(string Name, string Mode)
    {
        List<SaveFileModel> SaveFiles = GetSaveFiles();
        if (SaveFiles[0].IsEmpty)
        {
            Debug.Log("Saving to save file 1");
            if(UpdateForNewFile(Name, Mode, SaveFiles[0])){
                return 0;
            }
            return -1;
        }
        else if (SaveFiles[1].IsEmpty)
        {
            Debug.Log("Saving to save file 2");
            if(UpdateForNewFile(Name, Mode, SaveFiles[1])){
                return 1;
            }
            return -1;
        }
        else if (SaveFiles[2].IsEmpty){
            Debug.Log("Saving to save file 3");
            if(UpdateForNewFile(Name, Mode, SaveFiles[2])){
                return 2;
            }
            return -1;
        }
        Debug.Log("Error, no save slot available");
        return -1;
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
