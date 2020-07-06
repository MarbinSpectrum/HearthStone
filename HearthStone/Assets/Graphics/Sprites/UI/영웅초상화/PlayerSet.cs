using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerSet : MonoBehaviour
{
    public Material playerSetMat;
    public float x = 0;
    public float y = 0;
    public float a = 0;
    void Update()
    {
        if(playerSetMat)
        {
            playerSetMat.SetFloat("_MaxX", x);
            playerSetMat.SetFloat("_MaxY", y);
            playerSetMat.SetColor("_OutlineColor", new Color(191,113,12)*a);
        }
    }
}
