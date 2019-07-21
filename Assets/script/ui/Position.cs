using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Position : MonoBehaviour
{
    void Update()
    {
        Vector3 playerPos = Camera.main.transform.position - new Vector3(0, 1, 0);
        double x = Math.Round(playerPos.x, 3, MidpointRounding.AwayFromZero);
        double y = Math.Round(playerPos.y, 3, MidpointRounding.AwayFromZero);
        double z = Math.Round(playerPos.z, 3, MidpointRounding.AwayFromZero);
        transform.GetChild(0).GetComponent<Text>().text = "XYZ: " + x + ", " + y + ", " + z;
    }
}
