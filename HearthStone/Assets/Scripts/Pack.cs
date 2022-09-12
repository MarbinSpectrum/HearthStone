using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardLevel { ����, Ư��, ���, �Ϲ� };

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

    #region[��޺� ī�� ����Ʈ]
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
            string cLevel = dataTable.ToString(j, "���");
            string cName = dataTable.ToString(j, "ī���̸�");
            float cPer = dataTable.ToFloat(j, "Ȯ��(%)");
            CardLevel cardLevel = (CardLevel)System.Enum.Parse(
                typeof(CardLevel), cLevel);
            temp[cardLevel].Add(new PackCard(cName, cPer));
        }

        return temp;
    }
    #endregion

    public Pack()
    {
        //��޺��� ī�带 �����ش�.
        Dictionary<CardLevel, List<PackCard>> cardData = LevelCardList();

        //�ּ� ������ ���ī�带 �Ѿȿ� �־��ش�.
        card.Add(GetPackCard(cardData[CardLevel.���]));

        for (int i = 0; i < MAX_PACK_CARD - 1; i++)
        {
            //MAX_PACK_CARD = 5
            //������ ī����� �־��ش�.
            card.Add(GetPackCard(cardData));
        }

        //�Ѿ��� ī����� �����ش�.
        for (int i = 0; i < 100; i++)
        {
            int a = Random.Range(0, 5);
            int b = Random.Range(0, 5);
            string temp = card[a];
            card[a] = card[b];
            card[b] = temp;
        }
    }

    #region[�ѿ� ���� ī�带 ����]
    private string GetPackCard(Dictionary<CardLevel, List<PackCard>> cards)
    {
        int maxPer = 0;
        int nowPer = 0;
        int cLevelSize = System.Enum.GetValues(typeof(CardLevel)).Length;
        for (int i = 0; i < cLevelSize; i++)
        {
            //��ü Ȯ������ �����ش�.
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
                    //Ȯ���� ���� ī�� ����
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