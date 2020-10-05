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
                SoundManager.instance.StopBGM();
                SoundManager.instance.PlayCharacterSE("말퓨리온", 영웅상태.죽을시);
                BattleUI.instance.gameStart = false;
                StartCoroutine(GameWin());

            }
            else if (heroHpManager.nowPlayerHp <= 0)
            {
                SoundManager.instance.StopBGM();
                SoundManager.instance.PlayCharacterSE(heroPowerManager.playerHeroName, 영웅상태.죽을시);
                BattleUI.instance.gameStart = false;
                StartCoroutine(GameDefeat());
            }
        }
    }

    public IEnumerator GameDefeat()
    {
        EffectManager.instance.HeroExplodeEffect(playerHero.transform.position);
        yield return new WaitForSeconds(0.2f);
        heroAtkManager.playerObjectAni.SetBool("Break", true);
        SoundManager.instance.PlaySE("영웅깨짐");
        heroHpManager.playerHp.SetActive(false);
        heroHpManager.playerShieldAni.gameObject.SetActive(false);
        heroPowerManager.playerHeroPowerObjAni.gameObject.SetActive(false);

        for (int i = 0; i < playerHeroBreak.Length; i++)
            playerHeroBreak[i].sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
        EffectManager.instance.VibrationEffect(1.5f, 300, 10);

        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlaySE("영웅폭발");
        BattleUI.instance.gameEndDontTouch.SetActive(true);
        yield return new WaitForSeconds(6f);
        BattleUI.instance.gameDefeat.SetActive(true);
        BattleUI.instance.gameDefeatImg.sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
        SoundManager.instance.PlaySE("패배시");
        QuestClear.instance.QuesetClear();
    }

    public IEnumerator GameWin()
    {
        EffectManager.instance.HeroExplodeEffect(enemyHero.transform.position);
        yield return new WaitForSeconds(0.2f);
        heroAtkManager.enemyObjectAni.SetBool("Break", true);
        SoundManager.instance.PlaySE("영웅깨짐");
        heroHpManager.enemyHp.SetActive(false);
        heroHpManager.enemyShieldAni.gameObject.SetActive(false);
        heroPowerManager.enemyHeroPowerObjAni.gameObject.SetActive(false);

        for (int i = 0; i < enemyHeroBreak.Length; i++)
            enemyHeroBreak[i].sprite = heroSpr[heroPowerManager.enemyHeroName == "말퓨리온" ? 0 : 1];
        EffectManager.instance.VibrationEffect(1.5f, 300, 10);

        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlaySE("영웅폭발");
        BattleUI.instance.gameEndDontTouch.SetActive(true);
        yield return new WaitForSeconds(6f);
        BattleUI.instance.gameWin.SetActive(true);
        BattleUI.instance.gameWinImg.sprite = heroSpr[heroPowerManager.playerHeroName == "말퓨리온" ? 0 : 1];
        QuestManager.instance.CharacterWin(heroPowerManager.playerHeroName == "말퓨리온" ? Job.드루이드 : Job.도적);
        SoundManager.instance.PlaySE("승리시");
        SoundManager.instance.PlaySE("승리의환호");
        SoundManager.instance.PlaySE("승리의폭죽");
        QuestClear.instance.QuesetClear();
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
