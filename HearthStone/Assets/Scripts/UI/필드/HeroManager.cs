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

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        playerHero.raycastTarget = !CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대");
        enemyHero.raycastTarget = !DragCardObject.instance.dragCard;
    }
}
