using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum 턴
{
    플레이어 , 상대방
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public 턴 turn;
    public bool turnEndTrigger;
    [HideInInspector] public bool turnAniEnd;

    [Header("-----------------------------------------------------------")]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]
    [Space]

    public ManaManager manaManager;
    public Animator turnAni;
    float time = 0;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(turnEndTrigger)
        {
            turnEndTrigger = false;
            if(turn == 턴.플레이어)
            {
                turn = 턴.상대방;
                manaManager.enemyMaxMana++;
                manaManager.enemyNowMana = manaManager.enemyMaxMana;
                time = 2;
            }
            else
            {
                turn = 턴.플레이어;
                manaManager.playerMaxMana++;
                manaManager.playerNowMana = manaManager.playerMaxMana;
                turnAni.SetTrigger("내턴");
                time = 2;
            }
        }

        if (time >= 0)
            time -= Time.deltaTime;
        else if (!turnAniEnd)
            turnAniEnd = true;

    }
}
