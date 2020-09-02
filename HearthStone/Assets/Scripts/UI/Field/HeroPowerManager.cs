using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject showHeroPower;
    public Text heroPowerName;
    public Text heroPowerExplain;
    public Image []heroPowerImage;


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


            Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(abilityName));

            showHeroPower.SetActive(true);
            heroPowerName.text = abilityName;
            for (int i = 0; i < heroPowerImage.Length; i++)
                heroPowerImage[i].enabled = heroPowerImage[i].transform.name == abilityName;
            if(abilityName == "변신")
                heroPowerExplain.text = "방어도를 + 1 얻고 이번 턴에 내 영웅이 공격력을 +1 얻습니다.";
            else if (abilityName == "단검의 대가")
                heroPowerExplain.text = "1/2 무기를 장착합니다.";
            Invoke("CloseHeroPower", 1.5f);

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

    void CloseHeroPower()
    {
        showHeroPower.SetActive(false);
    }
}
