using UnityEngine;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    [Header("World Settings")]
    public int worldWidth = 300;
    public int worldHeight = 20;
    public float blockSize = 0.16f;

    [Header("Noise Settings")]
    public float noiseScale = 150f;
    public int seed;
    public int octaves = 4;

    [Header("Generation Settings")]
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public Vector2 offset;

    [Header("References")]
    public GameObject grassBlockPrefab;
    public GameObject dirtBlockPrefab;
    public Transform worldParent;

    private Block[,] world;
    private Dictionary<Vector2Int, GameObject> blockObjects;

    void Start()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        world = new Block[worldWidth, worldHeight];
        blockObjects = new Dictionary<Vector2Int, GameObject>();

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                world[x, y] = new Block(Block.BlockType.Air);
            }
        }

        if (seed == 0)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        GenerateTerrain();
        GenerateVegetation();
        RenderWorld();
    }

    void GenerateTerrain()
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        float[] heights = new float[worldWidth];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int x = 0; x < worldWidth; x++)
        {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;

            for (int i = 0; i < octaves; i++)
            {
                float sampleX = (x - worldWidth / 2f) / noiseScale * frequency + octaveOffsets[i].x;
                float sampleY = 0;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                noiseHeight += perlinValue * amplitude;

                amplitude *= persistence;
                frequency *= lacunarity;
            }

            heights[x] = noiseHeight;

            if (noiseHeight > maxNoiseHeight)
            {
                maxNoiseHeight = noiseHeight;
            }

            if (noiseHeight < minNoiseHeight)
            {
                minNoiseHeight = noiseHeight;
            }
        }

        for (int x = 0; x < worldWidth; x++)
        {
            float normalizedHeight = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heights[x]);
            int groundHeight = Mathf.RoundToInt(normalizedHeight * (worldHeight / 2f)) + worldHeight / 4;

            for (int y = 0; y < worldHeight; y++)
            {
                if (y < groundHeight)
                {
                    if (y < groundHeight - 1)
                    {
                        world[x, y] = new Block(Block.BlockType.Dirt);
                    }
                    else
                    {
                        world[x, y] = new Block(Block.BlockType.Grass);
                    }
                }
                else
                {
                    world[x, y] = new Block(Block.BlockType.Air);
                }
            }
        }
    }

    void GenerateVegetation()
    {
        System.Random prng = new System.Random(seed + 2);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (world[x, y].type == Block.BlockType.Grass)
                {
                    if (prng.NextDouble() < 0.1f)
                    {
                        int vegetationY = y + 1;
                        if (vegetationY < worldHeight)
                        {
                            world[x, vegetationY] = new Block(Block.BlockType.Air);
                        }
                    }
                }
            }
        }
    }

    void RenderWorld()
    {
        foreach (var blockObj in blockObjects.Values)
        {
            if (blockObj != null)
            {
                Destroy(blockObj);
            }       
        }
        blockObjects.Clear();

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (world[x, y].type != Block.BlockType.Air)
                {
                    CreateBlockObject(x, y, world[x, y]);
                }
            }
        }
    }

    void CreateBlockObject(int x, int y, Block block)
    {
        Vector3 position = new Vector3(x * blockSize, y * blockSize, 0);
        GameObject prefab = GetBlockPrefab(block.type);
        GameObject blockObj = Instantiate(prefab, position, Quaternion.identity, worldParent);

        if (block.isSolid)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer >= 0 && groundLayer <= 31)
            {
                blockObj.layer = groundLayer;
            }
        }

        blockObjects[new Vector2Int(x, y)] = blockObj;
    }

    GameObject GetBlockPrefab(Block.BlockType type)
    {
        switch (type)
        {
            case Block.BlockType.Grass:
                return grassBlockPrefab;
            case Block.BlockType.Dirt:
                return dirtBlockPrefab;
            default:
                return dirtBlockPrefab;
        }
    }
}