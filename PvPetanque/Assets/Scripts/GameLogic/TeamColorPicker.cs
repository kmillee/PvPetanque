using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ColorOption
{
    
    public string colorName;
    public Color color;
}


public class TeamColorPicker : MonoBehaviour
{
    public GameObject pickerPanel;
    public Team currentTeamToModify;

    public Transform topLineContainer;
    public Transform bottomLineContainer;

    public List<ColorOption> availableColors;

    private List<Button> allColorButtons = new List<Button>();

    //UI elements to modify
    public GameObject TeamAPanel;
    public GameObject TeamBPanel;

    private void Awake()
    {
        SetupColorButtons();
    }

    public void OpenColorPickerForTeamA()
    {
        OpenColorPicker(Team.TeamA);
    }
    public void OpenColorPickerForTeamB()
    {
        OpenColorPicker(Team.TeamB);
    }

    public void OpenColorPicker(Team team)
    {
        currentTeamToModify = team;
        pickerPanel.SetActive(true);
        UpdateColorButtons(); // update options based on opposite team color
    }

    public void CloseColorPicker()
    {
        pickerPanel.SetActive(false);
    }

    void SetupColorButtons()
    {
        allColorButtons.Clear();

        // Combine both lines into one list of buttons
        foreach (Transform child in topLineContainer)

            allColorButtons.Add(child.GetComponent<Button>());

        foreach (Transform child in bottomLineContainer)
            allColorButtons.Add(child.GetComponent<Button>());

        for (int i = 0; i < allColorButtons.Count && i < availableColors.Count; i++)
        {
            var button = allColorButtons[i];
            var option = availableColors[i];

            // Set color visually
            button.GetComponent<Image>().color = option.color;

            // Set label
            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.text = option.colorName;
                label.color = GetTextColorForBackground(option.color); // Set text color for contrast

            // Clear old listeners
            button.onClick.RemoveAllListeners();

            // Set new action
            Color colorCopy = option.color;
            button.onClick.AddListener(() => ApplyTeamColor(colorCopy));

        }
    }

    void ApplyTeamColor(Color selectedColor)
    {
        Debug.Log($"Color {selectedColor} applied to {currentTeamToModify}");

        if (currentTeamToModify == Team.TeamA)
        {
            MatchSettingsData.teamColorA = selectedColor;
            TeamAPanel.GetComponent<Image>().color = selectedColor; // Update UI panel color
            TeamAPanel.GetComponentInChildren<TextMeshProUGUI>().color = GetTextColorForBackground(selectedColor);
            

        }
        else
        {
            MatchSettingsData.teamColorB = selectedColor;
            TeamBPanel.GetComponent<Image>().color = selectedColor; // Update UI panel color
            TeamBPanel.GetComponentInChildren<TextMeshProUGUI>().color = GetTextColorForBackground(selectedColor);
        }

        CloseColorPicker();
    }


    // Update the color buttons to disable those that match the other team's color
    private void UpdateColorButtons()
    {
        Color otherTeamColor = currentTeamToModify == Team.TeamA
            ? MatchSettingsData.teamColorB
            : MatchSettingsData.teamColorA;

        for (int i = 0; i < allColorButtons.Count; i++)
        {
            var button = allColorButtons[i];
            var optionColor = availableColors[i].color; 

            bool shouldEnable = !ColorsApproximatelyEqual(optionColor, otherTeamColor);
            button.interactable = shouldEnable;

            // Update button colors based on whether it is enabled or not
            var colors = button.colors;
            colors.normalColor = shouldEnable ? optionColor : new Color(0.5f, 0.5f, 0.5f, 1f);
            colors.highlightedColor = shouldEnable ? optionColor * 1.2f : new Color(0.6f, 0.6f, 0.6f, 1f);
            colors.pressedColor = shouldEnable ? optionColor * 0.9f : new Color(0.4f, 0.4f, 0.4f, 1f);
            button.colors = colors;
        }
    }

    // Check if two colors are approximately equal
    private bool ColorsApproximatelyEqual(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
            Mathf.Approximately(a.g, b.g) &&
            Mathf.Approximately(a.b, b.b);
    }
    
    // Adapt color based on the background color for better contrast
    public Color GetTextColorForBackground(Color bgColor)
    {
        float luminance = 0.2126f * bgColor.r + 0.7152f * bgColor.g + 0.0722f * bgColor.b; // luminance using  formula for brightness
        return luminance > 0.5f ? Color.black : Color.white;
    }


}
