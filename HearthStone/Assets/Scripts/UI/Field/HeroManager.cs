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
    public Image playerHero;
    public Image enemyHero;

    [HideInInspector] public int playerFreeze;
    [HideInInspector] public int enemyFreeze;

    public GameObject playerFreezeObj;
    public GameObject enemyFreezeObj;

    private void Awake()
    {
        instance = this;
    }

    public void SetFreeze(bool enemy)
    {
        if (enemy)
            enemyFreeze = 2;
        else
            playerFreeze = 2;
    }

    void Update()
    {
        playerFreezeObj.SetActive(playerFreeze > 0);
        enemyFreezeObj.SetActive(enemyFreeze > 0);
        playerHero.raycastTarget = !CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대");
        enemyHero.raycastTarget = !DragCardObject.instance.dragCard;
    }
}
