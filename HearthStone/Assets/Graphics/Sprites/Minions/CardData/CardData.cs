using UnityEngine;

public class CardData
{
    public const int MAX_CARD_NUM = 9;

    public DataMng.TableType cardJob;
    public string cardLevel;
    public string cardName;
    public string cardType;
    public int cardCost;
    public int cardAttack;
    public int cardHp;
    public string cardExplain;
    public int cardBuyPowder;
    public int cardSellPowder;

    public CardData
        (
        DataMng.TableType acardJob,
        string acardLevel,
        string acardName,
        string acardType,
        int acardCost,
        int acardAttack,
        int acardHp,
        string acardExplain,
        int acardBuyPowder,
        int acardSellPowder
        )
    {
        cardJob = acardJob;
        cardLevel = acardLevel;
        cardName = acardName;
        cardType = acardType;
        cardCost = acardCost;
        cardAttack = acardAttack;
        cardHp = acardHp;
        cardExplain = acardExplain;
        cardBuyPowder = acardBuyPowder;
        cardSellPowder = acardSellPowder;
    }

    public CardData()
    {
        cardName = "NULL";
    }

    public CardData(string name)
    {
        NewCardData(name);
    }

    public void NewCardData(string name)
    {
        DataMng dataMng = DataMng.instance;
        Vector2Int pair = dataMng.GetPairByName(name);
        LowBase table = dataMng.m_dic[(DataMng.TableType)pair.x];

        cardJob = (DataMng.TableType)pair.x;
        cardLevel = table.ToString(pair.y, "등급");
        cardName = table.ToString(pair.y, "카드이름");
        cardType = table.ToString(pair.y, "카드종류");
        cardCost = table.ToInteger(pair.y, "코스트");
        cardAttack = table.ToInteger(pair.y, "공격력");
        cardHp = table.ToInteger(pair.y, "체력");
        cardExplain = table.ToString(pair.y, "카드설명");
        cardBuyPowder = table.ToInteger(pair.y, "BuyPowder");
        cardSellPowder = table.ToInteger(pair.y, "SellPowder");
    }

}