using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class HeightMapTerrain : MonoBehaviour
{
    public Texture2D heightMap;
    public float heightMultiplier = 5.0f;
    public float offsetHeight = 5.0f;
    public float scale = 5.0f;
    public Material material;
    public float powerHeight = 116.96f;
    public int chunkSize = 64; // Size of one side of a chunk, adjust based on your needs

    void GenerateMeshForChunk(int chunkX, int chunkY)
    {
        int sizeX = heightMap.width;
        int sizey = heightMap.height;
        
        int startX = chunkX * chunkSize;
        int startY = chunkY * chunkSize;
        int endX = Mathf.Min(startX + chunkSize, heightMap.width) + 1;
        int endY = Mathf.Min(startY + chunkSize, heightMap.height) + 1;

        int width = chunkSize + 1;
        int height = chunkSize + 1;
        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Generate vertices
        for (int y = startY, i = 0; y < endY; i++, y++)
        {
            for (int x = startX, j = 0; x < endX; j++, x++)
            {
                try
                {
                    float heightValue = Mathf.Pow(powerHeight, heightMap.GetPixel(x, y).grayscale);
                    vertices[i * width + j] = new Vector3(x * scale, heightValue * heightMultiplier + offsetHeight, y * scale);
                    uvs[i * width + j] = new Vector2((float)x / (sizeX - 1), (float)y / (sizey - 1));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        
        // Generate triangles
        int triIndex = 0;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int baseIndex = y * width + x;
                triangles[triIndex++] = baseIndex;
                triangles[triIndex++] = baseIndex + width;
                triangles[triIndex++] = baseIndex + width + 1;

                triangles[triIndex++] = baseIndex;
                triangles[triIndex++] = baseIndex + width + 1;
                triangles[triIndex++] = baseIndex + 1;
            }
        }

        // Create new Mesh
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals(); // To make sure the lighting gets calculated correctly.

        GameObject g = new GameObject("Chunk_" + chunkX + "_" + chunkY);
        g.transform.parent = parentObject.transform;
        
        // Assign Mesh
        MeshFilter meshFilter = g.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // Assign material
        MeshRenderer meshRenderer = g.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    private GameObject parentObject;
    
    [Button("Generate")]
    private void GenerateMesh()
    {
        if(parentObject != null)
            DestroyImmediate(parentObject);
        
        parentObject = new GameObject("Ground");
        
        int numChunks = Mathf.CeilToInt(heightMap.width / (float)chunkSize);
        for (int chunkX = 0; chunkX < numChunks; chunkX++)
        {
            for (int chunkY = 0; chunkY < numChunks; chunkY++)
            {
                GenerateMeshForChunk(chunkX, chunkY);
            }
        }
    }
}
