using UnityEngine;

public class ColourBlindCameraController : MonoBehaviour
{
    public Material material;

    public enum ColourBlindMode
    {
        Normal=0,
        Protanopia=1,
        Deuteranopia=2,
        Tritanopia=3
    }

    public ColourBlindMode mode = ColourBlindMode.Normal;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetInt("_Mode", (int)mode);
            Graphics.Blit(source, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    public void SetMode(int NewMode)
    {
        mode = (ColourBlindMode)NewMode;
        SavePreference();
        Debug.Log("Colour Blind Mode set to: " + mode.ToString());
    }
    public void LoadPreference()
    {
        if (PlayerPrefs.HasKey("ColourBlindMode"))
        {
            mode = (ColourBlindMode)PlayerPrefs.GetInt("ColourBlindMode");
        }
        else
        {
            mode = ColourBlindMode.Normal;
        }
    }
    public void SavePreference()
    {
        PlayerPrefs.SetInt("ColourBlindMode", (int)mode);
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
