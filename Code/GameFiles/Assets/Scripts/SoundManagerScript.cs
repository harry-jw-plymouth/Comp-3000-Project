using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource BackgroundOST;

    private void Awake()
    {
        BackgroundOST = GetComponent<AudioSource>();
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
