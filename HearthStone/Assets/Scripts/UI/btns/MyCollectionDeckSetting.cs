﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCollectionDeckSetting : Btn
{
    public Button btn;

    #region[Awake]
    public override void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            pointerClick();
        });
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //    btnImg.sprite = btnSprites[(int)ButtonState.보통];
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
        if (Input.GetMouseButtonDown(0))
        {
            // btnImg.sprite = btnSprites[(int)ButtonState.누름];
            //SoundManager.instance.PlaySE("버튼클릭");
        }
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        //   btnImg.sprite = btnSprites[(int)ButtonState.보통];
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
        MyCollectionsMenu.instance.DeckSetting();
    }
    #endregion
}
