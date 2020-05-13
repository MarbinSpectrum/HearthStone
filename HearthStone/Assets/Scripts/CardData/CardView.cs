using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Sprite[] num;

    GameObject[] cost;
    Image[] costImage;

    public int costData;

    GameObject[] attack;
    Image[] attackImage;
    public int attackData;

    GameObject[] hp;
    Image[] hpImage;
    public int hpData;

    Text cardName;
    public string cardNameData;

    Text cardExplain;
    public string cardExplainData;

    #region[Awake]
    public void Awake()
    {
        cost = new GameObject[2];
        costImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            cost[i] = transform.Find("하수인카드").Find("코스트").Find(i + "").gameObject;
            costImage[i] = cost[i].GetComponent<Image>();
        }

        attack = new GameObject[2];
        attackImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            attack[i] = transform.Find("하수인카드").Find("공격력").Find(i + "").gameObject;
            attackImage[i] = attack[i].GetComponent<Image>();
        }

        hp = new GameObject[2];
        hpImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            hp[i] = transform.Find("하수인카드").Find("체력").Find(i + "").gameObject;
            hpImage[i] = hp[i].GetComponent<Image>();
        }

        cardName = transform.Find("하수인카드").Find("이름").GetComponent<Text>();

        cardExplain = transform.Find("하수인카드").Find("설명").GetComponent<Text>();
    }
    #endregion

    #region[LateUpdate]
    void LateUpdate()
    {
        ShowCost();
        ShowAttack();
        ShowHp();
        ShowName();
        ShowExplain();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[ShowCost]
    void ShowCost()
    {
        if (costData > 99)
            costData = 99;
        if (costData < 0)
            costData = 0;
        string c = costData.ToString();

        if (c.Length > 1)
        {
            cost[0].SetActive(true);
            costImage[0].sprite = num[c[1] - '0'];
            cost[1].SetActive(true);
            costImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            cost[0].SetActive(true);
            costImage[0].sprite = num[c[0] - '0'];
            cost[1].SetActive(false);
        }
    }
    #endregion

    #region[ShowAttack]
    void ShowAttack()
    {
        if (attackData > 99)
            attackData = 99;
        if (attackData < 0)
            attackData = 0;
        string c = attackData.ToString();

        if (c.Length > 1)
        {
            attack[0].SetActive(true);
            attackImage[0].sprite = num[c[1] - '0'];
            attack[1].SetActive(true);
            attackImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            attack[0].SetActive(true);
            attackImage[0].sprite = num[c[0] - '0'];
            attack[1].SetActive(false);
        }
    }
    #endregion

    #region[ShowHp]
    void ShowHp()
    {
        if (hpData > 99)
            hpData = 99;
        if (hpData < 0)
            hpData = 0;
        string c = hpData.ToString();

        if (c.Length > 1)
        {
            hp[0].SetActive(true);
            hpImage[0].sprite = num[c[1] - '0'];
            hp[1].SetActive(true);
            hpImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            hp[0].SetActive(true);
            hpImage[0].sprite = num[c[0] - '0'];
            hp[1].SetActive(false);
        }
    }
    #endregion

    #region[ShowName]
    void ShowName()
    {
        cardName.text = cardNameData;
    }
    #endregion

    #region[ShowExplain]
    void ShowExplain()
    {
        cardExplain.text = cardExplainData;
    }
    #endregion

}
