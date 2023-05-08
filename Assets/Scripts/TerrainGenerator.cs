using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("References")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    [Header("Terrain")]
    public Vector2 regionSize = new (100, 100);
    public bool terrain;
    public bool grass;
    public bool flowers;
    public bool rocks;
    public bool trees;
    [SerializeField] private bool seededObjects = true;

    [Header("Perlin Noise")]
    public float heightMultiplier;
    public float zoom;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public AnimationCurve heightCurve;
    public Vector2 offset;
    public float falloffAValue;
    public float falloffBValue;
    

    [Header("Grass")]
    public GameObject[] grassPrefabs;
    public float grassSpawnHeight;
    public int grassAmount;
    [Range(0, 180)]
    public float grassXVariation;
    [Range(0, 180)]
    public float grassYVariation;
    [Range(0, 180)]
    public float grassZVariation;

    [Header("Flowers")]
    public GameObject[] flowerPrefabs;
    public float flowerSpawnHeight;
    public int flowerAmount;
    [Range(0, 180)]
    public float flowerXVariation;
    [Range(0, 180)]
    public float flowerYVariation;
    [Range(0, 180)]
    public float flowerZVariation;

    [Header("Trees")]
    public GameObject[] treePrefabs;
    public float treeSpawnHeight;
    public int treeAmount;
    [Range(0, 180)]
    public float treeXVariation;
    [Range(0, 180)]
    public float treeYVariation;
    [Range(0, 180)]
    public float treeZVariation;

    [Header("Rocks")]
    public GameObject[] rockPrefabs;
    public float rockSpawnHeight;
    public int rockAmount;
    [Range(0, 180)]
    public float rockXVariation;
    [Range(0, 180)]
    public float rockYVariation;
    [Range(0, 180)]
    public float rockZVariation;

    Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        if(seededObjects)
            Random.InitState(seed);

        if (terrain)
            SpawnMesh();
        if (grass)
            RandomlySpawn(grassPrefabs, regionSize, grassSpawnHeight, grassAmount, grassXVariation, grassYVariation, grassZVariation);
        if (flowers)
            RandomlySpawn(flowerPrefabs, regionSize, flowerSpawnHeight, flowerAmount, flowerXVariation, flowerYVariation, flowerZVariation);
        if (rocks)
            RandomlySpawn(rockPrefabs, regionSize, rockSpawnHeight, rockAmount, rockXVariation, rockYVariation, rockZVariation);
        if (trees)
            RandomlySpawn(treePrefabs, regionSize, treeSpawnHeight, treeAmount, treeXVariation, treeYVariation, treeZVariation);
    }

    private void OnValidate()
    {
        if (regionSize.x < 1)
            regionSize.x = 1;
        if (regionSize.y < 1)
            regionSize.y = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 1)
            octaves = 1;
        if (zoom <= 0)
            zoom = 0.0001f;
    }

    private void SpawnMesh()
    {
        mesh.Clear();

        mesh.vertices = PerlinNoise.GenerateVertices(regionSize, seed, zoom, octaves, persistance, lacunarity, heightMultiplier, heightCurve, offset, Falloff.GenerateFalloff(regionSize, falloffAValue, falloffBValue));
        mesh.triangles = GenerateTriangles(regionSize);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshCollider.sharedMesh = mesh;
    }

    private void RandomlySpawn(GameObject[] prefabs, Vector2 regionSize, float spawnHeight, int amount, float xVariation, float yVariation, float zVariation)
    {
        for (int i = 0; i < amount;)
        {
            if (Physics.Raycast(new Vector3(Random.Range(0, regionSize.x), 100, Random.Range(0, regionSize.y)), Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > spawnHeight && hit.collider.Equals(meshCollider))
                {
                    Quaternion rotation = new Quaternion();
                    rotation.eulerAngles = new Vector3(Random.Range(-xVariation, xVariation), Random.Range(-yVariation, yVariation), Random.Range(-zVariation, zVariation));
                    Instantiate(prefabs[Random.Range(0, prefabs.Length)], hit.point, rotation);
                    i++;
                }
            }
        }
    }

    private int[] GenerateTriangles(Vector2 regionSize)
    {
        int[] triangles = new int[(int)(regionSize.x * regionSize.y * 6)];

        int currentVertex = 0;
        int currentTriangle = 0;

        for (int z = 0; z < regionSize.y; z++, currentVertex++)
        {
            for (int x = 0; x < regionSize.x; x++, currentVertex++, currentTriangle += 6)
            {
                triangles[currentTriangle + 0] = currentVertex + 0;
                triangles[currentTriangle + 1] = currentVertex + (int)regionSize.x + 1;
                triangles[currentTriangle + 2] = currentVertex + 1;
                triangles[currentTriangle + 3] = currentVertex + 1;
                triangles[currentTriangle + 4] = currentVertex + (int)regionSize.x + 1;
                triangles[currentTriangle + 5] = currentVertex + (int)regionSize.x + 2;
            }
        }

        return triangles;
    }
}
