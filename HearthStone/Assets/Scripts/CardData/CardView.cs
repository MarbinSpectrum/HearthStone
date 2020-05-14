using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    하수인,
    주문,
    무기
}

public class CardView : MonoBehaviour
{
    public CardType cardType;

    public Sprite[] num;

    #region[하수인카드 정보]
    GameObject MinionsCard;
    [HideInInspector] public int MinionsCostData;
    [HideInInspector] public int MinionsAttackData;
    [HideInInspector] public int MinionsHpData;
    [HideInInspector] public string MinionsCardNameData;
    [HideInInspector] public string MinionsCardExplainData;
    [HideInInspector] public Image MinionsCardLevel;
    [HideInInspector] public Image MinionsCardImg;
    GameObject[] MinionsCost;
    Image[] MinionsCostImage;
    GameObject[] MinionsAttack;
    Image[] MinionsAttackImage;
    GameObject[] MinionsHp;
    Image[] MinionsHpImage;
    Text MinionsCardName;
    Text MinionsCardExplain;
    #endregion

    #region[주문카드 정보]
    GameObject SpellCard;
    [HideInInspector] public int SpellCostData;
    [HideInInspector] public string SpellCardNameData;
    [HideInInspector] public string SpellCardExplainData;
    [HideInInspector] public Image SpellCardLevel;
    [HideInInspector] public Image SpellCardImg;
    GameObject[] SpellCost;
    Image[] SpellCostImage;
    Text SpellCardName;
    Text SpellCardExplain;
    #endregion

    #region[무기카드 정보]
    GameObject WeaponCard;
    [HideInInspector] public int WeaponCostData;
    [HideInInspector] public int WeaponAttackData;
    [HideInInspector] public int WeaponHpData;
    [HideInInspector] public string WeaponCardNameData;
    [HideInInspector] public string WeaponCardExplainData;
    [HideInInspector] public Image WeaponCardLevel;
    [HideInInspector] public Image WeaponCardImg;
    GameObject[] WeaponCost;
    Image[] WeaponCostImage;
    GameObject[] WeaponAttack;
    Image[] WeaponAttackImage;
    GameObject[] WeaponHp;
    Image[] WeaponHpImage;
    Text WeaponCardName;
    Text WeaponCardExplain;
    #endregion

    #region[Awake]
    public void Awake()
    {
        #region[하수인카드]
        MinionsCard = transform.Find("하수인카드").gameObject;
        MinionsCardLevel = transform.Find("하수인카드").Find("등급").GetComponent<Image>();
        MinionsCardImg = transform.Find("하수인카드").Find("카드이미지").GetComponent<Image>();

        MinionsCost = new GameObject[2];
        MinionsCostImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            MinionsCost[i] = transform.Find("하수인카드").Find("코스트").Find(i + "").gameObject;
            MinionsCostImage[i] = MinionsCost[i].GetComponent<Image>();
        }

