using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSound : MonoBehaviour
{
    [Header("소환시")]
    public AudioClip spawnSound;

    [Header("공격시")]
    public AudioClip attackSound;

    [Header("죽을시")]
    public AudioClip deathSound;

    [Header("효과시")]
    public AudioClip effectSound;
}
