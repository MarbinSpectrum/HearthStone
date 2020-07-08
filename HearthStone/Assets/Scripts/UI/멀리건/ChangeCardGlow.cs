using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCardGlow : MonoBehaviour
{
    public Image thisImg;
    public Sprite minionImg_legend;
    public Sprite minionImg;
    public Sprite spellImg;
    public Sprite weaponImg;
    public CardView cardView;
    public CardChangeBtn cardChangeBtn;

    void Update()
    {
        thisImg.enabled = !cardChangeBtn.change;

        if (thisImg && cardView)
        {
            if (cardView.cardType == CardType.하수인)
            {
                if(cardView.cardLevel.Equals("전설"))
                    thisImg.sprite = minionImg_legend;
                else
                    thisImg.sprite = minionImg;
            }
            else if (cardView.cardType == CardType.주문)
                thisImg.sprite = spellImg;
            else if (cardView.cardType == CardType.무기)
                thisImg.sprite = weaponImg;

        }
    }
}
