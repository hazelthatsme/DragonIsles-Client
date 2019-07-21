using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    private void Start()
    {
        InvokeRepeating("EvaluateFPS", 0f, 1f);
    }

    void EvaluateFPS()
    {
        double fps = Math.Round(1.0f / Time.deltaTime, 2, MidpointRounding.AwayFromZero);
        transform.GetChild(0).GetChild(0).GetComponent<Text>().text = fps + "FPS";
    }
}
