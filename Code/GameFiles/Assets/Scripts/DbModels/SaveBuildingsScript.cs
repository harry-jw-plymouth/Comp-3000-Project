using SQLite4Unity3d;
using UnityEngine;

[Table ("Building")]
public class SaveBuildingsScript
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID { get; set; }

    public int TypeIndex {  get; set; }
    public int Xpos { get; set; }
    public int Ypos { get; set; }
}
