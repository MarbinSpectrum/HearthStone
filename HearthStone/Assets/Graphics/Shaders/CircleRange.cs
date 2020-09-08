using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleRange : MonoBehaviour
{

    public Vector2 center = new Vector2(0.5f,0.5f);
    public Vector2 pivot = new Vector2(0, 0);
    public float range = 0;

    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Range", range);
        mpb.SetVector("_Center", center);
        mpb.SetVector("_Pivot", pivot);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
