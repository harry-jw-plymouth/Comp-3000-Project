using SQLite4Unity3d;
using UnityEngine;

[Table ("Building")]
public class SaveBuildingModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID { get; set; }

    public int TypeIndex {  get; set; }
    public float Xpos { get; set; }
    public float Ypos { get; set; }

    public int OriginX {  get; set; }
    public int OriginY { get; set; }
}
