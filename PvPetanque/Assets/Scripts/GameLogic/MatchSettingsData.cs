using UnityEngine;
using System.Collections.Generic;

public static class MatchSettingsData
{
    // Match settings data class to store and manage game settings
    public static int goalScore = 13;
    public static int ballsPerTeam = 6;
    public static string teamNameA = "Team A";
    public static string teamNameB = "Team B";

    public static Color teamColorA = Color.red;
    public static Color teamColorB = Color.blue;

    public static Team firstTeam = Team.TeamA; // Default first team


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
    }
}