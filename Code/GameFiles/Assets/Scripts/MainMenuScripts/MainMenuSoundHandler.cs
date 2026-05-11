using UnityEditor;
using UnityEngine;

public class MainMenuSoundHandler : MonoBehaviour
{
    [SerializeField]static public float MusicVolume = 0.5f;
   [SerializeField] static float SFXVolume = 0.5f;

    public MainMenuSoundHandler MMSoundHandler;

    public AudioSource MainMenuBackgroundOST;
     public AudioSource ButtonClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPreferences();
        // play background ost and set it to loop
        MainMenuBackgroundOST.Play();
        MainMenuBackgroundOST.loop = true;
        UpdateVolume();
    }
    public void UpdateVolume()
    {
       MainMenuBackgroundOST.volume = MusicVolume;

        ButtonClick.volume = SFXVolume;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public static void PassSettings(float music, float Sfx)
    {
        PlayerPrefs.SetFloat("MainMenuMusicVolume", music);
        PlayerPrefs.SetFloat("MainMenuSFXVolume", Sfx);
        PlayerPrefs.Save();

    }
    public void LoadPreferences()
    {
        if (PlayerPrefs.HasKey("MainMenuMusicVolume"))
        {
            MusicVolume = PlayerPrefs.GetFloat("MainMenuMusicVolume");
        }
        else
        {
            MusicVolume = 0.5f;
        }
        if (PlayerPrefs.HasKey("MainMenuSFXVolume"))
        {
            SFXVolume = PlayerPrefs.GetFloat("MainMenuSFXVolume");
        }
        else
        {
            SFXVolume = 0.5f;
        }
    }
    // play sound effect for button clicks
    public void PlayButtonClickSound()
    {
        
        ButtonClick.Play();
    }
}
