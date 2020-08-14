using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GoToHandEffect : MonoBehaviour
{
    public Material dropEffectMat;
    public CardView dropEffectCardView;
    public Animator dropEffectAni;
    public bool cardHide = true;
    [Space(20)]
    [Range(0, 1)]
    public float lerp;
    [Range(0,1)]
    public float alpha;


    void Update()
    {
        if (dropEffectMat)
        {
            dropEffectMat.SetFloat("_Alpha", alpha);
            dropEffectMat.SetFloat("_Lerp", lerp);
        }

        if(!cardHide && dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("GotoHandHide"))
            cardHide = true;
    }
}
