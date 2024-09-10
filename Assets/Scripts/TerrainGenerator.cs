using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.AI;
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
    
    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;  // Reference to the NavMeshSurface component

    // Offsets for Perlin Noise to ensure different terrain each play
    private float noiseOffsetX;
    private float noiseOffsetZ;
    
    [Header("Tree Settings")]
    public GameObject[] treePrefabs;
    public int maxTrees = 50;

    private List<Vector3> availableSnowPositions = new List<Vector3>();

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
        GenerateRandomTrees();
        BakeNavMesh();
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

                Vector3 blockPosition = new Vector3(x, Mathf.Floor(height), z);
                // Instantiate block at calculated position
                Vector3 position = new Vector3(x, height, z);
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);
                block.transform.SetParent(this.transform);
                
                if (blockPrefab == snowBlockPrefab)
                {
                    availableSnowPositions.Add(blockPosition);
                }
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

        for (int i = 0; i <= gridSize / 2; i++) // Fixed to iterate through half the grid size for correct path length
        {
            Vector3 pathPosition = startPosition + direction * i;

            for (int w = -halfWidth; w <= halfWidth; w++)
            {
                Vector3 blockPosition;

                if (direction == Vector3.back || direction == Vector3.forward)
                {
                    // Path from North or South (affects Z axis)
                    blockPosition = pathPosition + new Vector3(w, 0, 0); // Modify X-axis width
                }
                else
                {
                    // Path from West or East (affects X axis)
                    blockPosition = pathPosition + new Vector3(0, 0, w); // Modify Z-axis width
                }

                // Set the block to path height (level 3)
                Vector3 occupiedPosition = new Vector3(blockPosition.x, 3, blockPosition.z);
                occupiedPositions.Add(occupiedPosition);
                AddToOccupiedWithBuffer(occupiedPosition);

                // Instantiate path block at calculated position
                Instantiate(pathBlockPrefab, occupiedPosition, Quaternion.identity, this.transform);
            }
        }
    }

    void CreateDefenderPlatforms()
{
    int sideSpacing = gridSize / (2 * platformCountPerSide); // Determine spacing between platforms on each side

    // Platforms along the north path
    for (int i = 0; i < platformCountPerSide; i++)
    {
        // Platform on the left and right side of the north path
        CreatePlatformsAlongPath(Vector3.back, gridSize / 2, gridSize - 1 - (sideSpacing * i));
    }

    // Platforms along the west path
    for (int i = 0; i < platformCountPerSide; i++)
    {
        // Platform on the left and right side of the west path
        CreatePlatformsAlongPath(Vector3.right, 1 + (sideSpacing * i), gridSize / 2); // Adjust position for west path platforms
    }

    // Platforms along the east path
    for (int i = 0; i < platformCountPerSide; i++)
    {
        // Platform on the left and right side of the east path
        CreatePlatformsAlongPath(Vector3.left, gridSize - 1 - (sideSpacing * i), gridSize / 2); // Adjust position for east path platforms
    }
}

void CreatePlatformsAlongPath(Vector3 direction, float x, float z)
{
    // Adjust position for platforms relative to paths and elevate them by 1 block (Y = 3)
    Vector3 leftSidePlatform, rightSidePlatform;

    if (direction == Vector3.back || direction == Vector3.forward)
    {
        // Path is along Z-axis (north/south)
        leftSidePlatform = new Vector3(x - pathWidth / 2 - 2, 3, z);  // Shift platforms 2 blocks to the left of the path
        rightSidePlatform = new Vector3(x + pathWidth / 2 + 2, 3, z); // Shift platforms 2 blocks to the right of the path
    }
    else
    {
        // Path is along X-axis (west/east)
        leftSidePlatform = new Vector3(x, 3, z - pathWidth / 2 - 2);  // Shift platforms 2 blocks below the path
        rightSidePlatform = new Vector3(x, 3, z + pathWidth / 2 + 2); // Shift platforms 2 blocks above the path
    }

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
    Vector3 occupiedPosition = new Vector3(position.x, 2, position.z); // Set Y to 3 for platform
    occupiedPositions.Add(occupiedPosition);

    // Instantiate a platform block (use the specified prefab)
    GameObject platform = Instantiate(platformPrefab, occupiedPosition, Quaternion.identity);
    platform.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);  // Adjust size if needed
    platform.transform.SetParent(this.transform);
}

    void SpawnMainTower()
    {
        // The tower will be placed at the center of the grid where the paths converge
        Vector3 towerPosition = new Vector3(gridSize / 2, 4, gridSize / 2);  // Tower is on platform level (Y = 5)
        
        // Instantiate the main tower
        Instantiate(mainTowerPrefab, towerPosition, Quaternion.identity, this.transform);
    }
    
    // Function to bake the NavMesh after everything is generated
    void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();  // Bake the NavMesh at runtime
    }
    
    void GenerateRandomTrees()
    {
        int treeCount = 0;

        while (treeCount < maxTrees && availableSnowPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSnowPositions.Count);
            Vector3 treePosition = availableSnowPositions[randomIndex];

            treePosition.y += 1;

            Vector3 checkPosition = new Vector3(treePosition.x, 0, treePosition.z);

            // Check if the tree is trying to be placed on an occupied position or within the 1-block buffer around paths/platforms
            if (!IsPositionOccupied(checkPosition))
            {
                GameObject randomTreePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                Instantiate(randomTreePrefab, treePosition, Quaternion.identity, this.transform);

                treeCount++;
            }

            availableSnowPositions.RemoveAt(randomIndex);
        }
    }

    // Checks if a given position (ignoring Y) is occupied by paths, platforms, or within the 1-block buffer
    bool IsPositionOccupied(Vector3 position)
    {
        foreach (Vector3 occupied in occupiedPositions)
        {
            // We check if the position is within 1 block of any occupied position in the X or Z directions
            if (Mathf.Abs(occupied.x - position.x) <= 1 && Mathf.Abs(occupied.z - position.z) <= 1)
            {
                return true;  // The position or its surrounding area is occupied
            }
        }
        return false;  // The position is free for tree placement
    }

    // Adds the occupied position and a 1-block buffer around it to the occupiedPositions set
    void AddToOccupiedWithBuffer(Vector3 position)
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

