using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject cochonnetPrefab; // Prefab of the cochonnet to spawn
    public GameObject ballPrefab; // Prefab of the ball to spawn
    public Transform spawnPoint; // Point where the ball will be spawned

    public Material ballMaterial;

    public Ball spawnBall(Team currentTeam)
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        return spawnBall(newBall, currentTeam);
    }
    
    public Ball spawnBall(GameObject ball, Team currentTeam)
    {
        ball.transform.SetParent(transform);
        
        Ball ballScript = ball.AddComponent<Ball>();
        
        // Set team before assigning material
        ballScript.Team = currentTeam;

        Renderer renderer; // = ball.GetComponent<Renderer>();
        if (ball.TryGetComponent<Renderer>(out renderer))
        {
            Material teamMaterial = new Material(ballMaterial);
            teamMaterial.color = ballScript.Team == Team.TeamA
                ? MatchSettingsData.teamColorA
                : MatchSettingsData.teamColorB;
            renderer.material = teamMaterial;
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
