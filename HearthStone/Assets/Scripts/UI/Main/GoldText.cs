using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldText : MonoBehaviour
{
    [SerializeField] private Text goldText;

    private void Update()
    {
        ShowGoldText();
    }

    private void ShowGoldText()
    {
        //데이터 매니저를 받아온다.
        DataMng dataMng = DataMng.instance;

        if (dataMng != null && goldText != null)
        {
            //플레이어 데이터를 받아온다.
            PlayData playData = dataMng.playData;

            //골드 수치를 표시해준다.
            int gold = playData.gold;
            goldText.text = gold.ToString();
        }
    }
}
