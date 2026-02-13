using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PowerHandlerScript : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    [SerializeField] TextMeshProUGUI DisplayText;
    int FrequencyOfPowerUpdate = 100;
    int FrequencyCounter = 0;

    int BasePowerCap = 25000;

    [SerializeField] int PowerReserves = 10000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePowerDisplay();
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
    void DoPowerEmpty()
    {

    }
    int GetPowerCap()
    {
        return BasePowerCap;
    }
    private void UpdatePowerDisplay() {
        DisplayText.text = PowerReserves.ToString() + "/" + GetPowerCap();
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
            DoPowerEmpty();
        }
        if (PowerReserves > GetPowerCap())
        {
            PowerReserves = GetPowerCap();
        }
        UpdatePowerDisplay();
        




        

    }

}
