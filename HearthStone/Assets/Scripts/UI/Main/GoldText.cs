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
        //������ �Ŵ����� �޾ƿ´�.
        DataMng dataMng = DataMng.instance;

        if (dataMng != null && goldText != null)
        {
            //�÷��̾� �����͸� �޾ƿ´�.
            PlayData playData = dataMng.playData;

            //��� ��ġ�� ǥ�����ش�.
            int gold = playData.gold;
            goldText.text = gold.ToString();
        }
    }
}
