using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TutorialPage[] pages;
    public TextMeshProUGUI textBox;
    public Image imageBox;
    public VideoPlayer videoPlayer;
    public GameObject nextButton;
    public GameObject prevButton;

    private int currentIndex = 0;

    void Start()
    {
        ShowPage(0);
    }

    public void ShowPage(int index)
    {
        if (index < 0 || index >= pages.Length) return;

        currentIndex = index;

        var page = pages[index];

        textBox.text = page.pageText;

        if (page.pageImage != null)
        {
            imageBox.gameObject.SetActive(true);
            imageBox.sprite = page.pageImage;
        }
        else
        {
            imageBox.gameObject.SetActive(false);
        }

        if (page.pageVideo != null)
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.clip = page.pageVideo;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }

        prevButton.SetActive(currentIndex > 0);
        nextButton.SetActive(currentIndex < pages.Length - 1);
    }

    public void NextPage()
    {
        ShowPage(currentIndex + 1);
    }

    public void PrevPage()
    {
        ShowPage(currentIndex - 1);
    }
}
