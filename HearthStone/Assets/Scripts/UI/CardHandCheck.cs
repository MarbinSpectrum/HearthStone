using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandCheck : MonoBehaviour
{
    public static CardHandCheck instance;

    public Image thisImg;
    public Sprite minionImg_legend;
    public Sprite minionImg;
    public Sprite spellImg;
    public Sprite weaponImg;

    public CardView checkCard;
    public Transform glow;

    public void Awake()
    {
        instance = this;
        checkCard.hide = true;
    }

    public void Update()
    {
        glow.gameObject.SetActive(!checkCard.hide);
        glow.position = transform.position;

        if (checkCard)
        {
            if (checkCard.cardType == CardType.하수인)
            {
                if (checkCard.cardLevel.Equals("전설"))
                    thisImg.sprite = minionImg_legend;
                else
                    thisImg.sprite = minionImg;
            }
            else if (checkCard.cardType == CardType.주문)
                thisImg.sprite = spellImg;
            else if (checkCard.cardType == CardType.무기)
                thisImg.sprite = weaponImg;

        }
    }

}
