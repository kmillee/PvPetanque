using UnityEngine;

public class BallSpawner : MonoBehaviour
{

    public GameObject ballPrefab; // Prefab of the ball to spawn
    public Transform spawnPoint; // Point where the ball will be spawned
    
    void Start()
    {
        spawnBall();
    }
    public void spawnBall()
    {
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
    }
}
