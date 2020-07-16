using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionObject : MonoBehaviour
{
    [Header("하수인 정보")]
    public string minion_name;

    public int hp;
    int baseHp;

    public int atk;
    int baseAtk;

    public bool enemy;

    public int canAttackNum;

    [HideInInspector] public bool turnStartTrigger;
    [HideInInspector] public bool turnEndTrigger;

    public List<MinionAbility> ability = new List<MinionAbility>();

    //[Header("-----------------------------------------")]
    [Space(20)]
    public bool InitTrigger;
    [Header("-----------------------------------------")]
    [Space(100)]

    public SpriteRenderer[] hp_spr;
    public SpriteRenderer[] atk_spr;
    public MeshRenderer meshRenderer;
    public GameObject canAttackObj;
    public DamageNum damageEffect;
    public Animator animator;


    //오브젝트 넘버
    [HideInInspector] public int num;

    #region[Update]
    void Update()
    {
        Init();
        UpdateTrigger();
        TurnStart();
        TurnEnd();
        UpdateStat();
        SetObjectNum();
        damageEffect.hpSystem = (enemy ? "적_하수인_" : "아군_하수인_") + num;
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
    }
    #endregion

    #region[스텟 업데이트]
    public void UpdateStat()
    {
        int tempHp = Mathf.Abs(hp);

        if (hp < 0)
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
            if (hp > baseHp)
                hp_spr[0].color = Color.green;
            else if (hp < baseHp)
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
                if (hp > baseHp)
                    hp_spr[i].color = Color.green;
                else if (hp < baseHp)
                    hp_spr[i].color = Color.red;
                else
                    hp_spr[i].color = Color.white;
        }

        if (atk < 10)
        {
            atk_spr[0].sprite = DataMng.instance.num[atk];
            atk_spr[0].gameObject.SetActive(true);
            atk_spr[1].gameObject.SetActive(false);
            atk_spr[2].gameObject.SetActive(false);
            if (atk > baseAtk)
                atk_spr[0].color = Color.green;
            else if (atk < baseAtk)
                atk_spr[0].color = Color.red;
            else
                atk_spr[0].color = Color.white;
        }
        else
        {
            atk_spr[1].sprite = DataMng.instance.num[atk % 10];
            atk_spr[2].sprite = DataMng.instance.num[atk / 10];
            atk_spr[0].gameObject.SetActive(false);
            atk_spr[1].gameObject.SetActive(true);
            atk_spr[2].gameObject.SetActive(true);
            for (int i = 1; i <= 2; i++)
                if (atk > baseAtk)
                    atk_spr[i].color = Color.green;
                else if (atk < baseAtk)
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
            canAttackObj.SetActive(atk != 0 && canAttackNum > 0 && !MinionField.instance.MinionAttackCheck());
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
        canAttackNum = 0;
    }
    #endregion

    #region[미니언 죽음]
    public void MinionDeath()
    {
        animator.SetTrigger("Death");
        StartCoroutine(MinionDeath_C(1f));
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
        hp = baseHp;
        atk = baseAtk;

        canAttackNum = 0;
    }
    #endregion
}
