using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject creditsPanel;
    public GameObject howToplay;
    void Update()
    {
        if (creditsPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCredits();
            CloseHTP();
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("room_1"); // Loads the first game level
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true); // Shows the credits panel
    }

    public void OpenHTP()
    {
        howToplay.SetActive(true); // Shows the credits panel
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false); // Hides the credits panel
    }
    public void CloseHTP()
    {
        howToplay.SetActive(false); // Hides the credits panel
    }
    public void QuitGame()
    {
        Application.Quit(); // Exits the game (only works in a built game)
    }
}
