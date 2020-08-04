using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMinionBtn : Btn
{
    public MinionObject minionObject;

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
        if(!SpellManager.instance.selectSpellEvent)
        {
            if (!DragLineRenderer.instance.CheckActObj(minionObject.gameObject))
                MinionManager.instance.MinionSelect(minionObject);
            else
                MinionManager.instance.MinionSelectCancle();
        }
        else
        {
            if (!DragLineRenderer.instance.CheckActObj(minionObject.gameObject))
                SpellManager.instance.MinionSelect(minionObject);
            else
                SpellManager.instance.MinionSelectCancle();
        }
    }
    #endregion


}
