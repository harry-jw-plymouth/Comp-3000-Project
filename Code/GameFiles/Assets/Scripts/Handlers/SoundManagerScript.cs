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
    // Set music volume to new value
    public void ChangeMusicVolume(float newVolume)
    {
        MusicVolume = newVolume;
        UpdateVolume();

    }
    // Set SFX volume to new value
    public void ChangeSFXVolume(float newVolume)
    {
        SFXVolume = newVolume;
        UpdateVolume();

    }
    //update each sound to play at the correct volume
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
    // save the players prefernces
    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
        PlayerPrefs.Save();
    }
    // return the volume set for SFX
    public float GetSFXVolume()
    {
        return SFXVolume;
    }
    //  Return the volume selected for music
    public float GetMusicVolume()
    {
        return MusicVolume;
    }
    // Load player prefernces for volume
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
    // check if conditions are met for playing ambience, play/pause accordingly
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
    // start playing ambience and background sound and set volume based on player preferences
    void Start()
    {
        MainAmbience.loop = true;
        WaterAmbience.loop = true;
        BackgroundOST.Play();
        LoadPreferences();


        BackgroundOST.volume = MusicVolume;
    }
    // play click sound for when buttons are clicked
    public void PlayButtonClick()
    {
        ButtonClick.Play();
    }
    // play sound effect for when shop buildings are placed
    public void PlayShopSoundEffect()
    {
        ShopPlacement.Play();
    }
    // play destruction sound when buildings removed
    public void PlayBuildingRemove()
    {
        RemoveBuilding.Play();
    }
    // play construction sound for when buildings are placed
    public void PlayPlaceBuilding()
    {
        AddBuilding.Play();
    }
    // play sound for when buses start running
    public void PlayStartBus()
    {
        StartBus.Play();
    }
    // play sound for when trains start running
    public void PlayStartTrain()
    {
        StartTrain.Play();
    }
    // play digging sound for editing tiles
    public void PlayEditTile()
    {
        EditTile.Play();
    }
    // Update is called once per frame
    void Update()
    {
        // check conditions met for playing ambience
        DoAmbience();
    }
}