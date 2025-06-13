using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;



public enum RoundPhase //should add draw phase maybe (or will be done in the menu)
{
    CochonnetThrow,
    PlayerTurn,
    EndRound,
    EndGame
}
// TODO: for now, a new turn begins with teamA, needs to introduce a draw phase
public class GameManager : MonoBehaviour
{
    public static GameManager instance; //singleton instance

[Header("UI - Panels")]
    [SerializeField] private GameObject teamAPanel;
    [SerializeField] private GameObject teamBPanel;
    [SerializeField] private GameObject teamAScorePanel;
    [SerializeField] private GameObject teamBScorePanel;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject regularUI;
    [SerializeField] private GameObject teamAItemPanel; 
    [SerializeField] private GameObject teamBItemPanel; 

    [Header("UI - Text Elements")]
    [SerializeField] private TextMeshProUGUI teamANameText;
    [SerializeField] private TextMeshProUGUI teamBNameText;
    [SerializeField] private TextMeshProUGUI winningTeamText;
    [SerializeField] private TextMeshProUGUI teamABallsText;
    [SerializeField] private TextMeshProUGUI teamBBallsText;
    [SerializeField] private TextMeshProUGUI teamAScoreText;
    [SerializeField] private TextMeshProUGUI teamBScoreText;
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI currentDistanceText;
    [SerializeField] private TextMeshProUGUI bestDistanceText;

    [Header("Game Settings")]
    [SerializeField] private int maxBallsPerTeam = 6;
    [SerializeField] private int TargetScore = 13;

    [Header("Game Logic")]
    [SerializeField] private BallSpawner ballSpawner;
    public Cochonnet cochonnet;
    public Team currentTeam; // Assuming team switching logic needs public access

    private int teamAScore = 0;
    private int teamBScore = 0;
    private int pointsThisRound = 0;

    [Header("Ball Tracking")]
    private List<Ball> allBalls = new List<Ball>();
    [SerializeField] private List<Ball> teamABalls = new List<Ball>();
    [SerializeField] private List<Ball> teamBBalls = new List<Ball>();
    private Ball closest;

    [Header("Game State")]
    public RoundPhase roundPhase = RoundPhase.CochonnetThrow;



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
        teamAPanel.GetComponent<Image>().color = MatchSettingsData.teamColorA;
        teamBPanel.GetComponent<Image>().color = MatchSettingsData.teamColorB;

        teamAItemPanel.GetComponent<Image>().color = MatchSettingsData.teamColorA;
        teamBItemPanel.GetComponent<Image>().color = MatchSettingsData.teamColorB;

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

