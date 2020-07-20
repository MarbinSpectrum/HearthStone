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
    int flagPlayerHp;

    [Header("적 영웅 체력")]
    [Space(10)]
    [Range(0, 30)]
    public int maxEnemyHp;
    [Range(0, 30)]
    public int nowEnemyHp;
    int flagEnemyHp;

    [Header("------------------------------------------------")]
    [Space(30)]

    public SpriteRenderer[] playerHpNum;
    public Animator playerHpAin;
    public SpriteRenderer[] enemyHpNum;
    public Animator enemyHpAin;
    public GameObject playerHp;
    public GameObject enemyHp;
    public DamageNum playerHeroDamage;
    public DamageNum enemyHeroDamage;

    void Update()
    {
        playerHeroDamage.hpSystem = "아군_영웅";
        enemyHeroDamage.hpSystem = "적_영웅";

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

        if(flagEnemyHp != nowEnemyHp)
        {
            flagEnemyHp = nowEnemyHp;
            enemyHpAin.SetTrigger("Change");
        }

        if (flagPlayerHp != nowPlayerHp)
        {
            flagPlayerHp = nowPlayerHp;
            playerHpAin.SetTrigger("Change");
        }

        #region[플레이어 영웅 체력 표시]
        int tempnowPlayerHp = Mathf.Abs(nowPlayerHp);

        if (nowPlayerHp < 0)
        {
            if (tempnowPlayerHp < 10)
            {
                playerHpNum[3].gameObject.SetActive(true);
                playerHpNum[4].gameObject.SetActive(false);
            }
            else
            {
                playerHpNum[3].gameObject.SetActive(false);
                playerHpNum[4].gameObject.SetActive(true);
            }
        }
        else
        {
            playerHpNum[3].gameObject.SetActive(false);
            playerHpNum[4].gameObject.SetActive(false);
        }

        if (tempnowPlayerHp < 10)
        {
            playerHpNum[1].gameObject.SetActive(false);
            playerHpNum[2].gameObject.SetActive(false);
            playerHpNum[0].gameObject.SetActive(true);
            playerHpNum[0].sprite = DataMng.instance.num[tempnowPlayerHp % 10];
        }
        else
        {
            playerHpNum[1].gameObject.SetActive(true);
            playerHpNum[2].gameObject.SetActive(true);
            playerHpNum[0].gameObject.SetActive(false);
            playerHpNum[1].sprite = DataMng.instance.num[tempnowPlayerHp / 10];
            playerHpNum[2].sprite = DataMng.instance.num[tempnowPlayerHp % 10];
        }

        for (int i = 0; i < 3; i++)
            if (nowPlayerHp < maxPlayerHp)
                playerHpNum[i].color = Color.red;
            else
                playerHpNum[i].color = Color.white;
        #endregion

        #region[적 영웅 체력 표시]
        int tempnowEnemyHp = Mathf.Abs(nowEnemyHp);

        if (nowEnemyHp < 0)
        {
            if (tempnowEnemyHp < 10)
            {
                enemyHpNum[3].gameObject.SetActive(true);
                enemyHpNum[4].gameObject.SetActive(false);
            }
            else
            {
                enemyHpNum[3].gameObject.SetActive(false);
                enemyHpNum[4].gameObject.SetActive(true);
            }
        }
        else
        {
            enemyHpNum[3].gameObject.SetActive(false);
            enemyHpNum[4].gameObject.SetActive(false);
        }

        if (tempnowEnemyHp < 10)
        {
            enemyHpNum[1].gameObject.SetActive(false);
            enemyHpNum[2].gameObject.SetActive(false);
            enemyHpNum[0].gameObject.SetActive(true);
            enemyHpNum[0].sprite = DataMng.instance.num[tempnowEnemyHp % 10];
        }
        else
        {
            enemyHpNum[1].gameObject.SetActive(true);
            enemyHpNum[2].gameObject.SetActive(true);
            enemyHpNum[0].gameObject.SetActive(false);
            enemyHpNum[1].sprite = DataMng.instance.num[tempnowEnemyHp / 10];
            enemyHpNum[2].sprite = DataMng.instance.num[tempnowEnemyHp % 10];
        }

        for (int i = 0; i < 3; i++)
            if (nowEnemyHp < maxEnemyHp)
                enemyHpNum[i].color = Color.red;
            else
                enemyHpNum[i].color = Color.white;
        #endregion
    }
}
