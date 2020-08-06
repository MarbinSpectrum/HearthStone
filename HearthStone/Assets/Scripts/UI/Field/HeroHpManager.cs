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

    [Header("플레이어 영웅 방어도")]
    public int playerShield;
    int flagPlayerShield;

    [Header("적 영웅 체력")]
    [Space(10)]
    [Range(0, 30)]
    public int maxEnemyHp;
    [Range(0, 30)]
    public int nowEnemyHp;
    int flagEnemyHp;

    [Header("적 영웅 방어도")]
    public int enemyShield;
    int flagEnemyShield;

    [Header("------------------------------------------------")]
    [Space(30)]

    public SpriteRenderer[] playerHpNum;
    public Animator playerHpAni;

    public SpriteRenderer[] playerShieldNum;
    public Animator playerShieldHpAni;
    public Animator playerShieldAni;

    public SpriteRenderer[] enemyHpNum;
    public Animator enemyHpAni;

    public SpriteRenderer[] enemyShieldNum;
    public Animator enemyShieldHpAni;
    public Animator enemyShieldAni;

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
            enemyHpAni.SetTrigger("Change");
        }

        if (flagPlayerHp != nowPlayerHp)
        {
            flagPlayerHp = nowPlayerHp;
            playerHpAni.SetTrigger("Change");
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

        #region[플레이어 영웅 방어도 표시]
        int tempNowPlayerShield = Mathf.Abs(playerShield);
        if (enemyShield > 0)
            tempNowPlayerShield = Mathf.Min(tempNowPlayerShield, 99);
        else
            tempNowPlayerShield = Mathf.Max(tempNowPlayerShield, -99);

        if (playerShield < 0)
        {
            if (tempNowPlayerShield < 10)
            {
                playerShieldNum[3].gameObject.SetActive(true);
                playerShieldNum[4].gameObject.SetActive(false);
            }
            else
            {
                playerShieldNum[3].gameObject.SetActive(false);
                playerShieldNum[4].gameObject.SetActive(true);
            }
        }
        else
        {
            playerShieldNum[3].gameObject.SetActive(false);
            playerShieldNum[4].gameObject.SetActive(false);
        }

        if (tempNowPlayerShield < 10)
        {
            playerShieldNum[1].gameObject.SetActive(false);
            playerShieldNum[2].gameObject.SetActive(false);
            playerShieldNum[0].gameObject.SetActive(true);
            playerShieldNum[0].sprite = DataMng.instance.num[tempNowPlayerShield % 10];
        }
        else
        {
            playerShieldNum[1].gameObject.SetActive(true);
            playerShieldNum[2].gameObject.SetActive(true);
            playerShieldNum[0].gameObject.SetActive(false);
            playerShieldNum[1].sprite = DataMng.instance.num[tempNowPlayerShield / 10];
            playerShieldNum[2].sprite = DataMng.instance.num[tempNowPlayerShield % 10];
        }
        #endregion

        if (flagEnemyShield != enemyShield)
        {
            flagEnemyShield = enemyShield;
            enemyShieldHpAni.SetTrigger("Change");
        }

        if (flagPlayerShield != playerShield)
        {
            flagPlayerShield = playerShield;
            playerShieldHpAni.SetTrigger("Change");
        }

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

        #region[적 영웅 방어도 표시]
        int tempNowEnemyShield = Mathf.Abs(enemyShield);
        if (enemyShield > 0)
            tempNowEnemyShield = Mathf.Min(tempNowEnemyShield, 99);
        else
            tempNowEnemyShield = Mathf.Max(tempNowEnemyShield, -99);

        if (enemyShield < 0)
        {
            if (tempNowEnemyShield < 10)
            {
                enemyShieldNum[3].gameObject.SetActive(true);
                enemyShieldNum[4].gameObject.SetActive(false);
            }
            else
            {
                enemyShieldNum[3].gameObject.SetActive(false);
                enemyShieldNum[4].gameObject.SetActive(true);
            }
        }
        else
        {
            enemyShieldNum[3].gameObject.SetActive(false);
            enemyShieldNum[4].gameObject.SetActive(false);
        }

        if (tempNowEnemyShield < 10)
        {
            enemyShieldNum[1].gameObject.SetActive(false);
            enemyShieldNum[2].gameObject.SetActive(false);
            enemyShieldNum[0].gameObject.SetActive(true);
            enemyShieldNum[0].sprite = DataMng.instance.num[tempNowEnemyShield % 10];
        }
        else
        {
            enemyShieldNum[1].gameObject.SetActive(true);
            enemyShieldNum[2].gameObject.SetActive(true);
            enemyShieldNum[0].gameObject.SetActive(false);
            enemyShieldNum[1].sprite = DataMng.instance.num[tempNowEnemyShield / 10];
            enemyShieldNum[2].sprite = DataMng.instance.num[tempNowEnemyShield % 10];
        }
        #endregion
    }
}
