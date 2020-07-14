using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardCheckBtn : Btn
{
    public CardView cardView;
    public int cardNum;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
        EventTrigger.Entry pUp = new EventTrigger.Entry();
        pUp.eventID = EventTriggerType.PointerUp;
        pUp.callback.AddListener((data) =>
        {
            pointerUp();
        });
        btnEvent.triggers.Add(pUp);
    }
    #endregion

    #region[Update]
    public override void Update()
    {

    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if(Input.GetMouseButton(0))
            ActBtn();
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        ActBtn();
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        PickUpCard();
    }
    #endregion

    #region[pointerUp]
    public void pointerUp()
    {
        CardHandCheck.instance.checkCard.hide = true;
        cardView.hide = false;
        CardViewManager.instance.UpdateCardView();
        DragCardObject.instance.HideDragCard();
    }
    #endregion

    #region[pointerClick]
    public override void pointerClick()
    {

    }
    #endregion

    #region[카드 픽업]
    public void PickUpCard()
    {
        CardHandCheck.instance.checkCard.hide = true;
        cardView.hide = false;
        CardViewManager.instance.UpdateCardView();
        if (BattleUI.instance.gameStart && TurnManager.instance.turnAniEnd && TurnManager.instance.turn == 턴.플레이어 && CardHand.instance.canUse[cardNum])
        {
            DragCardObject.instance.ShowDragCard(CardHandCheck.instance.checkCard);
            DragCardObject.instance.dragCardNum = cardNum;
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        if (DragCardObject.instance.dragCard)
            return;
        if (!DragCardObject.instance.dropEffect.dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Stop"))
            return;
        cardView.hide = true;
        CardViewManager.instance.CardShow(ref CardHandCheck.instance.checkCard, cardView);
        DragCardObject.instance.HideDragCard();
        CardHandCheck.instance.checkCard.hide = false;
        CardViewManager.instance.UpdateCardView();
        CardHandCheck.instance.transform.position = new Vector3(transform.position.x, CardHandCheck.instance.transform.position.y, CardHandCheck.instance.transform.position.z);
    }
    #endregion


}