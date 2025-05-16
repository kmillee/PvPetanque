using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject cochonnetPrefab; // Prefab of the cochonnet to spawn
    public GameObject ballPrefab; // Prefab of the ball to spawn
    public Transform spawnPoint; // Point where the ball will be spawned

    public Material teamAMaterial; // Material for Team A
    public Material teamBMaterial; // Material for Team 
    
    
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
        if (ballScript.team == Team.TeamA)
        {
            renderer.material = teamAMaterial;
        }
        else
        {
            renderer.material = teamBMaterial;
        }
        
    }


    public void spawnCochonnet()
    {
        Instantiate(cochonnetPrefab, spawnPoint.position, Quaternion.identity);
    }

}
