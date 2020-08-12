﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObject : MonoBehaviour
{
    [Header("하수인 이름")]
    public string minion_name;

    [Header("체력")]
    public int final_hp;
    [HideInInspector] public int baseHp;
    int flagHp;

    [Header("공격력")]
    public int final_atk;
    [HideInInspector] public int nowAtk;
    [HideInInspector] public int baseAtk;
    int flagAtk;

    [Header("주문공격력")]
    public int final_spellAtk;
    [HideInInspector] public int nowSpell;

    [Header("공격여부")]
    //공격가능횟수
    public int canAttackNum;
    public bool canAttack;

    [Header("전설")]
    public bool legend;
    public GameObject legendObj;

    [Header("처음냈을때 수면 상태")]
    public bool sleep;

    [Header("침묵")]
    public bool silence;
    public GameObject silenceObj;

    [Header("빙결")]
    bool freeze;
    public GameObject freezeObj;
    public bool freezeTrigger;
    [HideInInspector] public int freezeCount = 0;

    [Header("은신")]
    public bool stealth;
    public GameObject stealthObj;

    [Header("도발")]
    public bool taunt;
    public GameObject tauntObj;

    [HideInInspector] public bool turnStartTrigger;
    [HideInInspector] public bool turnEndTrigger;

    [HideInInspector] public bool spellRun;

    public List<MinionAbility> abilityList = new List<MinionAbility>();
    /// <summary> x(공격력) y(체력) z(주문공격력) w(한턴동안만 유지되는 여부) </summary>
    [HideInInspector] public List<Vector4> buffList = new List<Vector4>();

    //[Header("-----------------------------------------")]
    [Space(20)]
    public bool InitTrigger;
    public bool TestInitTrigger;
    public bool gotoHandTrigger;

    [Header("-----------------------------------------")]
    [Space(100)]

    public bool enemy;
    public SpriteRenderer[] hp_spr;
    public Animator hpAni;
    public SpriteRenderer[] atk_spr;
    public Animator atkAni;
    public MeshRenderer meshRenderer;
    public GameObject canAttackObj;
    public DamageNum damageEffect;
    public Animator animator;
    //오브젝트 넘버
    /*[HideInInspector]*/
    public int num;

    #region[Awake]
    public void Awake()
    {
        MinionManager.instance.minionList.Add(this);
    }
    #endregion

    #region[Start]
    public void Start()
    {

    }
    #endregion

    #region[Update]
    void Update()
    {
        Init();
        UpdateTrigger();
        TurnStart();
        TurnEnd();
        FreezeUpdate();
        UpdateStat();
        SetObjectNum();
        GoToHand();
    }
    #endregion

    #region[오브젝트 번호 탐색]
    public void SetObjectNum()
    {
        if(enemy)
        {
            for(int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                if(this.Equals(EnemyMinionField.instance.minions[i]))
                {
                    num = i;
                    break;
                }
        }
        else
        {
            for (int i = 0; i < MinionField.instance.minions.Length; i++)
                if (this.Equals(MinionField.instance.minions[i]))
                {
                    num = i;
                    break;
                }
        }
        damageEffect.hpSystem = (enemy ? "적_하수인_" : "아군_하수인_") + num;
    }
    #endregion

    #region[스텟 업데이트]
    public void UpdateStat()
    {
        int tempHp = Mathf.Abs(final_hp);

        if (final_hp < 0)
        {
            if (tempHp < 10)
            {
                hp_spr[3].gameObject.SetActive(true);
                hp_spr[4].gameObject.SetActive(false);
            }
            else
            {
                hp_spr[3].gameObject.SetActive(false);
                hp_spr[4].gameObject.SetActive(true);
            }
        }
        else
        {
            hp_spr[3].gameObject.SetActive(false);
            hp_spr[4].gameObject.SetActive(false);
        }

        if (tempHp < 10)
        {
            hp_spr[0].sprite = DataMng.instance.num[tempHp];
            hp_spr[0].gameObject.SetActive(true);
            hp_spr[1].gameObject.SetActive(false);
            hp_spr[2].gameObject.SetActive(false);
            if (final_hp > baseHp)
                hp_spr[0].color = Color.green;
            else if (final_hp < baseHp)
                hp_spr[0].color = Color.red;
            else
                hp_spr[0].color = Color.white;

        }
        else
        {
            hp_spr[1].sprite = DataMng.instance.num[tempHp % 10];
            hp_spr[2].sprite = DataMng.instance.num[tempHp / 10];
            hp_spr[0].gameObject.SetActive(false);
            hp_spr[1].gameObject.SetActive(true);
            hp_spr[2].gameObject.SetActive(true);
            for (int i = 1; i <= 2; i++)
                if (final_hp > baseHp)
                    hp_spr[i].color = Color.green;
                else if (final_hp < baseHp)
                    hp_spr[i].color = Color.red;
                else
                    hp_spr[i].color = Color.white;
        }

        if (final_atk < 10)
        {
            atk_spr[0].sprite = DataMng.instance.num[final_atk];
            atk_spr[0].gameObject.SetActive(true);
            atk_spr[1].gameObject.SetActive(false);
            atk_spr[2].gameObject.SetActive(false);
            if (final_atk > baseAtk)
                atk_spr[0].color = Color.green;
            else if (final_atk < baseAtk)
                atk_spr[0].color = Color.red;
            else
                atk_spr[0].color = Color.white;
        }
        else
        {
            atk_spr[1].sprite = DataMng.instance.num[final_atk % 10];
            atk_spr[2].sprite = DataMng.instance.num[final_atk / 10];
            atk_spr[0].gameObject.SetActive(false);
            atk_spr[1].gameObject.SetActive(true);
            atk_spr[2].gameObject.SetActive(true);
            for (int i = 1; i <= 2; i++)
                if (final_atk > baseAtk)
                    atk_spr[i].color = Color.green;
                else if (final_atk < baseAtk)
                    atk_spr[i].color = Color.red;
                else
                    atk_spr[i].color = Color.white;
        }
    }
    #endregion

    #region[지속적인 처리]
    public void UpdateTrigger()
    {
        if (!enemy)
            canAttackObj.SetActive(
                !GameEventManager.instance.EventCheck() && 
                final_atk != 0 && 
                TurnManager.instance.turn == 턴.플레이어 && 
                canAttack && 
                canAttackNum > 0 &&
                !sleep &&
                !freeze &&
                !MinionField.instance.MinionAttackCheck() && 
                animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환완료"));

        tauntObj.SetActive(taunt);
        stealthObj.SetActive(stealth);
        legendObj.SetActive(legend);
        freezeObj.SetActive(freeze);
        silenceObj.SetActive(silence);

        if (flagAtk != final_atk)
        {
            flagAtk = final_atk;
            atkAni.SetTrigger("Change");
        }
        if (flagHp != final_hp)
        {
            flagHp = final_hp;
            hpAni.SetTrigger("Change");
        }

        SpellRunCheck();
        if (!enemy && HeroManager.instance.heroAtkManager.playerWeaponDurability > 0)
            MinionManager.instance.EquipWeaponMinionAbility(this);
        else if (enemy && HeroManager.instance.heroAtkManager.enemyWeaponDurability > 0)
            MinionManager.instance.EquipWeaponMinionAbility(this);
    }
    #endregion

    #region[주문시전시]
    public void SpellRunCheck()
    {
        if (!spellRun)
            return;
        spellRun = false;
        MinionManager.instance.SpellRunMinionAbility(this);
    }
    #endregion

    #region[턴 시작시 처리]
    public void TurnStart()
    {
        if (!turnStartTrigger)
            return;
        turnStartTrigger = false;
        freezeCount--;
        canAttackNum = 1;
    }
    #endregion

    #region[턴 종료시 처리]
    public void TurnEnd()
    {
        if (!turnEndTrigger)
            return;
        turnEndTrigger = false;
        sleep = false;
        freezeCount--;
        //버프제거
        for (int i = 0; i < buffList.Count; i++)
            if (buffList[i].w == 1)
                buffList[i] = Vector4.zero;
    }
    #endregion

    #region[손에서 하수인 소환시]
    public void CardHandMinionSpawn()
    {
        MinionManager.instance.SpawnMinionAbility(this);   
    }
    #endregion

    #region[하수인 패로 되돌리기]
    void GoToHand()
    {
        if (gotoHandTrigger)
        {
            gotoHandTrigger = false;
            if (!enemy)
                StartCoroutine(GotoHand_PlayerMinion());
            else
                StartCoroutine(GotoHand_EnemyMinion());
        }
    }

    private IEnumerator GotoHand_PlayerMinion()
    {
        animator.SetTrigger("GoHand");
        DragCardObject.instance.GotoHandEffect(Camera.main.WorldToScreenPoint(transform.position),minion_name);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환안됨"))
        {
            GameEventManager.instance.EventSet(1f);
            yield return new WaitForSeconds(0.001f);
        }

        abilityList.Clear();
        buffList.Clear();
        taunt = false;
        stealth = false;

        MinionObject temp = this;
        for (int i = num; i < MinionField.instance.minionNum - 1; i++)
            MinionField.instance.minions[i] = MinionField.instance.minions[i + 1];
        MinionField.instance.minions[MinionField.instance.minionNum - 1] = temp;
        MinionField.instance.minionNum--;

        if (CardHand.instance.nowHandNum < 10)
            for (int i = 0; i < BattleUI.instance.playerCardAni.Length; i++)
            {
                if (BattleUI.instance.playerCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                {
                    CardHand.instance.DrawCard();
                    CardHand.instance.CardMove(minion_name, CardHand.instance.nowHandNum - 1, transform.position, CardHand.instance.defaultSize, 0);
                    CardViewManager.instance.UpdateCardView();
                    break;
                }
            }
    }

    private IEnumerator GotoHand_EnemyMinion(float speed = 1)
    {
        animator.SetTrigger("GoHand");
        animator.speed = speed;
        DragCardObject.instance.GotoHandEffect(Camera.main.WorldToScreenPoint(transform.position), minion_name);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환안됨"))
        {
            GameEventManager.instance.EventSet(1f);
            yield return new WaitForSeconds(0.001f);
        }
        animator.speed = 1;
        abilityList.Clear();
        buffList.Clear();
        taunt = false;
        stealth = false;

        MinionObject temp = this;
        for (int i = num; i < EnemyMinionField.instance.minionNum - 1; i++)
            EnemyMinionField.instance.minions[i] = EnemyMinionField.instance.minions[i + 1];
        EnemyMinionField.instance.minions[EnemyMinionField.instance.minionNum - 1] = temp;
        EnemyMinionField.instance.minionNum--;

        if (EnemyCardHand.instance.nowHandNum < 10)
            for (int i = 0; i < BattleUI.instance.playerCardAni.Length; i++)
            {
                if (BattleUI.instance.playerCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                {
                    EnemyCardHand.instance.DrawCard(false);
                    EnemyCardHand.instance.CardMove(EnemyCardHand.instance.nowHandNum - 1, transform.position, EnemyCardHand.instance.defaultSize, 180);
                    CardViewManager.instance.UpdateCardView();
                    break;
                }
            }
    }
    #endregion

    #region[미니언 죽음]
    public void MinionDeath()
    {
        animator.SetTrigger("Death");
        StartCoroutine(MinionDeath_C(1.25f));
    }

    private IEnumerator MinionDeath_C(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (!enemy)
        {
            MinionObject temp = this;
            for (int i = num; i < MinionField.instance.minionNum - 1; i++)
                MinionField.instance.minions[i] = MinionField.instance.minions[i + 1];
            MinionField.instance.minions[MinionField.instance.minionNum - 1] = this;
            MinionField.instance.minionNum--;
        }
        else
        {
            MinionObject temp = this;
            for (int i = num; i < EnemyMinionField.instance.minionNum - 1; i++)
                EnemyMinionField.instance.minions[i] = EnemyMinionField.instance.minions[i + 1];
            EnemyMinionField.instance.minions[EnemyMinionField.instance.minionNum - 1] = this;
            EnemyMinionField.instance.minionNum--;
        }
        MinionManager.instance.DeathMinionAbility(this);
        MinionRemoveProcess();
    }
    #endregion

    #region[소멸 프로세스]
    public void MinionRemoveProcess()
    {
        abilityList.Clear();
        buffList.Clear();
        canAttackNum = 0;
        taunt = false;
        stealth = false;
        silence = false;
        freezeCount = 0;
        freeze = false;
        canAttack = false;
    }
    #endregion

    #region[빙결]
    public void FreezeUpdate()
    {
        freeze = freezeCount > 0;
        if (!freezeTrigger)
            return;
        freezeTrigger = false;
        freezeCount = 2;
    }
    #endregion

    #region[침묵]
    public void ActSilence()
    {
        abilityList.Clear();
        buffList.Clear();
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "체력");
        final_hp = Mathf.Min(baseHp, final_hp);
        baseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "공격력");
        nowAtk = Mathf.Min(baseAtk, nowAtk);
        taunt = false;
        stealth = false;
        freeze = false;

        silence = true;
    }
    #endregion

    #region[초기화]
    public void Init()
    {
        if (!InitTrigger && !TestInitTrigger)
            return;
        if (TestInitTrigger)
            animator.SetTrigger("NormalSpawn");
        InitTrigger = false;
        TestInitTrigger = false;
        meshRenderer.material = MinionManager.instance.minionMaterial[minion_name];
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "체력");
        baseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "공격력");
        legend = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "등급").Equals("전설");
        buffList.Clear();
        final_hp = baseHp;
        nowAtk = baseAtk;
        nowSpell = 0;
        canAttackNum = 1;
        canAttack = true;
        sleep = true;
        taunt = false;
        stealth = false;
        silence = false;
        freeze = false;

        ////////////////////////////////////////////////////////////////////////////////////////

        string ability_string = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");
        abilityList.Clear();
        abilityList = MinionManager.instance.MinionAbilityParsing(ability_string);
      
        MinionManager.instance.BaseMinionAbility(this);

    }
    #endregion

    #region[능력발휘]
    public void ActCondition(MinionAbility.Condition c)
    {
        for(int i = 0; i < abilityList.Count; i++)
        {
            if(abilityList[i].Condition_type == c)
            {
                MinionAbility.Ability a = abilityList[i].Ability_type;
                switch(a)
                {
                    case MinionAbility.Ability.능력치부여:


                        break;


                }


            }



        }
    }


    #endregion
}
