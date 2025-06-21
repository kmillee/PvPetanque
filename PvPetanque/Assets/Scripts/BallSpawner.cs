using System;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject cochonnetPrefab; // Prefab of the cochonnet to spawn
    public GameObject ballPrefab; // Prefab of the ball to spawn
    public Transform spawnPoint; // Point where the ball will be spawned

    public Material ballMaterial;

    public TeamItemSlot teamASlot;
    public TeamItemSlot teamBSlot;


    public void spawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Ball ballScript = newBall.GetComponent<Ball>();

        // Set team before assigning material
        ballScript.Team = GameManager.instance.currentTeam;

        Renderer renderer = newBall.GetComponent<Renderer>();

        Material teamMaterial;
        if (ballScript.Team == Team.TeamA)
        {
            teamMaterial = new Material(ballMaterial);
            teamMaterial.color = MatchSettingsData.teamColorA;
            if(teamASlot != null)
                teamASlot.SetTeamObject(newBall);
        }
        else
        {
            teamMaterial = new Material(ballMaterial);
            teamMaterial.color = MatchSettingsData.teamColorB;
            if(teamBSlot != null)
                teamBSlot.SetTeamObject(newBall);
        }
        renderer.material = teamMaterial;
    }

    public Ball spawnBall(Team currentTeam)
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        return spawnBall(newBall, currentTeam);
    }

    
    public Ball spawnBall(GameObject ball, Team currentTeam)
    {
        ball.transform.SetParent(transform);
        
        Ball ballScript = ball.GetComponent<Ball>();
        if (ballScript == null)
            ballScript = ball.AddComponent<Ball>();
        
        // Set team before assigning material
        ballScript.Team = currentTeam;

        Renderer renderer;
        if (ball.TryGetComponent(out renderer))
        {
            Material teamMaterial = new Material(ballMaterial);
            teamMaterial.color = ballScript.Team == Team.TeamA
                ? MatchSettingsData.teamColorA
                : MatchSettingsData.teamColorB;
            renderer.material = teamMaterial;
        }
        
        // Bound the effect to the ball
        switch (currentTeam)
        {
            case Team.TeamA:
                teamASlot.SetTeamObject(ball);
                break;
            case Team.TeamB:
                teamBSlot.SetTeamObject(ball);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(currentTeam), currentTeam, null);
        }
        
        return ballScript;
    }
    

    public Cochonnet spawnCochonnet()
    {
        GameObject cochonnet = Instantiate(cochonnetPrefab, spawnPoint.position, Quaternion.identity, transform);
        Cochonnet cochonnetScript;
        if (!cochonnet.TryGetComponent<Cochonnet>(out cochonnetScript))
        {
            Debug.Log("No cochonnet script in cochonnet prefab");
        }

        return cochonnetScript;
    }

}
