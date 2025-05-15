using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


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
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI winningTeamText;

    public Cochonnet cochonnet;
    public BallSpawner ballSpawner; //reference to the ball spawner

    private List<Ball> allBalls = new List<Ball>(); //how many balls are on the field
    public List<Ball> teamABalls = new List<Ball>(); //how many balls are on team A
    public List<Ball> teamBBalls = new List<Ball>(); //how many balls are on team B
    public int maxBallsPerTeam = 6; //max balls per team

    public Team currentTeam;
    private Ball closest; //closest ball to the cochonnet

    public RoundPhase roundPhase = RoundPhase.CochonnetThrow;

    public void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // Initialize the game
        currentTeam = Team.TeamA; //start with team A
        currentPlayerText.text = $"Current Turn: Team {currentTeam}";
        winningTeamText.text = "Winning: None";

        // Spawn the cochonnet
        ballSpawner.spawnCochonnet();
    }

    // will be used to check if the cochonnet has stopped moving
    // void Update()
    // {
    //     if (gamePhase == GamePhase.CochonnetThrow)
    //     {
    //         if (cochonnet != null && cochonnet.HasStoppedMoving())
    //         {
    //             gamePhase = GamePhase.PlayerTurn;
    //             StartNextPlayerTurn();
    //         }
    //     }
    // }

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
        }
        else if (ball.team == Team.TeamB)
        {
            teamBBalls.Add(ball);
        }
    }

    public void DetermineClosestBall()
    {
        if (cochonnet == null || allBalls.Count == 0) return;

        float minDist = Mathf.Infinity;
        closest = null;


        foreach (Ball b in allBalls)
        {

            float dist = Vector3.Distance(b.transform.position, cochonnet.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = b;
            }
        }

        if (closest != null)
        {
            Debug.Log($"Team {closest.team} is currently winning with a distance of {minDist}m!");
            winningTeamText.text = $"Winning: Team {closest.team}";
        }
    }

    public void NextTurn()
    {
        if (roundPhase == RoundPhase.CochonnetThrow)
        {
            roundPhase = RoundPhase.PlayerTurn;
            Debug.Log("Cochonnet thrown, now it's time for the players to play!");
            currentPlayerText.text = $"Current Turn: Team {currentTeam}";
            return;
        }

        bool teamAcanPlay = teamABalls.Count < maxBallsPerTeam;
        bool teamBcanPlay = teamBBalls.Count < maxBallsPerTeam;

        if (!teamAcanPlay && !teamBcanPlay)
        {
            Debug.Log("No more balls left for both teams!");
            roundPhase = RoundPhase.End;
            Debug.Log($"Team {closest.team} has won!");
            winningTeamText.text = $"Winning: Team {closest.team}";
            return;
        }

        if (!teamAcanPlay || !teamBcanPlay)
        {   //if one team has no balls left, the other team can play until they run out of balls
            currentTeam = teamAcanPlay ? Team.TeamA : Team.TeamB;
            Debug.Log($"Team {currentTeam} can play.");
            currentPlayerText.text = $"Current Turn: Team {currentTeam}";
            return;
        }

        //default case 
        currentTeam = (currentTeam == Team.TeamA) ? Team.TeamB : Team.TeamA;
        currentPlayerText.text = $"Current Turn: {currentTeam}";
    

    }


}

