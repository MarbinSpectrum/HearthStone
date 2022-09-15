using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public const int MAX_DECK_CARD = 30;
    public const int MAX_DECK_NUM = 9;

    [SerializeField] public string name;
    [SerializeField] public List<string> card;
    [SerializeField] public Job job;

    public Deck(string name, Job job, List<string> card)
    {
        this.name = name;
        this.card = card;
        this.job = job;
    }

    public Deck(string name, Job job, string textData)
    {
        this.name = name;
        this.job = job;
        this.card = new List<string>();

        textData = textData.Replace("\r", string.Empty);
        string[] cards = textData.Split('\n');
        for (int i = 0; i < cards.Length; i++)
            card.Add(cards[i]);
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

        for (int i = 0; i < card.Count; i++)
        {
            string name = DataMng.instance.playData.GetCardName(card[i]);
            int num = DataMng.instance.playData.GetCardNumber(card[i]);
            Vector2Int pair = DataMng.instance.GetPairByName(name);
            string level = DataMng.instance.ToString(pair.x, pair.y, "등급");
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
        if (CountCardNum() != 30)
        {
            return false;
        }
        return true;
    }

    public static void Shuffle(List<string> list, int n)
    {
        for (int i = 0; i < n; i++)
        {
            //n번 카드를 교환한다.
            int a = Random.Range(0, list.Count);
            int b = Random.Range(0, list.Count);
            
            //a번위치와 b번위치 카드를 교환한다.
            string temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }

    public List<string> GetInGameDeck()
    {
        List<string> deck = new List<string>();
        foreach (string data in card)
        {
            string[] dataPair = data.Split('~');
            string name = dataPair[0];
            int num = int.Parse(dataPair[1]);
            for (int i = 0; i < num; i++)
                deck.Add(name);
        }
        return deck;
    }
}