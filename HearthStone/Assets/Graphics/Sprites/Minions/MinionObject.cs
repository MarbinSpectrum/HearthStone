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

    [Space(20)]
    public bool InitTrigger;
    [Header("-----------------------------------------")]
    [Space(20)]
    public SpriteRenderer[] hp_spr;
    public SpriteRenderer[] atk_spr;
    public MeshRenderer meshRenderer;

    void Update()
    {
        if(InitTrigger)
        {
            InitTrigger = false;
            Init();
        }
        UpdateStat();
    }

    public void UpdateStat()
    {
        if (hp < 10)
        {
            hp_spr[0].sprite = DataMng.instance.num[hp];
            hp_spr[0].gameObject.SetActive(true);
            hp_spr[1].gameObject.SetActive(false);
            hp_spr[2].gameObject.SetActive(false);
        }
        else
        {
            hp_spr[1].sprite = DataMng.instance.num[hp%10];
            hp_spr[2].sprite = DataMng.instance.num[hp/10];
            hp_spr[0].gameObject.SetActive(false);
            hp_spr[1].gameObject.SetActive(true);
            hp_spr[2].gameObject.SetActive(true);
        }

        if (atk < 10)
        {
            atk_spr[0].sprite = DataMng.instance.num[atk];
            atk_spr[0].gameObject.SetActive(true);
            atk_spr[1].gameObject.SetActive(false);
            atk_spr[2].gameObject.SetActive(false);
        }
        else
        {
            atk_spr[1].sprite = DataMng.instance.num[atk % 10];
            atk_spr[2].sprite = DataMng.instance.num[atk / 10];
            atk_spr[0].gameObject.SetActive(false);
            atk_spr[1].gameObject.SetActive(true);
            atk_spr[2].gameObject.SetActive(true);
        }
    }

    public void Init()
    {
        meshRenderer.material = MinionManager.instance.minionMaterial[minion_name];
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(minion_name));
        baseHp = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "체력");
        baseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "공격력");
        hp = baseHp;
        atk = baseAtk;
    }
}
