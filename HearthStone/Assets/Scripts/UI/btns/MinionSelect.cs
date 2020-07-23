using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinionSelect : Btn
{
    public MinionObject minionObject;

    public GameObject select_normal;
    public GameObject select_taunt;
    public GameObject select_legend;
    bool select = false;

    public bool enemy;

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
        enemy = minionObject.enemy;
        if(Input.GetMouseButtonUp(0))
            SetSelect(false);
        if (minionObject.taunt)
        {
            select_taunt.SetActive(select);
            select_normal.SetActive(false);
        }
        else
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(select);
        }
        select_legend.SetActive(minionObject.legend && select);

    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if (Input.GetMouseButton(0))
            SetSelect(true);
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
        SetSelect(false);
        if (Input.GetMouseButton(0))
        {
            AttackManager.instance.PopDamageObj(minionObject.damageEffect);
            if(MinionDrag.dragMinionNum != -1)
                AttackManager.instance.PopDamageObj(MinionField.instance.minions[MinionDrag.dragMinionNum].damageEffect);
        }
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
        SetSelect(false);
    }
    #endregion

    #region[하수인 선택 이펙트 설정]
    public void SetSelect(bool flag)
    {
        if (!flag)
        {
            DragLineRenderer.instance.selectTarget = false;
            DragLineRenderer.instance.dragTargetPos = Vector2.zero;
            select = false;
        }
        else if ((enemy && DragLineRenderer.instance.CheckMask(타겟.적하수인)) || (!enemy && DragLineRenderer.instance.CheckMask(타겟.아군하수인)))
        {
            if (!DragLineRenderer.instance.CheckMask(타겟.실행주체))
                if (DragLineRenderer.instance.CheckActObj(gameObject))
                    return;
            if (!minionObject.stealth && MinionManager.instance.CheckTaunt(minionObject))
            {
                DragLineRenderer.instance.selectTarget = true;
                DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);
                AttackManager.instance.AddDamageObj(minionObject.damageEffect, MinionField.instance.minions[MinionDrag.dragMinionNum].final_atk);
                AttackManager.instance.AddDamageObj(MinionField.instance.minions[MinionDrag.dragMinionNum].damageEffect, minionObject.final_atk);
            }
            select = true;
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
