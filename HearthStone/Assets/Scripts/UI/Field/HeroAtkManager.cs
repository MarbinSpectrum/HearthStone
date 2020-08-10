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
    public int playerWeaponDurability;
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
    public int enemyWeaponDurability;
    int flagEnemyWeaponDurability;

    [Header("적 최종공격력")]
    public int enemyFinalAtk;
    int flagEnemyFinalAtk;

    [Header("------------------------------------------------")]
    [Space(30)]

    public SpriteRenderer[] playerAtkNum;
    public Animator playerAtkNumAni;

    public GameObject playerWeapon;
    public SpriteRenderer[] playerWeapons;
    public SpriteRenderer[] playerWeaponAtkNum;
    public Animator playerWeaponAtkNumAni;
    public SpriteRenderer[] playerWeaponDurabilityNum;
    public Animator playerWeaponDurabilityNumAni;
    public int playerCanAttackNum = 0;
    public GameObject playerAttackGlow;
    public GameObject playerCanAttack;
    public Animator playerObjectAni;

    public SpriteRenderer[] enemyAtkNum;
    public Animator enemyAtkNumAni;

    public GameObject enemyWeapon;
    public SpriteRenderer[] enemyWeapons;
    public SpriteRenderer[] enemyWeaponAtkNum;
    public Animator enemyWeaponAtkNumAni;
    public SpriteRenderer[] enemyWeaponDurabilityNum;
    public Animator enemyWeaponDurabilityNumAni;
    public int enemyCanAttackNum = 0;
    public GameObject enemyAttackGlow;
    public GameObject enemyCanAttack;
    public Animator enemyObjectAni;

    public float maxSpeed = 1000;
    public float addSpeed = 100;
    public float minSpeed = 1000;
    public float subSpeed = 100;
    public float targetDis = 100;

    public void Update()
    {
        HeroAtkUpdate();
        HeroWeaponUpdate();
        playerAttackGlow.SetActive(!GameEventManager.instance.EventCheck() && playerCanAttackNum > 0 && playerFinalAtk > 0 && TurnManager.instance.turn == 턴.플레이어 && BattleUI.instance.gameStart);
        playerCanAttack.SetActive(playerAttackGlow.activeSelf);
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
        playerWeapon.SetActive(playerWeaponDurability > 0);
        if (playerWeaponDurability <= 0 && playerWeaponAtk != 0)
            playerWeaponAtk = 0;

        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(playerWeaponName));
        int playerWeaponBaseHp = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "체력");
        int playerWeaponBaseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "공격력");
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
        enemyWeapon.SetActive(enemyWeaponAtk > 0);
        if (enemyWeaponDurability <= 0 && enemyWeaponAtk != 0)
            enemyWeaponAtk = 0;

        Vector2 pair2 = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(enemyWeaponName));
        int enemyWeaponBaseHp = DataMng.instance.ToInteger((DataMng.TableType)pair2.x, (int)pair2.y, "체력");
        int enemyWeaponBaseAtk = DataMng.instance.ToInteger((DataMng.TableType)pair2.x, (int)pair2.y, "공격력");
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
        if(enemy)
        {
            playerCanAttackNum = 1;
            enemyAtk = 0;
        }
        else
        {
            enemyCanAttackNum = 1;
            playerAtk = 0;
        }
    }

    public void HeroAttack(bool enemy,Vector3 targetPos)
    {
        if (enemy)
        {
            enemyWeaponDurability--;
            enemyCanAttackNum--;
            StartCoroutine(HeroAttack(enemyObjectAni, targetPos));
        }
        else
        {
            playerWeaponDurability--;
            playerCanAttackNum--;
            StartCoroutine(HeroAttack(playerObjectAni, targetPos));
        }
    }

    private IEnumerator HeroAttack(Animator heroObject,Vector3 targetPos)
    {
        GameEventManager.instance.EventSet(3);
        playerObjectAni.SetBool("Attack",true);
        yield return new WaitForSeconds(1f);

        Vector3 defaultPos = heroObject.transform.position;

        playerObjectAni.SetBool("Attack", false);

        float speed = 10;
        while(Vector2.Distance(heroObject.transform.position,targetPos) > targetDis * Time.deltaTime)
        {
            Vector3 dic = targetPos - heroObject.transform.position;
            dic = dic.normalized;
            heroObject.transform.position += dic * speed * Time.deltaTime;
            speed += addSpeed;
            speed = Mathf.Min(speed, maxSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        AttackManager.instance.AttackEffectRun();

        while (Vector2.Distance(heroObject.transform.position, defaultPos) > targetDis * Time.deltaTime)
        {
            Vector3 dic = defaultPos - heroObject.transform.position;
            dic = dic.normalized;
            heroObject.transform.position += dic * speed * Time.deltaTime;
            speed -= subSpeed;
            speed = Mathf.Max(speed, minSpeed);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        heroObject.transform.position = defaultPos;
    }
}
