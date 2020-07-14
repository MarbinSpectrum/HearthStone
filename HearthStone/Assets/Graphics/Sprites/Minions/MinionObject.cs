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

    public bool canAttack;

    [HideInInspector] public bool turnStartTrigger;
    [HideInInspector] public bool turnEndTrigger;

    [Space(20)]
    public bool InitTrigger;
    [Header("-----------------------------------------")]
    [Space(20)]
    public SpriteRenderer[] hp_spr;
    public SpriteRenderer[] atk_spr;
    public MeshRenderer meshRenderer;
    public GameObject canAttackObj;

    #region[Update]
    void Update()
    {
        Init();
        TurnStart();
        TurnEnd();
        UpdateStat();
    }
    #endregion

    #region[스텟 업데이트]
    public void UpdateStat()
    {
        if (hp < 10)
        {
            hp_spr[0].sprite = DataMng.instance.num[hp];
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
            hp_spr[1].sprite = DataMng.instance.num[hp%10];
            hp_spr[2].sprite = DataMng.instance.num[hp/10];
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

    #region[턴 시작시 처리]
    public void TurnStart()
    {
        if (!turnStartTrigger)
            return;
        turnStartTrigger = false;
        canAttack = true;
        canAttackObj.SetActive(canAttack);
    }
    #endregion

    #region[턴 종료시 처리]
    public void TurnEnd()
    {
        if (!turnEndTrigger)
            return;
        turnEndTrigger = false;
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

        canAttack = false;
        canAttackObj.SetActive(canAttack);
    }
    #endregion
}
