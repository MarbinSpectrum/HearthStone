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
        [SerializeField] public string name;
        [SerializeField] public List<string> card;
        [SerializeField] public DataMng.TableType job;

        public Deck(string name, DataMng.TableType job, List<string> card)
        {
            this.name = name;
            this.card = card;
            this.job = job;
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
    }

    public List<Deck> deck = new List<Deck>();

    public int magicPowder;
    public PlayData(bool b)
    {
        if (!b)
            return;
        if (DataMng.instance != null)
        {
            deck.Add(new Deck("도적",DataMng.TableType.도적, new List<string>() { "마음가짐~2" }));
            deck.Add(new Deck("드루이드", DataMng.TableType.드루이드, new List<string>() { "세나리우스~2" }));
            Debug.Log("NewData");
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

    public void Print()
    {
        string str = JsonUtility.ToJson(new DataMng.Serialization<Deck>(deck));
        Debug.Log(str);
        //for (int i = 0; i < hasCard.Count; i++)
        //        Debug.Log(hasCard[i]);
    }
}
