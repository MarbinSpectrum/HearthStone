using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnChangeBtn : Btn
{
    #region[Awake]
    public override void Awake()
    {
        if (btnEvent)
            AddEvent();
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
        ActBtn();
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

    #region[ActBtn]
    public override void ActBtn()
    {
        if (GameEventManager.instance.EventCheck())
            return;

        if (btnAni.GetCurrentAnimatorStateInfo(0).IsName("턴종료_멈춤"))
        {
            btnAni.SetTrigger("상대턴");
            SoundManager.instance.PlaySE("턴종료버튼누름");
            TurnManager.instance.turnEndTrigger = true;
        }
        else if (btnAni.GetCurrentAnimatorStateInfo(0).IsName("상대턴_멈춤"))
        {
            //btnAni.SetTrigger("내턴");
            //TurnManager.instance.turnEndTrigger = true;
        }
    }
    #endregion


}
