using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroHpManager : MonoBehaviour
{
    [Header("플레리어 영웅 체력")]
    [Range(0, 30)]
    public int maxPlayerHp;
    [Range(0, 30)]
    public int nowPlayerHp;

    [Header("적 영웅 체력")]
    [Space(10)]
    [Range(0, 30)]
    public int maxEnemyHp;
    [Range(0, 30)]
    public int nowEnemyHp;

    [Header("------------------------------------------------")]
    [Space(30)]

    public Image[] playerHpNum;
    public Image[] enemyHpNum;
    public GameObject playerHp;
    public GameObject enemyHp;

    void Update()
    {
        nowPlayerHp = Mathf.Min(nowPlayerHp, maxPlayerHp);
        nowEnemyHp = Mathf.Min(nowEnemyHp, maxEnemyHp);

        if (!DataMng.instance)
        {
            playerHpNum[0].gameObject.SetActive(false);
            playerHpNum[1].gameObject.SetActive(false);
            playerHpNum[2].gameObject.SetActive(false);
            enemyHpNum[0].gameObject.SetActive(false);
            enemyHpNum[1].gameObject.SetActive(false);
            enemyHpNum[2].gameObject.SetActive(false);
            return;
        }

        if (nowPlayerHp < 10)
        {
            playerHpNum[1].gameObject.SetActive(false);
            playerHpNum[2].gameObject.SetActive(false);
            playerHpNum[0].gameObject.SetActive(true);
            playerHpNum[0].sprite = DataMng.instance.num[nowPlayerHp % 10];
        }
        else
        {
            playerHpNum[1].gameObject.SetActive(true);
            playerHpNum[2].gameObject.SetActive(true);
            playerHpNum[0].gameObject.SetActive(false);
            playerHpNum[1].sprite = DataMng.instance.num[nowPlayerHp / 10];
            playerHpNum[2].sprite = DataMng.instance.num[nowPlayerHp % 10];
        }

        if (nowEnemyHp < 10)
        {
            enemyHpNum[1].gameObject.SetActive(false);
            enemyHpNum[2].gameObject.SetActive(false);
            enemyHpNum[0].gameObject.SetActive(true);
            enemyHpNum[0].sprite = DataMng.instance.num[nowEnemyHp % 10];
        }
        else
        {
            enemyHpNum[1].gameObject.SetActive(true);
            enemyHpNum[2].gameObject.SetActive(true);
            enemyHpNum[0].gameObject.SetActive(false);
            enemyHpNum[1].sprite = DataMng.instance.num[nowEnemyHp / 10];
            enemyHpNum[2].sprite = DataMng.instance.num[nowEnemyHp % 10];
        }
    }
}
