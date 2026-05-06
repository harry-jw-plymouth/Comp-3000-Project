using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlacedBuilding
{
   public  Building buildingType;
    public int[] OriginPos;
    public GameObject Sprite;
    public GameObject PowerWarning;
    Vector3 BuildingPos;
    List<int> NPCsInBuildingIndexes=new List<int>();
    List<int>Inhabitants=new List<int>();
    bool CurrentlyInRangeOfPower;
    int TimeWithoutPower = 0;
    // constructor
    public PlacedBuilding(Building buildingType, int[] originPos, GameObject sprite)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
        Sprite = sprite;
        CurrentlyInRangeOfPower= false;
    }
    // return if building is train station
    public bool GetIfTrainStation()
    {
        return buildingType.GetIfIsTrainStation();
    }
    // set sprite for power warning
    public void SetWarningSprite(GameObject sprite)
    {
        PowerWarning =  sprite;
        PowerWarning.GetComponent<SpriteRenderer>().enabled = false;
    }
    // return building type shape
    public int[,] GetShape()
    {
        return buildingType.GetShape();
    }
    // return building position as int version
    public Vector3Int GetBuildingPosAsInt()
    {
        return new Vector3Int((int)BuildingPos.x,(int)BuildingPos.y,(int)BuildingPos.z);
    }
    // display warning of power being low for building
    public void DisplayWarning(bool status)
    {
        if (status)
        {
            if (TimeWithoutPower < 5)
            {
                TimeWithoutPower++;
            }
        }
        else
        {
            TimeWithoutPower = 0;
        }
        PowerWarning.GetComponent<SpriteRenderer>().enabled = status;
        if (buildingType.GetIfIsPowerPlant())
        {
            PowerWarning.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    // return how muchh money the building generates, returns a reduced value if building not powered
    public int GetMoneyGeneration()
    {
        int Base = buildingType.GetTaxGeneration();
        int FinalAmount = 0;
        if (CurrentlyInRangeOfPower)
        {
            return Base;
        }
        else
        {
            if (Base > 0)
            {
                FinalAmount = Base / 2;
                FinalAmount -= TimeWithoutPower;
            }
            else
            {
                FinalAmount = Base * 2;
                FinalAmount -= TimeWithoutPower;
            }
        }
        return FinalAmount;
    } 
    // destory sprite for power warning to prevent errors
    public void DestroyWarning()
    {
        Object.Destroy(PowerWarning);
    } 
    // return enviornmental impact of building
    public int GetEnviromentalValue()
    {
        return buildingType.GetEnviromentalValue();
    }
    // set whether building in range of a power plant
    public void  SetInRangeOfPowerPlant(bool InRange)
    {
        CurrentlyInRangeOfPower= InRange;
    }
    // return if building in range of power plant 
    public bool GetIfInRangeOfPowerPlant()
    {
        return CurrentlyInRangeOfPower;
    }
    // return list of NPC IDs in building
    public List<int> GetInhabitants()
    {
        return Inhabitants;
    }
    // add NPC to being in building
    public void AddInhabitantIndex(int Index)
    {
        Inhabitants.Add(Index);
    }
    // remove specific NPC from building
    public void RemoveSpecificInhabitantIndex(int Index)
    {
        Inhabitants.Remove(Index);
    }
    // return NPC IDs in building
    public List<int> GetNPCsInBuilding()
    {
        return NPCsInBuildingIndexes;
    }
    // add NPC index to list of NPCs in building
    public void AddNPCIndex(int Index)
    {
        NPCsInBuildingIndexes.Add(Index);
    }
    // remove NPC from being inside building
    public void RemoveSpecificIndex(int Index)
    {
        NPCsInBuildingIndexes.Remove(Index);
    }
    // return index of buildinf type in building manager list
    public int GetTypeIndex()
    {
        return buildingType.GetTypeIndex();
    }
    // set position of buildinf
    public void SetBuildingPos(Vector3 NewPos)
    {
        BuildingPos = NewPos;
    }
    // return postion building placed at
    public Vector3 GetBuildingPos()
    {
        return BuildingPos;
    }
    //returns what building type the building is
    public Building GetType()
    {
        return buildingType;
    }
    // return if building type assigned is a power plant
    public bool GetIfIsPowerPlant()
    {
        return buildingType.GetIfIsPowerPlant();
    }
    // return if building type assigned is a shop
    public bool GetIfIsShop()
    {
        return buildingType.GetIfBuildingIsAShop();
    }
    // return if building type assigned is a hospital
    public bool GetIfIsHospital()
    {
        return buildingType.GetIfIsHospital();
    }
    // return if building type assigned is entertainment
    public bool GetIfBuildingIsEntertainment()
    {
        return buildingType.GetIfEntertainment();
    }
}
