using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackOpenSpace : Btn
{
    bool flag;

    bool bgmFlag;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        if(Input.GetMouseButtonUp(0) && flag)
        {
            OpenPackMenu.instance.packOpenAni.SetBool("Light", false);
            flag = false;
            bgmFlag = true;
            OpenPackMenu.instance.packAni.SetBool("Open", true);
            OpenPackMenu.instance.OpenCardPack();
            SoundManager.instance.PlaySE("팩개봉");
        }

        if (!bgmFlag && flag)
        {
            SoundManager.instance.PlayBGM("팩개봉시설");

            bgmFlag = true;
        }
        else if(bgmFlag && !flag)
        {
            SoundManager.instance.PlayBGM("");
            bgmFlag = false;
        }
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if (OpenPackMenu.instance.dragObj.gameObject.activeSelf)
        {
            OpenPackMenu.instance.packOpenAni.SetBool("Light", true);
            flag = true;
            bgmFlag = false;
        }
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
        if (OpenPackMenu.instance.dragObj.gameObject.activeSelf)
        {
            OpenPackMenu.instance.packOpenAni.SetBool("Light", false);
            flag = false;
            bgmFlag = true;
        }
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
       
    }
    #endregion
}