        MinionsAttack = new GameObject[2];
        MinionsAttackImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            MinionsAttack[i] = transform.Find("하수인카드").Find("공격력").Find(i + "").gameObject;
            MinionsAttackImage[i] = MinionsAttack[i].GetComponent<Image>();
        }

        MinionsHp = new GameObject[2];
        MinionsHpImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            MinionsHp[i] = transform.Find("하수인카드").Find("체력").Find(i + "").gameObject;
            MinionsHpImage[i] = MinionsHp[i].GetComponent<Image>();
        }

        MinionsCardName = transform.Find("하수인카드").Find("이름").GetComponent<Text>();
        MinionsCardExplain = transform.Find("하수인카드").Find("설명").GetComponent<Text>();
        #endregion

        #region[주문카드]
        SpellCard = transform.Find("주문카드").gameObject;
        SpellCardLevel = transform.Find("주문카드").Find("등급").GetComponent<Image>();
        SpellCardImg = transform.Find("주문카드").Find("카드이미지").GetComponent<Image>();

        SpellCost = new GameObject[2];
        SpellCostImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            SpellCost[i] = transform.Find("주문카드").Find("코스트").Find(i + "").gameObject;
            SpellCostImage[i] = SpellCost[i].GetComponent<Image>();
        }

        SpellCardName = transform.Find("주문카드").Find("이름").GetComponent<Text>();
        SpellCardExplain = transform.Find("주문카드").Find("설명").GetComponent<Text>();
        #endregion

        #region[무기카드]
        WeaponCard = transform.Find("무기카드").gameObject;
        WeaponCardLevel = transform.Find("무기카드").Find("등급").GetComponent<Image>();
        WeaponCardImg = transform.Find("무기카드").Find("카드이미지").GetComponent<Image>();

        WeaponCost = new GameObject[2];
        WeaponCostImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            WeaponCost[i] = transform.Find("무기카드").Find("코스트").Find(i + "").gameObject;
            WeaponCostImage[i] = WeaponCost[i].GetComponent<Image>();
        }

        WeaponAttack = new GameObject[2];
        WeaponAttackImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            WeaponAttack[i] = transform.Find("무기카드").Find("공격력").Find(i + "").gameObject;
            WeaponAttackImage[i] = WeaponAttack[i].GetComponent<Image>();
        }

        WeaponHp = new GameObject[2];
        WeaponHpImage = new Image[2];
        for (int i = 0; i < 2; i++)
        {
            WeaponHp[i] = transform.Find("무기카드").Find("체력").Find(i + "").gameObject;
            WeaponHpImage[i] = WeaponHp[i].GetComponent<Image>();
        }

        WeaponCardName = transform.Find("무기카드").Find("이름").GetComponent<Text>();
        WeaponCardExplain = transform.Find("무기카드").Find("설명").GetComponent<Text>();
        #endregion
    }
    #endregion

    #region[LateUpdate]
    void LateUpdate()
    {
        ShowCard();
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

    #region[ShowCard]
    public void ShowCard()
    {
        switch (cardType)
        {
            case CardType.하수인:
                MinionsCard.SetActive(true);
                SpellCard.SetActive(false);
                WeaponCard.SetActive(false);
                break;
            case CardType.주문:
                MinionsCard.SetActive(false);
                SpellCard.SetActive(true);
                WeaponCard.SetActive(false);
                break;
            case CardType.무기:
                MinionsCard.SetActive(false);
                SpellCard.SetActive(false);
                WeaponCard.SetActive(true);
                break;
        }
    }
    #endregion

    #region[ShowCost]
    void ShowCost()
    {
        string c;

        #region[하수인코스트]
        if (MinionsCostData > 99)
            MinionsCostData = 99;
        if (MinionsCostData < 0)
            MinionsCostData = 0;
        c = MinionsCostData.ToString();

        if (c.Length > 1)
        {
            MinionsCost[0].SetActive(true);
            MinionsCostImage[0].sprite = num[c[1] - '0'];
            MinionsCost[1].SetActive(true);
            MinionsCostImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            MinionsCost[0].SetActive(true);
            MinionsCostImage[0].sprite = num[c[0] - '0'];
            MinionsCost[1].SetActive(false);
        }
        #endregion

        #region[주문코스트]
        if (SpellCostData > 99)
            SpellCostData = 99;
        if (SpellCostData < 0)
            SpellCostData = 0;
        c = SpellCostData.ToString();

        if (c.Length > 1)
        {
            SpellCost[0].SetActive(true);
            SpellCostImage[0].sprite = num[c[1] - '0'];
            SpellCost[1].SetActive(true);
            SpellCostImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            SpellCost[0].SetActive(true);
            SpellCostImage[0].sprite = num[c[0] - '0'];
            SpellCost[1].SetActive(false);
        }
        #endregion

        #region[무기코스트]
        if (WeaponCostData > 99)
            WeaponCostData = 99;
        if (WeaponCostData < 0)
            WeaponCostData = 0;
        c = WeaponCostData.ToString();

        if (c.Length > 1)
        {
            WeaponCost[0].SetActive(true);
            WeaponCostImage[0].sprite = num[c[1] - '0'];
            WeaponCost[1].SetActive(true);
            SpellCostImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            WeaponCost[0].SetActive(true);
            WeaponCostImage[0].sprite = num[c[0] - '0'];
            WeaponCost[1].SetActive(false);
        }
        #endregion
    }
    #endregion

    #region[ShowAttack]
    void ShowAttack()
    {
        string c;

        #region[하수인 공격력]
        if (MinionsAttackData > 99)
            MinionsAttackData = 99;
        if (MinionsAttackData < 0)
            MinionsAttackData = 0;
        c = MinionsAttackData.ToString();

        if (c.Length > 1)
        {
            MinionsAttack[0].SetActive(true);
            MinionsAttackImage[0].sprite = num[c[1] - '0'];
            MinionsAttack[1].SetActive(true);
            MinionsAttackImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            MinionsAttack[0].SetActive(true);
            MinionsAttackImage[0].sprite = num[c[0] - '0'];
            MinionsAttack[1].SetActive(false);
        }
        #endregion

        #region[무기 공격력]
        if (WeaponAttackData > 99)
            WeaponAttackData = 99;
        if (WeaponAttackData < 0)
            WeaponAttackData = 0;
        c = WeaponAttackData.ToString();

        if (c.Length > 1)
        {
            WeaponAttack[0].SetActive(true);
            WeaponAttackImage[0].sprite = num[c[1] - '0'];
            WeaponAttack[1].SetActive(true);
            WeaponAttackImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            WeaponAttack[0].SetActive(true);
            WeaponAttackImage[0].sprite = num[c[0] - '0'];
            WeaponAttack[1].SetActive(false);
        }
        #endregion
    }
    #endregion

    #region[ShowHp]
    void ShowHp()
    {
        string c;

        #region[하수인 체력]
        if (MinionsHpData > 99)
            MinionsHpData = 99;
        if (MinionsHpData < 0)
            MinionsHpData = 0;
        c = MinionsHpData.ToString();

        if (c.Length > 1)
        {
            MinionsHp[0].SetActive(true);
            MinionsHpImage[0].sprite = num[c[1] - '0'];
            MinionsHp[1].SetActive(true);
            MinionsHpImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            MinionsHp[0].SetActive(true);
            MinionsHpImage[0].sprite = num[c[0] - '0'];
            MinionsHp[1].SetActive(false);
        }
        #endregion

        #region[무기 체력]
        if (WeaponHpData > 99)
            WeaponHpData = 99;
        if (WeaponHpData < 0)
            WeaponHpData = 0;
        c = WeaponHpData.ToString();

        if (c.Length > 1)
        {
            WeaponHp[0].SetActive(true);
            WeaponHpImage[0].sprite = num[c[1] - '0'];
            WeaponHp[1].SetActive(true);
            WeaponHpImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            WeaponHp[0].SetActive(true);
            WeaponHpImage[0].sprite = num[c[0] - '0'];
            WeaponHp[1].SetActive(false);
        }
        #endregion
    }
    #endregion

    #region[ShowName]
    void ShowName()
    {
        MinionsCardName.text = MinionsCardNameData;
        SpellCardName.text = SpellCardNameData;
        WeaponCardName.text = WeaponCardNameData;
    }
    #endregion

    #region[ShowExplain]
    void ShowExplain()
    {
        MinionsCardExplain.text = MinionsCardExplainData;
        SpellCardExplain.text = SpellCardExplainData;
        WeaponCardExplain.text = WeaponCardExplainData;
    }
    #endregion

}
