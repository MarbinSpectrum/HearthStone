using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StealthEffect : MonoBehaviour
{
    [SerializeField] private Renderer renderer;

    [SerializeField] private Color color;
    [SerializeField] private float power;
    [SerializeField] private float noiseSize;
    [SerializeField] private float speed;
    [SerializeField] [Range(0,1)] private float addFog;
    [SerializeField] [Range(0, 1)] private float alphaCut;

    private MaterialPropertyBlock mpb;

    private void Update()
    {
        if (renderer == null)
            return;

        if(mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(mpb, 0);
        mpb.SetColor("MainColor", color);
        mpb.SetFloat("Power", power);
        mpb.SetFloat("NoiseSize", noiseSize);
        mpb.SetFloat("Speed", speed);
        mpb.SetFloat("AddFog", addFog);
        mpb.SetFloat("AlphaCut", alphaCut);
        renderer.SetPropertyBlock(mpb, 0);
    }
}
