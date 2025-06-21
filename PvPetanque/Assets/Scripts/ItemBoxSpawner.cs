using UnityEngine;

// This script is responsible for spawning power-ups box in the game.
// There is a percentage chance that a power-up box will spawn every turn. 
// A power-up box is collected when a player's ball collides with it. When collected, the box will disappear.
public class ItemBoxSpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager; // Reference to the GameManager to access match settings
    private TeamItemSlot teamASlot;
    private TeamItemSlot teamBSlot;
    [SerializeField] private GameObject itemBoxPrefab; // Prefab of the power-up box
    [SerializeField] private float spawnChance = 0.2f;
    [SerializeField] private int spawnTries = 5; // Number of attempts to spawn a power-up box
    [SerializeField] private MeshCollider terrainCollider; 
    [SerializeField] private int seed = 143; 
    private Vector3 center;
    private Vector3 extents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // Initialize random seed
        Random.InitState(seed);

        if (terrainCollider == null)
        {
            Debug.LogError("MeshCollider component not found on the GameObject.");
            return;
        }
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            // Get team slots from the GameManager
            teamASlot = gameManager.GetTeamItemSlotA();
            teamBSlot = gameManager.GetTeamItemSlotB();
        }


        // Get the center and extents of the terrain
        center = terrainCollider.bounds.center;
        extents = terrainCollider.bounds.extents;
    }

    public void SpawnitemBox()
    {

        Collider prefabCollider = itemBoxPrefab.GetComponent<Collider>();
        float boxExtentX = prefabCollider.bounds.extents.x;
        float boxExtentZ = prefabCollider.bounds.extents.z;
        float boxExtentY = prefabCollider.bounds.extents.y;

        for (int i = 0; i < spawnTries; i++)
        {
            if (Random.value >= spawnChance) continue; // Spawn not successful, skip to next try

            // Generate a random position within the terrain bounds, taking into account prefab's size
            float randX = Random.Range(-extents.x + boxExtentX + 0.2f, extents.x - boxExtentX - 0.2f);
            float randZ = Random.Range(-extents.z + boxExtentZ + 0.2f, extents.z - boxExtentZ - 0.2f);
            Vector3 worldXZ = new Vector3(randX + center.x, terrainCollider.bounds.max.y, randZ + center.z);

            // Get y position by raycasting down from the generated position
            RaycastHit hit;

            // Find ground position
            if (Physics.Raycast(worldXZ, Vector3.down, out hit, terrainCollider.bounds.max.y + 1f))
            {

                float itemBoxHeight = boxExtentY + hit.point.y;

                Vector3 randomPosition = new Vector3(hit.point.x, itemBoxHeight + 0.1f, hit.point.z);

                // Instantiate the power-up box at the random position
                GameObject itemBoxObj = Instantiate(itemBoxPrefab, randomPosition, Quaternion.identity);
                itemBoxObj.transform.SetParent(transform);

                // Set team slots for the item box
                ItemBox itemBox = itemBoxObj.GetComponent<ItemBox>();
                if (itemBox != null)
                    itemBox.SetTeamSlots(teamASlot, teamBSlot);
            }
        }
        
    }

    public void ClearItemBoxes()
    {
        // Clear all item boxes in the scene
        ItemBox[] itemBoxes = FindObjectsOfType<ItemBox>();
        foreach (var itemBox in itemBoxes)
        {
            Destroy(itemBox.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
