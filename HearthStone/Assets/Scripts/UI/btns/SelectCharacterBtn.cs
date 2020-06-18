using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterBtn : Btn
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
        DataMng.instance.playData.deck.Add(
            new PlayData.Deck("나만의 " + MyCollectionsMenu.instance.nowJob.ToString() + " 덱", 
            MyCollectionsMenu.instance.nowJob, 
            new List<string>() { }
            ));
       // MyCollectionsMenu.instance.newDeckPos = transform.parent.GetComponent<RectTransform>();
        MyCollectionsMenu.instance.MovePage((int)MyCollectionsMenu.instance.nowJob);
        MyCollectionsMenu.instance.DeckCardView(DataMng.instance.playData.deck.Count - 1);
    }
    #endregion


}
