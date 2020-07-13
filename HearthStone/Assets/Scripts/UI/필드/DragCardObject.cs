﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragCardObject : MonoBehaviour
{
    public static DragCardObject instance;

    RectTransform rectTransform;

    public CardView dragCardView;
    public CardView dropEffectCardView;
    public Animator dropEffectAni;
    public GameObject dragPoint;
    public Image glowImg;
    public int dragCardNum = 0;
    public bool dragCard;
    public bool mouseInMyField;
    public bool mouseInEnemyField;
    public bool mouseInField;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        instance = this;
        dragCardView.hide = true;
        //dropEffectCardView.hide = true;
    }

    public void Update()
    {
        if (!Input.GetMouseButton(0))
            HideDragCard();
        dragPoint.SetActive(!dragCardView.hide);
        rectTransform.anchoredPosition = Input.mousePosition;

        if (dragCardView.cardType == CardType.무기)
            glowImg.sprite = CardHand.instance.weaponImg;
        else if (dragCardView.cardType == CardType.주문)
            glowImg.sprite = CardHand.instance.spellImg;
        else if (dragCardView.cardType == CardType.하수인)
        {
            if (dragCardView.cardLevel.Equals("전설"))
                glowImg.sprite = CardHand.instance.minionImg_legend;
            else
                glowImg.sprite = CardHand.instance.minionImg;
        }
        glowImg.enabled = !dragCardView.hide && CardHand.instance.canUse[dragCardNum];
        dragCard = !dragCardView.hide;
    }

    public void DragAndDropCard()
    {
        if (CardHand.instance.canUse[dragCardNum])
            CardHand.instance.UseCard(dragCardNum);
        HideDragCard();
    }

    public void HideDragCard()
    {
        dragCardView.hide = true;
        mouseInMyField = false;
        mouseInEnemyField = false;
        mouseInField = false;
    }

    public void CheckMouseInMyField(bool b)
    {
        if (!dragCard)
            return;
        mouseInMyField = b && dragCard;
    }

    public void CheckMouseInEnemyField(bool b)
    {
        if (!dragCard)
            return;
        mouseInEnemyField = b && dragCard;
    }

    public void CheckMouseInField(bool b)
    {
        if (!dragCard)
            return;
        mouseInField = b && dragCard;
    }

    public void ShowDropEffect()
    {
        dropEffectAni.SetTrigger("Effect");
    }

    public void ShowDragCard(CardView cardView)
    {
        dragCardView.hide = false;
        CardViewManager.instance.CardShow(ref dragCardView, cardView);
        CardViewManager.instance.CardShow(ref dropEffectCardView, cardView);
        CardViewManager.instance.UpdateCardView(0.001f);
    }
}
