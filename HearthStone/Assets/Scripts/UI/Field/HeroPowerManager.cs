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


    private void Update()
    {
        playerCanUseGlowObj.SetActive(BattleUI.instance.gameStart && 
            TurnManager.instance.turn == Turn.플레이어 && 
            ManaManager.instance.playerNowMana >= 2 && 
            playerCanUse && playerHeroPowerObjAni.GetCurrentAnimatorStateInfo(0).IsName("HeroPowerObj"));
        playerHeroPowerBtn.SetActive(playerCanUseGlowObj.activeSelf);
    }

    public void UseHeroAbility(bool enemy)
    {
        string abilityName = " ";
        if(enemy)
        {
            if (enemyHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (enemyHeroName.Equals("말퓨리온"))
                abilityName = "변신";

            Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(abilityName));
            int cost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");

            if (!enemyCanUse || ManaManager.instance.enemyNowMana < cost)
                return;

            ManaManager.instance.enemyNowMana -= 2;
            enemyCanUse = false;
            enemyHeroPowerObjAni.SetBool("CanUse", false);

            showHeroPower.SetActive(true);
            heroPowerName.text = abilityName;
            for (int i = 0; i < heroPowerImage.Length; i++)
                heroPowerImage[i].enabled = heroPowerImage[i].transform.name == abilityName;

            string explain = DataMng.instance.ToString(pair.x, pair.y, "카드설명");
            heroPowerExplain.text = explain;

            Invoke("CloseHeroPower", 1.5f);

        }
        else
        {
            if (playerHeroName.Equals("발리라"))
                abilityName = "단검의 대가";
            else if (playerHeroName.Equals("말퓨리온"))
                abilityName = "변신";

            Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(abilityName));
            int cost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");

            if (!playerCanUse || ManaManager.instance.playerNowMana < cost)
            {
                //마나가 부족하거나 사용할 수 없다.
                return;
            }
            ManaManager.instance.playerNowMana -= cost;
             
            playerCanUse = false;
            playerHeroPowerObjAni.SetBool("CanUse", false);

            QuestManager.instance.HeroAbility();
        }

        SpellManager.instance.RunSpell(abilityName, enemy, true);

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

        Vector2Int pair = DataMng.instance.GetPairByName(
            DataParse.GetCardName(abilityName));
        string ability = DataMng.instance.ToString(pair.x, pair.y, "명령어");

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
