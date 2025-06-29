using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public enum RoundPhase //should add draw phase maybe (or will be done in the menu)
{
    CochonnetThrow,
    PlayerTurn,
    EndRound,
    EndGame,
    
}

public class BoolReference
{
    public bool Value = true;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //singleton instance

    [Header("Item Slots")]
    [SerializeField] private TeamItemSlot teamASlot;
    [SerializeField] private TeamItemSlot teamBSlot;

    [Header("UI - Panels")]
    [SerializeField] private GameObject teamAPanel;
    [SerializeField] private GameObject teamBPanel;
    [SerializeField] private GameObject teamAScorePanel;
    [SerializeField] private GameObject teamBScorePanel;
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private GameObject endMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject teamAItemPanel;
    [SerializeField] private GameObject teamBItemPanel;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private ConfettiController confettiController;

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
    [SerializeField] private int targetScore = 13;
    [SerializeField] private List<GameEffect> selectedItems;

    [Header("Game Logic")]
    [SerializeField] private ThrowManager throwManager;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    [Header("Game State")]
    public List<Ball> teamABalls = new List<Ball>(); //how many balls are on team A
    public List<Ball> teamBBalls = new List<Ball>(); //how many balls are on team B
    private List<Ball> allBalls = new List<Ball>(); //how many balls are on the field
    public List<Ball> disqualifiedTeamABalls = new List<Ball>();
    public List<Ball> disqualifiedTeamBBalls = new List<Ball>();
    public List<Ball> disqualifiedBalls = new List<Ball>();
    private GameObject cochonnet;
    private Cochonnet cochonnetScript;
    private Ball closest; //closest ball to the cochonnet
    private float closestDistance;

    private int teamAScore; //score for team A
    private int teamBScore; //score for team B
    private int pointsThisRound; //points for this round

    // in case of item bonus ball
    private int maxBallsTeamA;
    private int maxBallsTeamB;

    public RoundPhase roundPhase;
    public Team currentTeam;
    private ItemBoxSpawner itemBoxSpawner;

