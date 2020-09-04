using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeroManager : MonoBehaviour
{
    public static HeroManager instance;

    [Header("------------------------------------------------")]
    [Space(30)]

    public HeroHpManager heroHpManager;
    public HeroAtkManager heroAtkManager;
    public HeroPowerManager heroPowerManager;
    public Image playerHero;
    public Image enemyHero;
    public GameObject playerFreezeObj;
    public GameObject enemyFreezeObj;

    private int playerFreezeCount = 0;
    private int enemyFreezeCount = 0;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        playerFreezeObj.SetActive(playerFreezeCount > 0);
        enemyFreezeObj.SetActive(enemyFreezeCount > 0);

        //playerHero.raycastTarget = !CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대");
        //enemyHero.raycastTarget = !DragCardObject.instance.dragCard;
        if(!GameEventManager.instance.EventCheck() && BattleUI.instance.gameStart)
        {
            if(heroHpManager.nowEnemyHp <= 0)
            {
                heroAtkManager.enemyObjectAni.SetBool("Break", true);
                heroHpManager.enemyHp.SetActive(false);
                heroHpManager.enemyShieldAni.gameObject.SetActive(false);
                heroPowerManager.enemyHeroPowerObjAni.gameObject.SetActive(false);

                BattleUI.instance.gameStart = false;
                BattleUI.instance.gameEndDontTouch.SetActive(true);
                EffectManager.instance.VibrationEffect(0, 300, 10);
            }
            else if (heroHpManager.nowPlayerHp <= 0)
            {
                heroAtkManager.playerObjectAni.SetBool("Break", true);
                heroHpManager.playerHp.SetActive(false);
                heroHpManager.playerShieldAni.gameObject.SetActive(false);
                heroPowerManager.playerHeroPowerObjAni.gameObject.SetActive(false);

                BattleUI.instance.gameStart = false;
                BattleUI.instance.gameEndDontTouch.SetActive(true);
                EffectManager.instance.VibrationEffect(0, 300, 10);
            }
        }
    }



    public void SetFreeze(bool enemy)
    {
        if (enemy)
            enemyFreezeCount = 2;
        else
            playerFreezeCount = 2;
    }

    public void MeltFreeze()
    {
        enemyFreezeCount--;
        playerFreezeCount--;
    }
}
