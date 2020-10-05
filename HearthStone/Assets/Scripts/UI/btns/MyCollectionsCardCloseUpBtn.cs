using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyCollectionsCardCloseUpBtn : Btn
{
    public int cardNum;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) =>
        {
            BeginDrag();
        });
        btnEvent.triggers.Add(beginDrag);
        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) =>
        {
            EndDrag();
        });
        btnEvent.triggers.Add(endDrag);
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

    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {

    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {

    }
    #endregion

    #region[pointerClick]
    public override void pointerClick()
    {
        if(!CardDragObject.instance.isDrag)
            ActBtn();
    }
    #endregion

    #region[BeginDrag]
    public virtual void BeginDrag()
    {
        if (MyCollectionsMenu.instance.nowDeck == -1)
            return;

        string cardName = MyCollectionsMenu.instance.cardDatas[MyCollectionsMenu.instance.nowJobIndex][MyCollectionsMenu.instance.nowCardIndex + cardNum].cardName;
        Vector2 pair = DataMng.instance.GetPairByName(cardName);
        string level = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToString((int)pair.y, "등급");
        int maxNum = level.Equals("전설") ? 1 : 2;

        if (Mathf.Min(maxNum,DataMng.instance.playData.GetCardNum(cardName)) - DataMng.instance.playData.deck[MyCollectionsMenu.instance.nowDeck].HasCardNum(cardName) <= 0)
            return;

        SoundManager.instance.PlaySE("작은버튼");
        CardDragObject.instance.isDrag = true;
        MyCollectionsMenu.instance.CardShow(ref CardDragObject.instance.cardView, MyCollectionsMenu.instance.nowJobIndex, MyCollectionsMenu.instance.nowCardIndex + cardNum);
        CardDragObject.instance.cardView.gameObject.SetActive(false);
        CardDragObject.instance.cardDrag.cardName_Data = cardName;
       
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[EndDrag]
    public virtual void EndDrag()
    {

    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        MyCollectionsMenu.instance.CardCloseUp(cardNum);
    }
    #endregion


}
