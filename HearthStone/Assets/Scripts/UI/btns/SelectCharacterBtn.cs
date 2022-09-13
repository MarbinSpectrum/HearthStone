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
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;
        MyCollectionsMenu myCollectionsMenu = MyCollectionsMenu.instance;

        if (playData.deck.Count >= PlayData.Deck.MAX_DECK_NUM)
        {
            //현재 덱 최대 매수는 9개다.
            //PlayData.Deck.MAX_DECK_NUM = 9
            return;
        }

        playData.deck.Add(
            new PlayData.Deck("나만의 " + myCollectionsMenu.nowJob.ToString() + " 덱",
            myCollectionsMenu.nowJob, 
            new List<string>() { }
            ));

        myCollectionsMenu.MovePage((int)myCollectionsMenu.nowJob);
        myCollectionsMenu.DeckCardView(playData.deck.Count - 1);
    }
    #endregion


}
