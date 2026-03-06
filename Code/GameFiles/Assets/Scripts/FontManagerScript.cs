using TMPro;
using UnityEngine;

public class FontManagerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TMP_FontAsset ArcadeFont;

    void Start()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            text.font = ArcadeFont;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
