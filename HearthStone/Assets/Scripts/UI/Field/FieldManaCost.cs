using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManaCost : MonoBehaviour
{
    [HideInInspector] public int nowMana;
    [HideInInspector] public int maxMana;

    public SpriteRenderer[] nowManaNum;
    public SpriteRenderer[] maxManaNum;

    private void Update()
    {
        if (!DataMng.instance)
        {
            nowManaNum[0].gameObject.SetActive(false);
            nowManaNum[1].gameObject.SetActive(false);
            nowManaNum[2].gameObject.SetActive(false);
            maxManaNum[0].gameObject.SetActive(false);
            maxManaNum[1].gameObject.SetActive(false);
            maxManaNum[2].gameObject.SetActive(false);
            return;
        }

        int now_s = nowMana % 10;
        int now_t = nowMana / 10;
        if(now_t <= 0)
        {
            nowManaNum[0].gameObject.SetActive(false);
            nowManaNum[1].gameObject.SetActive(false);
            nowManaNum[2].gameObject.SetActive(true);
            nowManaNum[2].sprite = DataMng.instance.num[now_s];
        }
        else
        {
            nowManaNum[0].gameObject.SetActive(true);
            nowManaNum[1].gameObject.SetActive(true);
            nowManaNum[2].gameObject.SetActive(false);
            nowManaNum[0].sprite = DataMng.instance.num[now_t];
            nowManaNum[1].sprite = DataMng.instance.num[now_s];
        }

        int max_s = maxMana % 10;
        int max_t = maxMana / 10;
        if (max_t <= 0)
        {
            maxManaNum[0].gameObject.SetActive(false);
            maxManaNum[1].gameObject.SetActive(false);
            maxManaNum[2].gameObject.SetActive(true);
            maxManaNum[2].sprite = DataMng.instance.num[max_s];
        }
        else
        {
            maxManaNum[0].gameObject.SetActive(true);
            maxManaNum[1].gameObject.SetActive(true);
            maxManaNum[2].gameObject.SetActive(false);
            maxManaNum[0].sprite = DataMng.instance.num[max_t];
            maxManaNum[1].sprite = DataMng.instance.num[max_s];
        }
    }
}
