using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroPowerManager : MonoBehaviour
{
    [Header("나의 영웅 능력")]
    [HideInInspector] public string playerHeroName;
    public List<SpellAbility> playerheroPower = new List<SpellAbility>();
    public bool playerCanUse = true;
    public Animator playerHeroPowerObjAni;
    public GameObject playerCanUseGlowObj;
    public GameObject playerHeroPowerBtn;

    [Header("적의 영웅 능력")]
    [HideInInspector] public string enemyHeroName;
    public List<SpellAbility> enemyheroPower = new List<SpellAbility>();
    public bool enemyCanUse = true;
    public Animator enemyHeroPowerObjAni;
    public GameObject enemyCanUseGlowObj;

    public void Update()
    {
        playerCanUseGlowObj.SetActive(BattleUI.instance.gameStart && TurnManager.instance.turn == 턴.플레이어 && ManaManager.instance.playerNowMana >= 2 && playerCanUse && playerHeroPowerObjAni.GetCurrentAnimatorStateInfo(0).IsName("HeroPowerObj"));
        playerHeroPowerBtn.SetActive(playerCanUseGlowObj.activeSelf);
    }

    public void UseHeroAbility(bool enemy)
    {
        string abilityName = " ";
        if(enemy)
        {
            if (!enemyCanUse || ManaManager.instance.enemyNowMana < 2)
                return;
            ManaManager.instance.enemyNowMana -= 2;
            if (enemyHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (enemyHeroName.Equals("말퓨리온"))
                abilityName = "변신";
            enemyCanUse = false;
            enemyHeroPowerObjAni.SetBool("CanUse", false);
        }
        else
        {
            if (!playerCanUse || ManaManager.instance.playerNowMana < 2)
                return;
            ManaManager.instance.playerNowMana -= 2;
            if (playerHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (playerHeroName.Equals("말퓨리온"))
                abilityName = "변신";
            playerCanUse = false;
            playerHeroPowerObjAni.SetBool("CanUse", false);
            //DragCardObject.instance.ShowDropEffectSpell(Input.mousePosition, 0);
        }

        SpellManager.instance.RunSpell(abilityName, enemy,true);

    }

    public void HeroTurnEnd(bool enemy)
    {
        if (enemy)
        {
            playerCanUse = true;
            playerHeroPowerObjAni.SetBool("CanUse", true);
        }
        else
        {
            enemyCanUse = true;
            enemyHeroPowerObjAni.SetBool("CanUse", true);
        }
    }


    public void SetHeroPower(string heroName,bool enemy)
    {
        string abilityName = " ";
        if (enemy)
        {
            enemyHeroName = heroName;
            if (enemyHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (enemyHeroName.Equals("말퓨리온"))
                abilityName = "변신";
        }
        else
        {
            playerHeroName = heroName;
            if (playerHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (playerHeroName.Equals("말퓨리온"))
                abilityName = "변신";
        }

        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(abilityName));
        string ability = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");
        if (!enemy)
            playerheroPower = SpellManager.instance.SpellParsing(ability);
        else
            enemyheroPower = SpellManager.instance.SpellParsing(ability);
    }

}
