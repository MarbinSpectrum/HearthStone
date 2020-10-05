using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PackBreak : MonoBehaviour
{
    [Range(0,1)]
    public float value = 0;

    private MeshRenderer meshRenderer;

    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateShader();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShader();
    }

    private void UpdateShader()
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("Value", value);
        meshRenderer.SetPropertyBlock(mpb);
    }
}
