using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardCheckBtn : Btn
{
    public CardView cardView;
    public int cardNum;
    public bool select;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        cardView.hide = select || (DragCardObject.instance.dragCard && DragCardObject.instance.dragCardNum == cardNum);
        CardViewManager.instance.UpdateCardView();
        if (Input.GetMouseButtonUp(0))
            pointerUp();
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
        select = false;
        if (DragLineRenderer.instance.lineRenderer.enabled)
            return;
        if (DragCardObject.instance.dragCard)
            return;
        if (!DragCardObject.instance.dropEffect.dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Stop"))
            return;

        CardHandCheck.instance.checkCard.hide = true;
        CardViewManager.instance.UpdateCardView(0.001f);
        PickUpCard();
    }
    #endregion

    #region[pointerUp]
    public void pointerUp()
    {
        select = false;
        DragCardObject.instance.HideDragCard();
        CardHandCheck.instance.checkCard.hide = true;
        CardViewManager.instance.UpdateCardView(0.001f);
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
        CardViewManager.instance.UpdateCardView();
        if (!GameEventManager.instance.EventCheck() && BattleUI.instance.gameStart && TurnManager.instance.turnAniEnd && TurnManager.instance.turn == 턴.플레이어 && CardHand.instance.canUse[cardNum])
        {
            DragCardObject.instance.ShowDragCard(cardView);
            DragCardObject.instance.dragCardNum = cardNum;
        }
    }

    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        if (GameEventManager.instance.EventCheck())
            return;
        if (!BattleUI.instance.gameStart)
            return;
        if (DragLineRenderer.instance.lineRenderer.enabled)
            return;
        if (DragCardObject.instance.dragCard)
            return;
        if (!DragCardObject.instance.dropEffect.dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Stop"))
            return;
        select = true;
        //cardView.hide = true;
        CardViewManager.instance.CardShow(ref CardHandCheck.instance.checkCard, cardView);
        DragCardObject.instance.HideDragCard();
        CardHandCheck.instance.checkCard.hide = false;
        CardViewManager.instance.UpdateCardView();
        CardHandCheck.instance.transform.position = new Vector3(transform.position.x, CardHandCheck.instance.transform.position.y, CardHandCheck.instance.transform.position.z);
    }
    #endregion


}