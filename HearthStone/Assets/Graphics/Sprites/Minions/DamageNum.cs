using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNum : MonoBehaviour
{
    public int damage = 0;
    public string hpSystem;

    [Header("------------------------------------------------")]
    [Space(30)]

    public SpriteRenderer[] damageNum;
    public Animator animator;

    void Update()
    {
        if (!DataMng.instance)
        {
            damageNum[0].gameObject.SetActive(false);
            damageNum[1].gameObject.SetActive(false);
            damageNum[2].gameObject.SetActive(false);
            return;
        }

        if (damage < 10)
        {
            damageNum[1].gameObject.SetActive(false);
            damageNum[2].gameObject.SetActive(false);
            damageNum[0].gameObject.SetActive(true);
            damageNum[3].gameObject.SetActive(true);
            damageNum[4].gameObject.SetActive(false);
            damageNum[0].sprite = DataMng.instance.num[damage % 10];
        }
        else
        {
            damageNum[1].gameObject.SetActive(true);
            damageNum[2].gameObject.SetActive(true);
            damageNum[0].gameObject.SetActive(false);
            damageNum[3].gameObject.SetActive(false);
            damageNum[4].gameObject.SetActive(true);
            damageNum[1].sprite = DataMng.instance.num[damage / 10];
            damageNum[2].sprite = DataMng.instance.num[damage % 10];
        }
    }
}
