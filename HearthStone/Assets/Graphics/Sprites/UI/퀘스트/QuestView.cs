using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestView : MonoBehaviour
{
    [SerializeField] private int questNum;
    [SerializeField] private Image questWindow;
    [SerializeField] private Text questName;
    [SerializeField] private Text questValue;
    [SerializeField] private Text questExplain;
    [SerializeField] private Text questProgress;

    private void Update()
    {
        QuestUpdate();
    }

    private void QuestUpdate()
    {
        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
        {
            return;
        }

        PlayData playData = dataMng.playData;
        if (playData == null)
        {
            return;
        }

        List<Quest> quests = playData.quests;
        bool isAct = (quests.Count > questNum);

        questWindow.gameObject.SetActive(isAct);
        questName.gameObject.SetActive(isAct);
        questValue.gameObject.SetActive(isAct);
        questExplain.gameObject.SetActive(isAct);
        questProgress.gameObject.SetActive(isAct);

        if (isAct == false)
        {
            //활성 비활성
            return;
        }

        //플레이어의 퀘스트를 받아오고

        QuestManager questManager = QuestManager.instance;
        LowBase questData = questManager.questData;
        //퀘스트매니저에 등록되어있는
        //퀘스트정보를 토대로 현재퀘스트가 어떤 것인지 표시한다.
        
        int qIdx = quests[questNum].questNum + 1;
        string qName = questData.ToString(qIdx, "QuestName");
        string qExplain = questData.ToString(qIdx, "QuestExplain");
        int qValue = questData.ToInteger(qIdx, "QuestValue");
        int qReward = questData.ToInteger(qIdx, "QuestReward");

        questName.text = qName;
        questValue.text = qReward.ToString();
        questExplain.text = qExplain;
        questProgress.text = quests[questNum].value + "/" + qValue.ToString();
    }
}
