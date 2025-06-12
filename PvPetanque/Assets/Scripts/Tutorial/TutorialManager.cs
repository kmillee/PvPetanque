using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialPage[] pages;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private Image imageBox;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;
    

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

    // Set text
    titleText.text = page.pageTitle;
    textBox.text = page.pageText;

    // Show or hide image
    if (page.pageImage != null)
    {
        imageBox.gameObject.SetActive(true);
        imageBox.sprite = page.pageImage;
    }
    else
    {
        imageBox.gameObject.SetActive(false);
    }

    // Show or hide video
    if (page.pageVideo != null)
    {
        videoPlayer.gameObject.SetActive(true);
        videoPlayer.clip = page.pageVideo;
        videoPlayer.isLooping = true; // 🔁 Loop the video
        videoPlayer.Play();
    }
    else
    {
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false);
    }
    rawImage.gameObject.SetActive(page.pageVideo != null);

    // Navigation buttons visibility
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
