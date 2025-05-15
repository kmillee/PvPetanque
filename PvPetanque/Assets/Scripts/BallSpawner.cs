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
        //spawnCochonnet();
        //spawnBall();
    }
    public void spawnBall()
    {
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Ball ballScript = ballPrefab.GetComponent<Ball>(); 
        // Set the material based on the team
        if (ballScript.team == Team.TeamA)
            ballPrefab.GetComponent<Renderer>().material = teamAMaterial;
        else{
            ballPrefab.GetComponent<Renderer>().material = teamBMaterial;
        }

    }

    public void spawnCochonnet()
    {
        Instantiate(cochonnetPrefab, spawnPoint.position, Quaternion.identity);
    }

}
