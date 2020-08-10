using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroDrag : Btn
{
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

    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
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
        DragLineRenderer.instance.lineRenderer.enabled = false;
        DragLineRenderer.instance.InitMask();
        if (DragLineRenderer.instance.dragTargetPos != Vector2.zero)
            HeroManager.instance.heroAtkManager.HeroAttack(enemy, DragLineRenderer.instance.dragTargetPos);
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