    public TeamItemSlot GetTeamItemSlotA()
    {
        return teamASlot;
    }
    public TeamItemSlot GetTeamItemSlotB()
    {
        return teamBSlot;
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this); // persist from menu to game scene
        }
        else
        {
            Destroy(this); // Prevent duplicates
        }
    }

    private void Start()
    {
        // Initialize the game  
        LoadData();
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

        // Spawn obstacles
        obstacleSpawner.spawnObstacle();
        yield return new WaitForSeconds(1f);

        // Rounds
        while (!(teamAScore >= targetScore || teamBScore >= targetScore))
        {
            yield return RoundCoroutine();
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
        RepulseEffectManager.Instance.ClearRepulsors();
        AttractEffectManager.Instance.ClearAttractors();

        UpdateTeamScoreUI();
        UpdateBallCountUI();
        UpdateCurrentTeamUI();
        winningTeamText.text = "None";

        // Clear then Spawn item boxes
        if (itemBoxSpawner != null)
        {
            itemBoxSpawner.ClearItemBoxes();
            itemBoxSpawner.SpawnItemBox(); // Spawn 1 item box at the start of each round
        }


        // Cochonnet time !
        do
        {
            roundPhase = RoundPhase.CochonnetThrow;
            cochonnetScript = ballSpawner.spawnCochonnet();
            cochonnet = cochonnetScript.gameObject;
            yield return throwManager.BallThrowCoroutine(cochonnetScript);
            yield return WaitForBallsToStop(new BoolReference());
        }
        while (cochonnetScript.IsDisqualified);

        Debug.Log("Cochonnet thrown, now it's time for the players to play!");

        // First turn 
        UpdateCurrentTeamUI();
        yield return TurnCoroutine();

        // Players take turn
        roundPhase = RoundPhase.PlayerTurn;
        bool teamAcanPlay = true;
        bool teamBcanPlay = true;
        while (teamAcanPlay || teamBcanPlay)
        {
            // Update current team
            if (closest)
            {
                currentTeam = (closest.Team == Team.TeamB && teamAcanPlay) || !teamBcanPlay ? Team.TeamA : Team.TeamB;
            }
            else
            {
                currentTeam = currentTeam == Team.TeamA ? Team.TeamB : Team.TeamA;
            }
            UpdateCurrentTeamUI();
            yield return TurnCoroutine();

            // Update teamCanPlay
            teamAcanPlay = teamABalls.Count + disqualifiedTeamABalls.Count < maxBallsTeamA;
            teamBcanPlay = teamBBalls.Count + disqualifiedTeamBBalls.Count < maxBallsTeamB;
        }

        // End of round
        Debug.Log("No more balls left for both teams!");
        Debug.Log($"{closest.Team} has won this round!");

        roundPhase = RoundPhase.EndRound;
        currentPlayerText.text = "Round Ended!";

        maxBallsTeamA = MatchSettingsData.ballsPerTeam;
        maxBallsTeamB = MatchSettingsData.ballsPerTeam;

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
                Debug.Log("Closest ball has no team ???");
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator TurnCoroutine()
    {
        // Adjust item slots UI
        foreach (var slot in FindObjectsOfType<TeamItemSlot>())
        {
            slot.RefreshUI();
        }

        RepulseEffectManager.Instance.OnTurnStart();
        AttractEffectManager.Instance.OnTurnStart();

        Debug.Log("Next!");
        // Try to spawn a new item box
        if (itemBoxSpawner != null)
            itemBoxSpawner.SpawnItemBox(0.5f, 1); // 50% chance to spawn an item box every turn


        // Throw a new ball
        Ball ballScript = ballSpawner.spawnBall(currentTeam);
        RegisterBall(ballScript);

        Coroutine showDistanceCoroutine = StartCoroutine(UpdateDistanceUntilBallsStop(ballScript));
        Coroutine throwBallCoroutine = StartCoroutine(throwManager.BallThrowCoroutine(ballScript));
        yield return throwBallCoroutine;
        yield return showDistanceCoroutine;

        yield return WaitForBallsToStop(new BoolReference());

        // Apply the swap effect if needed
        if (SwapEffectManager.Instance != null && SwapEffectManager.Instance.effect != null)
        {
            Debug.Log("Applying swap effect to the ball.");
            // Debug.Log($"effect: {SwapEffectManager.Instance.effect}");
            SwapEffectManager.Instance.applySwap(ballScript.gameObject);
        }


        // Update scores
        ComputeTurnScores();

        // Update UI
        UpdateBestDistanceUI();
        UpdateLeadingTeamUI();
        UpdateBallCountUI();
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

    public void OnBallDisqualified(Ball ball)
    {
        displayNotification("Ball is disqualified!");
        switch (ball.Team)
        {
            case Team.TeamA:
                allBalls.Remove(ball);
                disqualifiedBalls.Add(ball);
                teamABalls.Remove(ball);
                disqualifiedTeamABalls.Add(ball);
                break;
            case Team.TeamB:
                allBalls.Remove(ball);
                disqualifiedBalls.Add(ball);
                teamBBalls.Remove(ball);
                disqualifiedTeamBBalls.Add(ball);
                break;
            case Team.Cochonnet:
                disqualifiedBalls.Add(ball);
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

        foreach (Ball ball in disqualifiedBalls)
        {
            Destroy(ball.gameObject);
        }
        allBalls.Clear();
        teamABalls.Clear();
        teamBBalls.Clear();
        disqualifiedTeamABalls.Clear();
        disqualifiedTeamBBalls.Clear();
        disqualifiedBalls.Clear();

        if (cochonnet) { Destroy(cochonnet.gameObject); } // destroy the cochonnet
        cochonnet = null; // reset the cochonnet reference
    }

    // Game logic
    private IEnumerator WaitForBallsToStop(BoolReference isMoving)
    {
        while (isMoving.Value)
        {
            isMoving.Value = false;
            foreach (Ball ball in allBalls)
            {
                if (ball.IsMoving() || !ball.HitGround || !ball.Launched)
                {
                    isMoving.Value = true;
                }
            }

            if (cochonnetScript.IsMoving())
            {
                isMoving.Value = true;
            }

            if (cochonnetScript.IsDisqualified)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ComputeClosest()
    {
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
    }

    private void ComputeTurnScores()
    {
        if (cochonnet == null || allBalls.Count == 0) return;

        ComputeClosest();

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
    private void LoadData()
    {
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
        maxBallsTeamA = maxBallsPerTeam;
        maxBallsTeamB = maxBallsPerTeam;
        targetScore = MatchSettingsData.goalScore;
        currentTeam = MatchSettingsData.firstTeam; // Set the current team based on MatchSettingsData

        selectedItems = new List<GameEffect>(MatchSettingsData.selectedItems);
        itemBoxSpawner = FindObjectOfType<ItemBoxSpawner>();
    }
    private void UpdateBallCountUI()
    {
        teamABallsText.text = $"{teamABalls.Count}|{maxBallsTeamA - disqualifiedTeamABalls.Count}";
        teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsTeamB - disqualifiedTeamBBalls.Count}";
    }
    private void UpdateTeamScoreUI()
    {
        teamAScoreText.text = $"{teamAScore}";
        teamBScoreText.text = $"{teamBScore}";
    }
    private void UpdateCurrentTeamUI()
    {
        currentPlayerText.text = $"Current Turn: {TeamData.GetTeamName(currentTeam)}";
        displayNotification($"{TeamData.GetTeamName(currentTeam)}'s turn!", TeamData.GetTeamColor(currentTeam));
    }

    private void UpdateBestDistanceUI()
    {
        if (float.IsFinite(closestDistance))
        {
            bestDistanceText.text = $"Distance to beat: {closestDistance:F2} m";
            bestDistanceText.color =
                closest.Team == Team.TeamA ? MatchSettingsData.teamColorA : MatchSettingsData.teamColorB;
        }
        else
        {
            bestDistanceText.text = $"Distance to beat: -";
            bestDistanceText.color = Color.gray;
        }
    }

    private void UpdateCurrentDistanceUI(float distance)
    {
        if (float.IsFinite(distance))
        {
            currentDistanceText.text = $"Distance : {distance:F2} m";
        }
        else
        {
            currentDistanceText.text = "Distance : -";
        }
    }
    private void UpdateLeadingTeamUI()
    {
        if (closest == null)
        {
            winningTeamText.text = "";
            return;
        }
        winningTeamText.text = $"{TeamData.GetTeamName(closest.Team)} ({pointsThisRound} pts)";
    }
    private void UpdateWinnerUI()
    {
        Team winner = teamAScore > teamBScore ? Team.TeamA : Team.TeamB;
        winningTeamText.text = $"{TeamData.GetTeamName(winner)} wins!";
    }

    private IEnumerator UpdateDistanceUntilBallsStop(Ball ball)
    {
        BoolReference aBallIsMoving = new();
        StartCoroutine(WaitForBallsToStop(aBallIsMoving));
        while (aBallIsMoving.Value)
        {
            float dist = ball.IsDisqualified
                ? float.PositiveInfinity
                : Vector3.Distance(ball.transform.position, cochonnet.transform.position);
            ComputeClosest();

            UpdateCurrentDistanceUI(dist);
            UpdateBestDistanceUI();

            yield return new WaitForSeconds(0.2f);
        }
    }


    private void ShowGameUI()
    {
        gameUI.SetActive(true);
        endGameUI.SetActive(false);
    }
    private void ShowEndGameUI()
    {

        Team winnerTeam = teamAScore > teamBScore ? Team.TeamA : Team.TeamB;
        string winnerName = TeamData.GetTeamName(winnerTeam);
        Color color = TeamData.GetTeamColor(winnerTeam);
        Debug.Log("Showing end game UI");
        endGameUI.SetActive(true);

        confettiController.Play(color);

        TextMeshProUGUI endGameText = endMenu.GetComponentInChildren<TextMeshProUGUI>();

        endMenu.GetComponentInChildren<Image>().color = color;
        endGameText.color = GetTextColorForBackground(color);
        endGameText.text = $"{winnerName} wins!";



    }
    public void ChangeToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void RestartGame()
    {
        // Reset UI
        ShowGameUI();
        StartGame();
    }
    public Color GetTextColorForBackground(Color bgColor)
    {
        float luminance = 0.2126f * bgColor.r + 0.7152f * bgColor.g + 0.0722f * bgColor.b; // luminance using  formula for brightness
        return luminance > 0.5f ? Color.black : Color.white;
    }

    // item logic
    public void IncreaseMaxBalls(Team team)
    {
        if (team == Team.TeamA)
        {
            maxBallsTeamA++;
            teamABallsText.text = $"{teamABalls.Count}|{maxBallsTeamA}";
        }
        else if (team == Team.TeamB)
        {
            maxBallsTeamB++;
            teamBBallsText.text = $"{teamBBalls.Count}|{maxBallsTeamB}";
        }

        Debug.Log($"Max balls increased for {team}: now {GetMaxBalls(team)} balls.");
    }

    public int GetMaxBalls(Team team)
    {
        return team == Team.TeamA ? maxBallsTeamA : maxBallsTeamB;
    }

    //display the current team panel with a notification for a few seconds
    private void displayNotification(string message, Color color = default)
    {
        if (notificationPanel == null)
        {
            Debug.LogWarning("Notification panel is not set in GameManager.");
            return;
        }

        if (color == default)
        {
            color = Color.black; // Default color if none is provided
        }
        // Set the notification panel active and update the text
        notificationPanel.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.7f);

        notificationPanel.SetActive(true);
        TextMeshProUGUI notificationText = notificationPanel.GetComponentInChildren<TextMeshProUGUI>();
        notificationText.text = message;
        notificationText.color = GetTextColorForBackground(color);

        StartCoroutine(HideNotificationPanelAfterDelay(2f));

    }

    private IEnumerator HideNotificationPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        notificationPanel.SetActive(false);
    }



}

