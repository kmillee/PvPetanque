using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public enum RoundPhase //should add draw phase maybe (or will be done in the menu)
{
    CochonnetThrow,
    PlayerTurn,
    EndRound,
    EndGame,
    
}




// TODO: for now, a new turn begins with teamA, needs to introduce a draw phase
public class GameManager : MonoBehaviour
{
    public static GameManager instance; //singleton instance

    [Header("UI")]
    [SerializeField] public GameObject teamAPanel;
    [SerializeField] public GameObject teamBPanel;

    [SerializeField] public GameObject teamAScorePanel;
    [SerializeField] public GameObject teamBScorePanel;
    [SerializeField] public TextMeshProUGUI teamANameText; // Text element for team A name
    [SerializeField] public TextMeshProUGUI teamBNameText; // Text element for team B name
    [SerializeField] public GameObject endGameUI; // UI element to show at the end of the game
    [SerializeField] public GameObject regularUI; // UI element to show during the game
    [SerializeField] public TextMeshProUGUI winningTeamText;
    [SerializeField] public TextMeshProUGUI teamABallsText;
    [SerializeField] public TextMeshProUGUI teamBBallsText;

    [SerializeField] public TextMeshProUGUI teamAScoreText;
    [SerializeField] public TextMeshProUGUI teamBScoreText;

    [SerializeField] public TextMeshProUGUI currentPlayerText;
    [SerializeField] public TextMeshProUGUI currentDistanceText;
    [SerializeField] public TextMeshProUGUI bestDistanceText; // UI element to display distance
    
    [Header("Parameters")]
    [SerializeField] private ThrowManager throwManager; 
    [SerializeField] private BallSpawner ballSpawner; //reference to the ball spawner
    
    [SerializeField] private int maxBallsPerTeam = 6; // max balls per team
    [SerializeField] private int targetScore = 13;
        
    
    [Header("Game State")]
    public List<Ball> teamABalls = new List<Ball>(); //how many balls are on team A
    public List<Ball> teamBBalls = new List<Ball>(); //how many balls are on team B
    private List<Ball> allBalls = new List<Ball>(); //how many balls are on the field
    private GameObject cochonnet;
    private Ball closest; //closest ball to the cochonnet
    private float closestDistance;
    
    private int teamAScore; //score for team A
    private int teamBScore; //score for team B
    private int pointsThisRound; //points for this round
    
