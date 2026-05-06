using SQLite4Unity3d;
using UnityEngine;

// model for saving train routes to db table
[Table ("TrainRoutes")]
public class TrainRouteModel
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public int AssociatedSaveID {  get; set; }

    public int StartXpos { get; set; }
    public int StartYpos { get; set; }

    public int EndXpos { get; set; }
    public int EndYpos { get; set; }



}
