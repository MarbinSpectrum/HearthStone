using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GotoMainBtn : Btn
{
    bool flag = false;

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
        ActBtn();
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

    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        if (flag) return;
        flag = true;
        GlobalFade.instance.FadeAni(0, 2);
        Invoke("GotoMain",2);
    }
    #endregion

    void GotoMain()
    {
        if (BattleMenu.instance)
        {
            BattleMenu.instance.gameObject.SetActive(true);
            BattleMenu.instance.findBattleAni.gameObject.SetActive(true);
        }
        SceneManager.LoadScene("Main");
    }

}
