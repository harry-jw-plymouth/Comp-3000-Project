using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SavesCanvas; //scene 1
    public GameObject StartButton; // scene 0
    int CurrentScene = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OnStartClicked()
    {
        StartButton.SetActive(false);
        SavesCanvas.SetActive(true);

        
    }
    public void OnGenerateNewClicked()
    {
        SceneManager.LoadScene("GameScene");
    }


}
