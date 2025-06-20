using UnityEngine;
using UnityEngine.SceneManagement;

public class inGameButtonMangager : MonoBehaviour
{
    public GameObject quitConfirmMenu;

    public void changeToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // ask player to confirm quitting (menu appears or disappears)
    public void switchQuitUI()
    {
        if (quitConfirmMenu.activeSelf)
        {
            quitConfirmMenu.SetActive(false);
        }
        else
        {
            quitConfirmMenu.SetActive(true);
        }

    }
}
