using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackOpenSpace : Btn
{
    private bool inOpenSpace;
    private bool bgmFlag;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        UpdateOpenPack();
        UpdatePackOpenBgm();
    }
    #endregion

    #region[UpdateOpenPack]
    private void UpdateOpenPack()
    {
        OpenPackMenu openPackMenu = OpenPackMenu.instance;
        if (openPackMenu == null)
            return;

        if (Input.GetMouseButtonUp(0) && inOpenSpace)
        {
            //팩을 개봉장소까지 끌고갔을 경우
            inOpenSpace = false;
            bgmFlag = true;

            //카드팩을 개봉
            openPackMenu.OpenCardPack();
        }
    }
    #endregion

    #region[UpdatePackOpenBgm]
    private void UpdatePackOpenBgm()
    {
        if (!bgmFlag && inOpenSpace)
        {
            SoundManager.instance.PlayBGM("팩개봉시설");
            bgmFlag = true;
        }
        else if (bgmFlag && !inOpenSpace)
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
            inOpenSpace = true;
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
            inOpenSpace = false;
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
