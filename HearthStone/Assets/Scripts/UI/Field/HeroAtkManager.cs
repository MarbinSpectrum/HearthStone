using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroAtkManager : MonoBehaviour
{
    [Header("플레이어 영웅 공격력")]
    public int playerAtk;

    [Header("플레이어 무기")]
    public string playerWeaponName;
    public int playerWeaponAtk;
    int flagPlayerWeaponAtk;
    private int playerWeaponDurability;
    int flagPlayerWeaponDurability;

    [Header("플레이어 최종공격력")]
    public int playerFinalAtk;
    int flagPlayerFinalAtk;

    [Header("적 영웅 공격력")]
    [Header("------------------------------------------------")]
    [Space(10)]

    public int enemyAtk;

    [Header("적 무기")]
    public string enemyWeaponName;
    public int enemyWeaponAtk;
    int flagEnemyWeaponAtk;
    private int enemyWeaponDurability;
    int flagEnemyWeaponDurability;

    [Header("적 최종공격력")]
    public int enemyFinalAtk;
    int flagEnemyFinalAtk;

    [Header("------------------------------------------------")]
    [Space(30)]

    public SpriteRenderer[] playerAtkNum;
    public Animator playerAtkNumAni;

    public GameObject playerWeapon;
    bool playerWeaponFlag = false;
    public SpriteRenderer[] playerWeapons;
    public SpriteRenderer[] playerWeaponAtkNum;
    public Animator playerWeaponAtkNumAni;
    public SpriteRenderer[] playerWeaponDurabilityNum;
    public Animator playerWeaponDurabilityNumAni;
    public int playerCanAttackNum = 0;
    public GameObject playerAttackGlow;
    public GameObject playerCanAttack;
    public Animator playerObjectAni;
    public Animator playerWeaponCover;

    public SpriteRenderer[] enemyAtkNum;
    public Animator enemyAtkNumAni;

    public GameObject enemyWeapon;
    bool enemyWeaponFlag = false;
    public SpriteRenderer[] enemyWeapons;
    public SpriteRenderer[] enemyWeaponAtkNum;
    public Animator enemyWeaponAtkNumAni;
    public SpriteRenderer[] enemyWeaponDurabilityNum;
    public Animator enemyWeaponDurabilityNumAni;
    public int enemyCanAttackNum = 0;
    public GameObject enemyAttackGlow;
    public GameObject enemyCanAttack;
    public bool enemyAttackCheck = false;
    public Animator enemyObjectAni;
    public Animator enemyWeaponCover;


    Animator heroObject;
    Vector3 startHeroPos;
    Vector3 defaultPos;
    Vector3 targetPos;
    float time = 0;
    int attackFlag = -1;

    [Range(0.1f, 3)]
    public float speed;
    public float targetDis = 10;

    public void Update()
    {
        HeroAtkUpdate();
        HeroWeaponUpdate();
        HeroAttack_Update();
        playerAttackGlow.SetActive(HeroCanAttack());
        playerCanAttack.SetActive(HeroCanAttack());
        enemyAttackCheck = !GameEventManager.instance.EventCheck() && 
            enemyCanAttackNum > 0 && enemyFinalAtk > 0 && 
            TurnManager.instance.turn == Turn.상대방 && 
            BattleUI.instance.gameStart;
        if (playerWeaponDurability > 0 && !playerWeaponFlag)
            playerWeaponFlag = true;
        if (enemyWeaponDurability > 0 && !enemyWeaponFlag)
            enemyWeaponFlag = true;

    }

    public bool HeroCanAttack()
    {
        return !GameEventManager.instance.EventCheck()
            && playerCanAttackNum > 0 && playerFinalAtk > 0 &&
            TurnManager.instance.turn == Turn.플레이어
            && BattleUI.instance.gameStart;
    }

    public void HeroAttack_Update()
    {
        if (attackFlag == 0)
        {
            SoundManager.instance.PlaySE("영웅공격시작");
            GameEventManager.instance.EventSet(3);
            heroObject.SetBool("Attack", true);
            attackFlag = 1;
        }
        else if (attackFlag == 1)
        {
            time += Time.deltaTime;
            if (time >= 1)
            {
                attackFlag = 2;
                time = 0;
            }
        }
        else if (attackFlag == 2)
        {
            defaultPos = heroObject.transform.position;
            heroObject.SetBool("Attack", false);
            startHeroPos = heroObject.transform.position;
            attackFlag = 3;
            time = 0;
        }
        else if (attackFlag == 3)
        {
            Vector3 dic = targetPos - startHeroPos;
            dic = dic.normalized;
            if (time < 1)
            {
                Vector3 heroPos = Vector3.Lerp(startHeroPos, targetPos - dic * targetDis, time);
                heroObject.transform.position = heroPos;
                time += Time.deltaTime * speed;
            }
            else
            {
                attackFlag = 4;
            }
        }
        else if (attackFlag == 4)
        {
            startHeroPos = heroObject.transform.position;
            SoundManager.instance.PlaySE("영웅공격끝");
            AttackManager.instance.AttackEffectRun();
            attackFlag = 5;
            time = 0;
        }
        else if (attackFlag == 5)
        {
            if (time < 1)
            {
                Vector3 heroPos = Vector3.Lerp(startHeroPos, defaultPos, time);
                heroObject.transform.position = heroPos;
                time += Time.deltaTime * speed;
            }
            else
            {
                heroObject.transform.position = defaultPos;
                attackFlag = 6;
            }
        }
        else if (attackFlag == 6)
        {
            heroObject.transform.position = defaultPos;
            if (heroObject.Equals(playerObjectAni))
                playerWeaponDurability--;
            else if (heroObject.Equals(enemyObjectAni))
                enemyWeaponDurability--;
            time = 0;
            attackFlag = -1;
        }
    }

    #region[영웅 공격력 업데이트]
    public void HeroAtkUpdate()
    {
        playerFinalAtk = playerAtk + playerWeaponAtk;
        playerFinalAtk = Mathf.Max(playerFinalAtk, 0);
        if (flagPlayerFinalAtk != playerFinalAtk)
        {
            flagPlayerFinalAtk = playerFinalAtk;
            playerAtkNumAni.SetTrigger("Change");
        }

        enemyFinalAtk = enemyAtk + enemyWeaponAtk;
        enemyFinalAtk = Mathf.Max(enemyFinalAtk, 0);
        if (flagEnemyFinalAtk != enemyFinalAtk)
        {
            flagEnemyFinalAtk = enemyFinalAtk;
            enemyAtkNumAni.SetTrigger("Change");
        }

        playerAtkNumAni.gameObject.SetActive(playerFinalAtk > 0);
        enemyAtkNumAni.gameObject.SetActive(enemyFinalAtk > 0);

        if (enemyWeaponDurability <= 0 && enemyWeaponAtk != 0)
            enemyWeaponAtk = 0;

        if (playerWeaponDurability <= 0 && playerWeaponAtk != 0)
            playerWeaponAtk = 0;

        #region[플레이어 영웅 공격력 표시]
        int tempnowPlayerAtk = Mathf.Abs(playerFinalAtk);

        if (tempnowPlayerAtk < 10)
        {
            playerAtkNum[1].gameObject.SetActive(false);
            playerAtkNum[2].gameObject.SetActive(false);
            playerAtkNum[0].gameObject.SetActive(true);
            playerAtkNum[0].sprite = DataMng.instance.num[tempnowPlayerAtk % 10];
        }
        else
        {
            playerAtkNum[1].gameObject.SetActive(true);
            playerAtkNum[2].gameObject.SetActive(true);
            playerAtkNum[0].gameObject.SetActive(false);
            playerAtkNum[1].sprite = DataMng.instance.num[tempnowPlayerAtk / 10];
            playerAtkNum[2].sprite = DataMng.instance.num[tempnowPlayerAtk % 10];
        }
        #endregion

        #region[적 영웅 공격력 표시]
        int tempnowEnemyAtk = Mathf.Abs(enemyFinalAtk);

        if (tempnowEnemyAtk < 10)
        {
            enemyAtkNum[1].gameObject.SetActive(false);
            enemyAtkNum[2].gameObject.SetActive(false);
            enemyAtkNum[0].gameObject.SetActive(true);
            enemyAtkNum[0].sprite = DataMng.instance.num[tempnowEnemyAtk % 10];
        }
        else
        {
            enemyAtkNum[1].gameObject.SetActive(true);
            enemyAtkNum[2].gameObject.SetActive(true);
            enemyAtkNum[0].gameObject.SetActive(false);
            enemyAtkNum[1].sprite = DataMng.instance.num[tempnowEnemyAtk / 10];
            enemyAtkNum[2].sprite = DataMng.instance.num[tempnowEnemyAtk % 10];
        }
        #endregion
    }
    #endregion

    #region[영웅 무기 업데이트]
    public void HeroWeaponUpdate()
    {
        playerWeaponAtk = Mathf.Max(playerWeaponAtk, 0);
        playerWeaponDurability = Mathf.Max(playerWeaponDurability, 0);
        if (flagPlayerWeaponAtk != playerWeaponAtk)
        {
            flagPlayerWeaponAtk = playerWeaponAtk;
            playerWeaponAtkNumAni.SetTrigger("Change");
        }
        if (flagPlayerWeaponDurability != playerWeaponDurability)
        {
            flagPlayerWeaponDurability = playerWeaponDurability;
            playerWeaponDurabilityNumAni.SetTrigger("Change");
        }

        if(playerWeaponDurability <= 0 && playerWeaponFlag)
        {
            StartCoroutine(WeaponVibration(HeroManager.instance.heroAtkManager.playerWeapon.gameObject, 0, 50, 2));
            StartCoroutine(WeaponBreak(HeroManager.instance.heroAtkManager.playerWeapon.gameObject, 0.7f));
            playerWeaponFlag = false;
        }
        else if (playerWeaponDurability > 0)
            playerWeapon.SetActive(true);

        if (playerWeaponDurability <= 0 && playerWeaponAtk != 0)
            playerWeaponAtk = 0;

        Vector2Int pair = DataMng.instance.GetPairByName(
            DataParse.GetCardName(playerWeaponName));
        int playerWeaponBaseHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
        int playerWeaponBaseAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
        for (int i = 0; i < playerWeapons.Length; i++)
            playerWeapons[i].gameObject.SetActive(playerWeapons[i].transform.name.Equals(playerWeaponName));

        #region[플레이어 무기 공격력 표시]
        int tempnowPlayerWeaponAtk = Mathf.Abs(playerWeaponAtk);

        if (tempnowPlayerWeaponAtk < 10)
        {
            playerWeaponAtkNum[1].gameObject.SetActive(false);
            playerWeaponAtkNum[2].gameObject.SetActive(false);
            playerWeaponAtkNum[0].gameObject.SetActive(true);
            playerWeaponAtkNum[0].sprite = DataMng.instance.num[tempnowPlayerWeaponAtk % 10];
        }
        else
        {
            playerWeaponAtkNum[1].gameObject.SetActive(true);
            playerWeaponAtkNum[2].gameObject.SetActive(true);
            playerWeaponAtkNum[0].gameObject.SetActive(false);
            playerWeaponAtkNum[1].sprite = DataMng.instance.num[tempnowPlayerWeaponAtk / 10];
            playerWeaponAtkNum[2].sprite = DataMng.instance.num[tempnowPlayerWeaponAtk % 10];
        }

        for (int i = 0; i < 3; i++)
            if (playerWeaponAtk < playerWeaponBaseAtk)
                playerWeaponAtkNum[i].color = Color.red;
            else if (playerWeaponAtk > playerWeaponBaseAtk)
                playerWeaponAtkNum[i].color = Color.green;
            else
                playerWeaponAtkNum[i].color = Color.white;
        #endregion

        #region[플레이어 무기 내구도 표시]
        int tempnowPlayerWeaponDurability = Mathf.Abs(playerWeaponDurability);

        if (tempnowPlayerWeaponDurability < 10)
        {
            playerWeaponDurabilityNum[1].gameObject.SetActive(false);
            playerWeaponDurabilityNum[2].gameObject.SetActive(false);
            playerWeaponDurabilityNum[0].gameObject.SetActive(true);
            playerWeaponDurabilityNum[0].sprite = DataMng.instance.num[tempnowPlayerWeaponDurability % 10];
        }
        else
        {
            playerWeaponDurabilityNum[1].gameObject.SetActive(true);
            playerWeaponDurabilityNum[2].gameObject.SetActive(true);
            playerWeaponDurabilityNum[0].gameObject.SetActive(false);
            playerWeaponDurabilityNum[1].sprite = DataMng.instance.num[tempnowPlayerWeaponDurability / 10];
            playerWeaponDurabilityNum[2].sprite = DataMng.instance.num[tempnowPlayerWeaponDurability % 10];
        }

        for (int i = 0; i < 3; i++)
            if (playerWeaponDurability < playerWeaponBaseHp)
                playerWeaponDurabilityNum[i].color = Color.red;
            else if (playerWeaponDurability > playerWeaponBaseHp)
                playerWeaponDurabilityNum[i].color = Color.green;
            else
                playerWeaponDurabilityNum[i].color = Color.white;
        #endregion




        enemyWeaponAtk = Mathf.Max(enemyWeaponAtk, 0);
        enemyWeaponDurability = Mathf.Max(enemyWeaponDurability, 0);
        if (flagEnemyWeaponAtk != enemyWeaponAtk)
        {
            flagEnemyWeaponAtk = enemyWeaponAtk;
            enemyWeaponAtkNumAni.SetTrigger("Change");
        }
        if (flagEnemyWeaponDurability != enemyWeaponDurability)
        {
            flagEnemyWeaponDurability = enemyWeaponDurability;
            enemyWeaponDurabilityNumAni.SetTrigger("Change");
        }

        if (enemyWeaponDurability <= 0 && enemyWeaponFlag)
        {
            StartCoroutine(WeaponVibration(HeroManager.instance.heroAtkManager.enemyWeapon.gameObject, 0, 50, 2));
            StartCoroutine(WeaponBreak(HeroManager.instance.heroAtkManager.enemyWeapon.gameObject, 0.7f));
            enemyWeaponFlag = false;
        }
        else if (enemyWeaponDurability > 0)
            enemyWeapon.SetActive(true);

        if (enemyWeaponDurability <= 0 && enemyWeaponAtk != 0)
            enemyWeaponAtk = 0;

        Vector2Int pair2 = DataMng.instance.GetPairByName(
            DataParse.GetCardName(enemyWeaponName));
        int enemyWeaponBaseHp = DataMng.instance.ToInteger(pair2.x, pair2.y, "체력");
        int enemyWeaponBaseAtk = DataMng.instance.ToInteger(pair2.x, pair2.y, "공격력");
        for (int i = 0; i < enemyWeapons.Length; i++)
            enemyWeapons[i].gameObject.SetActive(enemyWeapons[i].transform.name.Equals(enemyWeaponName));

        #region[적 무기 공격력 표시]
        int tempnowEnemyWeaponAtk = Mathf.Abs(enemyWeaponAtk);

        if (tempnowPlayerWeaponAtk < 10)
        {
            enemyWeaponAtkNum[1].gameObject.SetActive(false);
            enemyWeaponAtkNum[2].gameObject.SetActive(false);
            enemyWeaponAtkNum[0].gameObject.SetActive(true);
            enemyWeaponAtkNum[0].sprite = DataMng.instance.num[tempnowEnemyWeaponAtk % 10];
        }
        else
        {
            enemyWeaponAtkNum[1].gameObject.SetActive(true);
            enemyWeaponAtkNum[2].gameObject.SetActive(true);
            enemyWeaponAtkNum[0].gameObject.SetActive(false);
            enemyWeaponAtkNum[1].sprite = DataMng.instance.num[tempnowEnemyWeaponAtk / 10];
            enemyWeaponAtkNum[2].sprite = DataMng.instance.num[tempnowEnemyWeaponAtk % 10];
        }

        for (int i = 0; i < 3; i++)
            if (enemyWeaponAtk < enemyWeaponBaseAtk)
                enemyWeaponAtkNum[i].color = Color.red;
            else if (enemyWeaponAtk > enemyWeaponBaseAtk)
                enemyWeaponAtkNum[i].color = Color.green;
            else
                enemyWeaponAtkNum[i].color = Color.white;
        #endregion

        #region[적 무기 내구도 표시]
        int tempnowEnemyWeaponDurability = Mathf.Abs(enemyWeaponDurability);

        if (tempnowPlayerWeaponDurability < 10)
        {
            enemyWeaponDurabilityNum[1].gameObject.SetActive(false);
            enemyWeaponDurabilityNum[2].gameObject.SetActive(false);
            enemyWeaponDurabilityNum[0].gameObject.SetActive(true);
            enemyWeaponDurabilityNum[0].sprite = DataMng.instance.num[tempnowEnemyWeaponDurability % 10];
        }
        else
        {
            enemyWeaponDurabilityNum[1].gameObject.SetActive(true);
            enemyWeaponDurabilityNum[2].gameObject.SetActive(true);
            enemyWeaponDurabilityNum[0].gameObject.SetActive(false);
            enemyWeaponDurabilityNum[1].sprite = DataMng.instance.num[tempnowEnemyWeaponDurability / 10];
            enemyWeaponDurabilityNum[2].sprite = DataMng.instance.num[tempnowEnemyWeaponDurability % 10];
        }

        for (int i = 0; i < 3; i++)
            if (enemyWeaponDurability < enemyWeaponBaseHp)
                enemyWeaponDurabilityNum[i].color = Color.red;
            else if (enemyWeaponDurability > enemyWeaponBaseHp)
                enemyWeaponDurabilityNum[i].color = Color.green;
            else
                enemyWeaponDurabilityNum[i].color = Color.white;
        #endregion

    }
    #endregion

    public void HeroAtkTurnEnd(bool enemy)
    {
        HeroManager.instance.heroAtkManager.enemyWeaponCover.SetBool("Open", !enemy);
        HeroManager.instance.heroAtkManager.playerWeaponCover.SetBool("Open", enemy);
        if (enemy)
        {
            if (HeroManager.instance.heroAtkManager.enemyWeaponDurability > 0)
                StartCoroutine(WeaponSound(1, false));
            if (HeroManager.instance.heroAtkManager.playerWeaponDurability > 0)
                StartCoroutine(WeaponSound(0, true));

            StartCoroutine(WeaponVibration(HeroManager.instance.heroAtkManager.enemyWeapon.gameObject, 0.8f, 20, 1));
            playerCanAttackNum = 1;
            enemyAtk = 0;
        }
        else
        {
            if (HeroManager.instance.heroAtkManager.enemyWeaponDurability > 0)
                StartCoroutine(WeaponSound(0, true));
            if (HeroManager.instance.heroAtkManager.playerWeaponDurability > 0)
                StartCoroutine(WeaponSound(1, false));

            StartCoroutine(WeaponVibration(HeroManager.instance.heroAtkManager.playerWeapon.gameObject, 0.8f, 20, 1));
            enemyCanAttackNum = 1;
            playerAtk = 0;
        }
    }

    public void HeroAttack(bool enemy,Vector3 target)
    {
        if (enemy)
        {
            enemyCanAttackNum--;
            attackFlag = 0;
            heroObject = enemyObjectAni;
            targetPos = new Vector3(target.x, target.y, 0);
        }
        else
        {
            playerCanAttackNum--;
            attackFlag = 0;
            heroObject = playerObjectAni;
            SoundManager.instance.PlayCharacterSE(HeroManager.instance.heroPowerManager.playerHeroName, 영웅상태.공격시);
            targetPos = target;
        }
    }

    private IEnumerator WeaponSound(float waitTime, bool open)
    {
        yield return new WaitForSeconds(waitTime);
        if (!open)
            SoundManager.instance.PlaySE("무기닫기");
        else
            SoundManager.instance.PlaySE("무기열기");
    }

    private IEnumerator WeaponBreak(GameObject obj,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        obj.SetActive(false);
        EffectManager.instance.BreakSmokeEffect(obj.transform.position);
    }

    private IEnumerator WeaponVibration(GameObject obj,float waitTime, int n, float power)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 v = obj.transform.position;
        float down = power / (float)n;
        for (int i = 0; i < n; i++)
        {
            obj.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(power, 0, 0);
            power -= down;
            yield return new WaitForSeconds(0.01f);
        }
        obj.transform.position = v;
    }

    public void SetPlayerDurability(int v)
    {
        playerWeaponDurability = v;
    }

    public int GetPlayerDurability()
    {
        return playerWeaponDurability;
    }

    public void SetEnemyDurability(int v)
    {
        enemyWeaponDurability = v;
    }

    public int GetEnemyDurability()
    {
        return enemyWeaponDurability;
    }
}
