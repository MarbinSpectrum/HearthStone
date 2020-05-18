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

    public Sprite[] rogueMinions;
    public Sprite[] rogueSpell;
    public Sprite[] rogueWeapon;

    public Sprite[] druidMinions;
    public Sprite[] druidSpell;
    public Sprite[] druidWeapon;

    public Sprite[] neutralityMinions;
    public Sprite[] neutralitySpell;
    public Sprite[] neutralityWeapon;

    [HideInInspector] public string cardJob = "중립";
    [HideInInspector] public string cardLevel = "기본";

    #region[하수인카드 정보]
    GameObject MinionsCard;
    [HideInInspector] public int MinionsCostData;
    [HideInInspector] public int MinionsAttackData;
    [HideInInspector] public int MinionsHpData;
    [HideInInspector] public string MinionsCardNameData;
    [HideInInspector] public string MinionsCardExplainData;

    GameObject[] MinionsCost;
    Image[] MinionsCostImage;
    GameObject[] MinionsAttack;
    Image[] MinionsAttackImage;
    GameObject[] MinionsHp;
    Image[] MinionsHpImage;
    Text MinionsCardName;
    Text MinionsCardExplain;
    Image MinionsCardLevel;
    Image MinionsCardImg;
    #endregion

    #region[주문카드 정보]
    GameObject SpellCard;
    [HideInInspector] public int SpellCostData;
    [HideInInspector] public string SpellCardNameData;
    [HideInInspector] public string SpellCardExplainData;

    GameObject[] SpellCost;
    Image[] SpellCostImage;
    Text SpellCardName;
    Text SpellCardExplain;
    Image SpellCardLevel;
    Image SpellCardImg;
    #endregion

    #region[무기카드 정보]
    GameObject WeaponCard;
    [HideInInspector] public int WeaponCostData;
    [HideInInspector] public int WeaponAttackData;
    [HideInInspector] public int WeaponHpData;
    [HideInInspector] public string WeaponCardNameData;
    [HideInInspector] public string WeaponCardExplainData;

    GameObject[] WeaponCost;
    Image[] WeaponCostImage;
    GameObject[] WeaponAttack;
    Image[] WeaponAttackImage;
    GameObject[] WeaponHp;
    Image[] WeaponHpImage;
    Text WeaponCardName;
    Text WeaponCardExplain;
    Image WeaponCardLevel;
    Image WeaponCardImg;
    #endregion

    #region[Awake]
    public void Awake()
    {
        #region[하수인카드]
        MinionsCard = transform.Find("하수인카드").gameObject;
        MinionsCardLevel = transform.Find("하수인카드").Find("등급").GetComponent<Image>();
        MinionsCardImg = transform.Find("하수인카드").Find("이미지마스크").Find("카드이미지").GetComponent<Image>();

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
        SpellCardImg = transform.Find("주문카드").Find("이미지마스크").Find("카드이미지").GetComponent<Image>();

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
        WeaponCardImg = transform.Find("무기카드").Find("이미지마스크").Find("카드이미지").GetComponent<Image>();

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
        ShowCardFrame();
        ShowCardImg();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[등급에 따른 카드 프레임표시]
    public void ShowCardFrame()
    {
        int getLevel = 0;
        switch(cardLevel)
        {
            case "일반":
                getLevel = 1;
                break;
            case "희귀":
                getLevel = 2;
                break;
            case "특별":
                getLevel = 3;
                break;
            case "전설":
                getLevel = 4;
                break;
            case "기본":
            case "토큰":
                getLevel = 0;
                break;
        }

        #region[드루이드]
        if (cardJob.Equals("드루이드"))
        {
            if(cardType == CardType.하수인)
                MinionsCardLevel.sprite = druidMinions[getLevel];
            else if (cardType == CardType.주문)
                SpellCardLevel.sprite = druidSpell[getLevel];
            else if (cardType == CardType.무기)
                WeaponCardLevel.sprite = druidWeapon[getLevel];
        }
        #endregion

        #region[도적]
        else if (cardJob.Equals("도적"))
        {
            if (cardType == CardType.하수인)
                MinionsCardLevel.sprite = rogueMinions[getLevel];
            else if (cardType == CardType.주문)
                SpellCardLevel.sprite = rogueSpell[getLevel];
            else if (cardType == CardType.무기)
                WeaponCardLevel.sprite = rogueWeapon[getLevel];
        }
        #endregion

        #region[기타]
        else
        {
            if (cardType == CardType.하수인)
                MinionsCardLevel.sprite = neutralityMinions[getLevel];
            else if (cardType == CardType.주문)
                SpellCardLevel.sprite = neutralitySpell[getLevel];
            else if (cardType == CardType.무기)
                WeaponCardLevel.sprite = neutralityWeapon[getLevel];
        }
        #endregion
    }
    #endregion

    #region[카드 이미지표시]
    public void ShowCardImg()
    {
        try
        {
            if (cardType == CardType.하수인)
                MinionsCardImg.sprite = DataMng.instance.cardImg[MinionsCardNameData];
            else if (cardType == CardType.주문)
                SpellCardImg.sprite = DataMng.instance.cardImg[SpellCardNameData];
            else if (cardType == CardType.무기)
                WeaponCardImg.sprite = DataMng.instance.cardImg[WeaponCardNameData];
        }
        catch { }
    }
    #endregion

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
        string temp = "";
        for(int i = 0; i < MinionsCardExplainData.Length; i++)
        {
            if (i < MinionsCardExplainData.Length - 1 && MinionsCardExplainData[i].Equals('\\') && MinionsCardExplainData[i + 1].Equals('n'))
            {
                temp += "\n";
                i++;
            }
            else
                temp += MinionsCardExplainData[i];
        }
        MinionsCardExplain.text = temp;

        temp = "";
        for (int i = 0; i < SpellCardExplainData.Length; i++)
        {
            if (i < SpellCardExplainData.Length - 1 && SpellCardExplainData[i].Equals('\\') && SpellCardExplainData[i + 1].Equals('n'))
            {
                temp += "\n";
                i++;
            }
            else
                temp += SpellCardExplainData[i];
        }
        SpellCardExplain.text = temp;

        temp = "";
        for (int i = 0; i < WeaponCardExplainData.Length; i++)
        {
            if (i < WeaponCardExplainData.Length - 1 && WeaponCardExplainData[i].Equals('\\') && WeaponCardExplainData[i + 1].Equals('n'))
            {
                temp += "\n";
                i++;
            }
            else
                temp += WeaponCardExplainData[i];
        }
        WeaponCardExplain.text = temp;
    }
    #endregion

}
