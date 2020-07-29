using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCollectionsSelectJobBtn : Btn
{
    public int jobnum;

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
        SoundManager.instance.PlaySE("버튼클릭");
        ActBtn();
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        MyCollectionsMenu.instance.MovePage(jobnum);
        MyCollectionsMenu.instance.ActSelectJobBtn(false);

    }
    #endregion


}