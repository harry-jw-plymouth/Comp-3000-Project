using UnityEngine;

public class MainMenuSoundHandler : MonoBehaviour
{
     public AudioSource ButtonClick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
