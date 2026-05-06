using UnityEngine;
using SQLite4Unity3d;

//model for saving bus routes to database
// saves record ID, the ID of the save associated, the start x and y Positions and the end x and y positions
[Table("BusRoutes")]
public class BusRouteModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID { get; set; }

    public int StartXpos { get; set; }
    public int StartYpos { get; set; }

    public int EndXpos { get; set; }
    public int EndYpos { get; set; }
}
