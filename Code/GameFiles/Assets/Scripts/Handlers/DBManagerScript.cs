using SQLite4Unity3d;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DBManager : MonoBehaviour
{
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

    // initialise all tables in the db
    void InitialiseDb()
    {
        string DBPath = Path.Combine(Application.persistentDataPath, SaveFileTableName);
        db = new SQLiteConnection(DBPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        db.CreateTable<SaveFileModel>();
        db.CreateTable<SaveMapModel>();
        db.CreateTable<SaveBuildingModel>();
        db.CreateTable<SaveNPCInfoModel>();
        db.CreateTable<TrainRouteModel>();
        db.CreateTable<BusRouteModel>();
        Debug.Log("Database loaded");
    }
    // convert map data to bytes to be stored in the db 
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
    // Add a map to the database, convert to correct format using GetMapForDB
    public static void AddNewMapToDB(int AssociatedID, int width, int Height, Square[,]Grid )
    {
        SaveMapModel Map = new SaveMapModel { AssociatedSaveID = AssociatedID, GridWidth=width, GridHeight=Height,GridData=GetMapForDB(Grid,Height,width) };
        db.Insert(Map);
    }
    //return a list of all map data from the database
    public static List<SaveMapModel> GetMapsFromDB()
    {
        return db.Table<SaveMapModel>().ToList();
    }
    // return map data for a specific ID 
    public static SaveMapModel GetSpecificMap(int AssociatedID)
    {
        SaveMapModel Map = db.Table<SaveMapModel>()
                     .FirstOrDefault(x => x.AssociatedSaveID == AssociatedID);
        return Map;
    }
    // update a map in the data base with a specific ID association
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
   // add the NPC info for a save
    public static void AddNewNPCInfo(int AssociatedId,int Amount)
    {
        SaveNPCInfoModel New = new SaveNPCInfoModel
        {
            AssociatedSaveID = AssociatedId,
            NumberOfNPCs = Amount
        };
        db.Insert(New);
    }
    // create a new file, return the ID if succesful or -1 if not 
    public static int AttemptToCreateNewFile(string Name, string Mode)
    {
        List<SaveFileModel> SaveFiles = GetSaveFiles();
        if (SaveFiles[0].IsEmpty)
        {
            Debug.Log("Saving to save file 1");
            if(UpdateForNewFile(Name, Mode, SaveFiles[0],0)){
                return 0;
            }
            return -1;
        }
        else if (SaveFiles[1].IsEmpty)
        {
            Debug.Log("Saving to save file 2");
            if(UpdateForNewFile(Name, Mode, SaveFiles[1],1)){
                return 1;
            }
            return -1;
        }
        else if (SaveFiles[2].IsEmpty){
            Debug.Log("Saving to save file 3");
            if(UpdateForNewFile(Name, Mode, SaveFiles[2],2)){
                return 2;
            }
            return -1;
        }
        Debug.Log("Error, no save slot available");
        return -1;
    }
    // update a save file with a specific ID 
    static bool UpdateForNewFile(string Name, string Mode, SaveFileModel Current,int associationID) {
        var SaveFile=db.Table<SaveFileModel>().Where(x=>x.Id==Current.Id).FirstOrDefault();

        if (SaveFile == null) {
            Debug.Log("Error updating save file");
            return false;
        }
        SaveFile.IDToAssociate = associationID;
        SaveFile.Name = Name;
        SaveFile.Type= Mode;
        SaveFile.IsEmpty = false;
        SaveFile.NumberOfNPCs = Current.NumberOfNPCs;
        SaveFile.Money = Current.Money;
        SaveFile.Power = Current.Power;
        SaveFile.Waste = Current.Waste;

        db.Update(SaveFile);
        return true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    // Clear all database tables
    public static void ClearDB()
    {
        db.DeleteAll<SaveFileModel>();
        db.DeleteAll<SaveBuildingModel>();
        db.DeleteAll<SaveMapModel>();
        db.DeleteAll<SaveNPCInfoModel>();
        db.DeleteAll<TrainRouteModel>();
        db.DeleteAll<BusRouteModel>();
    }
    //Clear db then create blank saves for each slot 
    public static void ResetSaves()
    {
        ClearDB();
        CreateNewFile("", "", true,10,10000,10000,0);
        CreateNewFile("", "", true,10,10000,10000,0);
        CreateNewFile("", "", true,10,10000,10000,0);
    }
    // Reset one save
    public static void ResetOneSave(int SavePosition)
    {
        ClearBuildingsForSave(SavePosition);
        UpdateBusRoutesForSave(SavePosition, new List<BusRoute>());
        UpdateTrainRoutesForSave(SavePosition, new List<Route>());
        UpdateSaveForDeletion(SavePosition);

    }
    //update one save after being deleted
    public static bool UpdateSaveForDeletion(int ID)
    {
        var SaveFile = db.Table<SaveFileModel>().Where(x => x.IDToAssociate == ID).FirstOrDefault();

        if (SaveFile == null)
        {
            Debug.Log("Error updating save file");
            return false;
        }
        SaveFile.NumberOfNPCs = 10;
        SaveFile.Money = 10000;
        SaveFile.Power = 10000;
        SaveFile.Waste = 10000;
        SaveFile.Name = "";
        SaveFile.Type = "";
        SaveFile.IsEmpty = true;

        db.Update(SaveFile);
        return true;
    }

    // update save file with specfic ID
    public static bool UpdateSave( int NPCAmount,int CurrentID,int CurrentMoney,int CurrentPower,int CurrentWaste)
    {
        var SaveFile = db.Table<SaveFileModel>().Where(x => x.IDToAssociate == CurrentID).FirstOrDefault();

        if (SaveFile == null)
        {
            Debug.Log("Error updating save file");
            return false;
        }
        SaveFile.NumberOfNPCs = NPCAmount;
        SaveFile.Money = CurrentMoney;
        SaveFile.Power = CurrentPower;
        SaveFile.Waste = CurrentWaste;

        db.Update(SaveFile);
        return true;

    }
    // Add train route to db table for save
    public static void AddRoute(Vector3Int StartStation, Vector3Int EndStation,int CurrentID)
    {
        TrainRouteModel Route = new TrainRouteModel { AssociatedSaveID = CurrentID, StartXpos = StartStation.x, StartYpos = StartStation.y, EndXpos = EndStation.x, EndYpos = EndStation.y };
        db.Insert(Route);
    }
    // Add bus route to db table for save
    public static void AddBusRoute(Vector3Int StartStopPos,Vector3Int EndStopPos,int CurrentID)
    {
        BusRouteModel Route = new BusRouteModel { AssociatedSaveID = CurrentID, 
            StartXpos = StartStopPos.x, StartYpos = StartStopPos.y, 
            EndXpos = EndStopPos.x, EndYpos = EndStopPos.y };
        int result=db.Insert(Route);
        Debug.Log("Bus route save restult: " + result); 
    }
    // return all train routes for a save file
    public static List<TrainRouteModel> GetAllTrainRoutesForID(int ID)
    {
        return db.Table<TrainRouteModel>().Where(x => x.AssociatedSaveID == ID).ToList();
    }
    // return all bus routes for a save file
    public static List<BusRouteModel> GetAllBusRoutesForID(int ID)
    {
        return db.Table <BusRouteModel>().Where(x => x.AssociatedSaveID == ID).ToList();
    }
    // Remove all train routes for a save 
    public static void ClearTrainRoutesForSave(int ID)
    {
        db.Execute("DELETE FROM TrainRoutes WHERE AssociatedSaveID = ?", ID);
    }
    // Remove all bus routes for a save 
    public static void ClearBusRoutesForSave(int ID)
    {
        db.Execute("DELETE FROM BusRoutes WHERE AssociatedSaveID = ?", ID);
    }
    //clear all train routes for a save then upload up to date routes
    public static void UpdateTrainRoutesForSave(int ID,List<Route> routes)
    {
        ClearTrainRoutesForSave(ID);
        for (int i = 0; i < routes.Count; i++)
        {
            AddRoute(routes[i].StartStation.GetBuildingPosAsInt(), routes[i].EndStation.GetBuildingPosAsInt(),ID);
        }
    }
    //clear all Bus routes for a save then upload up to date routes
    public static void UpdateBusRoutesForSave(int ID, List<BusRoute> routes)
    {
        ClearBusRoutesForSave(ID);
        for (int i = 0; i < routes.Count; i++)
        {
            AddBusRoute(routes[i].StartStop, routes[i].EndStop, ID);
        }
    }
    // Create a new save file 
    public static void CreateNewFile(string FileName,string FileType, bool FileIsEmpty,int NPCAmount, int CurrentMoney,int CurrentPower,int CurrentWaste)
    {
        SaveFileModel Save = new SaveFileModel { Name=FileName,Type=FileType,IsEmpty=FileIsEmpty,NumberOfNPCs=NPCAmount, Money=CurrentMoney, Power=CurrentPower, Waste=CurrentWaste};
        db.Insert(Save);
    }
    // Debugging function for displaying save files
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
    // Return int showing what game mode the save file is
    public int GetSaveTypeForID(int Associatedid)
    {
        List<SaveFileModel> Saves = GetSaveFiles();
        if (Saves[Associatedid].Type == "Standard")
        {
            return 1;
        }
        else
        {
            return 0;
        }
        
    }
    // return a list of all save files
    public static List<SaveFileModel> GetSaveFiles()
    {
        return db.Table<SaveFileModel>().ToList();
    }
    // get save file for specific ID 
    public static SaveFileModel GetSpecificSaveFile(int AssociatedID)
    {
        SaveFileModel Current= db.Table<SaveFileModel>()
                     .FirstOrDefault(x => x.IDToAssociate == AssociatedID);
        return Current;
    }
    // Add new building for save with specified ID
    public static void AddNewBuilding(int AssociatedId, PlacedBuilding Current)
    {
        SaveBuildingModel New = new SaveBuildingModel { AssociatedSaveID = AssociatedId,
            TypeIndex=Current.GetTypeIndex(),
            Xpos=Current.GetBuildingPos().x,
            Ypos=Current.GetBuildingPos().y,
            OriginX = Current.OriginPos[0],
            OriginY = Current.OriginPos[1]
        };
        db.Insert(New);
    }
    // Clear building data for specific save ID
    public static void ClearBuildingsForSave(int IDToClear)
    {
        db.Execute("DELETE FROM Building WHERE AssociatedSaveID = ?", IDToClear);
    }
    //Add all building data to save for ID
    public static void AddAllBuildingsForSave(int ID,List<PlacedBuilding> BuildingsToAdd)
    {
        ClearBuildingsForSave(ID);
        for (int i = 0; i < BuildingsToAdd.Count; i++)
        {
            AddNewBuilding(ID, BuildingsToAdd[i]);
        }
    }
    // return all buildings for specific save 
    public static List<SaveBuildingModel> GetAllBuildingsForSave(int SaveID)
    {
        return db.Table<SaveBuildingModel>().Where(x=>x.AssociatedSaveID== SaveID).ToList();
    }
}

