using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject snowBlockPrefab; // Snow block prefab
    public GameObject iceBlockPrefab; // Ice block prefab
    public GameObject walkableBlockPrefab; // Walkable terrain block prefab
    public GameObject enemySpawnPointPrefab; // Enemy spawn point prefab
    public GameObject towerSpawnPointPrefab; // Player main tower prefab

    public int width = 20; // Grid width
    public int depth = 10; // Grid depth
    public float blockSize = 1.0f; // Size of each block
    public int blocksPerFrame = 20; // How many blocks to generate per frame for faster generation

    private GameObject[,] spawnedBlocks; // Track spawned blocks to avoid overlap

    void Start()
    {
        spawnedBlocks = new GameObject[width, depth]; // Track spawned blocks to avoid overlap
        StartCoroutine(GenerateRandomizedTerrain()); // Start generating the terrain
    }

    IEnumerator GenerateRandomizedTerrain()
    {
        int blocksGenerated = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Randomly choose block type with a higher chance of snow block
                GameObject blockToSpawn = GenerateTerrainBlock(x, z);

                // Instantiate the block at the correct position
                Vector3 position = new Vector3(x * blockSize, 0, z * blockSize);
                GameObject newBlock = Instantiate(blockToSpawn, position, Quaternion.identity, transform);

                // Track the newly spawned block
                spawnedBlocks[x, z] = newBlock;

                // Track the number of blocks generated per frame
                blocksGenerated++;

                // Once the set number of blocks has been generated, yield to the next frame
                if (blocksGenerated >= blocksPerFrame)
                {
                    blocksGenerated = 0;
                    yield return null; // Pause and wait for the next frame
                }
            }
        }

        // Place spawn points after the terrain is generated
        PlaceSpawnPoints();

        // Once the terrain is generated, create the walkable paths
        GenerateStraightPaths();
    }

    GameObject GenerateTerrainBlock(int x, int z)
    {
        // Check if we are in an area that should clump ice into a pond
        if (Random.value < 0.2f) // 20% chance to create an ice pond
        {
            return iceBlockPrefab;
        }
        else // The rest will be snow blocks
        {
            return snowBlockPrefab;
        }
    }

    void PlaceSpawnPoints()
    {
        // Place enemy spawn point at the left side of the grid, centered vertically
        Vector3 enemySpawnPosition = new Vector3(0 * blockSize, 0, (depth / 2) * blockSize);
        Instantiate(enemySpawnPointPrefab, enemySpawnPosition, Quaternion.identity, transform);

        // Place player tower at the right side of the grid, centered vertically
        Vector3 towerSpawnPosition = new Vector3((width - 1) * blockSize, 0, (depth / 2) * blockSize);
        Instantiate(towerSpawnPointPrefab, towerSpawnPosition, Quaternion.identity, transform);
    }

    void GenerateStraightPaths()
    {
        int pathWidth = 2; // Width of each path
        int numPaths = 3; // Number of paths

        // Define the start (enemy spawn) and end (player tower) locations
        Vector3 start = new Vector3(0, 0, depth / 2); // Enemy spawn point
        Vector3 end = new Vector3(width - 1, 0, depth / 2); // Player tower point

        // Define spacing between paths
        float pathSpacing = 4.0f; // Distance between the center of each path

        // Calculate start and end positions for each path
        Vector3[] pathStarts = new Vector3[numPaths];
        Vector3[] pathEnds = new Vector3[numPaths];

        for (int i = 0; i < numPaths; i++)
        {
            float offset = (i - (numPaths - 1) / 2f) * pathSpacing;
            pathStarts[i] = new Vector3(start.x, start.y, start.z + offset);
            pathEnds[i] = new Vector3(end.x, end.y, end.z + offset);
        }

        // Create paths
        for (int i = 0; i < numPaths; i++)
        {
            Vector3 startPosition = pathStarts[i];
            Vector3 endPosition = pathEnds[i];

            // Generate path from start to end
            for (int x = Mathf.RoundToInt(startPosition.x); x <= Mathf.RoundToInt(endPosition.x); x++)
            {
                float t = (x - startPosition.x) / (endPosition.x - startPosition.x);
                float z = Mathf.Lerp(startPosition.z, endPosition.z, t);

                for (int j = -pathWidth / 2; j <= pathWidth / 2; j++)
                {
                    int zPos = Mathf.Clamp(Mathf.RoundToInt(z) + j, 0, depth - 1);

                    // Destroy existing blocks and place walkable blocks
                    if (spawnedBlocks[x, zPos] != null)
                    {
                        Destroy(spawnedBlocks[x, zPos]);
                    }

                    Vector3 position = new Vector3(x * blockSize, 0, zPos * blockSize);
                    Instantiate(walkableBlockPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}