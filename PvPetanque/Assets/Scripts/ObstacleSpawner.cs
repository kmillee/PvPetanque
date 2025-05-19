using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

    public GameObject[] obstacles; // Array of obstacle prefabs
    int obstacleCount = 10; // Number of obstacles to spawn
    public MeshCollider meshCollider; // MeshCollider component for the terrain
    public int seed = 143; // Seed for random number generation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Random.InitState(seed);
        
        if (meshCollider == null)
        {
            Debug.LogError("MeshCollider component not found on the GameObject.");
            return;
        }

        spawnObstacle();
    }

    void spawnObstacle() {
        // Check if the obstacles array is not empty
        if (obstacles.Length == 0)
        {
            Debug.LogError("No obstacle prefabs assigned in the inspector.");
            return;
        }


        Vector3 center = meshCollider.bounds.center;
        Vector3 extents = meshCollider.bounds.extents;
        float height = meshCollider.bounds.max.y + 1f; 
        
        for(int i = 0 ; i < obstacleCount; i++) {
            GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Length)];
           
            float randX = Random.Range(-extents.x, extents.x);
            float randZ = Random.Range(-extents.z, extents.z);
            Vector3 worldXZ = new Vector3(randX + center.x, height, randZ + center.z);
           
            RaycastHit hit;
        
            // Find ground position
            if(Physics.Raycast(worldXZ, Vector3.down, out hit, height)){
                Collider collider = obstaclePrefab.GetComponent<Collider>();
                Vector3 randomPosition = new Vector3(hit.point.x, hit.point.y + 1f, hit.point.z);
                
                // Randomly rotate the obstacle in all axis
                float randomXRotation = Random.Range(0f, 360f);
                float randomYRotation = Random.Range(0f, 360f);
                float randomZRotation = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(randomXRotation, randomYRotation, randomZRotation);

                GameObject obstacle = Instantiate(obstaclePrefab, randomPosition, randomRotation);

                // Scale down the obstacle
                float randomScale = Random.Range(0.05f, 0.1f);
                obstacle.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                obstacle.transform.SetParent(transform, true);

            }
        }
    }
}
