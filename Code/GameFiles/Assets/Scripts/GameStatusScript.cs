using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GameStatusScript : MonoBehaviour
{
    [SerializeField] NPChandler npcHandler;
    [SerializeField] GridCreator gridCreator;
    [SerializeField] TextMeshProUGUI DisplayText;
    int FrequencyOfRatingUpdate = 1000;
    int FrequencyCounter = 0;
    [SerializeField] static RatingInfo CurrentInfo = new RatingInfo();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public static List<string> GetReport()
    {
        List<string> Info=CurrentInfo.GetReport();
        CurrentInfo.ClearReports();
        return Info;
    }

    // Update is called once per frame
    void Update()
    {
        FrequencyCounter++;
        if(FrequencyCounter == FrequencyOfRatingUpdate)
        {
            FrequencyCounter = 0;
            CalculateCityRating();

        }
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
        CurrentInfo.SetPowerRating(GridCreator.GetPowerGeneration(), GridCreator.GetPowerGeneration());

        //Calculate rating
        CurrentInfo.CalulcateRating();
        float Rating = CurrentInfo.GetRating();
        //display rating
        DisplayText.text = Rating.ToString();

    }
    
}
