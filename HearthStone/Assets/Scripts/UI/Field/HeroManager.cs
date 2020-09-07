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
    public SpriteRenderer[] playerHeroBreak;
    public GameObject enemyFreezeObj;
    public SpriteRenderer[] enemyHeroBreak;
    public Sprite []heroSpr;

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

                for (int i = 0; i < enemyHeroBreak.Length; i++)
                    enemyHeroBreak[i].sprite = heroSpr[heroPowerManager.enemyHeroName == "말퓨리온" ? 0 : 1];
                EffectManager.instance.HeroExplodeEffect(enemyHero.transform.position);
                EffectManager.instance.VibrationEffect(1.5f, 300, 10);
                StartCoroutine(GameWin(5.5f));

            }
            else if (heroHpManager.nowPlayerHp <= 0)
            {
                heroAtkManager.playerObjectAni.SetBool("Break", true);
                heroHpManager.playerHp.SetActive(false);
                heroHpManager.playerShieldAni.gameObject.SetActive(false);
                heroPowerManager.playerHeroPowerObjAni.gameObject.SetActive(false);

                BattleUI.instance.gameStart = false;
                BattleUI.instance.gameEndDontTouch.SetActive(true);

                for (int i = 0; i < playerHeroBreak.Length; i++)
                    playerHeroBreak[i].sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
                EffectManager.instance.HeroExplodeEffect(playerHero.transform.position);
                EffectManager.instance.VibrationEffect(1.5f, 300, 10);
                StartCoroutine(GameDefeat(5.5f));

            }
        }
    }

    public IEnumerator GameDefeat(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        BattleUI.instance.gameDefeat.SetActive(true);
        BattleUI.instance.gameDefeatImg.sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
    }

    public IEnumerator GameWin(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        BattleUI.instance.gameWin.SetActive(true);
        BattleUI.instance.gameWinImg.sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
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
