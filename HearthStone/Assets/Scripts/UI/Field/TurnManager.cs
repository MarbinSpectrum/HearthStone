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
                CardHand.instance.UsePreparation = 0;
                time = 0.5f;
                turn = 턴.상대방;
                manaManager.enemyMaxMana++;
                manaManager.enemyMaxMana = Mathf.Min(manaManager.enemyMaxMana, 10);
                manaManager.enemyNowMana = manaManager.enemyMaxMana;
                HeroManager.instance.heroAtkManager.HeroAtkTurnEnd(false);
                HeroManager.instance.heroPowerManager.HeroTurnEnd(false);
                MinionManager.instance.MinionsTurnStartTrigger(true);
                MinionManager.instance.MinionsTurnEndTrigger(false);
                DruidAI.instance.AI_Run();
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
        if(HeroManager.instance.heroAtkManager.playerCanAttack.activeSelf)
            canAttack = true;

        bool canUseCard = false;
        for (int i = 0; i < CardHand.instance.nowHandNum; i++)
            if (CardHand.instance.canUse[i])
                canUseCard = true;
        if(HeroManager.instance.heroPowerManager.playerCanUse && manaManager.playerNowMana >= 2)
            canUseCard = true;

        if (canAttack || canUseCard || !BattleUI.instance.gameStart || GameEventManager.instance.EventCheck())
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
                    EnemyCardHand.instance.DrawCard();
                else
                    CardHand.instance.CardDrawAct();

            }
        }
    }
    #endregion
}
