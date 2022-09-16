using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayData
{
    public List<string> hasCard = new List<string>();
    public List<Deck> deck = new List<Deck>();
    public List<Pack> packs = new List<Pack>();
    public List<Quest> quests = new List<Quest>();
    public int magicPowder;
    public int gold;
    public PlayData()
    {
        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j <= dataMng.m_dic[(DataMng.TableType)i].m_table.Count; j++)
            {
                if (dataMng.ToString(i, j, "등급").Equals("기본") ||
                   dataMng.ToString(i, j, "등급").Equals("일반"))
                {
                    string cardName = dataMng.ToString(i, j, "카드이름");
                    SetCardNum(cardName, 2);
                }
            }
        }

        magicPowder = 80000;
        gold = 100000;
        for (int i = 0; i < 3; i++)
            AddQuest();
    }

    #region[퀘스트 추가]
    public void AddQuest()
    {
        if (quests.Count >= 3)
            return;
        int c = 100;
        int r = 0;
        while (c > 0)
        {
            c--;
            r = Random.Range(0, 6);
            bool check = false;
            for (int j = 0; j < quests.Count; j++)
            {
                if (quests[j].questNum == r)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                quests.Add(new Quest(r));
                break;
            }
        }
    }
    #endregion

    #region[카드갯수 얻기]
    public int GetCardNum(string s)
    {
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = DataParse.GetCardName(hasCard[i]);
            int cardN = DataParse.GetCardNumber(hasCard[i]);
            if (cardName.Equals(s))
                return cardN;
        }
        return 0;
    }
    #endregion

    #region[카드갯수 설정]
    public void SetCardNum(string s,int n)
    {
        string data = DataParse.GetParseData(s, n);
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = DataParse.GetCardName(hasCard[i]);

            if (cardName.Equals(s))
            {
                hasCard[i] = data;
                return;
            }
        }
        hasCard.Add(data);
    }
    #endregion

    #region[카드추가]
    public void AddCard(string s, int n)
    {
        int cardNum = GetCardNum(s);
        SetCardNum(s, cardNum + n);
    }
    public void AddCard(string s)
    {
        AddCard(s, 1);
    }
    #endregion

    #region[카드제거]
    public void RemoveCard(string s, int n)
    {
        int cardNum = GetCardNum(s);
        if(cardNum >= n)
            SetCardNum(s, cardNum - n);
    }

    public void RemoveCard(string s)
    {
        RemoveCard(s, 1);
    }
    #endregion
}