    public RoundPhase roundPhase;
    public Team currentTeam;

    
    
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this); // persist from menu to game scene
        }
        else
        {
            Destroy(this); // Prevent duplicates
        }
    }

    private void Start()
    {
        // Initialize the game  
        StartUI();
        StartGame();
    }

    private void StartGame()
    {
        // Initialize variables
        targetScore = MatchSettingsData.goalScore;
        currentTeam = MatchSettingsData.firstTeam;
        
        // Start a round
        StartCoroutine(GameCoroutine());
    }
    
    
    
    private IEnumerator GameCoroutine()
    {
        // Start game
        InitializeScore();
        while (true)
        {
            yield return RoundCoroutine();
            if (teamAScore >= targetScore || teamBScore >= targetScore) { break; } // The game is finished !
        }

        // End of game
        Debug.Log("End of the game!");
        
        roundPhase = RoundPhase.EndGame;
        currentPlayerText.text = "Game Over!";
        UpdateWinnerUI();
        ShowEndGameUI();
    }
    
    
    
    private IEnumerator RoundCoroutine()
    {
        ClearBalls();
        
        UpdateTeamScoreUI();
        UpdateBallCountUI();
        UpdateCurrentTeamUI();
        winningTeamText.text = "None";

        // Cochonnet time !
        roundPhase = RoundPhase.CochonnetThrow;
        cochonnet = ballSpawner.spawnCochonnet();
        yield return throwManager.BallThrowCoroutine(cochonnet);
            
        Debug.Log("Cochonnet thrown, now it's time for the players to play!");
        
        // First turn 
        UpdateCurrentTeamUI();
        yield return TurnCoroutine();
            
        // Players take turn
        roundPhase = RoundPhase.PlayerTurn;
        while (true)
        {
            bool teamAcanPlay = teamABalls.Count < maxBallsPerTeam;
            bool teamBcanPlay = teamBBalls.Count < maxBallsPerTeam;

            if (!teamAcanPlay && !teamBcanPlay) break;
            
            // Update current team
            currentTeam = (closest.Team == Team.TeamB && teamAcanPlay) || !teamBcanPlay ? Team.TeamA : Team.TeamB;
            UpdateCurrentTeamUI();
            yield return TurnCoroutine();
        }
            
        // End of round
        Debug.Log("No more balls left for both teams!");
        Debug.Log($"{closest.Team} has won this round!");
        
        roundPhase = RoundPhase.EndRound;
        currentPlayerText.text = "Round Ended!";   
        
        switch (closest.Team)
        {
            case Team.TeamA:
                teamAScore = Mathf.Min(teamAScore + pointsThisRound, targetScore);
                teamAScoreText.text = $"{teamAScore}";
                break;
            case Team.TeamB:
                teamBScore = Mathf.Min(teamBScore + pointsThisRound, targetScore);
                teamBScoreText.text = $"{teamBScore}";                    
                break;
            default:
                Debug.Log("Closest ball has no team.");
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator TurnCoroutine()
    {
        Debug.Log("Next!");
        
        // Throw a new ball
        Ball ballScript = ballSpawner.spawnBall(currentTeam);
        RegisterBall(ballScript);
        yield return throwManager.BallThrowCoroutine(ballScript.gameObject);
                
        // Update scores
        ComputeTurnScores();
                
        // Update UI
        UpdateBestDistanceUI();
        UpdateLeadingTeamUI();
    }
    
    
    // Ball Logic
    private void RegisterBall(Ball ball)
    {
        allBalls.Add(ball);
        
        switch (ball.Team)
        {
            case Team.TeamA:
                teamABalls.Add(ball); 
                break;
            case Team.TeamB:
                teamBBalls.Add(ball);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Setup logic
    private void InitializeScore()
    {
        teamAScore = 0;
        teamBScore = 0;
    }
    private void ClearBalls()
    {
        foreach (Ball ball in allBalls)
        {
            Destroy(ball.gameObject);
        }
        allBalls.Clear();
        teamABalls.Clear();
        teamBBalls.Clear();
        if(cochonnet) { Destroy(cochonnet.gameObject); } // destroy the cochonnet
        cochonnet = null; // reset the cochonnet reference
    }
    
    // Game logic
    private void ComputeTurnScores()
    {
        if (cochonnet == null || allBalls.Count == 0) return;

        // find the closest ball to the cochonnet (and which team it belongs to)
        closestDistance = Mathf.Infinity;
        closest = null;
        foreach (Ball b in allBalls)
        {
            float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = b;
            }
        }
        
        Team winningTeam = closest.Team;
        List<Ball> winnerBalls = winningTeam == Team.TeamA ? teamABalls : teamBBalls;
        List<Ball> loserBalls = winningTeam == Team.TeamA ? teamBBalls : teamABalls;
        
        // Find the closest loser ball distance to the cochonnet
        float opponentClosestDist = Mathf.Infinity;
        foreach (Ball b in loserBalls)
        {
            float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
            if (dist < opponentClosestDist)
            {
                opponentClosestDist = dist;
            }
        }

        // Count how many balls from winning team are closer than the best opponent ball
        pointsThisRound = 0;
        foreach (Ball b in winnerBalls)
        {
            float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
            if (dist < opponentClosestDist)
            {
                pointsThisRound++;
            }
        }

    }

    // UI logic
    private void StartUI()
    {
        teamAPanel.GetComponent<Image>().color = MatchSettingsData.teamColorA;
        teamBPanel.GetComponent<Image>().color = MatchSettingsData.teamColorB;

        teamAScorePanel.GetComponent<Image>().color = MatchSettingsData.teamColorA;
        teamBScorePanel.GetComponent<Image>().color = MatchSettingsData.teamColorB;
        teamAScorePanel.GetComponentInChildren<TextMeshProUGUI>().color = GetTextColorForBackground(MatchSettingsData.teamColorA);
        teamBScorePanel.GetComponentInChildren<TextMeshProUGUI>().color = GetTextColorForBackground(MatchSettingsData.teamColorB);

        teamANameText.text = $"{TeamData.GetTeamName(Team.TeamA)}";
        teamBNameText.text = $"{TeamData.GetTeamName(Team.TeamB)}";
        teamANameText.color = GetTextColorForBackground(MatchSettingsData.teamColorA);
        teamBNameText.color = GetTextColorForBackground(MatchSettingsData.teamColorB);

        teamABallsText.color = GetTextColorForBackground(MatchSettingsData.teamColorA);
        teamBBallsText.color = GetTextColorForBackground(MatchSettingsData.teamColorB);
    }
    private void UpdateBallCountUI()
    {
        teamABallsText.text = $"{teamABalls.Count}|{maxBallsPerTeam}";
        teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsPerTeam}";        
    }

    private void UpdateTeamScoreUI()
    {
        teamAScoreText.text = $"{teamAScore}";
        teamBScoreText.text = $"{teamBScore}";        
    }

    private void UpdateCurrentTeamUI()
    {
        currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";        
    }

    private void UpdateBestDistanceUI()
    {
        bestDistanceText.text = $"Distance to beat: {closestDistance:F2} m";
        bestDistanceText.color = closest.Team == Team.TeamA ? MatchSettingsData.teamColorA : MatchSettingsData.teamColorB;
    }

    private void UpdateLeadingTeamUI()
    {
        winningTeamText.text = $"{TeamData.GetTeamName(closest.Team)} ({pointsThisRound} pts)";
    }

    private void UpdateWinnerUI()
    {
        Team winner = teamAScore > teamBScore ? Team.TeamA : Team.TeamB;
        winningTeamText.text = $"{TeamData.GetTeamName(winner)} wins!";
    }
    
    private void ShowRegularUI()
    {
        regularUI.SetActive(true);
        endGameUI.SetActive(false);
    }
    
    private void ShowEndGameUI()
    {
        regularUI.SetActive(false);
        endGameUI.SetActive(true);
    }

    public void ChangeToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void RestartGame()
    {
        // Reset UI
        ShowRegularUI();
        StartGame();
    }
    
    public Color GetTextColorForBackground(Color bgColor)
    {
        float luminance = 0.2126f * bgColor.r + 0.7152f * bgColor.g + 0.0722f * bgColor.b; // luminance using  formula for brightness
        return luminance > 0.5f ? Color.black : Color.white;
    }


}

