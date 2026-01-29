using TMPro;
using UnityEngine;

public class GameStatusScript : MonoBehaviour
{
    [SerializeField] NPChandler npcHandler;
    [SerializeField] TextMeshProUGUI DisplayText;
    int FrequencyOfRatingUpdate = 1000;
    int FrequencyCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        RatingInfo CurrentInfo = new RatingInfo();
        CurrentInfo.SetHomeLessPercentage ( npcHandler.GetHomeLessPercentage());


        //Calculate rating
        CurrentInfo.CalulcateRating();
        float Rating = CurrentInfo.GetRating();
        //display rating
        DisplayText.text = Rating.ToString();

    }
    
}
