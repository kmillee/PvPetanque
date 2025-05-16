using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;


public enum RoundPhase //should add draw phase maybe (or will be done in the menu)
{
    CochonnetThrow,
    PlayerTurn,
    End
}
// for now, nextturn is just alternating between teams, not taking into account whose the closest from the cochonnet 
//(need to check if closest Ball works first)
public class GameManager : MonoBehaviour
{
    public static GameManager instance; //singleton instance
    public RoundPhase roundPhase = RoundPhase.CochonnetThrow;
    public Cochonnet cochonnet;
    public BallSpawner ballSpawner; //reference to the ball spawner

    // UI elements 
    public TextMeshProUGUI winningTeamText;
    public TextMeshProUGUI teamABallsText;
    public TextMeshProUGUI teamBBallsText;

    public TextMeshProUGUI teamAScoreText;
    public TextMeshProUGUI teamBScoreText;

    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI currentDistanceText;
    public TextMeshProUGUI bestDistanceText; // UI element to display distance

    private int teamAScore = 0; //score for team A
    private int teamBScore = 0; //score for team B

    private List<Ball> allBalls = new List<Ball>(); //how many balls are on the field
    public List<Ball> teamABalls = new List<Ball>(); //how many balls are on team A
    public List<Ball> teamBBalls = new List<Ball>(); //how many balls are on team B
    public int maxBallsPerTeam = 6; //max balls per team

    public Team currentTeam;
    private Ball closest; //closest ball to the cochonnet


    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Initialize the game
        currentTeam = Team.TeamA; //start with team A
        currentPlayerText.text = $"Current Turn: {currentTeam}";
        winningTeamText.text = "None";

        // Spawn the cochonnet
        ballSpawner.spawnCochonnet();
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
            teamABallsText.text = $"{teamABalls.Count} / {maxBallsPerTeam}";
        }
        else if (ball.team == Team.TeamB)
        {
            teamBBalls.Add(ball);
            teamBBallsText.text = $"{teamBBalls.Count} / {maxBallsPerTeam}";
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
                bestDistanceText.color = (b.team == Team.TeamA) ? Color.red : Color.blue; // color based on team
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
        int pointsThisRound = 0;
        foreach (Ball b in allBalls)
        {
            if (b.team == winningTeam)
            {
                float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
                if (dist < opponentClosestDist)
                    pointsThisRound++;
            }
        }

        winningTeamText.text = $"{closest.team} ({pointsThisRound} pts)";

    }



    public void NextTurn()
    {
        Debug.Log("Next turn!");
        var spawner = FindFirstObjectByType<BallSpawner>();

        // we wait for all balls to stop moving before proceeding

        if (roundPhase == RoundPhase.CochonnetThrow)
        {
            roundPhase = RoundPhase.PlayerTurn;
            Debug.Log("Cochonnet thrown, now it's time for the players to play!");
            currentPlayerText.text = $"Current Turn: {currentTeam}";
            spawner.Invoke(nameof(spawner.spawnBall), 1f);
            return;
        }

        bool teamAcanPlay = teamABalls.Count < maxBallsPerTeam;
        bool teamBcanPlay = teamBBalls.Count < maxBallsPerTeam;

        if (!teamAcanPlay && !teamBcanPlay)
        {
            Debug.Log("No more balls left for both teams!");
            roundPhase = RoundPhase.End;
            Debug.Log($"{closest.team} has won!");
            winningTeamText.text = $"Winning: {closest.team}";
            if (closest.team == Team.TeamA)
            {
                teamAScore++;
                teamAScoreText.text = $"{teamAScore}";
            }
            else
            {
                teamBScore++;
                teamBScoreText.text = $"{teamBScore}";
            }

            currentPlayerText.text = "Round Ended!";
            Debug.Log("Game has ended!");

            return;
        }

        if (!teamAcanPlay || !teamBcanPlay)
        {   //if one team has no balls left, the other team can play until they run out of balls
            currentTeam = teamAcanPlay ? Team.TeamA : Team.TeamB;
            Debug.Log($"{currentTeam} can play.");
            currentPlayerText.text = $"Current Turn: Team {currentTeam}";
            spawner.Invoke(nameof(spawner.spawnBall), 1f);
            return;
        }

        //default case 
        currentTeam = (closest.team == Team.TeamA) ? Team.TeamB : Team.TeamA;
        currentPlayerText.text = $"Current Turn: {currentTeam}";
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



}

