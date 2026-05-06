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
    //maximum amount of power a city can hold at one time
    int BasePowerCap = 25000;

    bool EmptyWarningDone = false;

    [SerializeField] int PowerReserves = 10000;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PowerReserves=SetPowerOnStart();
        UpdatePowerDisplay();
    }
    // Set power to 10000 if this is a new save file or the value in the save file if loading existing save file
    public int SetPowerOnStart()
    {
        if (MainMenu.CurrentSaveID != -1)
        {
            Debug.Log("Save file found, setting power to saved amount");
            return DBManager.GetSpecificSaveFile(MainMenu.CurrentSaveID).Power;
        }
        Debug.Log("No save file found, setting power to default amount");
        return 10000;
    }
    // return players current power reserves
    public int GetPowerReserves()
    {
        return PowerReserves;
    }
    // Update is called once per frame 
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
    // if electricity runs out and player doesnt act display pop up accordingly
    void DoPowerEmpty()
    {
        if (!EmptyWarningDone)
        {
            EmptyWarningDone = true;
            uiHandler.OpenNewPopUp("Power warning", "Power reserves are empty, buy some power and get some power generators going");
        }
        WarningCounter++;
        if (WarningCounter == WarningLimit) {
            // Complete power failure 
            uiHandler.OpenNewPopUp ("Your city is without power","Various city functions are now not working");
        }
    }
    //return maximum power the player can have at any one time
    int GetPowerCap()
    {
        return BasePowerCap;
    } 
    // increase/decrease power then update UI to display it, only change power if game mode is standard
    public void AdjustPower(int power)
    {
        if (MainMenu.GetCurrentGameMode() != 0) {
            PowerReserves += power;
        }
       
        UpdatePowerDisplay();
    }
    // get if the player has enough power for the amount they want to sell
    public bool GetIfEnoughPowerForSell(int AmountToSell)
    {
        return AmountToSell <= PowerReserves;
    }
    // update UI to display current power
    private void UpdatePowerDisplay() {
        DisplayText.text = PowerReserves.ToString() + "/" + GetPowerCap();
    }
    // if power is empty do handling for empty power, then update power amount and update power displayed in UI
    void ConsumePower()
    {
        int PowerGeneration = GridCreator.GetPowerGeneration();
        int PowerUsage=GridCreator.GetPowerUsage();
        PowerReserves += PowerGeneration-PowerUsage;

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
