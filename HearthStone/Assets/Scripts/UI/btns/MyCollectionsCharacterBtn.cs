using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCollectionsCharacterBtn : Btn
{
    public int jobNum;
    public bool act;

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
        //if (Input.GetMouseButtonDown(0))
        //    SoundManager.instance.PlaySE("버튼클릭");
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
        MyCollectionsMenu.instance.SelectCharacterOK(act, jobNum);
    }
    #endregion


}
