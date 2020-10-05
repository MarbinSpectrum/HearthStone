using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour
{
    public Quest quest;

    public int questNum;
    public Image questWindow;
    public Text questName;
    public Text questValue;
    public Text questExplain;
    public Text questProgress;

    void Update()
    {
        if(!DataMng.instance || DataMng.instance.playData.quests.Count <= questNum)
        {
            questWindow.gameObject.SetActive(false);
            questName.gameObject.SetActive(false);
            questValue.gameObject.SetActive(false);
            questExplain.gameObject.SetActive(false);
            questProgress.gameObject.SetActive(false);
            return;
        }

        questWindow.gameObject.SetActive(true);
        questName.gameObject.SetActive(true);
        questValue.gameObject.SetActive(true);
        questExplain.gameObject.SetActive(true);
        questProgress.gameObject.SetActive(true);

        if (DataMng.instance.playData.quests[questNum].questNum != (int)quest)
            quest = (Quest)DataMng.instance.playData.quests[questNum].questNum;

        if (quest == Quest.때려눕히기)
        {
            questName.text = "때려눕히기";
            questValue.text = "50";
            questExplain.text = "상대영웅에게 총\n30피해 입히기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/30";
        }
        else if (quest == Quest.도적_또는_드루이드의_달인)
        {
            questName.text = "도적 또는 드루이드의 달인";
            questValue.text = "60";
            questExplain.text = "도적 또는\n 드루이드로 3승";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/3";
        }
        else if (quest == Quest.도적_또는_드루이드로_승리)
        {
            questName.text = "도적 또는 드루이드로 승리";
            questValue.text = "50";
            questExplain.text = "도적 또는\n 드루이드로 2승";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/2";
        }
        else if (quest == Quest.도적_전문가)
        {
            questName.text = "도적 전문가";
            questValue.text = "60";
            questExplain.text = "도적 카드 30장 내기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/30";
        }
        else if (quest == Quest.드루이드_전문가)
        {
            questName.text = "드루이드 전문가";
            questValue.text = "60";
            questExplain.text = "드루이드 카드 30장 내기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/30";
        }
        else if (quest == Quest.주문술사)
        {
            questName.text = "주문술사";
            questValue.text = "50";
            questExplain.text = "주문카드 25장 내기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/25";
        }
        else if (quest == Quest.약자의반격)
        {
            questName.text = "약자의반격";
            questValue.text = "50";
            questExplain.text = "2마나 이하 하수인\n 20장 내기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/20";
        }
        else if (quest == Quest.초토화)
        {
            questName.text = "초토화";
            questValue.text = "50";
            questExplain.text = "하수인 25장 파괴하기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/25";
        }
        else if (quest == Quest.영웅의격려)
        {
            questName.text = "영웅의격려";
            questValue.text = "50";
            questExplain.text = "영웅능력 20번 사용하기";
            questProgress.text = DataMng.instance.playData.quests[questNum].value + "/20";
        }
    }
}
