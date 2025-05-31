using UnityEngine;
using UnityEngine.UI; // or TMPro if using TMP_Dropdown
using TMPro; // If using TextMeshPro for dropdowns
using System.Collections.Generic;

public class MatchSettingsManager : MonoBehaviour
{
    public TMP_Dropdown pointsDropdown; // Or TMP_Dropdown if you're using TextMeshPro
    public int[] availablePointOptions = { 11, 13, 15, 21 };

    public TMP_Text nameDisplayA;
    public TMP_InputField nameInputA;

    public TMP_Text nameDisplayB;
    public TMP_InputField nameInputB;

    // needs to implement a Color class to handle team colors
    // public GameObject colorFieldA;
    // public GameObject colorFieldB;

    void Start()
    {
        SetupDropdown();

        nameDisplayA.text = MatchSettingsData.teamNameA;
        nameInputA.text = MatchSettingsData.teamNameA;
        nameInputA.gameObject.SetActive(false);

        nameDisplayB.text = MatchSettingsData.teamNameB;
        nameInputB.text = MatchSettingsData.teamNameB;
        nameInputB.gameObject.SetActive(false);

        // Add listeners
        nameInputA.onEndEdit.AddListener(delegate { FinishNameEditA(); });
        nameInputB.onEndEdit.AddListener(delegate { FinishNameEditB(); });
    }

    void SetupDropdown()
    {
        pointsDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (int points in availablePointOptions)
        {
            options.Add(points.ToString());
        }

        pointsDropdown.AddOptions(options);

        // Default selection (13)
        int defaultIndex = System.Array.IndexOf(availablePointOptions, 13);
        pointsDropdown.value = defaultIndex;
        pointsDropdown.RefreshShownValue();

        // Assign listener
        pointsDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        int selectedPoints = availablePointOptions[index];
        MatchSettingsData.goalScore = selectedPoints;
        Debug.Log("Points to Win set to: " + selectedPoints);
    }


    /// <summary>
    /// ///// Name editing methods for Team A and Team B
    /// 
    /// When clicking on the button to edit the team name,
    /// it hides the display text and shows the input field.
    /// /// When the user finishes editing (e.g., by pressing Enter or clicking outside),
    /// /// it updates the team name in MatchSettingsData and hides the input field,
    /// /// showing the updated name in the display text.
    /// </summary>
    public void StartNameEditA()
    {
        nameDisplayA.gameObject.SetActive(false);
        nameInputA.text = MatchSettingsData.teamNameA;
        nameInputA.gameObject.SetActive(true);
        nameInputA.Select();
        nameInputA.ActivateInputField();
    }

    public void FinishNameEditA()
    {
        string newName = nameInputA.text.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            MatchSettingsData.teamNameA = newName;
        }

        nameDisplayA.text = MatchSettingsData.teamNameA;
        nameInputA.gameObject.SetActive(false);
        nameDisplayA.gameObject.SetActive(true);
    }

    public void StartNameEditB()
    {
        nameDisplayB.gameObject.SetActive(false);
        nameInputB.text = MatchSettingsData.teamNameB;
        nameInputB.gameObject.SetActive(true);
        nameInputB.Select();
        nameInputB.ActivateInputField();
    }

    public void FinishNameEditB()
    {
        string newName = nameInputB.text.Trim();
        if (!string.IsNullOrEmpty(newName))
        {
            MatchSettingsData.teamNameB = newName;
        }

        nameDisplayB.text = MatchSettingsData.teamNameB;
        nameInputB.gameObject.SetActive(false);
        nameDisplayB.gameObject.SetActive(true);
    }


}
