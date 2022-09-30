﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetCoinMat : MonoBehaviour
{
    public Material coinMat;

    [ColorUsage(true, true)] public Color fireColor;

    public float power;

    private void Update()
    {
        if (coinMat == null)
            return;

        coinMat.SetFloat("Power", power);
        coinMat.SetColor("FireColor", fireColor);
    }
}
