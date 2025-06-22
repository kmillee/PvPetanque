using UnityEngine;
using System.Collections.Generic;

public static class MatchSettingsData
{
    // Match settings data class to store and manage game settings
    [Header("Match Settings")]
    public static Team firstTeam = Team.TeamA; // Default first team

    public static int goalScore = 13;
    public static int ballsPerTeam = 6;

    // These items are available to pick from
    public static List<GameEffect> availableItems = new List<GameEffect>();

    // // These are the selected items (used in the match)
    public static HashSet<GameEffect> selectedItems = new HashSet<GameEffect>();


    //team names and colors
    [Header("Teams")]
    public static string teamNameA = "Team A";
    public static string teamNameB = "Team B";

    public static Color teamColorA = Color.red;
    public static Color teamColorB = Color.blue;


    // Method to reset settings to default values
    public static void ResetToDefaults()
    {
        goalScore = 13;
        ballsPerTeam = 6;
        teamNameA = "Team A";
        teamNameB = "Team B";
        teamColorA = Color.red;
        teamColorB = Color.blue;
        firstTeam = Team.TeamA;

        for (int i = 0; i < availableItems.Count; i++)
        {

            selectedItems.Add(availableItems[i]);

        }

        // selectedItems.Clear(); // clear selection
    }

    public static void InitializeItems(List<GameEffect> items)
    {
        availableItems.Clear();
        availableItems.AddRange(items);

        selectedItems.Clear();
        foreach (var item in items)
        {
            selectedItems.Add(item);
        }
    }


}