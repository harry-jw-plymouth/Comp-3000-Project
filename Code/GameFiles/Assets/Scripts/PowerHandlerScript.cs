using TMPro;
using UnityEngine;

public class PowerHandlerScript : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    [SerializeField] TextMeshProUGUI DisplayText;
    int FrequencyOfPowerUpdate = 100;
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
        PowerReserves += PowerGeneration-PowerUsage;
    //    Debug.Log("Power genersted:" + PowerGeneration);
    //    Debug.Log("Usage:" + PowerUsage);
    //    Debug.Log("Reserves after change:" + PowerReserves);
        if (PowerReserves < 0)
        {
            PowerReserves= 0;
        }

        DisplayText.text = PowerReserves.ToString();




        

    }

}
