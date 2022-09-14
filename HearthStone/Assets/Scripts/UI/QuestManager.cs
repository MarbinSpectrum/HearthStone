using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
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
    public LowBase questData = new LowBase();

    private bool DataLoadSuccess;
    public bool dataLoadSuccess
    {
        get { return DataLoadSuccess; }
    }

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

    #region[데이터 로드]
    public void StartLoadData()
    {
        if (dataLoadSuccess)
        {
            //이미 데이터 로드가 끝났다.
            return;
        }
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        //사운드 데이터 로드
        yield return new WaitUntil(() => LoadQuestData());

        DataLoadSuccess = true;
    }

    private bool LoadQuestData()
    {
        questData.Load("Table/퀘스트데이터");
        return true;
    }
    #endregion

    /// <summary> 상대영웅에게 n만큼 데미지.</summary>
    public void HeroDamage(int n)
    {
        //영웅에게 데미지를 주었다면
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        List<Quest> quest = playData.quests;
        for (int i = 0; i < quest.Count; i++)
            if ((QuestType)quest[i].questNum == QuestType.때려눕히기)
            {
                //때려눕히기 퀘스트들의 진행상태를 변경해준다.
                quest[i].value += n;
            }
    }


    /// <summary> 특정직업으로 승리.</summary>
    public void CharacterWin(Job job)
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
        {
            if ((job == Job.도적 || job == Job.드루이드) && 
                (QuestType)playData.quests[i].questNum == QuestType.도적_또는_드루이드의_달인)
                playData.quests[i].value++;
            else if ((job == Job.도적 || job == Job.드루이드) &&
                (QuestType)playData.quests[i].questNum == QuestType.도적_또는_드루이드로_승리)
                playData.quests[i].value++;
        }
    }

    /// <summary> 특정직업카드 사용.</summary>
    public void CharacterCard(Job job)
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
        {
            if (job == Job.도적 && (QuestType)playData.quests[i].questNum == QuestType.도적_전문가)
                playData.quests[i].value++;
            else if (job == Job.드루이드 && (QuestType)playData.quests[i].questNum == QuestType.드루이드_전문가)
                playData.quests[i].value++;
        }
    }

    /// <summary> 주문카드 사용.</summary>
    public void SpellCard()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
            if ((QuestType)playData.quests[i].questNum == QuestType.주문술사)
                playData.quests[i].value++;
    }


    /// <summary> 2이하의 하수인 사용.</summary>
    public void BumpOfChicken()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
            if ((QuestType)playData.quests[i].questNum == QuestType.약자의반격)
                playData.quests[i].value++;
    }

    /// <summary> 하수인파괴.</summary>
    public void DestroyMinion()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
            if ((QuestType)playData.quests[i].questNum == QuestType.초토화)
                playData.quests[i].value++;
    }

    /// <summary> 영웅능력사용.</summary>
    public void HeroAbility()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        for (int i = 0; i < playData.quests.Count; i++)
            if ((QuestType)playData.quests[i].questNum == QuestType.영웅의격려)
                playData.quests[i].value++;
    }
}
