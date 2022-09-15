using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChangeBtn : Btn
{
    [SerializeField] private GameObject changeItemObj;
    private bool change;
    [SerializeField] private int num;

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
        BattleUI battleUI = BattleUI.instance;
        Mulligan mulligan = battleUI.mulligan;

        SoundManager.instance.PlaySE("멀리건선택");

        change = !change;
        mulligan.ChangeMulligan(num, change);
        changeItemObj.SetActive(change);
    }
    #endregion


}
