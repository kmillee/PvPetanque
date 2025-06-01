using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject cochonnetPrefab; // Prefab of the cochonnet to spawn
    public GameObject ballPrefab; // Prefab of the ball to spawn
    public Transform spawnPoint; // Point where the ball will be spawned

    public Material ballMaterial;
    

    
    
    void Start()
    {
 
    }


    public void spawnBall()
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Ball ballScript = newBall.GetComponent<Ball>();

        // Set team before assigning material
        ballScript.team = GameManager.instance.currentTeam;

        Renderer renderer = newBall.GetComponent<Renderer>();

        Material teamMaterial;
        if (ballScript.team == Team.TeamA)
        {
            teamMaterial = new Material(ballMaterial);
            teamMaterial.color = MatchSettingsData.teamColorA;
        }
        else
        {
            teamMaterial = new Material(ballMaterial);
            teamMaterial.color = MatchSettingsData.teamColorB;
        }
        renderer.material = teamMaterial;
    }


    public void spawnCochonnet()
    {
        Instantiate(cochonnetPrefab, spawnPoint.position, Quaternion.identity);
    }

}
