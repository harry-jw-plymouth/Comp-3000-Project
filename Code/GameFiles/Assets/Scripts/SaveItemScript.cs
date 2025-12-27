using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class SaveItemScript : MonoBehaviour
{
    [SerializeField] private TMP_Text TextItem;
    private string TextDisplayed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Setup(string TextToDisplay)
    {
        TextItem.text = TextToDisplay;
        TextDisplayed = TextToDisplay;

    }
    public void OnItemClicked()
    {
        Debug.Log("Item clicked: ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
