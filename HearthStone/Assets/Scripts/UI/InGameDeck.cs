using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDeck : MonoBehaviour
{
    public static InGameDeck instance;
    public static int nowDeck;

    [HideInInspector] public List<string> playDeck = new List<string>();

    public void Shuffle(int n)
    {
        for(int i = 0; i < n; i++)
        {
            int a = Random.Range(0, playDeck.Count);
            int b = Random.Range(0, playDeck.Count);
            string temp = playDeck[a];
            playDeck[a] = playDeck[b];
            playDeck[b] = temp;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for(int i = 0; i < DataMng.instance.playData.deck[nowDeck].card.Count; i++)
        {
            string name = DataMng.instance.playData.GetCardName(DataMng.instance.playData.deck[nowDeck].card[i]);
            int num = DataMng.instance.playData.GetCardNumber(DataMng.instance.playData.deck[nowDeck].card[i]);
            for (int j = 0; j < num; j++)
                playDeck.Add(name);
        }
        Shuffle(1000);



    }
}
