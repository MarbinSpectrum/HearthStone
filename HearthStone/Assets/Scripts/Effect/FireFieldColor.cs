using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FireFieldColor : MonoBehaviour
{
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    [Range(0,1)]
    public float alpha = 0;
    void Update()
    {
        foreach (SpriteRenderer spr in spriteRenderers)
            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, alpha);
    }
}
