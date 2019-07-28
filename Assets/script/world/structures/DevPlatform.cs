using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevPlatform : MonoBehaviour
{
    public GameObject platformBlock;
    public GameObject playerPrefab;
    [Range(0, 5)]
    public int radius = 2;
    void Start()
    {
        for (int i = -radius; i <= radius; i++)
            for (int j = -radius; j <= radius; j++)
            {
                GameObject obj = Instantiate(platformBlock, Vector3.zero, Quaternion.identity, transform);
                obj.GetComponent<MeshRenderer>().enabled = true;
                obj.GetComponent<BoxCollider>().enabled = true;
                obj.transform.localPosition = new Vector3(i, 0, j);
            }

        Instantiate(playerPrefab, Vector3.zero + new Vector3(0, 1, 0), Quaternion.identity, transform);
    }
}
