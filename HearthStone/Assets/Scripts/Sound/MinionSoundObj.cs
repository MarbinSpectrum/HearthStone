using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSoundObj : ScriptableObject
{
    [Header("��ȯ��")]
    public AudioClip spawnSound;

    [Header("���ݽ�")]
    public AudioClip attackSound;

    [Header("������")]
    public AudioClip deathSound;

    [Header("ȿ����")]
    public AudioClip effectSound;
}
