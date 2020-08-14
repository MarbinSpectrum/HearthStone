using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDeck : MonoBehaviour
{
    public static InGameDeck instance;
    public static int nowDeck;

    [HideInInspector] public List<string> playDeck = new List<string>();
    [HideInInspector] public List<string> AIDeck = new List<string>();

    public void Shuffle(List<string> list,int n)
    {
        for(int i = 0; i < n; i++)
        {
            int a = Random.Range(0, list.Count);
            int b = Random.Range(0, list.Count);
            string temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }
    }

    private void Awake()
    {
        instance = this;
        AIDeck.Add("급속 성장");
        AIDeck.Add("급속 성장");
        AIDeck.Add("정신 자극");
        AIDeck.Add("정신 자극");
        AIDeck.Add("야생의 징표");
        AIDeck.Add("야생의 징표");
        AIDeck.Add("천벌");
        AIDeck.Add("천벌");
        AIDeck.Add("자연화");
        AIDeck.Add("자연화");
        AIDeck.Add("휘둘러치기");
        AIDeck.Add("휘둘러치기");
        AIDeck.Add("발톱의 드루이드");
        AIDeck.Add("발톱의 드루이드");
        AIDeck.Add("치유의 손길");
        AIDeck.Add("치유의 손길");
        AIDeck.Add("세나리우스");
        AIDeck.Add("무쇠껍질 수호정령");
        AIDeck.Add("무쇠껍질 수호정령");
        AIDeck.Add("자연의 군대");
        AIDeck.Add("자연의 군대");
        AIDeck.Add("야생의 포효");
        AIDeck.Add("야생의 포효");
        AIDeck.Add("대지 고리회 선견자");
        AIDeck.Add("대지 고리회 선견자");
        AIDeck.Add("센진 방패대가");
        AIDeck.Add("센진 방패대가");
        AIDeck.Add("실바나스 윈드러너");
        AIDeck.Add("케른 블러드후프");
        AIDeck.Add("그룰");
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
        Shuffle(playDeck,1000);
        Shuffle(AIDeck, 1000);
    }
}
