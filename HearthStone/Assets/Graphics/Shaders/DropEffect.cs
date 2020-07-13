using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DropEffect : MonoBehaviour
{
    public Material dropEffectMat;
    [Range(0,1)]
    public float alpha;
    void Update()
    {
        if (dropEffectMat)
        {
            dropEffectMat.SetFloat("_Alpha", alpha);
        }
    }
}
