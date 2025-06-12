using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{

    public GameObject[] obstacles; // Array of obstacle prefabs
    public int obstacleCount = 10; // Number of obstacles to spawn
    public MeshCollider meshCollider; // MeshCollider component for the terrain
    public int seed = 143; // Seed for random number generation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (meshCollider == null)
        {
            Debug.LogError("MeshCollider component not found on the GameObject.");
            return;
        }
    }

    public void spawnObstacle() 
    {
        Random.InitState(seed);
    
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
                Vector3 randomPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                
                // Randomly rotate the obstacle in all axis
                float randomXRotation = Random.Range(0f, 360f);
                float randomYRotation = Random.Range(0f, 360f);
                float randomZRotation = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(randomXRotation, randomYRotation, randomZRotation);

                GameObject obstacle = Instantiate(obstaclePrefab, randomPosition, randomRotation);

                // Scale down obstacle
                float randomScale = 1f;
                if (obstaclePrefab.name.Contains("Stick")) {
                    randomScale = Random.Range(0.005f, 0.01f);
                } else {
                    randomScale = Random.Range(0.01f, 0.1f);
                }

                obstacle.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                // Adjust y position based on longest axis
                float longestAxis = Mathf.Max(obstacle.transform.localScale.x, obstacle.transform.localScale.y, obstacle.transform.localScale.z);
                obstacle.transform.position = new Vector3(obstacle.transform.position.x, obstacle.transform.position.y + longestAxis + 1f, obstacle.transform.position.z);

                // ------- Physics Setup -------
                // Add Rigidbody component
                Rigidbody rb = obstacle.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = obstacle.AddComponent<Rigidbody>();
                }
                rb.useGravity = true; // Disable gravity for the obstacle

                // Add a collider to the obstacle
                if (obstacle.GetComponent<Collider>() == null)
                {
                    obstacle.AddComponent<BoxCollider>();
                }


                obstacle.transform.SetParent(transform, true);
            }
        }
    }
}
