using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameStatusScript : MonoBehaviour
{
    public static float CurrentRating = 0;
    [SerializeField] NPChandler npcHandler;
    [SerializeField] GridCreator gridCreator;
    [SerializeField] TextMeshProUGUI DisplayText;
    [SerializeField] TextMeshProUGUI MoneyDisplayText;
    int FrequencyOfRatingUpdate = 1000;
    int FrequencyOfTaxUpdate=1000;
    int MoneyCounter = 500;

    int PlayerMoneyCount;
    float FrequencyCounter = 0;
    public static List<string> Info = new List<string>();
    [SerializeField] static RatingInfo CurrentInfo = new RatingInfo();

    public static int CurrentGameMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentGameMode=MainMenu.GetCurrentGameMode();
        PlayerMoneyCount = GetPlayerStartingMoney();

    }
    public int GetPlayerStartingMoney()
    {
        if (MainMenu.GetCurrentSaveID() == -1)
        {

        }
        return 10000;
    }
    public static List<string> GetReport()
    {
        return Info;
    }
    public bool CheckIfPurchaseAffordable(int Cost)
    {
        return Cost <=PlayerMoneyCount;
    }
    public int GetChangeInMoney(List<PlacedBuilding>Buildings)
    {
        int TotalChange = 0;
        for (int i = 0; i < Buildings.Count; i++) {
            TotalChange += Buildings[i].GetMoneyGeneration() ;
        }
        return TotalChange;
    }
    public void DoMoney()
    {
        List<PlacedBuilding>Buildings=GridCreator.GetAllBuildings();
        int Change=GetChangeInMoney(Buildings);
        PlayerMoneyCount += Change;

        DisplayMoney();
    }
    public void AdjustMoney(int Amount)
    {
        PlayerMoneyCount += Amount;
        DisplayMoney();
    }
    public void CheckForMoneyCheck()
    {
        MoneyCounter++;
        if (MoneyCounter ==FrequencyOfTaxUpdate)
        {
            MoneyCounter = 0;
            DoMoney();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //check game mode is not sandbox /free mode 
        if (CurrentGameMode != 0)
        {
            CheckForMoneyCheck();
        }
  
        FrequencyCounter++;
        if(FrequencyCounter == FrequencyOfRatingUpdate)
        {
            FrequencyCounter = 0;
            CalculateCityRating();
            Info = new List<string>(CurrentInfo.GetReport());
            CurrentInfo.ClearReports();
        }
    }
    public void DisplayMoney()
    {
        MoneyDisplayText.text = PlayerMoneyCount.ToString();
    }
    public void DoPlaceBuildingCosts(Building buildingPlaced)
    {
        PlayerMoneyCount -= buildingPlaced.CostToBuild;
        DisplayMoney();
    }
    void CalculateCityRating()
    {
        //clear old reports
        CurrentInfo.ClearReports();
        //Set ratings of each type
        CurrentInfo.SetHomeLessPercentage ( npcHandler.GetHomeLessPercentage());
        CurrentInfo.SetShopRating(npcHandler.GetCurrentNumberOfNPCs(), GridCreator.GetNumberOfShops());
        CurrentInfo.SetHospitalRating(npcHandler.GetCurrentNumberOfNPCs(), GridCreator.GetNumberOfHospitals());
        CurrentInfo.SetRoadRating(gridCreator.GetNumberOfRoads(), gridCreator.GetNumberOfBuildings());
        CurrentInfo.SetPowerRating(GridCreator.GetPowerGeneration(), GridCreator.GetPowerUsage());
        CurrentInfo.SetEntertainmentRating(npcHandler.GetCurrentNumberOfNPCs(), GridCreator.GetNumberOfEntertainment());
        CurrentInfo.SetEnviromentalEffectRating(GridCreator.GetTotalEnviormentalEffects(), npcHandler.GetCurrentNumberOfNPCs());
        CurrentInfo.SetPowerReachRating(GridCreator.PlacedBuildings);
        CurrentInfo.SetTrainStationRating(npcHandler.GetCurrentNumberOfNPCs(), GridCreator.GetNumberOfTrainStations());

        //Calculate rating
        CurrentInfo.CalulcateRating();
        float Rating = CurrentInfo.GetRating();
        //display rating
        DisplayText.text = Rating.ToString();
        CurrentRating = Rating;

    }
    public static int GetRating()
    {
        return (int)CurrentRating;
    }
}
