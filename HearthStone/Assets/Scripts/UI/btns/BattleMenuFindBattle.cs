using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuFindBattle : Btn
{
    public bool find = false;

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
            btnImg.sprite = btnSprites[(int)ButtonState.보통];
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if (Input.GetMouseButton(0))
        {
            btnImg.sprite = btnSprites[(int)ButtonState.누름];
            SoundManager.instance.PlaySE("작은버튼");
        }
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            btnImg.sprite = btnSprites[(int)ButtonState.누름];
            SoundManager.instance.PlaySE("버튼클릭");
        }
    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        btnImg.sprite = btnSprites[(int)ButtonState.보통];
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
        Debug.Log("수집품");
        int deck = BattleMenu.instance.selectDeck;
        BattleMenu.instance.JobSelectCheck(-1);
        BattleMenu.instance.selectDeck = deck;
        BattleMenu.instance.FindBattle(find);
        if (find)
        {

            SoundManager.instance.PlayBGM("대전상대찾기");
        }
        else
            SoundManager.instance.StopBGM();
    }
    #endregion


}