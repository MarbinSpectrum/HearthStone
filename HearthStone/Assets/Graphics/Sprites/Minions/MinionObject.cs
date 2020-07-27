using System.Collections;
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

    [Header("은신")]
    public bool stealth;
    public GameObject stealthObj;

    [Header("도발")]
    public bool taunt;
    public GameObject tauntObj;

    [HideInInspector] public bool turnStartTrigger;
    [HideInInspector] public bool turnEndTrigger;

    public List<MinionAbility> abilityList = new List<MinionAbility>();
    /// <summary> x(공격력) y(체력) z(주문공격력) w(한턴동안만 유지되는 여부) </summary>
    [HideInInspector] public List<Vector4> buffList = new List<Vector4>();

    //[Header("-----------------------------------------")]
    [Space(20)]
    public bool InitTrigger;

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
                !MinionField.instance.MinionAttackCheck() && 
                animator.GetCurrentAnimatorStateInfo(0).IsName("하수인소환완료"));

        tauntObj.SetActive(taunt);
        stealthObj.SetActive(stealth);
        legendObj.SetActive(legend);

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
    }
    #endregion

    #region[턴 시작시 처리]
    public void TurnStart()
    {
        if (!turnStartTrigger)
            return;
        turnStartTrigger = false;
        canAttackNum = 1;
    }
    #endregion

    #region[턴 종료시 처리]
    public void TurnEnd()
    {
        if (!turnEndTrigger)
            return;
        turnEndTrigger = false;

        //버프제거
        for(int i = 0; i < buffList.Count; i++)
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
            {
                StartCoroutine(GotoHand_PlayerMinion());
            }
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
        abilityList.Clear();
        buffList.Clear();
        canAttackNum = 0;
        taunt = false;
        stealth = false;
        canAttack = false;
    }
    #endregion

    #region[초기화]
    public void Init()
    {
        if (!InitTrigger)
            return;
        InitTrigger = false;
        meshRenderer.material = MinionManager.instance.minionMaterial[minion_name];
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "체력");
        baseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "공격력");
        legend = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "등급").Equals("전설");
        buffList.Clear();
        final_hp = baseHp;
        nowAtk = baseAtk;
        nowSpell = 0;
        canAttackNum = 0;
        canAttack = true;

        ////////////////////////////////////////////////////////////////////////////////////////

        #region[미니언 능력 설정]

        abilityList.Clear();
        string ability_string = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");
        char[] splitChar = { '[', ']' };
        string[] ability_Split = ability_string.Split(splitChar);
        List<string> ability_Data = new List<string>();
        for (int i = 0; i < ability_Split.Length; i++)
            if (ability_Split[i].Length != 0)
                ability_Data.Add(ability_Split[i]);

        입력 inputType = 입력.조건;
        MinionAbility temp = null;
        int dataNum = 0;
        for (int i = 0; i < ability_Data.Count; i++)
        {
            if (inputType == 입력.조건)
            {
                temp = new MinionAbility();

                MinionAbility.Condition condition = MinionAbility.GetCondition(ability_Data[i]);
                temp.Condition_type = condition;

                if (MinionAbility.CheckDataCondition(condition))
                {
                    inputType = 입력.수치;
                    dataNum = 6;
                }
                else
                    inputType = 입력.능력;
            }
            else if (inputType == 입력.능력)
            {
                MinionAbility.Ability ability = MinionAbility.GetAbility(ability_Data[i]);
                temp.Ability_type = ability;

                if (MinionAbility.CheckDataAbility(ability))
                {
                    inputType = 입력.수치;
                    dataNum = 3;
                }
                else
                {
                    inputType = 입력.조건;
                    abilityList.Add(temp);
                }
            }
            else if (inputType == 입력.수치)
            {
                int value = 0;
                int.TryParse(ability_Data[i], out value);
                switch (dataNum)
                {
                    case 1:
                        temp.Ability_data = new Vector3(temp.Ability_data.x, temp.Ability_data.y, value);
                        break;
                    case 2:
                        temp.Ability_data = new Vector3(temp.Ability_data.x, value, temp.Ability_data.z);
                        break;
                    case 3:
                        temp.Ability_data = new Vector3(value, temp.Ability_data.y, temp.Ability_data.z);
                        break;
                    case 4:
                        temp.Condition_data = new Vector3(temp.Ability_data.x, temp.Ability_data.y, value);
                        break;
                    case 5:
                        temp.Condition_data = new Vector3(temp.Ability_data.x, value, temp.Ability_data.z);
                        break;
                    case 6:
                        temp.Condition_data = new Vector3(value, temp.Ability_data.y, temp.Ability_data.z);
                        break;
                }
                dataNum--;
                if(dataNum == 0)
                {
                    inputType = 입력.조건;
                    abilityList.Add(temp);
                }
                else if (dataNum == 3)
                    inputType = 입력.능력;
            }
        }

        #endregion

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
