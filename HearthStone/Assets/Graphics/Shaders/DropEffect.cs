﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DropEffect : MonoBehaviour
{
    public Material dropEffectMat;
    public RectTransform dropRectTransform;
    public CardView dropEffectCardView;
    public Animator dropEffectAni;

    [HideInInspector] public bool effectArrive;

    [HideInInspector] public Vector2 dropPos;
    [Space(20)]
    [Range(0,1)]
    public float alpha;
    [Range(0, 1920)]
    public float min_speed;
    [Range(0, 1920)]
    public float max_speed;


    void Update()
    {
        if (dropEffectMat)
            dropEffectMat.SetFloat("_Alpha", alpha);
        if (!Application.isPlaying)
            return;
        Vector2 v = dropPos - dropRectTransform.anchoredPosition;
        v *= Time.deltaTime * 10;
        if (Vector2.Distance(Vector2.zero, v) > Vector2.Distance(Vector2.zero, v.normalized * max_speed))
            v = v.normalized * max_speed;
        if (Vector2.Distance(Vector2.zero, v) < Vector2.Distance(Vector2.zero, v.normalized * min_speed))
            v = v.normalized * min_speed;
        if (Vector2.Distance(dropRectTransform.anchoredPosition, dropPos) < Vector2.Distance(Vector2.zero, v))
        {
            dropRectTransform.anchoredPosition = dropPos;
            effectArrive = true;
            if(dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Minion"))
                dropEffectAni.SetTrigger("Exit");
        }
        else if (dropRectTransform.anchoredPosition != dropPos)
        {
            effectArrive = false;
            dropRectTransform.anchoredPosition += v;
        }
    }
}
