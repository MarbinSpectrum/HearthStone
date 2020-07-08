using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHandBtn : Btn
{
    public bool closeUp;

    #region[Awake]
    public override void Awake()
    {
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
        ActBtn();
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        if (!BattleUI.instance.gameStart)
            return;

        if (closeUp)
            btnAni.SetTrigger("확대");
        else
            btnAni.SetTrigger("축소");
    }
    #endregion


}