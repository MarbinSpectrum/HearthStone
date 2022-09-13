using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayData
{
    public  List<string> hasCard = new List<string>();
    [System.Serializable]
    public class Deck
    {
        public const int MAX_DECK_CARD = 30;
        public const int MAX_DECK_NUM = 9;

        [SerializeField] public string name;
        [SerializeField] public List<string> card;
        [SerializeField] public DataMng.TableType job;

        public Deck(string name, DataMng.TableType job, List<string> card)
        {
            this.name = name;
            this.card = card;
            this.job = job;
        }

        public int HasCardNum(string s)
        {
            for (int i = 0; i < card.Count; i++)
            {
                string name = DataMng.instance.playData.GetCardName(card[i]);
                int num = DataMng.instance.playData.GetCardNumber(card[i]);
                if (name.Equals(s))
                    return num;
            }
            return 0;
        }

        public void AddCard(string s)
        {
            if (CountCardNum() >= 30)
                return;

            for(int i = 0; i < card.Count; i++)
            {
                string name = DataMng.instance.playData.GetCardName(card[i]);
                int num = DataMng.instance.playData.GetCardNumber(card[i]);
                Vector2 pair = DataMng.instance.GetPairByName(name);
                string level = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToString((int)pair.y, "등급");
                int maxNum = level.Equals("전설") ? 1 : 2;
                if (name.Equals(s))
                {
                    if (num >= maxNum)
                        return;
                    card[i] = s + "~" + (num + 1).ToString();
                    return;
                }
            }
            card.Add(s + "~1");
        }

        public void PopCard(string s)
        {
            for (int i = 0; i < card.Count; i++)
            {
                string name = DataMng.instance.playData.GetCardName(card[i]);
                int num = DataMng.instance.playData.GetCardNumber(card[i]);
                if (name.Equals(s))
                {
                    if (num >= 2)
                        card[i] = s + "~" + (num - 1).ToString();
                    else
                        card.RemoveAt(i);
                    return;
                }
            }

        }

        public int CountCardNum()
        {
            int c = 0;

            for (int i = 0; i < card.Count; i++)
            {
                int num = DataMng.instance.playData.GetCardNumber(card[i]);
                c += num;
            }

            return c;
        }

        public bool IsEffective()
        {
            if(CountCardNum() != 30)
            {
                return false;
            }
            return true;
        }
    }

     public List<Deck> deck = new List<Deck>();

    public List<Pack> packs = new List<Pack>();

    [System.Serializable]
    public class Quest
    {
        public int questNum;
        public int value;
        public Quest()
        {
            questNum = Random.Range(0, 6);
            value = 0;
        }

        public Quest(int n)
        {
            questNum = n;
            value = 0;
        }
    }

    public List<Quest> quests = new List<Quest>();

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

    public int magicPowder;
    public int gold;
    public PlayData(bool b)
    {
        if (!b)
            return;
        if (DataMng.instance != null)
        {
            //deck.Add(new Deck("도적",DataMng.TableType.도적, new List<string>() { "마음가짐~2" }));
            //deck.Add(new Deck("드루이드", DataMng.TableType.드루이드, new List<string>() { "세나리우스~2" }));
            for (int i = 0; i < 3; i++)
                for (int j = 1; j <= DataMng.instance.m_dic[(DataMng.TableType)i].m_table.Count; j++)
                {
                    string cardName = DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드이름");
                    cardName += "~0";
                    hasCard.Add(cardName);
                }

            for (int i = 0; i < 3; i++)
                for (int j = 1; j <= DataMng.instance.m_dic[(DataMng.TableType)i].m_table.Count; j++)
                {
                    if (DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "등급").Equals("기본") ||
                       DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "등급").Equals("일반"))
                    {
                        string cardName = DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드이름");
                        SetCardNum(cardName, 2);
                    }
                }

            magicPowder = 80000;
            gold = 100000;
            for (int i = 0; i < 3; i++)
                AddQuest();
        }
    }

    #region[카드이름 얻기]
    public string GetCardName(string s)
    {
        string cardName = "";
        for (int i = 0; i < s.Length; i++)
        {

            if (s[i] == '~')
                break;
            else
                cardName += s[i];
        }
        return cardName;
    }
    #endregion

    #region[카드갯수 얻기]
    public int GetCardNumber(string s)
    {
        string cardN = "";
        bool flag = false;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '~')
                flag = true;
            else if(flag)
                cardN += s[i];
        }
        try
        {
            int R = 0;
            int.TryParse(cardN, out R);
            return R;
        }
        catch { return 0; }
    }
    #endregion

    #region[카드갯수 얻기]
    public int GetCardNum(string s)
    {
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = GetCardName(hasCard[i]);
            int cardN = GetCardNumber(hasCard[i]);
            if (cardName.Equals(s))
                return cardN;
        }
        return 0;
    }
    #endregion

    #region[카드갯수 설정]
    public void SetCardNum(string s,int n)
    {
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = GetCardName(hasCard[i]);

            if (cardName.Equals(s))
            {
                cardName += "~" + n.ToString();
                hasCard[i] = cardName;
                return;
            }
        }
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
