using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Inverse : MonoBehaviour
{
    [SerializeField]
    private Material material;

    [Range(0, 1)]
    [SerializeField]
    private float alpha = 0;

    void Update()
    {
        if (material)
            material.SetFloat("_Alpha", alpha);
    }
}
