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
    }

    public List<Deck> deck = new List<Deck>();

    public int magicPowder;
    public PlayData(bool b)
    {
        if (!b)
            return;
        if (DataMng.instance != null)
        {
            deck.Add(new Deck("도적",DataMng.TableType.도적, new List<string>() { "마음가짐" }));
            deck.Add(new Deck("도적", DataMng.TableType.도적, new List<string>() { "마음가짐" }));
            deck.Add(new Deck("도적", DataMng.TableType.도적, new List<string>() { "마음가짐" }));
            deck.Add(new Deck("도적", DataMng.TableType.도적, new List<string>() { "마음가짐" }));
            deck.Add(new Deck("도적", DataMng.TableType.도적, new List<string>() { "마음가짐" }));
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

    #region[카드갯수 얻기]
    public int GetCardNum(string s)
    {
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = "";
            string cardN = "";
            bool flag = false;
            for (int j = 0; j < hasCard[i].Length; j++)
            {
                if (hasCard[i][j] == '~')
                    flag = true;
                else
                {
                    if (flag)
                        cardN += hasCard[i][j];
                    else
                        cardName += hasCard[i][j];
                }
            }
            if (cardName.Equals(s))
            {
                int R = 0;
                int.TryParse(cardN, out R);
                return R;
            }
        }
        return 0;
    }
    #endregion

    #region[카드갯수 설정]
    public void SetCardNum(string s,int n)
    {
        for (int i = 0; i < hasCard.Count; i++)
        {
            string cardName = "";
            for (int j = 0; j < hasCard[i].Length; j++)
            {
                if (hasCard[i][j] == '~')
                    break;
                cardName += hasCard[i][j];
            }

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
