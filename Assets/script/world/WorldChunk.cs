using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldChunk : MonoBehaviour
{
    World world;
    WorldRegion region;
    long seed;
    internal Dictionary<Vector3, Block> blockMap;
    internal Dictionary<Vector2, float> topBlock;
    Dictionary<Vector3, GameObject> placedBlocks;
    Vector2 perlinBase;
    bool active = false;
    //System.Random rand;
    [Header("Glade Generation Settings")]
    [Range(0, 1)]
    public float gladeBirdseyeThreshold = .5f;
    [Range(.6f, 1)]
    public float gladePerlinThreshold = .6f;
    [Range(0, 48)]
    public float gladeBirdseyeSmoothing = 24f;
    [Range(0, 48)]
    public float gladeVerticalSmoothing = 14f;
    [Header("Basin Generation Settings")]
    [Range(0, 1)]
    public float basinBirdseyeThreshold = .4f;
    [Range(0, 48)]
    public float basinBirdseyeSmoothing = 7f;

    internal void Configure(World world, long seed, Vector2 perlinBase)
    {
        this.world = world;
        this.seed = seed;
        this.perlinBase = perlinBase;

        region = transform.parent.GetComponent<WorldRegion>();

        Generate();
    }

    void UpdateBlocks()
    {
        foreach (Vector3 v in placedBlocks.Keys)
        {
            if (GetObscured(v))
            {
                placedBlocks[v].GetComponent<BoxCollider>().enabled = false;
                placedBlocks[v].GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                placedBlocks[v].GetComponent<BoxCollider>().enabled = true;
                placedBlocks[v].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    public void Generate()
    {
        blockMap = new Dictionary<Vector3, Block>();
        topBlock = new Dictionary<Vector2, float>();
        placedBlocks = new Dictionary<Vector3, GameObject>();

        for (int i = 0; i < 16; i++)
            for (int j = 0; j < 16; j++)
            {
                float realI = i + perlinBase.x;
                float realJ = j + perlinBase.y;
                GenerateGladeLayer(realI, realJ, new Vector3(i, 0, j));
                GenerateBasinLayer(realI, realJ, new Vector3(i, 0, j));
            }
    }

    public void Activate()
    {
        if (active) return;

        foreach (Vector3 v in blockMap.Keys)
        {
            if (placedBlocks.ContainsKey(v) || GetObscured(v)) continue;
            Block b = blockMap[v];
            GameObject instance;
            try { instance = world.blockPools[b.type].Dequeue(); }
            catch (Exception e)
            {
                Debug.LogError($"{e.GetType()}: Queue for BlockType {b.type} empty!");
                break;
            }
            instance.name = "b_" + v.x + "," + v.y + "," + v.z;
            instance.GetComponent<CubeFixer>().Fix();
            instance.transform.parent = transform;
            instance.transform.localPosition = v;
            placedBlocks.Add(v, instance);
        }

        active = true;
        //UpdateBlocks();
    }

    private bool GetObscured(Vector3 v)
    {
        bool flag1 = getFlag1(v);
        bool flag2 = getFlag2(v);
        bool flag3 = getFlag3(v);
        bool flag4 = getFlag4(v);
        bool flag5 = getFlag5(v);
        bool flag6 = getFlag6(v);

        if (flag1 && flag2 && flag3 && flag4 && flag5 && flag6) return true;
        else return false;
    }

    bool getFlag1(Vector3 v)
    {
        Vector3 posMX = v - new Vector3(1, 0, 0);
        if (posMX.x < 0)
        {
            posMX = transform.position + v - new Vector3(1, 0, 0);
            Vector3 chunkPos = new Vector3(Mathf.Floor(posMX.x / 16), 0, Mathf.Floor(posMX.z / 16));
            WorldChunk chunk = world.GetChunkAtPosition((int)chunkPos.x, (int)chunkPos.z);
            posMX = new Vector3(15, v.y, v.z);
            if (chunk != null)
            {
                if (chunk.blockMap.ContainsKey(posMX)) return true;
            }
            else return true;
        }
        else if (blockMap.ContainsKey(posMX)) return true;
        return false;
    }

    bool getFlag2(Vector3 v)
    {
        Vector3 posPX = v + new Vector3(1, 0, 0);
        if (posPX.x > 15)
        {
            posPX = transform.position + v + new Vector3(1, 0, 0);
            Vector3 chunkPos = new Vector3(Mathf.Floor(posPX.x / 16), 0, Mathf.Floor(posPX.z / 16));
            WorldChunk chunk = world.GetChunkAtPosition((int)chunkPos.x, (int)chunkPos.z);
            posPX = new Vector3(0, v.y, v.z);
            if (chunk != null)
            {
                if (chunk.blockMap.ContainsKey(posPX))
                {
                    return true;
                }
            }
            else return true;
        }
        else
        {
            if (blockMap.ContainsKey(posPX)) return true;
        }
        return false;
    }

    bool getFlag3(Vector3 v)
    {
        Vector3 posMZ = v - new Vector3(0, 0, 1);
        if (posMZ.z < 0)
        {
            posMZ = transform.position + v + new Vector3(1, 0, 0);
            Vector3 chunkPos = new Vector3(Mathf.Floor(posMZ.x / 16), 0, Mathf.Floor(posMZ.z / 16));
            WorldChunk chunk = world.GetChunkAtPosition((int)chunkPos.x, (int)chunkPos.z);
            posMZ = new Vector3(v.x, v.y, 15);
            if (chunk != null)
            {
                if (chunk.blockMap.ContainsKey(posMZ))
                {
                    return true;
                }
            }
            else return true;
        }
        else if (blockMap.ContainsKey(posMZ))
        {
            return true;
        }
        return false;
    }

    bool getFlag4(Vector3 v)
    {
        Vector3 posPZ = v + new Vector3(0, 0, 1);
        if (posPZ.z < 0)
        {
            posPZ = transform.position + v + new Vector3(0, 0, 1);
            Vector3 chunkPos = new Vector3(Mathf.Floor(posPZ.x / 16), 0, Mathf.Floor(posPZ.z / 16));
            WorldChunk chunk = world.GetChunkAtPosition((int)chunkPos.x, (int)chunkPos.z);
            posPZ = new Vector3(v.x, v.y, 0);
            if (chunk != null)
            {
                if (chunk.blockMap.ContainsKey(posPZ))
                {
                    return true;
                }
            }
            else return true;
        }
        else if (blockMap.ContainsKey(posPZ))
        {
            return true;
        }
        return false;
    }

    bool getFlag5(Vector3 v)
    {
        Vector3 posMY = v - new Vector3(0, 1, 0);
        if (blockMap.ContainsKey(posMY))
        {
            return true;
        }
        return false;
    }

    bool getFlag6(Vector3 v)
    {
        Vector3 posPY = v + new Vector3(0, 1, 0);
        if (blockMap.ContainsKey(posPY))
        {
            return true;
        }
        return false;
    }

    public void Deactivate()
    {
        if (!active) return;

        List<GameObject> objectsToQueue = new List<GameObject>();
        foreach (Vector3 v in placedBlocks.Keys)
            objectsToQueue.Add(placedBlocks[v]);

        foreach (GameObject o in objectsToQueue)
        {
            BlockType t = BlockType.DraconicStone;
            try { t = o.GetComponent<Block>().type; }
            catch (Exception) { t = BlockType.DraconicStone; }
            o.transform.parent = world.blockPoolObjects[t];
            o.GetComponent<CubeFixer>().Disable();
            world.blockPools[t].Enqueue(o);
        }
        active = false;
    }

    private void GenerateGladeLayer(float x, float z, Vector3 mapPos)
    {
        float chanceX = x - (seed % Mathf.Pow(10f, seed.ToString().Length - 8));
        float chanceZ = z + (seed % Mathf.Pow(10f, seed.ToString().Length - 8));
        float chance = Mathf.PerlinNoise(chanceX / gladeBirdseyeSmoothing, chanceZ / gladeBirdseyeSmoothing);
        if (chance > gladeBirdseyeThreshold)
        {
            for (int y = 90; y >= 20; y--)
            {
                float chanceY = y + (seed % Mathf.Pow(10f, seed.ToString().Length - 8));
                float chance2 = Perlin3D(chanceX / gladeBirdseyeSmoothing, chanceY / gladeVerticalSmoothing, chanceZ / gladeBirdseyeSmoothing);
                if (chance2 > gladePerlinThreshold)
                {
                    mapPos.y = y;
                    BlockType type = BlockType.DraconicStone;
                    if (!blockMap.ContainsKey(mapPos + new Vector3(0, 1, 0)))
                    {
                        type = BlockType.GladeGrass;
                        if (!topBlock.ContainsKey(new Vector2(x, z))) topBlock.Add(new Vector2(x, z), y);
                        else topBlock[new Vector2(x, z)] = y;

                        if (world.spawnpoint == Vector3.zero)
                        {
                            world.spawnpoint = mapPos + transform.position + new Vector3(0, 1, 0);
                            Debug.Log("Spawnpoint set: " + world.spawnpoint);
                            world.spawnChunk = this;
                        }
                    }
                    else if (topBlock[new Vector2(x, z)] - y <= 3) type = BlockType.GladeDirt;
                    Block b = new Block(mapPos, type);
                    blockMap.Add(mapPos, b);
                }
            }
        }
    }

    private void GenerateBasinLayer(float x, float z, Vector3 mapPos)
    {
        float chance = Mathf.PerlinNoise(x / basinBirdseyeSmoothing, z / basinBirdseyeSmoothing);
        if (chance > basinBirdseyeThreshold)
        {
            int start = Mathf.RoundToInt(Mathf.PerlinNoise(x / 8f, z / 8f));
            int end = Mathf.RoundToInt(Mathf.PerlinNoise(x / 9f, z / 9f));
            for (int y = start; y <= end; y++)
            {
                mapPos = new Vector3(mapPos.x, y, mapPos.z);
                blockMap.Add(mapPos, new Block(mapPos, BlockType.BasinCloud));
            }
        }
    }

    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}
