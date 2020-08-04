using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChooseOne : Btn
{
    public int eventNum;

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
        if (MinionManager.instance.selectChoose == -1)
            MinionManager.instance.selectChoose = eventNum;
        else if (SpellManager.instance.selectChoose == -1)
            SpellManager.instance.selectChoose = eventNum;
        BattleUI.instance.chooseOneDruid.SetBool("Hide", true);
    }
    #endregion


}
