using UnityEngine;

public class ColourBlindCameraController : MonoBehaviour
{
    public Material material;

    public int BlindMode = 0;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetInt("_Mode",BlindMode);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    // Update the mode for colour blind mode
    public void SetMode(int NewMode)
    {
        BlindMode = NewMode;
        SavePreference();
    }
    // Get preferences from saved player prefernces for persistent setting of colour blind mode
    public void LoadPreference()
    {
        if (PlayerPrefs.HasKey("ColourBlindMode"))
        {
            BlindMode = PlayerPrefs.GetInt("ColourBlindMode");
        }
        else
        {
            BlindMode = 0;
        }
    }
    // save player preferences 
    public void SavePreference()
    {
        PlayerPrefs.SetInt("ColourBlindMode", BlindMode);
        PlayerPrefs.Save();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadPreference();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
