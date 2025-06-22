using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameButtonMangager : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject quitConfirmMenu;
    public GameObject helpMenu;

    public void changeToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void switchSettingsUI()
    {
        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
        }
        else
        {
            settingsMenu.SetActive(true);
        }
    }

    public void switchHelpUI()
    {
        if (helpMenu.activeSelf)
        {
            settingsMenu.SetActive(true);
            helpMenu.SetActive(false);
        }
        else
        {
            settingsMenu.SetActive(false);
            helpMenu.SetActive(true);
        }
    }

    // ask player to confirm quitting (menu appears or disappears)
    public void switchQuitUI()
    {
        if (quitConfirmMenu.activeSelf)
        {
            quitConfirmMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(false);
            quitConfirmMenu.SetActive(true);
        }

    }

    
}
