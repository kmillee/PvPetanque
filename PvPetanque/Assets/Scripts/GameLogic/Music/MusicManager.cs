using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [Header("UI References")]
    public Button toggleSliderButton;
    public GameObject volumeSliderContainer; // parent object for slider (to hide/show)
    public GameObject clickAwayPanel; // assign in Inspector
    public Slider musicSlider;
    public Image musicIcon;

    [Header("Icons")]
    public Sprite musicOnIcon;
    public Sprite musicMutedIcon;

    private const string MusicVolumeKey = "MusicVolume";

    

    void Start()
    {
        // Load saved volume or default to 1
        float savedVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateIcon(savedVolume);

        // Listeners
        toggleSliderButton.onClick.AddListener(ToggleSlider);
        musicSlider.onValueChanged.AddListener(OnVolumeChanged);

    }

    void ToggleSlider()
    {
        bool newState = !volumeSliderContainer.activeSelf;
        volumeSliderContainer.SetActive(newState);
        clickAwayPanel.SetActive(newState); // activate click-away blocker when slider is open
    }

    void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        UpdateIcon(volume);
    }

    void UpdateIcon(float volume)
    {
        musicIcon.sprite = (volume <= 0.01f) ? musicMutedIcon : musicOnIcon;
    }


    public void HideSlider()
    {
        volumeSliderContainer.SetActive(false);
        clickAwayPanel.SetActive(false);
    }

}
