using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject snowBlockPrefab;    // Assign in Inspector
    public GameObject iceBlockPrefab;     // Assign in Inspector
    

    [Header("Terrain Settings")]
    public int gridSize = 40;             // 40x40 grid
    public float heightVariation = 4f;
    
    
    [Header("Tree Settings")]
    public GameObject[] treePrefabs;    // Array of tree prefabs (3 types of trees)
    public int maxTrees = 50;           // Limiter for total number of trees to spawn
    public float treeRadius = 1f;       // Radius to check for tree collision

    // Offsets for Perlin Noise to ensure different terrain each play
    private float noiseOffsetX;
    private float noiseOffsetZ;

    private List<Vector3> availableSnowPositions = new List<Vector3>(); // Positions of available snow blocks
    // HashSet to track positions occupied by paths and platforms
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    void Start()
    {
        // Initialize random offsets
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetZ = Random.Range(0f, 1000f);

        GenerateTerrain();
        GenerateRandomTrees();
    }

    // Generates the terrain with snow and ice blocks and stores snow block positions
    void GenerateTerrain()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Check if this position is occupied by a path or platform
                Vector3 positionToCheck = new Vector3(x, 0, z);
                if (occupiedPositions.Contains(positionToCheck))
                {
                    // Skip creating terrain block if position is occupied
                    continue;
                }

                // Randomly choose between snow and ice blocks (80% snow, 20% ice)
                GameObject blockPrefab = Random.value > 0.2f ? snowBlockPrefab : iceBlockPrefab;
                

                // Calculate Perlin Noise height with dynamic offsets
                float noiseScale = 0.1f; // Adjust for more or less variation
                float height = Mathf.PerlinNoise((x + noiseOffsetX) * noiseScale, (z + noiseOffsetZ) * noiseScale) * heightVariation;
                Vector3 blockPosition = new Vector3(x, Mathf.Floor(height), z);

                // Instantiate block at calculated position
                Vector3 position = new Vector3(x, height, z);
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);
                block.transform.SetParent(this.transform);
                
                // Add snow block positions to the list for tree placement
                if (blockPrefab == snowBlockPrefab)
                {
                    availableSnowPositions.Add(blockPosition);
                }
            }
        }
    }

    // Randomly generates trees on available snow blocks, avoiding paths, platforms, or other objects with specific tags
    void GenerateRandomTrees()
    {
        int treeCount = 0;

        while (treeCount < maxTrees && availableSnowPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSnowPositions.Count);
            Vector3 treePosition = availableSnowPositions[randomIndex];
            treePosition.y += 1; // Ensure the tree is placed above the terrain

            // Check if the tree is trying to be placed on or near an occupied position or collides with any paths or platforms
            if (!IsPositionOccupiedOrOverlapping(treePosition))
            {
                GameObject randomTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Instantiate(randomTreePrefab, treePosition, Quaternion.identity, this.transform);
                treeCount++;
            }

            availableSnowPositions.RemoveAt(randomIndex); // Remove the checked position
        }
    }

    // Checks if a given position (ignoring Y) is occupied by paths, platforms, or overlaps with any tagged objects
    bool IsPositionOccupiedOrOverlapping(Vector3 position)
    {
        // First, check against the pre-defined occupied positions
        foreach (Vector3 occupied in occupiedPositions)
        {
            if (Mathf.Abs(occupied.x - position.x) <= 1 && Mathf.Abs(occupied.z - position.z) <= 1)
            {
                return true;  // The position is near an occupied space
            }
        }

        // Check for any nearby objects with the tags "Path" or "Platform" within a given radius (treeRadius)
        Collider[] hitColliders = Physics.OverlapSphere(position, treeRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Path") || hitCollider.CompareTag("Platform"))
            {
                return true;  // The position is overlapping or too close to a path or platform
            }
        }

        return false;  // The position is free for tree placement
    }

    // Adds the occupied position and a 1-block buffer around it to the occupiedPositions set
    public void AddToOccupiedWithBuffer(Vector3 position)
    {
        occupiedPositions.Add(position);

        // Add surrounding positions to the occupiedPositions set to create the 1-block buffer
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                Vector3 bufferedPosition = new Vector3(position.x + dx, position.y, position.z + dz);
                occupiedPositions.Add(bufferedPosition);
            }
        }
    }
}