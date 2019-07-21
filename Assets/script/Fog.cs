using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public Shader fogShader;

    void Start()
    {
        Camera.main.RenderWithShader(fogShader, "");
    }
}
