using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestClear : MonoBehaviour
{
    public static QuestClear instance;

    public Animator animator;
    public GameObject Back;
    public GameObject window;
    public Text questName;
    public Text questValue;
    public Text questExplain;
    public List<int> clearQuest = new List<int>();

    void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if(clearQuest.Count > 0)
        {
            Back.SetActive(true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("QuestClearStop"))
            {
                animator.SetBool("Open", true);

                QuestType quest = (QuestType)clearQuest[0];

                if (quest == QuestType.때려눕히기)
                {
                    questName.text = "때려눕히기";
                    questValue.text = "50";
                    questExplain.text = "상대영웅에게 총 30피해 입히기";
                }
                else if (quest == QuestType.도적_또는_드루이드의_달인)
                {
                    questName.text = "도적 또는 드루이드의 달인";
                    questValue.text = "60";
                    questExplain.text = "도적 또는 드루이드로 3승";
                }
                else if (quest == QuestType.도적_또는_드루이드로_승리)
                {
                    questName.text = "도적 또는 드루이드로 승리";
                    questValue.text = "50";
                    questExplain.text = "도적 또는 드루이드로 2승";
                }
                else if (quest == QuestType.도적_전문가)
                {
                    questName.text = "도적 전문가";
                    questValue.text = "60";
                    questExplain.text = "도적 카드 30장 내기";
                }
                else if (quest == QuestType.드루이드_전문가)
                {
                    questName.text = "드루이드 전문가";
                    questValue.text = "60";
                    questExplain.text = "드루이드 카드 30장 내기";
                }
                else if (quest == QuestType.주문술사)
                {
                    questName.text = "주문술사";
                    questValue.text = "50";
                    questExplain.text = "주문카드 25장 내기";
                }
                else if (quest == QuestType.약자의반격)
                {
                    questName.text = "약자의반격";
                    questValue.text = "50";
                    questExplain.text = "2마나 이하 하수인 20장 내기";
                }
                else if (quest == QuestType.초토화)
                {
                    questName.text = "초토화";
                    questValue.text = "50";
                    questExplain.text = "하수인 25장 파괴하기";
                }
                else if (quest == QuestType.영웅의격려)
                {
                    questName.text = "영웅의격려";
                    questValue.text = "50";
                    questExplain.text = "영웅능력 20번 사용하기";
                }
            }
        }
        else
        {
            Back.SetActive(false);
        }
    }

    public void QuesetClear()
    {
        for(int i = 0; i < 3; i++)
            for(int j = 0; j < DataMng.instance.playData.quests.Count; j++)
            {
                if (DataMng.instance.playData.quests[j].questNum == 0 && DataMng.instance.playData.quests[j].value >= 30)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 50;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 1 && DataMng.instance.playData.quests[j].value >= 3)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 60;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 2 && DataMng.instance.playData.quests[j].value >= 2)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 50;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 3 && DataMng.instance.playData.quests[j].value >= 30)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 60;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 4 && DataMng.instance.playData.quests[j].value >= 30)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 60;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 5 && DataMng.instance.playData.quests[j].value >= 25)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 50;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 6 && DataMng.instance.playData.quests[j].value >= 20)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 60;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 7 && DataMng.instance.playData.quests[j].value >= 20)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 50;
                    break;
                }
                else if (DataMng.instance.playData.quests[j].questNum == 8 && DataMng.instance.playData.quests[j].value >= 25)
                {
                    clearQuest.Add(DataMng.instance.playData.quests[j].questNum);
                    DataMng.instance.playData.quests.RemoveAt(j);
                    DataMng.instance.playData.gold += 50;
                    break;
                }
            }

        if (DataMng.instance.playData.quests.Count < 3)
            SoundManager.instance.PlaySE("퀘스트클리어");

        while (DataMng.instance.playData.quests.Count < 3)
        {
            DataMng.instance.playData.AddQuest();
        }
    }
}
