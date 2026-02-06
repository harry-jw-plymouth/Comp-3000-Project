using UnityEngine;
using SQLite4Unity3d;

[Table ("NPCInfo")]
public class SaveNPCInfoModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID { get; set; }

    public int NumberOfNPCs {  get; set; }
}
