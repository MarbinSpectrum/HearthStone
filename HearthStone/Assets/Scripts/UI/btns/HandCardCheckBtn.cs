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

        EventTrigger.Entry pDrag = new EventTrigger.Entry();
        pDrag.eventID = EventTriggerType.Drag;
        pDrag.callback.AddListener((data) =>
        {
            PointDrag();
        });
        btnEvent.triggers.Add(pDrag);
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

    public void PointDrag()
    {
        if (DragLineRenderer.instance.lineRenderer.enabled)
            return;
        if (DragCardObject.instance.dragCard)
            return;
        if (!DragCardObject.instance.dropEffect.dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Stop"))
            return;
        if (!CardDragField.InMouse)
            return;
        if (!select)
            return;
        select = false;
        CardHandCheck.instance.checkCard.hide = true;
        CardViewManager.instance.UpdateCardView(0.001f);
        PickUpCard();
    }

    #region[pointerExit]
    public override void pointerExit()
    {
        select = false;
        CardHandCheck.instance.checkCard.hide = true;
        CardViewManager.instance.UpdateCardView(0.001f);
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
        if (GameEventManager.instance.EventCheck() == false && //이벤트가 없는 상태
            BattleUI.instance.gameStart && //게임이 시작되어있는 상태
            TurnManager.instance.turnAniEnd && //턴 애니메이션이 종료된상태
            TurnManager.instance.turn == Turn.플레이어 && //플레이어 턴인 상태
            CardHand.instance.canUse[cardNum]) //해당카드를 사용가능한 상태
        {
            //드래그카드를 표시하고
            DragCardObject.instance.ShowDragCard(cardView);

            //cardNum번째 카드를 드래그중이라고 표시한다.
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