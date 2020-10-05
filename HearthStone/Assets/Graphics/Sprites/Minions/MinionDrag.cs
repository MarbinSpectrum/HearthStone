﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinionDrag : Btn
{
    public static int dragMinionNum = -1;

    public MinionObject minionObject;

    public GameObject select_normal;
    public GameObject select_taunt;
    public GameObject select_legend;

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
        btnImg.raycastTarget = !DragCardObject.instance.dragCard;
        if (minionObject.taunt)
        {
            select_taunt.SetActive(true);
            select_normal.SetActive(false);
        }
        else
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(true);
        }
        select_legend.SetActive(minionObject.legend);
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
        dragMinionNum = minionObject.num;
        DragLineRenderer.instance.lineRenderer.enabled = true;
        DragLineRenderer.instance.startPos = transform.position;
        DragLineRenderer.instance.InitMask();
        DragLineRenderer.instance.AddMask(타겟.적영웅);
        DragLineRenderer.instance.AddMask(타겟.적하수인);
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

    #region[pointerUp]
    public void pointerUp()
    {
        dragMinionNum = -1;
        DragLineRenderer.instance.lineRenderer.enabled = false;
        DragLineRenderer.instance.InitMask();
        if(DragLineRenderer.instance.dragTargetPos != Vector2.zero)
        {
            int n = minionObject.num;
            GameEventManager.instance.EventAdd(2f);
            MinionField.instance.attack_ready = 1f;
            minionObject.stealth = false;
            MinionField.instance.minions_Attack_pos[n] = new Vector3(DragLineRenderer.instance.dragTargetPos.x, DragLineRenderer.instance.dragTargetPos.y, minionObject.transform.position.z);
            SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.공격);
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
