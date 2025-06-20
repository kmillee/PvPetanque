using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public GameObject settingsUI;
    public GameObject menuUI;
    public GameObject tutorialUI;

    public void changeToGame()
    {
        SceneManager.LoadScene("GameScene");
    }




    public void switchUI() // open settings and hide menu, or vice verdsa
    {

        if (settingsUI.activeSelf)
        {
            settingsUI.SetActive(false);
            menuUI.SetActive(true);
        }
        else
        {
            settingsUI.SetActive(true);
            menuUI.SetActive(false);
        }

    }

    public void switchTutorial()
    {
        if (tutorialUI.activeSelf)
        {
            tutorialUI.SetActive(false);
            menuUI.SetActive(true);
        }
        else
        {
            tutorialUI.SetActive(true);
            menuUI.SetActive(false);
        }
    }

    public void quitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
    
}
