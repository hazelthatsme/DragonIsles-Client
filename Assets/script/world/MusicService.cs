using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicService : MonoBehaviour
{
    AudioSource source;
    public List<AudioClip> dayMusic;
    public List<AudioClip> nightMusic;

    public void Configure(World world)
    {
        world.OnTick += Tick;
        source = Camera.main.GetComponent<AudioSource>();
    }

    void Tick(object sender, TickEventArgs e)
    {
        long relTime = (e.time % 24000) - 1;
        System.Random rand = new System.Random((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);
        if (relTime == 0 || relTime == 6000) source.PlayOneShot(dayMusic[rand.Next(dayMusic.Count - 1)]);
        if (relTime == 12000 || relTime == 18000) source.PlayOneShot(nightMusic[rand.Next(nightMusic.Count - 1)]);
    }
}
