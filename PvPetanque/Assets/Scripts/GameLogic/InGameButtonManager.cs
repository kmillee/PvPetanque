using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameButtonMangager : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject quitConfirmMenu;

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
