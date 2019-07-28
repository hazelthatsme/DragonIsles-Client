using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractWorld : MonoBehaviour
{
    internal event EventHandler<TickEventArgs> OnTick;
    private long time = 0;
    private int timeScale = 1;
    private long seed = 0;
    public GameObject regionPrefab;

    void Awake()
    {
        GetComponent<LightingCycle>().Configure(this);
        InvokeRepeating("Tick", .0f, .05f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) timeScale /= 2;
        if (Input.GetKeyDown(KeyCode.RightArrow)) timeScale *= 2;
    }

    void Tick()
    {
        time += timeScale;
        TickEvent(new TickEventArgs() { time = time });
    }

    protected virtual void TickEvent(TickEventArgs e)
    {
        OnTick?.Invoke(this, e);
    }
}
