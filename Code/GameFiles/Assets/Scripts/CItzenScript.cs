using UnityEngine;

public class CItzenScript : MonoBehaviour
{
    public ScriptedCitzen CitzenInfo;
    public SpriteRenderer Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        SetUp();

    }
    void SetUp()
    {
        Renderer.sprite=CitzenInfo.Sprite;
        gameObject.name = CitzenInfo.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
