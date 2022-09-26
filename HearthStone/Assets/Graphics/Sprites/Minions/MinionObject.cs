using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObject : MonoBehaviour
{
    public static bool minionSoundRun = false;

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
    public bool checkCanAttack;

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
    private void Update()
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
        checkCanAttack = !GameEventManager.instance.EventCheck(); //이벤트 중인가?
        checkCanAttack &= (final_atk > 0); //최종공격력이 0보다 큰가?
        checkCanAttack &= canAttack; //공격이 가능한 하수인인가?
        checkCanAttack &= canAttackNum > 0; //공격횟수가 남아있는가?
        checkCanAttack &= !sleep; //대기상태인가?
        checkCanAttack &= !freeze; //빙결상태인가?
        checkCanAttack &= SpawnAniEnd(); //해당하수인의 소환이 끝났는가?

        if (enemy)
        {
            //상대 하수인이라면
            checkCanAttack &= (TurnManager.instance.turn == Turn.상대방); //상대 턴인가?
            checkCanAttack &= !EnemyMinionField.instance.MinionAttackCheck(); //공격중인 상태인가?
        }
        else
        {
            //플레이어 하수인이라면
            checkCanAttack &= (TurnManager.instance.turn == Turn.플레이어);  //플레이어 턴인가?
            checkCanAttack &= !MinionField.instance.MinionAttackCheck(); //공격중인 상태인가?
            canAttackObj.SetActive(checkCanAttack);
        }

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
        if (!enemy && HeroManager.instance.EquipWeapon())
            MinionManager.instance.EquipWeaponMinionAbility(this);
        else if (enemy && HeroManager.instance.EnemyEquipWeapon())
            MinionManager.instance.EquipWeaponMinionAbility(this);
    }
    #endregion

    #region[소환완료 애니메이션이 끝남]
    public bool SpawnAniEnd()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환완료");
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
        sleep = false;
        canAttackNum = 1;
    }
    #endregion

    #region[턴 종료시 처리]
    public void TurnEnd()
    {
        if (!turnEndTrigger)
            return;
        turnEndTrigger = false;
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
        DragCardObject.instance.GotoHandEffect(transform.position, minion_name, false);
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
        {
            CardHand.instance.nowHandNum++;
            CardHand.instance.SetCardHand(minion_name, 
                CardHand.instance.nowHandNum - 1, transform.position, CardHand.instance.defaultSize, 0);
            CardViewManager.instance.UpdateCardView();
        }

        yield return new WaitForSeconds(0.001f);
    }

    private IEnumerator GotoHand_EnemyMinion()
    {
        animator.SetTrigger("GoHand");
        DragCardObject.instance.GotoHandEffect(transform.position, minion_name, true);
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
        for (int i = num; i < EnemyMinionField.instance.minionNum - 1; i++)
            EnemyMinionField.instance.minions[i] = EnemyMinionField.instance.minions[i + 1];
        EnemyMinionField.instance.minions[EnemyMinionField.instance.minionNum - 1] = temp;
        EnemyMinionField.instance.minionNum--;

        if (EnemyCardHand.instance.nowHandNum < 10)
        {
            EnemyCardHand.instance.nowHandNum++;
            EnemyCardHand.instance.nowCard.Add(temp.minion_name);
            EnemyCardHand.instance.CardMove(EnemyCardHand.instance.nowHandNum - 1, transform.position, EnemyCardHand.instance.defaultSize, 180);
            CardViewManager.instance.UpdateCardView();
        }

        yield return new WaitForSeconds(0.001f);
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
            MinionManager.instance.DeathMinionAbility(this);
            MinionField.instance.RemoveMinion(num);
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
        if(final_hp <= 0)
            SoundManager.instance.PlayMinionSE(minion_name, 미니언상태.죽음);

        //능력제거
        abilityList.Clear();

        //버프제거
        buffList.Clear();

        //상태초기화
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
        //몬스터효과 제거
        abilityList.Clear();

        //버프효과 제거
        buffList.Clear();

        //능력치를 기본 능력치 기준으로 바꾼다.
        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
        final_hp = Mathf.Min(baseHp, final_hp);
        baseAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
        nowAtk = Mathf.Min(baseAtk, nowAtk);

        //침묵을 제외한 상태이상 제거
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
        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
        baseAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
        legend = DataMng.instance.ToString(pair.x, pair.y, "등급").Equals("전설");
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

        string ability_string = DataMng.instance.ToString(pair.x, pair.y, "명령어");
        abilityList.Clear();
        abilityList = MinionManager.instance.MinionAbilityParsing(ability_string);
        SoundManager.instance.PlayMinionSE(minion_name, 미니언상태.소환);
        if (legend)
            SoundManager.instance.DownBGM(3.5f, 0.25f);
        MinionManager.instance.BaseMinionAbility(this);

    }
    #endregion
}
