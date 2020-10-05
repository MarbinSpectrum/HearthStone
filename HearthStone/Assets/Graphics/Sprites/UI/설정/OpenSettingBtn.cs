using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSettingBtn : Btn
{
    public bool flag;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        if (Input.GetMouseButtonUp(0))
            if (btnImg)
                btnImg.color = new Color(1, 196 / 255f, 0, 0);
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
        if (btnImg)
            btnImg.color = new Color(1, 196/255f, 0, 125 / 255f);
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
        if (flag == btnAni.GetBool("Open"))
            return;
        btnAni.SetBool("Open", flag);
    }
    #endregion
}
