using UnityEngine;
using UnityEngine.UI; // or TMPro if using TMP_Dropdown
using TMPro; // If using TextMeshPro for dropdowns
using System.Collections.Generic;

public class MatchSettingsManager : MonoBehaviour
{
    [Header("Points Dropdown")]
    [SerializeField] private TMP_Dropdown pointsDropdown;
    [SerializeField] private int[] availablePointOptions = { 11, 13, 15, 21 };

    [Header("Ball Count Dropdown")]
    [SerializeField] private TMP_Dropdown ballsDropdown;
    [SerializeField] private int[] availableBallOptions = { 2, 4, 6, 8, 10 };

    [Header("Team Name A")]
    [SerializeField] private TMP_Text nameDisplayA;
    [SerializeField] private TMP_InputField nameInputA;

    [Header("Team Name B")]
    [SerializeField] private TMP_Text nameDisplayB;
    [SerializeField] private TMP_InputField nameInputB;



    void Start()
    {
        SetupPointsDropdown();
        SetupBallsDropdown();

        // Team name setup
        nameDisplayA.text = MatchSettingsData.teamNameA;
        nameInputA.text = MatchSettingsData.teamNameA;
        nameInputA.gameObject.SetActive(false);

        nameDisplayB.text = MatchSettingsData.teamNameB;
        nameInputB.text = MatchSettingsData.teamNameB;
        nameInputB.gameObject.SetActive(false);

        // Add listeners for names
        nameInputA.onEndEdit.AddListener(delegate { FinishNameEditA(); });
        nameInputB.onEndEdit.AddListener(delegate { FinishNameEditB(); });
    }

    void SetupPointsDropdown()
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
        pointsDropdown.onValueChanged.AddListener(OnPointsDropdownChanged);
    }

    void OnPointsDropdownChanged(int index)
    {
        int selectedPoints = availablePointOptions[index];
        MatchSettingsData.goalScore = selectedPoints;
        Debug.Log("Points to Win set to: " + selectedPoints);
    }

    private void SetupBallsDropdown()
    {
        ballsDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (int balls in availableBallOptions)
            options.Add(balls.ToString());

        ballsDropdown.AddOptions(options);
        // Default selection (6)
        int defaultIndex = System.Array.IndexOf(availableBallOptions, 6);
        ballsDropdown.value = defaultIndex;
        ballsDropdown.RefreshShownValue();
        ballsDropdown.onValueChanged.AddListener(OnBallsDropdownChanged);
    }

    private void OnBallsDropdownChanged(int index)
    {
        int selectedBalls = availableBallOptions[index];
        MatchSettingsData.ballsPerTeam = selectedBalls;
        Debug.Log("Balls per Team set to: " + selectedBalls);
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
