using UnityEngine;
using SQLite4Unity3d;

[Table("SaveFile")]
public class SaveFileModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name { get; set; }
}
