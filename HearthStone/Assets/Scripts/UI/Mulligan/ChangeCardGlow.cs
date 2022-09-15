using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCardGlow : MonoBehaviour
{
    [SerializeField] private Image thisImg;
    [SerializeField] private Sprite minionImg_legend;
    [SerializeField] private Sprite minionImg;
    [SerializeField] private Sprite spellImg;
    [SerializeField] private Sprite weaponImg;
    [SerializeField] private CardView cardView;
    public bool isRun = true;

    private void Update()
    {
        UpdateGlowEffect();
    }

    private void UpdateGlowEffect()
    {

        if (thisImg == null)
            return;
        if (cardView == null)
            return;

        if (isRun == false)
        {
            thisImg.enabled = false;
            return;
        }
        thisImg.enabled = true;

        if (cardView.cardType == CardType.하수인)
        {
            if (cardView.cardLevel.Equals("전설"))
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
