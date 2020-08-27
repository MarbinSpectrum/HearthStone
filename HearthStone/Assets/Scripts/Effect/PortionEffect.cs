using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortionEffect : ThrowEffect
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void EndEffect()
    {
        EffectManager.instance.MagicEffect(transform.position);
        gameObject.SetActive(false);
    }
}
