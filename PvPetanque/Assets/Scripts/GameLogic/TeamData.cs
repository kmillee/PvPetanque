// Allow for quick access to team data in a game setting (from Team enum to MatchSettingsData).
using UnityEngine;
public static class TeamData
{
    public static string GetTeamName(Team team)
    {
        return team == Team.TeamA ? MatchSettingsData.teamNameA : MatchSettingsData.teamNameB;
    }

    public static Color GetTeamColor(Team team)
    {
        return team == Team.TeamA ? MatchSettingsData.teamColorA : MatchSettingsData.teamColorB;
    }




}
