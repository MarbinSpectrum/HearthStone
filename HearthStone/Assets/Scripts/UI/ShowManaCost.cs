using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[ExecuteInEditMode]
public class ShowManaCost : MonoBehaviour
{
    [HideInInspector] public int nowMana;
    [HideInInspector] public int maxMana;

    public Image[] manaObject;
    public Image[] nowManaNum;
    public Image[] maxManaNum;

    void Update()
    {
        for(int i = 0; i < manaObject.Length; i++)
            manaObject[i].enabled = (i < maxMana);
        for (int i = 0; i < manaObject.Length; i++)
            manaObject[i].color = (i < nowMana) ? Color.white : Color.gray;

        if (!DataMng.instance)
        {
            nowManaNum[0].gameObject.SetActive(false);
            nowManaNum[1].gameObject.SetActive(false);
            maxManaNum[0].gameObject.SetActive(false);
            maxManaNum[1].gameObject.SetActive(false);
            return;
        }
        int now_s = nowMana % 10;
        int now_t = nowMana / 10;
        nowManaNum[0].sprite = DataMng.instance.num[now_s];
        nowManaNum[1].sprite = DataMng.instance.num[now_t];
        nowManaNum[0].gameObject.SetActive(true);
        nowManaNum[1].gameObject.SetActive(now_t != 0);

        int max_s = maxMana % 10;
        int max_t = maxMana / 10;
        maxManaNum[0].sprite = DataMng.instance.num[max_s];
        maxManaNum[1].sprite = DataMng.instance.num[max_t];
        maxManaNum[0].gameObject.SetActive(true);
        maxManaNum[1].gameObject.SetActive(max_t != 0);
    }
}
