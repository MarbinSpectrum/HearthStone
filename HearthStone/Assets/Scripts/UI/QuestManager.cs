using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quest
{
    때려눕히기, //상대영웅에게 총 30피해 입히기 // 50골드
    도적_또는_드루이드의_달인, //도적 또는 드루이드로 3승 // 60골드
    도적_또는_드루이드로_승리,  //도적 또는 드루이드로 2승 // 50골드
    도적_전문가, //도적 카드 30장 내기 // 60골드
    드루이드_전문가,  //드루이드 카드 30장 내기 // 60골드
    주문술사,//주문카드 25장 내기 //50골드
    영웅의격려,//영웅능력 20번 사용하기 // 50골드
    약자의반격,//2마나 이하 하수인 20장 내기 //50골드
    초토화,//하수인 25장 파괴하기 //50골드
}

public enum Job
{
    도적,드루이드
}
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    #region[Awake]
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    /// <summary> 상대영웅에게 n만큼 데미지.</summary>
    public void HeroDamage(int n)
    {
        for(int i = 0; i < DataMng.instance.playData.quests.Count; i++)
            if ((Quest)DataMng.instance.playData.quests[i].questNum == Quest.때려눕히기)
                DataMng.instance.playData.quests[i].value += n;
    }


    /// <summary> 특정직업으로 승리.</summary>
    public void CharacterWin(Job job)
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
        {
            if ((job == Job.도적 || job == Job.드루이드) && (Quest)DataMng.instance.playData.quests[i].questNum == Quest.도적_또는_드루이드의_달인)
                DataMng.instance.playData.quests[i].value++;
            else if ((job == Job.도적 || job == Job.드루이드) && (Quest)DataMng.instance.playData.quests[i].questNum == Quest.도적_또는_드루이드로_승리)
                DataMng.instance.playData.quests[i].value++;
        }
    }

    /// <summary> 특정직업카드 사용.</summary>
    public void CharacterCard(Job job)
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
        {
            if (job == Job.도적 && (Quest)DataMng.instance.playData.quests[i].questNum == Quest.도적_전문가)
                DataMng.instance.playData.quests[i].value++;
            else if (job == Job.드루이드 && (Quest)DataMng.instance.playData.quests[i].questNum == Quest.드루이드_전문가)
                DataMng.instance.playData.quests[i].value++;
        }
    }

    /// <summary> 주문카드 사용.</summary>
    public void SpellCard()
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
            if ((Quest)DataMng.instance.playData.quests[i].questNum == Quest.주문술사)
                DataMng.instance.playData.quests[i].value++;
    }


    /// <summary> 2이하의 하수인 사용.</summary>
    public void BumpOfChicken()
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
            if ((Quest)DataMng.instance.playData.quests[i].questNum == Quest.약자의반격)
                DataMng.instance.playData.quests[i].value++;
    }

    /// <summary> 하수인파괴.</summary>
    public void DestroyMinion()
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
            if ((Quest)DataMng.instance.playData.quests[i].questNum == Quest.초토화)
                DataMng.instance.playData.quests[i].value++;
    }

    /// <summary> 영웅능력사용.</summary>
    public void HeroAbility()
    {
        for (int i = 0; i < DataMng.instance.playData.quests.Count; i++)
            if ((Quest)DataMng.instance.playData.quests[i].questNum == Quest.영웅의격려)
                DataMng.instance.playData.quests[i].value++;
    }
}
