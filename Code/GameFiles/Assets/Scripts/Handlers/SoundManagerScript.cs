using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource BackgroundOST;
    public AudioSource ButtonClick;
    public AudioSource ShopPlacement;
    public AudioSource RemoveBuilding;
    public AudioSource AddBuilding;
    public AudioSource StartBus;
    public AudioSource StartTrain;
    public AudioSource EditTile;

    public AudioSource WaterAmbience;
    public AudioSource MainAmbience;

    public bool WaterCurrentlyPlaying=false;
    float MusicVolume = 0.5f;
    float SFXVolume = 0.5f;

    private void Awake()
    {
        BackgroundOST = GetComponent<AudioSource>();
    }
    public void ChangeMusicVolume(float newVolume)
    {
        MusicVolume = newVolume;
        UpdateVolume();

    } 
    public void UpdateVolume()
    {
         BackgroundOST.volume = MusicVolume;

        ButtonClick.volume = SFXVolume;
        ShopPlacement.volume = SFXVolume;
        RemoveBuilding.volume = SFXVolume;
        AddBuilding.volume = SFXVolume;
        StartBus.volume = SFXVolume;

        MainAmbience.volume = SFXVolume;
        WaterAmbience.volume = SFXVolume;
    }
    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }
    public void LoadPreferences()
    {
        if(PlayerPrefs.HasKey("MusicVolume"))
        {
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            MusicVolume = 0.5f;
        }
        if(PlayerPrefs.HasKey("SFXVolume"))
        {
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
        else
        {
            SFXVolume = 0.5f;
        }
    }
    void DoAmbience()
    {
        if (WaterCurrentlyPlaying)
        {
            if (!GridCreator.GetIfWaterExists())
            {
                WaterAmbience.Stop();
            }
        }
        else
        {
            if (!GridCreator.GetIfWaterExists())
            {
                WaterAmbience.Play();
            }
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainAmbience.loop = true;
        WaterAmbience.loop = true;
        BackgroundOST.Play();
        LoadPreferences();


        BackgroundOST.volume = MusicVolume;
    }
    public void PlayButtonClick()
    {
        ButtonClick.Play();
    }
    public void PlayShopSoundEffect()
    {
        ShopPlacement.Play();
    }
    public void PlayBuildingRemove()
    {
        RemoveBuilding.Play();
    }
    public void PlayPlaceBuilding()
    {
        AddBuilding.Play();
    }
    public void PlayStartBus()
    {
        StartBus.Play();
    }
    public void PlayStartTrain()
    {
        StartTrain.Play();
    }
    public void PlayEditTile()
    {
        EditTile.Play();
    }
    // Update is called once per frame
    void Update()
    {
        DoAmbience();
    }
}
