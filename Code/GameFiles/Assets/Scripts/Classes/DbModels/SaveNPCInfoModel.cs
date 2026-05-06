using UnityEngine;
using SQLite4Unity3d;

// db table model for saving NPC info to the database
[Table ("NPCInfo")]
public class SaveNPCInfoModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID { get; set; }

    public int NumberOfNPCs {  get; set; }
}
