using UnityEngine;

public class MainMenuSoundHandler : MonoBehaviour
{
    public AudioSource MainMenuBackgroundOST;
     public AudioSource ButtonClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MainMenuBackgroundOST.Play();
        MainMenuBackgroundOST.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClickSound()
    {
        ButtonClick.Play();
    }
}