        maxBallsPerTeam = MatchSettingsData.ballsPerTeam;
        teamABallsText.text = $"{teamABalls.Count}|{maxBallsPerTeam}";
        teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsPerTeam}";
        
        TargetScore = MatchSettingsData.goalScore;
        currentTeam = Team.TeamA; //start with team A
        currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";
        winningTeamText.text = "None";

        // Spawn the cochonnet
        ballSpawner.spawnCochonnet();
    }

    private void NextRound()
    {
        // Reset the game state for a new round
        allBalls.Clear();
        teamABalls.Clear();
        teamBBalls.Clear();
        // destroy all balls
        foreach (Ball ball in FindObjectsOfType<Ball>())
        {
            Destroy(ball.gameObject);
        }
        Destroy(cochonnet.gameObject); // destroy the cochonnet
        cochonnet = null; // reset the cochonnet reference

        // Reset UI
        teamABallsText.text = $"{teamABalls.Count}|{maxBallsPerTeam}";
        teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsPerTeam}";
        winningTeamText.text = "None";

        currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";

        teamAScoreText.text = $"{teamAScore}";
        teamBScoreText.text = $"{teamBScore}";

        ballSpawner.spawnCochonnet(); // Spawn a new cochonnet
        roundPhase = RoundPhase.CochonnetThrow; // Reset the round phase
    }

    //set the cochonnet
    public void SetCochonnet(Cochonnet cochonnet)
    {
        this.cochonnet = cochonnet;
        Debug.Log("Cochonnet set!");
    }

    //update ball count when a ball is thrown
    public void RegisterBall(Ball ball)
    {
        allBalls.Add(ball);
        if (ball.team == Team.TeamA)
        {
            teamABalls.Add(ball);
            teamABallsText.text = $"{teamABalls.Count}|{maxBallsPerTeam}";
        }
        else if (ball.team == Team.TeamB)
        {
            teamBBalls.Add(ball);
            teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsPerTeam}";
        }
    }

    public void DetermineClosestBall()
    {

        if (cochonnet == null || allBalls.Count == 0) return;

        float minDist = Mathf.Infinity;
        closest = null;

        // find the closest ball to the cochonnet (and which team it belongs to)
        foreach (Ball b in allBalls)
        {

            float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = b;

                bestDistanceText.text = $"Distance to beat: {dist:F2} m";
                bestDistanceText.color = closest.team == Team.TeamA ? MatchSettingsData.teamColorA : MatchSettingsData.teamColorB;
            }
        }

        Team winningTeam = closest.team;



        // Find the closest opponent ball to the cochonnet
        float opponentClosestDist = Mathf.Infinity;
        foreach (Ball b in allBalls)
        {
            if (b.team != winningTeam)
            {
                float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
                if (dist < opponentClosestDist)
                    opponentClosestDist = dist;
            }
        }

        // Count how many balls from winning team are closer than the best opponent ball
        pointsThisRound = 0;
        foreach (Ball b in allBalls)
        {
            if (b.team == winningTeam)
            {
                float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
                if (dist < opponentClosestDist)
                    pointsThisRound++;
            }
        }
        string teamName = TeamData.GetTeamName(closest.team);
        winningTeamText.text = $"{teamName} ({pointsThisRound} pts)";

    }

    public void NextTurn()
    {
        Debug.Log("Next turn!");
        var spawner = FindFirstObjectByType<BallSpawner>();
        RepulseEffectManager.Instance.OnTurnStart();
        // we wait for all balls to stop moving before proceeding

        if (roundPhase == RoundPhase.CochonnetThrow)
        {
            roundPhase = RoundPhase.PlayerTurn;
            Debug.Log("Cochonnet thrown, now it's time for the players to play!");
            // currentPlayerText.text = currentTeam == Team.TeamA ? "Current Turn: Team A" : "Current Turn: Team B";
            currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";

            spawner.Invoke(nameof(spawner.spawnBall), 1f);
            return;
        }

        bool teamAcanPlay = teamABalls.Count < maxBallsPerTeam;
        bool teamBcanPlay = teamBBalls.Count < maxBallsPerTeam;

        if (!teamAcanPlay && !teamBcanPlay)
        {
            Debug.Log("No more balls left for both teams!");
            roundPhase = RoundPhase.EndRound;
            Debug.Log($"{closest.team} has won!");
            winningTeamText.text = $"Winning: {closest.team}";
            if (closest.team == Team.TeamA)
            {
                teamAScore = Mathf.Min(teamAScore + pointsThisRound, TargetScore);
                teamAScoreText.text = $"{teamAScore}";
            }
            else
            {
                teamBScore = Mathf.Min(teamBScore + pointsThisRound, TargetScore);
                teamBScoreText.text = $"{teamBScore}";
            }

            currentPlayerText.text = "Round Ended!";

            // check if the game is over
            if (teamAScore >= TargetScore || teamBScore >= TargetScore)
            {
                roundPhase = RoundPhase.EndGame;
                Debug.Log("End of the game!");
                winningTeamText.text = $"{(teamAScore > teamBScore ? "Team A" : "Team B")} wins!";

                currentPlayerText.text = "Game Over!";
                showEndGameUI();
                return;
            }

            NextRound();

            return;
        }

        if (!teamAcanPlay || !teamBcanPlay)
        {   //if one team has no balls left, the other team can play until they run out of balls
            currentTeam = teamAcanPlay ? Team.TeamA : Team.TeamB;
            Debug.Log($"{currentTeam} can play.");
            currentPlayerText.text = currentTeam == Team.TeamA ? "Current Turn: Team A" : "Current Turn: Team B";
            spawner.Invoke(nameof(spawner.spawnBall), 1f);
            return;
        }

        //default case 
        currentTeam = (closest.team == Team.TeamA) ? Team.TeamB : Team.TeamA;
        currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";

        spawner.Invoke(nameof(spawner.spawnBall), 1f);


    }


    public bool AreAllBallsStopped(float threshold = 0.0005f)
    {
        foreach (Ball ball in allBalls)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.magnitude > threshold)
                return false;
        }
        return true;
    }

    public void WaitAndProceedToNextTurn()
    {
        StartCoroutine(WaitForBallsToStopAndContinue());
    }

    private IEnumerator WaitForBallsToStopAndContinue()
    {
        yield return new WaitForSeconds(0.2f);

        // Wait until all balls stop moving
        while (!AreAllBallsStopped())
        {
            yield return new WaitForSeconds(0.2f); // small wait to avoid checking every frame
        }

        // Once balls stop, determine the closest ball and proceed
        DetermineClosestBall();


        NextTurn();
    }

    private void showEndGameUI()
    {
        regularUI.SetActive(false);
        endGameUI.SetActive(true);
    }

    public void changeToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void RestartGame()
    {
        // Reset all variables and game state
        roundPhase = RoundPhase.CochonnetThrow;
        teamAScore = 0;
        teamBScore = 0;

        teamABalls.Clear();
        teamBBalls.Clear();
        allBalls.Clear();

        teamAScoreText.text = "0";
        teamBScoreText.text = "0";
        winningTeamText.text = "None";

        // Destroy all existing balls (optional, but important)
        foreach (Ball ball in FindObjectsOfType<Ball>())
        {
            Destroy(ball.gameObject);
        }

        if (cochonnet != null)
        {
            Destroy(cochonnet.gameObject);
            cochonnet = null;
        }

        // Reset UI and turn
        endGameUI.SetActive(false);
        regularUI.SetActive(true);

        // Spawn a new cochonnet
        ballSpawner.spawnCochonnet();
    }
    
    public Color GetTextColorForBackground(Color bgColor)
    {
        float luminance = 0.2126f * bgColor.r + 0.7152f * bgColor.g + 0.0722f * bgColor.b; // luminance using  formula for brightness
        return luminance > 0.5f ? Color.black : Color.white;
    }


    public void OnBallDisqualified(Ball ball) {
        allBalls.Remove(ball);
        Destroy(ball.gameObject);
    }

}

