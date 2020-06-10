using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCollectionsSelectCostBtn : Btn
{
    public int costnum;

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
        if (Input.GetMouseButton(0))
            btnAni.SetBool("Glow", true);
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        if (Input.GetMouseButtonDown(0))
            btnAni.SetBool("Glow", true);
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        btnAni.SetBool("Glow", false);
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
        MyCollectionsMenu.instance.ActFilterCostBtn(costnum);

    }
    #endregion


}