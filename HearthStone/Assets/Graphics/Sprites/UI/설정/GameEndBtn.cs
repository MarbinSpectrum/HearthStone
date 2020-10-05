using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndBtn : Btn
{

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
        try
        {
            if (Input.GetMouseButton(0))
            {
                btnImg.sprite = btnSprites[(int)ButtonState.누름];
                SoundManager.instance.PlaySE("작은버튼");
            }
        }
        catch { }
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        try
        {
            if (Input.GetMouseButtonDown(0))
            {
                btnImg.sprite = btnSprites[(int)ButtonState.누름];
                SoundManager.instance.PlaySE("버튼클릭");
            }
        }
        catch { }
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        try
        {
            btnImg.sprite = btnSprites[(int)ButtonState.보통];
        }
        catch { }
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
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }
    #endregion
}
