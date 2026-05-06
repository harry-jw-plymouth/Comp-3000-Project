using UnityEngine;
using SQLite4Unity3d;

//database table for maps in the database
[Table ("Grid")]
public class SaveMapModel
{
    [PrimaryKey,  AutoIncrement]
    public int Id { get; set; }

    public int AssociatedSaveID {  get; set; }
    public int GridWidth {  get; set; }
    public int GridHeight { get; set; }

    public byte[] GridData {  get; set; }
    
}
