using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSound : MonoBehaviour
{
    [Header("게임시작시(아군)")]
    public AudioClip battleStart_Player;

    [Header("게임시작시(적군)")]
    public AudioClip battleStart_Enemy;

    [Header("덱이적다")]
    public AudioClip deckLittle;

    [Header("덱이없다")]
    public AudioClip deckEmpty;

    [Header("공격시")]
    public AudioClip attack;

    [Header("죽을시")]
    public AudioClip death;
}
