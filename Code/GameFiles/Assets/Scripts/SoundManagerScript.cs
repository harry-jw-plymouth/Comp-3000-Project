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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        BackgroundOST.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
