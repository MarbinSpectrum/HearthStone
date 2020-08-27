using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEnergy : ThrowEffect
{
    protected override void Update()
    {
        speed = 3;
        base.Update();
    }

    protected override void EndEffect()
    {
        EffectManager.instance.MagicEffect(transform.position);
        EffectManager.instance.VibrationEffect(0, 10, 2);
        gameObject.SetActive(false);
    }
}
