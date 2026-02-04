using TMPro;
using UnityEngine;

public class PowerHandlerScript : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    int FrequencyOfPowerUpdate = 10000;
    int FrequencyCounter = 0;

    [SerializeField] int PowerReserves = 10000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        FrequencyCounter++;
        if (FrequencyCounter == FrequencyOfPowerUpdate)
        {
            FrequencyCounter = 0;
            ConsumePower();

        }
    }
    void ConsumePower()
    {
        int PowerGeneration = GridCreator.GetPowerGeneration();
        int PowerUsage=GridCreator.GetPowerUsage();


        

    }

}
