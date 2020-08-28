using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallEffect : ThrowEffect
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void EndEffect()
    {
        EffectManager.instance.FireEffect(transform.position);
        gameObject.SetActive(false);
    }
}
