using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRegion : MonoBehaviour
{
    public GameObject chunkPrefab;
    internal Dictionary<Vector2, WorldChunk> chunkMap;
    World world;
    long seed;

    internal void Configure(World world, long seed)
    {
        this.world = world;
        this.seed = seed;

        Generate();
    }

    public void Generate()
    {
        chunkMap = new Dictionary<Vector2, WorldChunk>();

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
            {
                GameObject chunk = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity, transform);
                chunkMap.Add(new Vector2(i, j), chunk.GetComponent<WorldChunk>());
                Vector2 pos = new Vector2(i * 16, j * 16);
                chunk.name = "c_" + i + "," + j;
                chunk.transform.localPosition = new Vector3(pos.x, 0, pos.y);
                chunk.GetComponent<WorldChunk>().Configure(world, seed, 
                    new Vector2(transform.position.x + pos.x - (world.radius * 64f) + (seed / 100f) - 1.5E+6f, 
                    transform.position.z + pos.y - (world.radius * 64f) + (seed / 100f) - 1.5E+6f));
            }
    }

    public void Create()
    {
        //foreach (Vector2 v in chunkMap.Keys) chunkMap[v].Create();
    }
}
