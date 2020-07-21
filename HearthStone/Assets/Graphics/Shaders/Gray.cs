using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class Gray : MonoBehaviour
{

    [Range(0,1)]
    public float power;

    public Renderer renderer;
    public Material grayMat;

    void Update()
    {
        if (renderer)
            renderer.material.SetFloat("_GrayPower", power);
        if (grayMat)
            grayMat.SetFloat("_Power", power);
    }
}
