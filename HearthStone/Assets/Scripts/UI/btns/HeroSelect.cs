using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelect : Btn
{
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

    #region[영웅 선택 이펙트 설정]
    public void SetSelect(bool b)
    {
        if ((enemy && DragLineRenderer.instance.CheckMask(타겟.적영웅)) || (!enemy && DragLineRenderer.instance.CheckMask(타겟.아군영웅)))
        {
            DragLineRenderer.instance.selectTarget = b;
            DragLineRenderer.instance.dragTargetPos = transform.position;
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
