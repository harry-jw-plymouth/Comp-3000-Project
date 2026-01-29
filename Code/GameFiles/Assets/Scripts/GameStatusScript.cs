using UnityEngine;

public class GameStatusScript : MonoBehaviour
{
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

        }
    }
    void CalculateCityRating()
    {

    }
    
}
