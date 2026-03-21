using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource BackgroundOST;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        BackgroundOST.Play();
        LoadPreferences();


        BackgroundOST.volume = MusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
