using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlacedBuilding
{
   public  Building buildingType;
    public int[] OriginPos;
    public GameObject Sprite;
    Vector3 BuildingPos;
    List<int> NPCsInBuildingIndexes=new List<int>();
    List<int>Inhabitants=new List<int>();
    public PlacedBuilding(Building buildingType, int[] originPos, GameObject sprite)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
        Sprite = sprite;
        
        //BuildingPos = new Vector3( originPos[0],originPos[1],0);
    }
    public List<int> GetInhabitants()
    {
        return Inhabitants;
    }
    public void AddInhabitantIndex(int Index)
    {
        Inhabitants.Add(Index);
    }
    public void RemoveSpecificInhabitantIndex(int Index)
    {
        Inhabitants.Remove(Index);
    }
    public List<int> GetNPCsInBuilding()
    {
        return NPCsInBuildingIndexes;
    }
    public void AddNPCIndex(int Index)
    {
        NPCsInBuildingIndexes.Add(Index);
    }
    public void RemoveSpecificIndex(int Index)
    {
        NPCsInBuildingIndexes.Remove(Index);
    }
    public int GetTypeIndex()
    {
        return buildingType.GetTypeIndex();
    }
    public void SetBuildingPos(Vector3 NewPos)
    {
        BuildingPos = NewPos;
    }
    public Vector3 GetBuildingPos()
    {
        return BuildingPos;
    }
    public Building GetType()
    {
        return buildingType;
    }
    

    public bool GetIfIsShop()
    {
        return buildingType.GetIfBuildingIsAShop();
    }
    public bool GetIfIsHospital()
    {
        return buildingType.GetIfIsHospital();
    }
}
