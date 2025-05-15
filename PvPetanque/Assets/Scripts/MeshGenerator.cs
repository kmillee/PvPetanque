using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    public int width=10; // In world unit
    public int length=10; // In world unit
    public int resolution=5; // Number of segments along each axis

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
     int resX = width * resolution;
     int resZ = length * resolution;

    vertices = new Vector3[(resX + 1) * (resZ + 1)];
        triangles = new int[resX * resZ * 6];

        // Generate vertex positions
        for(int x = 0; x <= resX; x++)
        {
            for(int z = 0; z <= resZ; z++)
            {
                // Convert vertex world-space from grid coordinates 
                float xPos = (float)x / resolution;
                float zPos = (float)z / resolution;

                // Sample perlin noise to get bumpiness
                float yPos = Mathf.PerlinNoise(xPos, zPos);

                // Set vertex position
                vertices[x + z * (resX + 1)] = new Vector3(xPos, yPos, zPos);
            }
        }

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
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
