// Allow for quick access to team data in a game setting (from Team enum to MatchSettingsData).
using UnityEngine;
public static class TeamData
{
    public static string GetTeamName(Team team)
    {
        return team == Team.TeamA ? MatchSettingsData.teamNameA : MatchSettingsData.teamNameB;
    }

    public static string GetTeamColor(Team team)
    {
        return team == Team.TeamA ? MatchSettingsData.teamColorA : MatchSettingsData.teamColorB;
    }

    public static Color GetUnityColor(Team team)
    {
        // Convert string color to Unity Color (extend this as needed)
        string colorStr = GetTeamColor(team).ToLower();
        return colorStr switch
        {
            "red" => Color.red,
            "blue" => Color.blue,
            "green" => Color.green,
            "yellow" => Color.yellow,
            _ => Color.white
        };
    }
}
