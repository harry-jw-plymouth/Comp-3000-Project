using UnityEngine;
using SQLite4Unity3d;

[Table("SaveFile")]
public class SaveFileModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int IDToAssociate {  get; set; }

    public string Name { get; set; }
    //"SandBox" for free mode, "Standard" for mode with money
    public string Type { get; set; }
    public bool IsEmpty {  get; set; }
    public int NumberOfNPCs {  get; set; }
}
