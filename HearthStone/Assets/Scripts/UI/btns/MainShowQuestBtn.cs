using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainShowQuestBtn : Btn
{
    public Image glowImg;
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
            if (glowImg)
                glowImg.enabled = false;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if (Input.GetMouseButton(0))
            if (glowImg)
                glowImg.enabled = true;
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        if (Input.GetMouseButtonDown(0))
            if (glowImg)
                glowImg.enabled = true;
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        if (glowImg)
            glowImg.enabled = false;
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
        Debug.Log("상점");
        MainMenu.instance.questUI.SetBool("Open", flag);
    }
    #endregion
}
