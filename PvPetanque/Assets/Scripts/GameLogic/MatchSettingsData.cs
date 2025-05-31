public static class MatchSettingsData
{
    // Match settings data class to store and manage game settings
    public static int goalScore = 13;
    public static int ballsPerTeam = 6;
    public static string teamNameA = "Team A";
    public static string teamNameB = "Team B";
    public static string teamColorA = "red";
    public static string teamColorB = "blue";
    // public static Color Team1Color = Color.red;
    // public static Color Team2Color = Color.blue;
    
    public static Team firstTeam = Team.TeamA; // Default first team


    // Method to reset settings to default values
    public static void ResetToDefaults()
    {

        goalScore = 13;
        ballsPerTeam = 6;
        teamNameA = "Team A";
        teamNameB = "Team B";
        teamColorA = "red";
        teamColorB = "blue";
        firstTeam = Team.TeamA;
    }
}