using UnityEngine;
using UnityEngine.Tilemaps;

public class NPChandler : MonoBehaviour
{
    int NumberOfNpcs;
    public GameObject NPCPrefab;

    Vector3Int MapCenter = new Vector3Int(GridCreator.WIDTH / 2, GridCreator.HEIGHT / 2, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NumberOfNpcs = GetNumberOfNPCs();
        LoadNPCs();
    }
    int GetNumberOfNPCs()
    {
        return 1;   
    }
    void LoadNPCs()
    {
        Vector3 worldPosition = GridCreator.GameMap.GetCellCenterWorld(MapCenter);
        for (int i = 0; i < NumberOfNpcs; i++) {
            Instantiate(NPCPrefab,worldPosition,Quaternion.identity );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
