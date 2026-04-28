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
    public TextMeshProUGUI MoneyChangeText;
    public TextMeshProUGUI PowerChangeText;
    public TextMeshProUGUI TimeDisplayText;
    public TextMeshProUGUI WasteAmountText;
    int UpdateFrequency = 3000;
    int FrequencyOfTaxUpdate=1000;
    int MoneyCounter = 500;

    int AmountOfWaste = 0;

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

        DisplayMoneyChange();
        DisplayPowerChange();
        CalculateCityRating();
        Info = new List<string>(CurrentInfo.GetReport());
        CurrentInfo.ClearReports();
        UpdateWasteAmount();

    }

    void UpdateWasteAmount()
    {
        int NumberOfWastageCenters = GridCreator.GetNumberOfWastageFacilities();

        int WasteCreated= GridCreator.GetWasteFromBuildings();
        AmountOfWaste+= WasteCreated-(NumberOfWastageCenters*40);
        AmountOfWaste= Mathf.Max(0, AmountOfWaste);

        WasteAmountText.text = "Waste amount: "+AmountOfWaste.ToString();
    }
    void CalculateEnviornmentRating()
    {
        CurrentInfo.SetAirQaulityRating(GridCreator.PlacedBuildings,GridCreator.NumberOfGreenery);
        CurrentInfo.SetGreeneryRating(GridCreator.NumberOfGreenery, GridCreator.GetNumberOfGreenBuildings(), npcHandler.GetCurrentNumberOfNPCs());
        CurrentInfo.SetEnviromentalEffectRating(GridCreator.GetTotalEnviormentalEffects(), npcHandler.GetCurrentNumberOfNPCs());
    }
    public int GetPlayerMoney()
    {
        return PlayerMoneyCount;
    }   
    public int GetPlayerStartingMoney()
    {
        if (MainMenu.GetCurrentSaveID() != -1)
        {
            
            return DBManager.GetSpecificSaveFile(MainMenu.GetCurrentSaveID()).Money;
        }
        Debug.Log("No save file found, giving default money");
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
    public int GetTimeToDisplay()
    {
        return (UpdateFrequency - (int)FrequencyCounter)/100;
    }
    public void UpateTimeDisplay() {
        TimeDisplayText.text =  GetTimeToDisplay().ToString();
    }
    public int GetAirQaulityRating()
    {
        return CurrentInfo.GetAirQualityRating();
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
    public void DisplayMoneyChange()
    {
        int Change=GetChangeInMoney(GridCreator.GetAllBuildings());
        MoneyChangeText.text =  "Money change:"+Change.ToString();

    }
    public void DisplayPowerChange()
    {
        int Change=GridCreator.GetPowerGeneration()-GridCreator.GetPowerUsage(); ;
        PowerChangeText.text =  "Power change:"+Change.ToString();
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
        UpateTimeDisplay();
        if(FrequencyCounter == UpdateFrequency)
        {
            FrequencyCounter = 0;
            CalculateCityRating();
            Info = new List<string>(CurrentInfo.GetReport());
            CurrentInfo.ClearReports();

            DoMoney();
            UpdateWasteAmount();
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
       

        CalculateEnviornmentRating();
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
