using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeroManager : MonoBehaviour
{
    public static HeroManager instance;

    [Header("------------------------------------------------")]
    [Space(30)]

    public HeroHpManager heroHpManager;
    public HeroAtkManager heroAtkManager;
    public HeroPowerManager heroPowerManager;
    public Image playerHero;
    public Image enemyHero;
    public GameObject playerFreezeObj;
    public GameObject enemyFreezeObj;
    private int playerFreezeCount = 0;
    private int enemyFreezeCount = 0;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        playerFreezeObj.SetActive(playerFreezeCount > 0);
        enemyFreezeObj.SetActive(enemyFreezeCount > 0);

        //playerHero.raycastTarget = !CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대");
        //enemyHero.raycastTarget = !DragCardObject.instance.dragCard;
    }

    public void SetFreeze(bool enemy)
    {
        if (enemy)
            enemyFreezeCount = 2;
        else
            playerFreezeCount = 2;
    }

    public void MeltFreeze()
    {
        enemyFreezeCount--;
        playerFreezeCount--;
    }
}
