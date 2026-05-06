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
        // Set Current game mode, player money count, city accumulated waste and display relevant info on UI
        CurrentGameMode=MainMenu.GetCurrentGameMode();
        PlayerMoneyCount = GetPlayerStartingMoney();
        AmountOfWaste= GetStartingPlayerWaste();

        DisplayMoneyChange();
        DisplayPowerChange();
        CalculateCityRating();
        Info = new List<string>(CurrentInfo.GetReport());
        CurrentInfo.ClearReports();
        UpdateWasteAmount();


    }
    // If new save file, return 0, otherwise return value from save file
    public int GetStartingPlayerWaste()
    {
        if (MainMenu.GetCurrentSaveID() != -1)
        { 
            return DBManager.GetSpecificSaveFile(MainMenu.GetCurrentSaveID()).Waste;
        }
        Debug.Log("No save file found, giving default waste");
        return 0;
    }
    // increase waste amount depending on waste produced by buildings and decrease it based on wastage facilities in operation
    void UpdateWasteAmount()
    {
        int NumberOfWastageCenters = GridCreator.GetNumberOfWastageFacilities();

        int WasteCreated= GridCreator.GetWasteFromBuildings();
        AmountOfWaste+= WasteCreated-(NumberOfWastageCenters*40);
        AmountOfWaste= Mathf.Max(0, AmountOfWaste);

        WasteAmountText.text = "Waste amount: "+AmountOfWaste.ToString();
    }
    // Call rating functions that calculate elements relating to the enviornment
    void CalculateEnviornmentRating()
    {
        CurrentInfo.SetAirQaulityRating(GridCreator.PlacedBuildings,GridCreator.NumberOfGreenery);
        CurrentInfo.SetWastageRating(AmountOfWaste, npcHandler.GetCurrentNumberOfNPCs());
        CurrentInfo.SetGreeneryRating(GridCreator.NumberOfGreenery, GridCreator.GetNumberOfGreenBuildings(), npcHandler.GetCurrentNumberOfNPCs());
        CurrentInfo.CalculateWaterPollutionRating(GridCreator.GetWaterPollution(), GridCreator.NumberOfWater);
        CurrentInfo.SetEnviromentalEffectRating(GridCreator.GetTotalEnviormentalEffects(), npcHandler.GetCurrentNumberOfNPCs());
    }
    // return current players money amount
    public int GetPlayerMoney()
    {
        return PlayerMoneyCount;
    }   
    // return currnet player city waste accumulation
    public int GetPlayerWaste()
    {
                return AmountOfWaste;
    }
    // set up player starting money, if new save file set to 10000, else pull value from database file
    public int GetPlayerStartingMoney()
    {
        if (MainMenu.GetCurrentSaveID() != -1)
        {
            
            return DBManager.GetSpecificSaveFile(MainMenu.GetCurrentSaveID()).Money;
        }
        Debug.Log("No save file found, giving default money");
        return 10000;
    }
    // return string of all issues noted when calculating city rating
    public static List<string> GetReport()
    {
        return Info;
    } 
    // return true if player has equal to or more than the cost 
    public bool CheckIfPurchaseAffordable(int Cost)
    {
        return Cost <=PlayerMoneyCount;
    }
    // calculate how much the player will gain/lose next time tax is collected
    public int GetChangeInMoney(List<PlacedBuilding>Buildings)
    {
        int TotalChange = 0;
        for (int i = 0; i < Buildings.Count; i++) {
            TotalChange += Buildings[i].GetMoneyGeneration() ;
        }
        return TotalChange;
    } 
    // return countdown to next tax collection
    public int GetTimeToDisplay()
    {
        return (UpdateFrequency - (int)FrequencyCounter)/100;
    }
    // display time left to next tax collection to UI
    public void UpateTimeDisplay() {
        TimeDisplayText.text =  GetTimeToDisplay().ToString();
    }
    // return air qaulity 
    public int GetAirQaulityRating()
    {
        return CurrentInfo.GetAirQualityRating();
    }
    // adjust money depending on building money generation then display the new value
    public void DoMoney()
    {
        List<PlacedBuilding>Buildings=GridCreator.GetAllBuildings();
        int Change=GetChangeInMoney(Buildings);
        PlayerMoneyCount += Change;

        DisplayMoney();
    }
    // adjust the player money as long as the game mode is not in sandbox 
    public void AdjustMoney(int Amount)
    {
        if (MainMenu.GetCurrentGameMode() != 0) {
            PlayerMoneyCount += Amount;
            DisplayMoney();
        }
    }
    // display the amount money will change on next tax collection to the UI
    public void DisplayMoneyChange()
    {
        int Change=GetChangeInMoney(GridCreator.GetAllBuildings());
        MoneyChangeText.text =  "Money change:"+Change.ToString();

    }
    // Display how much power will change on next power usage to UI
    public void DisplayPowerChange()
    {
        int Change=GridCreator.GetPowerGeneration()-GridCreator.GetPowerUsage(); ;
        PowerChangeText.text =  "Power change:"+Change.ToString();
    }
    // add buffer to doing check for money so that player loses/gains money at a reasonable rate
    public void CheckForMoneyCheck()
    {
        MoneyCounter++;
        if (MoneyCounter ==FrequencyOfTaxUpdate)
        {
            MoneyCounter = 0;
            DoMoney();
        }
    }
    // each frame do checks for whether money, rating and waste information should be updated
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
    // display current money to UI
    public void DisplayMoney()
    {
        MoneyDisplayText.text = PlayerMoneyCount.ToString();
    }
    // Subtract money from player money in accordance with building placed
    public void DoPlaceBuildingCosts(Building buildingPlaced)
    {
        PlayerMoneyCount -= buildingPlaced.CostToBuild;
        DisplayMoney();
    }
    // call all functions to determine city rating then display
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
    // return the city rating
    public static int GetRating()
    {
        return (int)CurrentRating;
    }
}
