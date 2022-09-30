using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMinionRenderer : MonoBehaviour
{
    public Renderer renderer;
    [Range(0, 1)] public float _GrayPower;
    private void Update()
    {
        if(renderer == null)
            return;
        renderer.material.SetFloat("_GrayPower", _GrayPower);
    }
}
