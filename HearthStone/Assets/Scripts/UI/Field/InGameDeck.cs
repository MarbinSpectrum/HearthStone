using System.Collections.Generic;
using UnityEngine;

public class InGameDeck : MonoBehaviour
{
    public static InGameDeck instance;

    private List<string> playDeck = new List<string>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        BattleMenu battleMenu = BattleMenu.instance;

        //BattleMenu에서 선택한 덱을 가져온다.
        Deck deck = battleMenu.nowDeck;

        //플레이어 덱 준비
        playDeck = deck.GetInGameDeck();

        //덱셔플
        Shuffle(1000);
    }

    public void Shuffle(int n)
    {
        //덱셔플
        Deck.Shuffle(playDeck, n);
    }

    public void PopTopCard()
    {
        playDeck.RemoveAt(0);
    }

    public void PushCard(string name)
    {
        playDeck.Add(name);
    }

    public int GetDeckCardNum()
    {
        return playDeck.Count;
    }

    public string GetTopCard()
    {
        return playDeck[0];
    }
}
