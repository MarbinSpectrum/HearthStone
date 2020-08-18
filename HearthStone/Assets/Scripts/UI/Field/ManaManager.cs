using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ManaManager : MonoBehaviour
{
    public static ManaManager instance;

    [Header("플레이어 현재마나")]
    [Range(0, 10)]
    public int playerNowMana;
    [Header("플레이어 최대마나")]
    [Range(0, 10)]
    public int playerMaxMana;

    [Header("상대방 현재마나")]
    [Header("-----------------------------------------------------------")]

    [Range(0, 10)]
    public int enemyNowMana;
    [Header("상대방 최대마나")]
    [Range(0, 10)]
    public int enemyMaxMana;

    [Header("-----------------------------------------------------------")]
    [Space(20)]

    public ShowManaCost showManaCost;
    public FieldManaCost playerManaCost;
    public FieldManaCost enemyManaCost;

    #region[Awake]
    public void Awake()
    {
        instance = this;
    }
    #endregion

    #region[Update]
    private void Update()
    {
        if (showManaCost)
        {
            showManaCost.nowMana = playerNowMana;
            showManaCost.maxMana = playerMaxMana;
        }
        if(playerManaCost)
        {
            playerManaCost.nowMana = playerNowMana;
            playerManaCost.maxMana = playerMaxMana;
        }
        if (enemyManaCost)
        {
            enemyManaCost.nowMana = enemyNowMana;
            enemyManaCost.maxMana = enemyMaxMana;
        }

        playerMaxMana = Mathf.Min(playerMaxMana, 10);
        playerNowMana = Mathf.Max(playerNowMana, 0);

        enemyMaxMana = Mathf.Min(enemyMaxMana, 10);
        enemyNowMana = Mathf.Max(enemyNowMana, 0);
    }
    #endregion
}
