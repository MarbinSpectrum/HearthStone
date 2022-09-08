using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainToOpenPacksBtn : Btn
{
    public Image glowImg;

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
            glowImg.enabled = true;
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        if (Input.GetMouseButtonDown(0))
            glowImg.enabled = true;
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
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
        SoundManager.instance.PlayBGM("");
        MainMenu mainMenu = MainMenu.instance;

        mainMenu.OpenBoard();
        mainMenu.PackOpenMenu(true);
    }
    #endregion
}
