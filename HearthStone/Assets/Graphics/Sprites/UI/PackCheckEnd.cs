using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackCheckEnd : Btn
{
    public int cardNum;

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
        ActBtn();
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

    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        btnAni.SetBool("Open", false);
        for (int i = 0; i < OpenPackMenu.instance.openPackBtn.Length; i++)
            OpenPackMenu.instance.openPackBtn[i].btnAni.SetBool("Open", false);
        for (int i = 0; i < 5; i++)
        {
            OpenPackMenu.instance.openPackBtn[i].flag = false;
            OpenPackMenu.instance.openPackBtn[i].value = 0;
            for (int j = 0; j < OpenPackMenu.instance.openPackBtn[i].glowImages.Length; j++)
                OpenPackMenu.instance.openPackBtn[i].glowImages[j].color = new Color(0, 0, 0, 0);
        }
        OpenPackMenu.instance.openCheckBtn.SetActive(false);
    }
    #endregion
}
