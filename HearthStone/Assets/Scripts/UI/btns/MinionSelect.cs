using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinionSelect : Btn
{
    public MinionObject minionObject;

    public GameObject select;

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
        if(!Input.GetMouseButton(0))
            SetSelect(false); ;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
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
    public void SetSelect(bool b)
    {
        if ((enemy && DragLineRenderer.instance.CheckMask(타겟.적하수인)) || (!enemy && DragLineRenderer.instance.CheckMask(타겟.아군하수인)))
        {
            DragLineRenderer.instance.selectTarget = b;
            if (b == true)
                DragLineRenderer.instance.dragTargetPos = transform.position;
            else
                DragLineRenderer.instance.dragTargetPos = Vector2.zero;
            select.SetActive(b);
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
