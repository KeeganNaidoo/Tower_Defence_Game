using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Block Prefabs")]
    public GameObject snowBlockPrefab;    // Assign in Inspector
    public GameObject iceBlockPrefab;     // Assign in Inspector
    public GameObject pathBlockPrefab;    // Assign in Inspector
    public GameObject platformPrefab;     // Specific platform prefab
    public GameObject mainTowerPrefab;    // Main tower prefab

    [Header("Terrain Settings")]
    public int gridSize = 40;             // 40x40 grid
    public float heightVariation = 4f;    // Max height variation

    [Header("Path Settings")]
    public int pathWidth = 3;             // Width of each path
    public int platformCountPerSide = 3;  // Number of platforms per side of each path

    // Offsets for Perlin Noise to ensure different terrain each play
    private float noiseOffsetX;
    private float noiseOffsetZ;

    // HashSet to track positions occupied by paths and platforms
    public HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    void Start()
    {
        // Initialize random offsets
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetZ = Random.Range(0f, 1000f);

        GenerateTerrain();
        CreatePaths();
        CreateDefenderPlatforms();
        SpawnMainTower();
    }

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

                // Instantiate block at calculated position
                Vector3 position = new Vector3(x, height, z);
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);
                block.transform.SetParent(this.transform);
            }
        }
    }

    void CreatePaths()
    {
        // Create a path from the north edge (z = gridSize - 1) to the center
        CreateStraightPath(new Vector3(gridSize / 2, 0, gridSize - 1), Vector3.back);

        // Create a path from the west edge (x = 0) to the center
        CreateStraightPath(new Vector3(0, 0, gridSize / 2), Vector3.right);

        // Create a path from the east edge (x = gridSize - 1) to the center
        CreateStraightPath(new Vector3(gridSize - 1, 0, gridSize / 2), Vector3.left);
    }

    void CreateStraightPath(Vector3 startPosition, Vector3 direction)
    {
        int halfWidth = pathWidth / 2;

        for (int i = 0; i < gridSize / 2; i++)
        {
            Vector3 pathPosition = startPosition + direction * i;

            // Create a path block for each position within the path width
            for (int w = -halfWidth; w <= halfWidth; w++)
            {
                Vector3 blockPosition;

                if (direction == Vector3.back || direction == Vector3.forward)
                {
                    blockPosition = pathPosition + new Vector3(w, 0, 0);
                }
                else // Vector3.right or Vector3.left
                {
                    blockPosition = pathPosition + new Vector3(0, 0, w);
                }

                // Record the position as occupied
                Vector3 occupiedPosition = new Vector3(blockPosition.x, 3, blockPosition.z); // Set Y to 2 for path
                occupiedPositions.Add(occupiedPosition);

                // Instantiate path block at elevated height (Y = 2)
                Instantiate(pathBlockPrefab, occupiedPosition, Quaternion.identity, this.transform);
            }
        }
    }

    void CreateDefenderPlatforms()
    {
        int sideSpacing = gridSize / (2 * platformCountPerSide); // Determine spacing between platforms on each side

        for (int i = 0; i < platformCountPerSide; i++)
        {
            // Create platforms for each side of the north, west, and east paths
            CreatePlatformsAlongPath(Vector3.back, gridSize / 2, gridSize - 1 - (sideSpacing * i));
            CreatePlatformsAlongPath(Vector3.right, gridSize / 2 - (sideSpacing * i), gridSize / 2);
            CreatePlatformsAlongPath(Vector3.left, gridSize / 2 + (sideSpacing * i), gridSize / 2);
        }
    }

    void CreatePlatformsAlongPath(Vector3 direction, float x, float z)
    {
        // Adjust position for platforms relative to paths and elevate them by 1 block (Y = 3)
        // Shift platforms an additional block further away from the path (distance = 2 units)
        Vector3 leftSidePlatform = new Vector3(x - pathWidth / 2 - 2, 4, z); // Shift 2 blocks left
        Vector3 rightSidePlatform = new Vector3(x + pathWidth / 2 + 2, 4, z); // Shift 2 blocks right

        // Ensure platforms don't interfere with the main tower at the center
        if (Mathf.Abs(leftSidePlatform.x - gridSize / 2) > 2 && Mathf.Abs(leftSidePlatform.z - gridSize / 2) > 2)
        {
            CreatePlatform(leftSidePlatform);
        }

        if (Mathf.Abs(rightSidePlatform.x - gridSize / 2) > 2 && Mathf.Abs(rightSidePlatform.z - gridSize / 2) > 2)
        {
            CreatePlatform(rightSidePlatform);
        }
    }

    void CreatePlatform(Vector3 position)
    {
        // Record the position as occupied
        Vector3 occupiedPosition = new Vector3(position.x, 2f, position.z); // Set Y to 3 for platform
        occupiedPositions.Add(occupiedPosition);

        // Instantiate a platform block (use the specified prefab)
        GameObject platform = Instantiate(platformPrefab, occupiedPosition, Quaternion.identity);
        platform.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);  // Adjust size if needed
        platform.transform.SetParent(this.transform);
    }

    void SpawnMainTower()
    {
        // The tower will be placed at the center of the grid where the paths converge
        Vector3 towerPosition = new Vector3(gridSize / 2, 3, gridSize / 2);  // Tower is on platform level (Y = 3)
        
        // Instantiate the main tower
        Instantiate(mainTowerPrefab, towerPosition, Quaternion.identity, this.transform);
    }
}