using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWheel : Btn
{
    public float changeSpeed = 0;

    float time = 0;

    #region[Awake]
    public override void Awake()
    {
        if (btnEvent)
            AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        if(time > 0)
        {
            btnAni.speed = changeSpeed;
            time -= Time.deltaTime;
        }
        else
            btnAni.speed = 1;
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
        time = 2;
    }
    #endregion


}
