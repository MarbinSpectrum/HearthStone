﻿using System.Collections;
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
    [HideInInspector] public bool turnAniEnd = true;

    [Header("-----------------------------------------------------------")]
    [Space(10)]

    public ManaManager manaManager;
    public Animator turnAni;
    public Animator turnBtnAni;
    public GameObject turnBtn;
    public Material turnBtnMat;
    [ColorUsage(true, true)]
    public Color normalStateColor;
    [ColorUsage(true,true)]
    public Color glowStateColor;

    float time = 2;

    public void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(turnEndTrigger)
        {
            turnEndTrigger = false;
            turnAniEnd = false;
            if (turn == 턴.플레이어)
            {
                HeroManager.instance.MeltFreeze();
                CardHand.instance.useCardNum = 0;
                time = 0.5f;
                turn = 턴.상대방;
                manaManager.enemyMaxMana++;
                manaManager.enemyMaxMana = Mathf.Min(manaManager.enemyMaxMana, 10);
                manaManager.enemyNowMana = manaManager.enemyMaxMana;
                HeroManager.instance.heroAtkManager.HeroAtkTurnEnd(false);
                HeroManager.instance.heroPowerManager.HeroTurnEnd(false);
                MinionManager.instance.MinionsTurnStartTrigger(true);
                MinionManager.instance.MinionsTurnEndTrigger(false);
            }
            else
            {
                HeroManager.instance.MeltFreeze();
                time = 1;
                turn = 턴.플레이어;              
                manaManager.playerMaxMana++;
                manaManager.playerMaxMana = Mathf.Min(manaManager.playerMaxMana, 10);
                manaManager.playerNowMana = manaManager.playerMaxMana;
                turnAni.SetTrigger("내턴");
                HeroManager.instance.heroAtkManager.HeroAtkTurnEnd(true);
                HeroManager.instance.heroPowerManager.HeroTurnEnd(true);
                MinionManager.instance.MinionsTurnStartTrigger(false);
                MinionManager.instance.MinionsTurnEndTrigger(true);
            }
        }

        CardDraw();
        CheckCanDo();
    }

    public void CheckCanDo()
    {
        bool canAttack = false;
        for (int i = 0; i < MinionField.instance.minionNum; i++)
            if (MinionField.instance.minions[i].canAttackNum != 0)
                canAttack = true;

        bool canUseCard = false;
        for (int i = 0; i < CardHand.instance.nowHandNum; i++)
            if (CardHand.instance.canUse[i])
                canUseCard = true;

        if(canAttack || canUseCard || !BattleUI.instance.gameStart || GameEventManager.instance.EventCheck())
            turnBtnMat.SetColor("_ImgColor", normalStateColor);
        else if(BattleUI.instance.gameStart && turn == 턴.플레이어)
            turnBtnMat.SetColor("_ImgColor", glowStateColor);
    }


    #region[턴 시작시 카드 드로우 처리]
    public void CardDraw()
    {
        if (BattleUI.instance.gameStart)
        {
            if (time >= 0)
                time -= Time.deltaTime;
            else if (!turnAniEnd)
            {
                turnAniEnd = true;
                if (turn == 턴.상대방)
                {
                    if (EnemyCardHand.instance.nowHandNum >= 10)
                    {
                       // Debug.Log("(컴퓨터) : 카드가 너무많아!!");
                    }
                    else
                    {
                        EnemyCardHand.instance.DrawCard();
                    }
                }
                else
                {
                    if (CardHand.instance.nowHandNum >= 10)
                    {
                        Debug.Log("(플레이어) : 카드가 너무많아!!");
                    }
                    else
                    {
                        for (int i = 0; i < BattleUI.instance.playerCardAni.Length; i++)
                        {
                            if (BattleUI.instance.playerCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                            {
                                BattleUI.instance.playerCardAni[i].SetTrigger("Draw");
                                CardHand.instance.DrawCard();
                                string s = InGameDeck.instance.playDeck[0];
                                InGameDeck.instance.playDeck.RemoveAt(0);
                                CardHand.instance.CardMove(s, CardHand.instance.nowHandNum - 1, CardHand.instance.drawCardPos.transform.position, CardHand.instance.defaultSize, 0);
                                CardViewManager.instance.UpdateCardView(0.001f);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
    #endregion
}
