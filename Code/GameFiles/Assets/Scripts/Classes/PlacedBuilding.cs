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
    public PlacedBuilding(Building buildingType, int[] originPos, GameObject sprite)
    {
        this.buildingType = buildingType;
        OriginPos = originPos;
        Sprite = sprite;
        CurrentlyInRangeOfPower= false;
       
        //BuildingPos = new Vector3( originPos[0],originPos[1],0);
    }
    public bool GetIfTrainStation()
    {
        return buildingType.GetIfIsTrainStation();
    }
    public void SetWarningSprite(GameObject sprite)
    {
        PowerWarning =  sprite;
        PowerWarning.GetComponent<SpriteRenderer>().enabled = false;
    }
    public int[,] GetShape()
    {
        return buildingType.GetShape();
    }
    public Vector3Int GetBuildingPosAsInt()
    {
        return new Vector3Int((int)BuildingPos.x,(int)BuildingPos.y,(int)BuildingPos.z);
    }
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
    public void DestroyWarning()
    {
        Object.Destroy(PowerWarning);
    }
    public int GetEnviromentalValue()
    {
        return buildingType.GetEnviromentalValue();
    }
    public void  SetInRangeOfPowerPlant(bool InRange)
    {
        CurrentlyInRangeOfPower= InRange;
    }
    public bool GetIfInRangeOfPowerPlant()
    {
        return CurrentlyInRangeOfPower;
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

    public bool GetIfIsPowerPlant()
    {
        return buildingType.GetIfIsPowerPlant();
    }
    

    public bool GetIfIsShop()
    {
        return buildingType.GetIfBuildingIsAShop();
    }
    public bool GetIfIsHospital()
    {
        return buildingType.GetIfIsHospital();
    }
    public bool GetIfBuildingIsEntertainment()
    {
        return buildingType.GetIfEntertainment();
    }
}
