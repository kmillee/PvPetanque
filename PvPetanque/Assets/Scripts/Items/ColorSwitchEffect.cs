using UnityEngine;

[CreateAssetMenu(menuName = "Game Effects/Switch Terrain Color")]
public class ColorSwitchEffect : GameEffect
{
    private Color originalColor;
    private Material terrainMaterialInstance;

    public override void Apply(GameObject target)
    {
        GameObject terrain = GameObject.Find("Sandv2"); // Change to your terrain's actual name

        if (terrain != null)
        {
            Renderer renderer = terrain.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color newColor = new Color(Random.value, Random.value, Random.value); // Generate a random color

                // Create an instance of the material to avoid affecting the shared asset
                terrainMaterialInstance = renderer.material;

                if (terrainMaterialInstance.HasProperty("_Color"))
                {
                    originalColor = terrainMaterialInstance.color;
                    terrainMaterialInstance.color = newColor;
                }
            }
        }
    }

    public override void Revert(GameObject target)
    {
        if (terrainMaterialInstance != null)
        {
            terrainMaterialInstance.color = originalColor;
        }
    }
}
