using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingCycle : MonoBehaviour
{
    public Gradient skyColorCycle;
    public Gradient ambientCycle;
    public Gradient fogCycle;

    internal void Configure(World world)
    {
        world.OnTick += Tick;
    }

    internal void Configure(AbstractWorld world)
    {
        world.OnTick += Tick;
    }

    void Tick(object sender, TickEventArgs e)
    {
        RenderSettings.ambientSkyColor = ambientCycle.Evaluate(e.time % 24000 / 24000f);
        RenderSettings.fogColor = fogCycle.Evaluate(e.time % 24000 / 24000f);
        Camera.main.backgroundColor = skyColorCycle.Evaluate(e.time % 24000 / 24000f);
    }
}
