﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    internal event EventHandler<TickEventArgs> OnTick;
    internal Dictionary<BlockType, GameObject> blockPrefabDictionary;
    internal List<WorldChunk> activeChunks;
    internal Dictionary<BlockType, Queue<GameObject>> blockPools;
    internal Dictionary<BlockType, Transform> blockPoolObjects;
    internal Vector3 spawnpoint = Vector3.zero;
    internal int radius = 1;
    internal int startTime;
    internal WorldChunk spawnChunk;
    private Dictionary<Vector2, WorldRegion> regionMap;
    private Vector3 chunkPos;
    private Vector3 cachePos;
    private Camera mainCamera;
    private long time = 0;
    private long seed;
    private int timeScale = 1;
    private List<WorldChunk> removeChunks;
    private int renderDistance = 0;
    private EntityPlayer player;

    [Header("Prefab Settings")]
    public GameObject regionPrefab;
    public GameObject playerPrefab;
    [Header("Pool Settings")]
    public List<BlockPoolInfo> pools;
    public List<BlockPrefab> blockPrefabs;

    void Awake()
    {
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity, transform.root).GetComponent<EntityPlayer>();
        mainCamera = Camera.main;

        seed = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        System.Random rand = new System.Random((int)seed);
        seed = rand.Next(int.MaxValue - 1);

        startTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        cachePos = new Vector3(int.MaxValue, 0, int.MaxValue);
        renderDistance = Mathf.RoundToInt(mainCamera.farClipPlane / 16f);

        blockPrefabDictionary = new Dictionary<BlockType, GameObject>();
        blockPools = new Dictionary<BlockType, Queue<GameObject>>();
        blockPoolObjects = new Dictionary<BlockType, Transform>();
        removeChunks = new List<WorldChunk>();

        for (int i = 0; i < blockPrefabs.Count; i++)
            blockPrefabDictionary.Add(blockPrefabs[i].type, blockPrefabs[i].prefab);

        foreach (BlockPoolInfo p in pools)
        {
            GameObject poolGo = new GameObject("p_" + p.type.ToString());
            poolGo.transform.parent = transform;
            GameObject prefab = blockPrefabDictionary[p.type];
            blockPools.Add(p.type, new Queue<GameObject>());
            blockPoolObjects.Add(p.type, poolGo.transform);
            for (int i = 0; i < p.poolSize; i++)
            {
                GameObject o = Instantiate(prefab, Vector3.zero, Quaternion.identity, poolGo.transform);
                blockPools[p.type].Enqueue(o);
            }
        }

        GetComponent<LightingCycle>().Configure(this);
        GetComponent<MusicService>().Configure(this);
        Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) timeScale /= 2;
        if (Input.GetKeyDown(KeyCode.RightArrow)) timeScale *= 2;

        UpdateWorld();
    }

    void Tick()
    {
        time += timeScale;
        TickEvent(new TickEventArgs() { time = time });

        if (player.transform.position.y < 0)
            player.transform.position = spawnpoint;
    }

    void UpdateWorld()
    {
        chunkPos = new Vector3(Mathf.Floor(player.transform.position.x / 16), 0, Mathf.Floor(player.transform.position.z / 16));

        if (cachePos != chunkPos)
        {
            cachePos = chunkPos;
            for (int i = (int)chunkPos.x - renderDistance; i <= (int)chunkPos.x + renderDistance; i++) {
                for (int j = (int)chunkPos.z - renderDistance; j <= (int)chunkPos.z + renderDistance; j++)
                {
                    //int regI = i < 0 ? Mathf.FloorToInt(i / 8f) : Mathf.CeilToInt(i / 8f);
                    //int regJ = j < 0 ? Mathf.FloorToInt(j / 8f) : Mathf.CeilToInt(j / 8f);

                    int regI = Mathf.FloorToInt(i / 8f);
                    int regJ = Mathf.FloorToInt(j / 8f);

                    int inRegI = (i * 16 - regI * 128) / 16;
                    int inRegJ = (j * 16 - regJ * 128) / 16;

                    Vector2 regV = new Vector2(regI, regJ);
                    Vector2 v = new Vector2(inRegI, inRegJ);

                    /*if (regionMap.ContainsKey(regV) &&
                        regionMap[regV].chunkMap.ContainsKey(v))
                        Debug.Log("Relative Chunk " + new Vector2(i, j) + " returned: " + regionMap[regV].chunkMap[v]);*/
                    
                    if (regionMap.ContainsKey(regV) &&
                        regionMap[regV].chunkMap.ContainsKey(v) &&
                        !activeChunks.Contains(regionMap[regV].chunkMap[v]))
                    {
                        regionMap[regV].chunkMap[v].Activate();
                        activeChunks.Add(regionMap[regV].chunkMap[v]);
                    }
                }
            }   

            removeChunks.Clear();

            foreach (WorldChunk c in activeChunks)
                if (Vector3.Distance(c.transform.position + new Vector3(8, 0, 8), player.transform.position) > mainCamera.farClipPlane)
                    removeChunks.Add(c);

            foreach (WorldChunk c in removeChunks)
            {
                c.Deactivate();
                activeChunks.Remove(c);
            }

        }
    }

    public WorldChunk GetChunkAtPosition(int i, int j)
    {
        int regI = i < 0 ? Mathf.FloorToInt(i / 8f) : Mathf.CeilToInt(i / 8f);
        int regJ = j < 0 ? Mathf.FloorToInt(j / 8f) : Mathf.CeilToInt(j / 8f);

        int inRegI = (i * 16 - regI * 128) / 16;
        int inRegJ = (j * 16 - regJ * 128) / 16;

        Vector2 regV = new Vector2(regI, regJ);
        Vector2 v = new Vector2(inRegI, inRegJ);

        try
        {
            WorldRegion reg = regionMap[regV];
            WorldChunk chnk = reg.chunkMap[v];

            return chnk;
        }
        catch (Exception) { return null; }
    }

    void Generate()
    {
        regionMap = new Dictionary<Vector2, WorldRegion>();
        activeChunks = new List<WorldChunk>();

        for (int i = -radius; i < radius; i++)
            for (int j = -radius; j < radius; j++)
            {
                GameObject region = Instantiate(regionPrefab, Vector3.zero, Quaternion.identity, transform);
                region.name = "r_" + i + "," + j;
                region.transform.localPosition = new Vector3(i * 128, 0, j * 128);
                regionMap.Add(new Vector2(i, j), region.GetComponent<WorldRegion>());
                region.GetComponent<WorldRegion>().Configure(this, seed);
            }

        spawnpoint = new Vector3(0, 21, 0);
        
        player.transform.position = spawnpoint;

        InvokeRepeating("Tick", .0f, .05f);
    }

    protected virtual void TickEvent(TickEventArgs e)
    {
        OnTick?.Invoke(this, e);
    }
}

[Serializable]
public class BlockPrefab
{
    public BlockType type;
    public GameObject prefab;
}

public class TickEventArgs : EventArgs
{
    public long time { get; set; }
}

[Serializable]
public class BlockPoolInfo
{
    public BlockType type;
    public int poolSize;
}