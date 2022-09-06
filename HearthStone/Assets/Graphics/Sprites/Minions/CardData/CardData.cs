
public class CardData
{
    public DataMng.TableType cardJob;
    public string cardLevel;
    public string cardName;
    public string cardType;
    public int cardCost;
    public int cardAttack;
    public int cardHp;
    public string cardExplain;

    public CardData
        (
        DataMng.TableType acardJob,
        string acardLevel,
        string acardName,
        string acardType,
        int acardCost,
        int acardAttack,
        int acardHp,
        string acardExplain
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
    }

    public static int CostCardPowder(string s)
    {
        if (s.Equals("일반"))
            return 40;
        else if (s.Equals("희귀"))
            return 100;
        else if (s.Equals("특급"))
            return 400;
        else if (s.Equals("전설"))
            return 1600;
        return 0;
    }

    public static int GetCardPowder(string s)
    {
        if (s.Equals("일반"))
            return 5;
        else if (s.Equals("희귀"))
            return 20;
        else if (s.Equals("특급"))
            return 100;
        else if (s.Equals("전설"))
            return 400;
        return 0;
    }
}