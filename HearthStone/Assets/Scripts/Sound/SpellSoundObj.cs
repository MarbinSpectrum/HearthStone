using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellSoundObj : ScriptableObject
{
    [Header("발동시")]
    public AudioClip playSound;

    [Header("효과시")]
    public AudioClip effectSound;
}
