using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackDrag : Btn
{
    protected EventTrigger.Entry pBeginDrag;
    protected EventTrigger.Entry pDragEnd;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
        pBeginDrag = new EventTrigger.Entry();
        pBeginDrag.eventID = EventTriggerType.BeginDrag;
        pBeginDrag.callback.AddListener((data) =>
        {
            pointerDrag(true);
        });
        btnEvent.triggers.Add(pBeginDrag);

        pDragEnd = new EventTrigger.Entry();
        pDragEnd.eventID = EventTriggerType.EndDrag;
        pDragEnd.callback.AddListener((data) =>
        {
            pointerDrag(false);
        });
        btnEvent.triggers.Add(pDragEnd);
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
        Debug.Log("다운");
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

    }
    #endregion

    #region[pointerDrag]
    private void pointerDrag(bool flag)
    {
        if(flag)
        {
            if (DataMng.instance.playData.packs.Count <= 0)
                return;
            OpenPackMenu.instance.dragObj.gameObject.SetActive(true);
        }
        else
        {
            SoundManager.instance.PlaySE("팩내려놓기");
            OpenPackMenu.instance.dragObj.gameObject.SetActive(false);
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion
}
