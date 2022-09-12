using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardLevel { 전설, 특급, 희귀, 일반 };

[System.Serializable]
public class Pack
{
    const int MAX_PACK_CARD = 5;

    struct PackCard
    {
        public string card;
        public float percent;
        public PackCard(string Card,float Percent)
        {
            card = Card;
            percent = Percent;
        }
    }

    [SerializeField] public List<string> card = new List<string>();

    #region[등급별 카드 리스트]
    Dictionary<CardLevel, List<PackCard>> LevelCardList()
    {
        Dictionary<CardLevel, List<PackCard>> temp = new Dictionary<CardLevel, List<PackCard>>();

        int cLevelSize = System.Enum.GetValues(typeof(CardLevel)).Length;
        for (int i = 0; i < cLevelSize; i++)
            temp.Add((CardLevel)i, new List<PackCard>());

        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
            return temp;

        LowBase dataTable = dataMng.cardPer;
        for (int j = 1; j <= dataTable.m_table.Count; j++)
        {
            string cLevel = dataTable.ToString(j, "등급");
            string cName = dataTable.ToString(j, "카드이름");
            float cPer = dataTable.ToFloat(j, "확률(%)");
            CardLevel cardLevel = (CardLevel)System.Enum.Parse(
                typeof(CardLevel), cLevel);
            temp[cardLevel].Add(new PackCard(cName, cPer));
        }

        return temp;
    }
    #endregion

    public Pack()
    {
        //등급별로 카드를 나눠준다.
        Dictionary<CardLevel, List<PackCard>> cardData = LevelCardList();

        //최소 한장의 희귀카드를 팩안에 넣어준다.
        card.Add(GetPackCard(cardData[CardLevel.희귀]));

        for (int i = 0; i < MAX_PACK_CARD - 1; i++)
        {
            //MAX_PACK_CARD = 5
            //나머지 카드들을 넣어준다.
            card.Add(GetPackCard(cardData));
        }

        //팩안의 카드들을 섞어준다.
        for (int i = 0; i < 100; i++)
        {
            int a = Random.Range(0, 5);
            int b = Random.Range(0, 5);
            string temp = card[a];
            card[a] = card[b];
            card[b] = temp;
        }
    }

    #region[팩에 넣을 카드를 선정]
    private string GetPackCard(Dictionary<CardLevel, List<PackCard>> cards)
    {
        int maxPer = 0;
        int nowPer = 0;
        int cLevelSize = System.Enum.GetValues(typeof(CardLevel)).Length;
        for (int i = 0; i < cLevelSize; i++)
        {
            //전체 확률값을 구해준다.
            cards[(CardLevel)i].ForEach((x) => { maxPer += (int)(x.percent * 1000); });
        }

        int r = Random.Range(0, maxPer);
        for (int i = 0; i < cLevelSize; i++)
        {
            foreach (PackCard card in cards[(CardLevel)i])
            {
                nowPer += (int)(card.percent * 1000);
                if (nowPer > r)
                {
                    //확률에 따른 카드 선정
                    return card.card;
                }
            }
        }
        return null;
    }

    private string GetPackCard(List<PackCard> cards)
    {
        int maxPer = 0;
        int nowPer = 0;
        cards.ForEach((x) => { maxPer += (int)(x.percent * 1000); });
        int r = Random.Range(0, maxPer);
        foreach (PackCard card in cards)
        {
            nowPer += (int)(card.percent * 1000);
            if (nowPer > r)
            {
                return card.card;
            }
        }
        return null;
    }
    #endregion
}