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
    [HideInInspector] public CardType cardType;

    public bool hide;
    [HideInInspector] public bool updateCard;

    string cardName;
    Sprite[] num;

    Sprite[] rogueMinions;
    Sprite[] rogueSpell;
    Sprite[] rogueWeapon;

    Sprite[] druidMinions;
    Sprite[] druidSpell;
    Sprite[] druidWeapon;

    Sprite[] neutralityMinions;
    Sprite[] neutralitySpell;
    Sprite[] neutralityWeapon;

    public Material holoBlue;
    public Material holoBlueMove;
    public Material normalMat;
    public Material normalMoveMat;

    [HideInInspector] public int cardN = 2;

    [HideInInspector] public string cardJob = "중립";
    [HideInInspector] public string cardLevel = "기본";

    [HideInInspector] public int cardCostOffset = 0;
    public bool viewSpellOffset = false;

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
        CardViewManager.instance.cardview.Add(this);

        num = DataMng.instance.num;

        #region[드루이드 카드 폼]
        druidMinions = new Sprite[5];
        druidMinions[0] = Resources.Load<Sprite>("Card/Druid/Druid_Card_Base");
        druidMinions[1] = Resources.Load<Sprite>("Card/Druid/Druid_Card_Normal");
        druidMinions[2] = Resources.Load<Sprite>("Card/Druid/Druid_Card_Rare");
        druidMinions[3] = Resources.Load<Sprite>("Card/Druid/Druid_Card_Special");
        druidMinions[4] = Resources.Load<Sprite>("Card/Druid/Druid_Card_Legend");

        druidSpell = new Sprite[5];
        druidSpell[0] = Resources.Load<Sprite>("Card/Druid/Druid_SpellCard_Base");
        druidSpell[1] = Resources.Load<Sprite>("Card/Druid/Druid_SpellCard_Normal");
        druidSpell[2] = Resources.Load<Sprite>("Card/Druid/Druid_SpellCard_Rare");
        druidSpell[3] = Resources.Load<Sprite>("Card/Druid/Druid_SpellCard_Special");
        druidSpell[4] = Resources.Load<Sprite>("Card/Druid/Druid_SpellCard_Legend");

        druidWeapon = new Sprite[5];
        druidWeapon[0] = Resources.Load<Sprite>("Card/Druid/Druid_WeaponCard_Base");
        druidWeapon[1] = Resources.Load<Sprite>("Card/Druid/Druid_WeaponCard_Normal");
        druidWeapon[2] = Resources.Load<Sprite>("Card/Druid/Druid_WeaponCard_Rare");
        druidWeapon[3] = Resources.Load<Sprite>("Card/Druid/Druid_WeaponCard_Special");
        druidWeapon[4] = Resources.Load<Sprite>("Card/Druid/Druid_WeaponCard_Legend");
        #endregion

        #region[도적 카드 폼]
        rogueMinions = new Sprite[5];
        rogueMinions[0] = Resources.Load<Sprite>("Card/Rogue/Rogue_Card_Base");
        rogueMinions[1] = Resources.Load<Sprite>("Card/Rogue/Rogue_Card_Normal");
        rogueMinions[2] = Resources.Load<Sprite>("Card/Rogue/Rogue_Card_Rare");
        rogueMinions[3] = Resources.Load<Sprite>("Card/Rogue/Rogue_Card_Special");
        rogueMinions[4] = Resources.Load<Sprite>("Card/Rogue/Rogue_Card_Legend");

        rogueSpell = new Sprite[5];
        rogueSpell[0] = Resources.Load<Sprite>("Card/Rogue/Rogue_SpellCard_Base");
        rogueSpell[1] = Resources.Load<Sprite>("Card/Rogue/Rogue_SpellCard_Normal");
        rogueSpell[2] = Resources.Load<Sprite>("Card/Rogue/Rogue_SpellCard_Rare");
        rogueSpell[3] = Resources.Load<Sprite>("Card/Rogue/Rogue_SpellCard_Special");
        rogueSpell[4] = Resources.Load<Sprite>("Card/Rogue/Rogue_SpellCard_Legend");

        rogueWeapon = new Sprite[5];
        rogueWeapon[0] = Resources.Load<Sprite>("Card/Rogue/Rogue_WeaponCard_Base");
        rogueWeapon[1] = Resources.Load<Sprite>("Card/Rogue/Rogue_WeaponCard_Normal");
        rogueWeapon[2] = Resources.Load<Sprite>("Card/Rogue/Rogue_WeaponCard_Rare");
        rogueWeapon[3] = Resources.Load<Sprite>("Card/Rogue/Rogue_WeaponCard_Special");
        rogueWeapon[4] = Resources.Load<Sprite>("Card/Rogue/Rogue_WeaponCard_Legend");
        #endregion

        #region[중립 카드 폼]
        neutralityMinions = new Sprite[5];
        neutralityMinions[0] = Resources.Load<Sprite>("Card/Neutrality/Card_Base");
        neutralityMinions[1] = Resources.Load<Sprite>("Card/Neutrality/Card_Normal");
        neutralityMinions[2] = Resources.Load<Sprite>("Card/Neutrality/Card_Rare");
        neutralityMinions[3] = Resources.Load<Sprite>("Card/Neutrality/Card_Special");
        neutralityMinions[4] = Resources.Load<Sprite>("Card/Neutrality/Card_Legend");

        neutralitySpell = new Sprite[5];
        neutralitySpell[0] = Resources.Load<Sprite>("Card/Neutrality/SpellCard_Base");
        neutralitySpell[1] = Resources.Load<Sprite>("Card/Neutrality/SpellCard_Normal");
        neutralitySpell[2] = Resources.Load<Sprite>("Card/Neutrality/SpellCard_Rare");
        neutralitySpell[3] = Resources.Load<Sprite>("Card/Neutrality/SpellCard_Special");
        neutralitySpell[4] = Resources.Load<Sprite>("Card/Neutrality/SpellCard_Legend");

        neutralityWeapon = new Sprite[5];
        neutralityWeapon[0] = Resources.Load<Sprite>("Card/Neutrality/WeaponCard_Base");
        neutralityWeapon[1] = Resources.Load<Sprite>("Card/Neutrality/WeaponCard_Normal");
        neutralityWeapon[2] = Resources.Load<Sprite>("Card/Neutrality/WeaponCard_Rare");
        neutralityWeapon[3] = Resources.Load<Sprite>("Card/Neutrality/WeaponCard_Special");
        neutralityWeapon[4] = Resources.Load<Sprite>("Card/Neutrality/WeaponCard_Legend");
        #endregion

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
        if(!MinionsCard.activeSelf && !SpellCard.activeSelf && !WeaponCard.activeSelf && !hide)
            updateCard = true;
        if (updateCard)
        {
            ShowCard();
            ShowAttack();
            ShowHp();
            ShowName();
            ShowExplain();
            ShowCardFrame();
            ShowCardImg();
            GetCardNum();
            updateCard = false;
        }
        ShowCost();
        ShowHolo();
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
            case "특급":
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
        if(hide)
        {
            MinionsCard.SetActive(false);
            SpellCard.SetActive(false);
            WeaponCard.SetActive(false);
            return;
        }
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
        int showCost = MinionsCostData + cardCostOffset;
        if (showCost > 99)
            showCost = 99;
        if (showCost < 0)
            showCost = 0;
        c = showCost.ToString();

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

        MinionsCostImage[0].color = (showCost > MinionsCostData) ? Color.red : (showCost < MinionsCostData) ? Color.green : Color.white;
        MinionsCostImage[1].color = (showCost > MinionsCostData) ? Color.red : (showCost < MinionsCostData) ? Color.green : Color.white;
        #endregion

        #region[주문코스트]
        showCost = SpellCostData + cardCostOffset;
        if (showCost > 99)
            showCost = 99;
        if (showCost < 0)
            showCost = 0;
        c = showCost.ToString();

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

        SpellCostImage[0].color = (showCost > SpellCostData) ? Color.red : (showCost < SpellCostData) ? Color.green : Color.white;
        SpellCostImage[1].color = (showCost > SpellCostData) ? Color.red : (showCost < SpellCostData) ? Color.green : Color.white;
        #endregion

        #region[무기코스트]
        showCost = WeaponCostData + cardCostOffset;
        if (showCost > 99)
            showCost = 99;
        if (showCost < 0)
            showCost = 0;
        c = showCost.ToString();

        if (c.Length > 1)
        {
            WeaponCost[0].SetActive(true);
            WeaponCostImage[0].sprite = num[c[1] - '0'];
            WeaponCost[1].SetActive(true);
            WeaponCostImage[1].sprite = num[c[0] - '0'];
        }
        else
        {
            WeaponCost[0].SetActive(true);
            WeaponCostImage[0].sprite = num[c[0] - '0'];
            WeaponCost[1].SetActive(false);
        }

        WeaponCostImage[0].color = (showCost > WeaponCostData) ? Color.red : (showCost < WeaponCostData) ? Color.green : Color.white;
        WeaponCostImage[1].color = (showCost > WeaponCostData) ? Color.red : (showCost < WeaponCostData) ? Color.green : Color.white;
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
        string temp = "";
        for (int i = 0; i < MinionsCardNameData.Length; i++)
        {
            if (MinionsCardNameData[i].Equals('('))
                break;
            else
                temp += MinionsCardNameData[i];
        }
        MinionsCardName.text = temp;

        temp = "";
        for (int i = 0; i < SpellCardNameData.Length; i++)
        {
            if (SpellCardNameData[i].Equals('('))
                break;
            else
                temp += SpellCardNameData[i];
        }
        SpellCardName.text = temp;

        temp = "";
        for (int i = 0; i < WeaponCardNameData.Length; i++)
        {
            if (WeaponCardNameData[i].Equals('('))
                break;
            else
                temp += WeaponCardNameData[i];
        }
        WeaponCardName.text = temp;
    }
    #endregion

    #region[ShowExplain]
    void ShowExplain()
    {
        string temp = "";
        #region[하수인 설명]
        for (int i = 0; i < MinionsCardExplainData.Length; i++)
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
        #endregion

        #region[주문 설명]
        SpellCardExplain.text = "";
        string checkNum = "없음";
        temp = "";
        for (int i = 0; i < SpellCardExplainData.Length; i++)
        {
            if (checkNum.Equals("없음"))
            {
                if (SpellCardExplainData[i].Equals('$'))
                    checkNum = "";
                else
                {
                    if (i < SpellCardExplainData.Length - 1 && SpellCardExplainData[i].Equals('\\') && SpellCardExplainData[i + 1].Equals('n'))
                    {
                        temp += "\n";
                        i++;
                    }
                    else
                        temp += SpellCardExplainData[i];
                }
            }
            else
            {
                if (SpellCardExplainData[i].Equals('$'))
                {
                    int findValue = 0;
                    int.TryParse(checkNum, out findValue);
                    if (SpellManager.instance && viewSpellOffset)
                    {
                        findValue += SpellManager.instance.playerSpellPower;
                        if (SpellManager.instance.playerSpellPower == 0)
                            temp += findValue;
                        else if (SpellManager.instance.playerSpellPower > 0)
                            temp += "<color=#00FF00>" + findValue + "</color>";
                        else if (SpellManager.instance.playerSpellPower < 0)
                            temp += "<color=#FF0000>" + findValue + "</color>";
                    }
                    else
                    {
                        temp += findValue;
                    }
                    checkNum = "없음";
                }
                else
                    checkNum += SpellCardExplainData[i];
            }
        }
        SpellCardExplain.text = temp;

        #endregion

        #region[무기 설명]
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
        #endregion
    }
    #endregion

    #region[GetCardNum]
    public void GetCardNum()
    {
        if (cardType == CardType.하수인)
            cardN = DataMng.instance.playData.GetCardNum(MinionsCardNameData);
        else if (cardType == CardType.주문)
            cardN = DataMng.instance.playData.GetCardNum(SpellCardNameData);
        else if (cardType == CardType.무기)
            cardN = DataMng.instance.playData.GetCardNum(WeaponCardNameData);

        if (cardLevel.Equals("토큰"))
            cardN = 1;
    }
    #endregion

    #region[ShowHolo]
    void ShowHolo()
    {
        if (BattleUI.instance)
            return;

        if (cardType == CardType.하수인)
        {
            if (cardN <= 0 && MinionsCardImg.material == holoBlue)
                return;
            if (cardN > 0 && MinionsCardImg.material != holoBlue)
                return;

            Material tempM = (cardN <= 0) ? holoBlueMove : normalMoveMat;
            Material tempMa = (cardN <= 0) ? holoBlue : normalMat;

            for (int i = 0; i < MinionsCostImage.Length; i++)
                MinionsCostImage[i].material = tempM;
            for (int i = 0; i < MinionsAttackImage.Length; i++)
                MinionsAttackImage[i].material = tempM;
            for (int i = 0; i < MinionsHpImage.Length; i++)
                MinionsHpImage[i].material = tempM;
            MinionsCardLevel.material = tempM;
            MinionsCardImg.material = tempMa;
        }
        else if (cardType == CardType.주문)
        {
            if (cardN <= 0 && SpellCardImg.material == holoBlue)
                return;
            if (cardN > 0 && SpellCardImg.material != holoBlue)
                return;
            Material tempM = (cardN <= 0) ? holoBlueMove : normalMoveMat;
            Material tempMa = (cardN <= 0) ? holoBlue : normalMat;
            for (int i = 0; i < SpellCostImage.Length; i++)
                SpellCostImage[i].material = tempM;
            SpellCardLevel.material = tempM;
            SpellCardImg.material = tempMa;
        }
        else if (cardType == CardType.무기)
        {
            if (cardN <= 0 && WeaponCardImg.material == holoBlue)
                return;
            if (cardN > 0 && WeaponCardImg.material != holoBlue)
                return;

            Material tempM = (cardN <= 0) ? holoBlueMove : normalMoveMat;
            Material tempMa = (cardN <= 0) ? holoBlue : normalMat;
            for (int i = 0; i < WeaponCostImage.Length; i++)
                WeaponCostImage[i].material = tempM;
            for (int i = 0; i < WeaponAttackImage.Length; i++)
                WeaponAttackImage[i].material = tempM;
            for (int i = 0; i < WeaponHpImage.Length; i++)
                WeaponHpImage[i].material = tempM;
            WeaponCardLevel.material = tempM;
            WeaponCardImg.material = tempMa;

        }
    }
    #endregion
}
