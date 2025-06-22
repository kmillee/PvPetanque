using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private TutorialPage[] allPages;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scrollableTextBox;
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;

    [SerializeField] private RectTransform textArea;
    [SerializeField] private GameObject mediaColumn;
    [SerializeField] private GameObject imagePrefab; 
    [SerializeField] private GameObject videoPrefab; 




    private List<TutorialPage> currentPages = new List<TutorialPage>();
    private int currentIndex = 0;

    void Start()
    {
        SelectCategory(TutorialCategory.Gameplay); // Default category
    }

    public void SelectCategory(TutorialCategory category)
    {
        currentPages = allPages
            .Where(p => p.category == category)
            .OrderBy(p => p.name) // Optional: control order
            .ToList();

        ShowPage(0);
    }

public void ShowPage(int index)
{
    if (currentPages == null || index < 0 || index >= currentPages.Count) return;

    currentIndex = index;
    var page = currentPages[index];

    titleText.text = page.pageTitle;
    scrollableTextBox.text = page.pageText;

    // Navigation buttons
    prevButton.SetActive(currentIndex > 0);
    nextButton.SetActive(currentIndex < currentPages.Count - 1);

    // Clear previous media
    foreach (Transform child in mediaColumn.transform)
        Destroy(child.gameObject);

    bool hasMedia = false;

    // Image
    if (page.pageImage != null)
    {
        hasMedia = true;
        GameObject imageGO = Instantiate(imagePrefab, mediaColumn.transform);
        imageGO.GetComponent<Image>().sprite = page.pageImage;
    }

    // Video
    if (page.pageVideo != null)
    {
        hasMedia = true;
        GameObject videoGO = Instantiate(videoPrefab, mediaColumn.transform);
        var player = videoGO.GetComponent<VideoPlayer>();
        player.clip = page.pageVideo;
        player.isLooping = true;
        player.Play();
    }

    // Enable or disable media column
    mediaColumn.SetActive(hasMedia);

    // Adjust text area width dynamically



}

    public void NextPage() => ShowPage(currentIndex + 1);
    public void PrevPage() => ShowPage(currentIndex - 1);

    public void SelectCategoryByIndex(int index)
    {
        SelectCategory((TutorialCategory)index);
    }

}
