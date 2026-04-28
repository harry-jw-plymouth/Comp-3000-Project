using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PowerHandlerScript : MonoBehaviour
{
    [SerializeField] GridCreator gridCreator;
    [SerializeField] TextMeshProUGUI DisplayText;
    int FrequencyOfPowerUpdate = 100;
    int FrequencyCounter = 0;
    int WarningCounter = 0;
    int WarningLimit = 50;

    public UIHandlerScript uiHandler;
    int BasePowerCap = 25000;

    bool EmptyWarningDone = false;

    [SerializeField] int PowerReserves = 10000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PowerReserves=SetPowerOnStart();
        UpdatePowerDisplay();
    }
    // Update is called once per frame
    public int SetPowerOnStart()
    {
        if (MainMenu.CurrentSaveID != -1)
        {
            Debug.Log("Save file found, setting power to saved amount");
            Debug.Log("Power amount:" + DBManager.GetSpecificSaveFile(MainMenu.CurrentSaveID).Power);
            return DBManager.GetSpecificSaveFile(MainMenu.CurrentSaveID).Power;
        }
        Debug.Log("No save file found, setting power to default amount");
        return 10000;
    }
    public int GetPowerReserves()
    {
        return PowerReserves;
    }
    void Update()
    {
        FrequencyCounter++;
        if (FrequencyCounter == FrequencyOfPowerUpdate)
        {
            if (MainMenu.GetCurrentGameMode() != 0)
            {
                FrequencyCounter = 0;
                ConsumePower();
            }

        }
    }
    void DoPowerEmpty()
    {
        if (!EmptyWarningDone)
        {
            EmptyWarningDone = true;
            uiHandler.ShowAlertPopUp("Power reserves are empty, buy some power and get some power generators going");
        }
        WarningCounter++;
        Debug.Log("Warning counter" + WarningCounter);
        if (WarningCounter == WarningLimit) {
            // Complete power failure 
            Debug.Log("Critical power failure");
            uiHandler.ShowAlertPopUp("Your city is without power");
        }
    }
    int GetPowerCap()
    {
        return BasePowerCap;
    }
    public void AdjustPower(int power)
    {
        PowerReserves+=power;
        UpdatePowerDisplay();
    }
    public bool GetIfEnoughPowerForSell(int AmountToSell)
    {
        return AmountToSell <= PowerReserves;
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
        if (PowerReserves < 0||PowerReserves==0)
        {
            PowerReserves= 0;
            DoPowerEmpty();
        }
        else
        {
            WarningCounter = 0;
            EmptyWarningDone = false;
        }
        if (PowerReserves > GetPowerCap())
        {
            PowerReserves = GetPowerCap();
        }
        UpdatePowerDisplay();
        




        

    }

}
