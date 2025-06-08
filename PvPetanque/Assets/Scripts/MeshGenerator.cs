using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    // private Color[] colors;

    public int width=10; // In world unit
    public int length=10; // In world unit
    public int resolution=5; // Number of segments along each axis
    public int seed = 143;

    [SerializeField] private float edgeRaisingDistance;

    [SerializeField] private AnimationCurve waterBiomeCurve;
    [SerializeField] private AnimationCurve grassBiomeCurve;
    [SerializeField] private AnimationCurve sandBiomeCurve;
    
    [SerializeField] private float biomeSize;

    [SerializeField] private Vector2 manualHeightNoiseOffset;
    [SerializeField] private Vector2 manualBiomeNoiseOffset;
    [SerializeField] private Vector2 manualNoiseOffset;
    
    
    private Vector2 heightNoiseOffset;
    private Vector2 biomeNoiseOffset;
    
    void Start()
    {
        Random.InitState(seed);
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        heightNoiseOffset = new Vector2(Random.value * 1000f, Random.value * 1000f) + manualHeightNoiseOffset + manualNoiseOffset;
        biomeNoiseOffset = new Vector2(Random.value * 1000f, Random.value * 1000f) + manualBiomeNoiseOffset + manualNoiseOffset;
        CreateShape();
        UpdateMesh();
    }

    private void OnValidate()
    {
        Start();
    }

    void CreateShape()
    {
        int resX = width * resolution;
        int resZ = length * resolution;

        vertices = new Vector3[(resX + 1) * (resZ + 1)];
        uv = new Vector2[(resX + 1) * (resZ + 1)];
        triangles = new int[resX * resZ * 6];
        // colors = new Color[(resX + 1) * (resZ + 1)];

        // Generate triangle indices 
        int vert = 0;
        for (int x = 0; x < resX; x++)
        {
            for (int z = 0; z < resZ; z++)
            {
                triangles[vert] = x + z * (resX + 1);
                triangles[vert + 1] = x + (z + 1) * (resX + 1);
                triangles[vert + 2] = (x + 1) + z * (resX + 1);
                triangles[vert + 3] = (x + 1) + z * (resX + 1);
                triangles[vert + 4] = x + (z + 1) * (resX + 1);
                triangles[vert + 5] = (x + 1) + (z + 1) * (resX + 1);
                vert += 6;
            }
        }
        
        
        
        // Generate vertex positions -> height based on perlin noise
        for (int x = 0; x <= resX; x++)
        {
            for(int z = 0; z <= resZ; z++)
            {
                // Convert vertex world-space from grid coordinates 
                float xPos = (float)x / (float)resolution;
                float zPos = (float)z / (float)resolution;
                
                float biome = Mathf.PerlinNoise(xPos / biomeSize + biomeNoiseOffset.x, zPos / biomeSize + biomeNoiseOffset.y);
                float height = Mathf.PerlinNoise(xPos + heightNoiseOffset.x, zPos + heightNoiseOffset.y);
                
                // Set vertex color
                // colors[x + z * (resX + 1)] = biome switch
                // {
                //     < 0.1f => Color.blue,
                //     < 0.2f => Color.green,
                //     _ => Color.yellow
                // };
                
                // "biome" based position
                float yPos = biome switch
                {
                    < 0.25f => waterBiomeCurve.Evaluate(height),
                    < 0.5f => grassBiomeCurve.Evaluate(height),
                    _ => sandBiomeCurve.Evaluate(height)
                };
                
                // edges position adjustement
                float distanceToEdge = Mathf.Min(xPos, zPos, width - xPos, length - zPos);
                if (distanceToEdge < edgeRaisingDistance)
                {
                    float adjustedDistanceToEdge = 1.0f - distanceToEdge / edgeRaisingDistance;
                    yPos = Mathf.Max(yPos, adjustedDistanceToEdge);
                }

                // Set vertex position
                vertices[x + z * (resX + 1)] = new Vector3(xPos, yPos, zPos);
                
                // Set UV coordinates
                uv[x + z * (resX + 1)] = new Vector2(xPos / width, zPos / width);
            }
        }
        
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        // mesh.colors = colors;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
    }
}
