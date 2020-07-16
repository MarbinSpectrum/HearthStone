using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMinionRenderer : MonoBehaviour
{
    public Renderer renderer;
    [Range(0,1)]
    public float _GrayPower;
    // Update is called once per frame
    void Update()
    {
        if(renderer)
        {
            renderer.material.SetFloat("_GrayPower", _GrayPower);
        }
    }
}
