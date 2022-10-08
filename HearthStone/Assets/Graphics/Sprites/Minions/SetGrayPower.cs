using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetGrayPower : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] [Range(0, 1)] private float _GrayPower;

    private MaterialPropertyBlock mpb;

    private void Update()
    {
        if(renderer == null)
            return;

        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(mpb, 0);
        mpb.SetFloat("_GrayPower", _GrayPower);
        renderer.SetPropertyBlock(mpb, 0);
    }
}
