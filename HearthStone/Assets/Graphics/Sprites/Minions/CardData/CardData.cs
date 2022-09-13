
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


}