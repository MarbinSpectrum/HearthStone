using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyCheckBtn : Btn
{
    public bool flag;

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
        ActBtn();
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {
        if(flag)
        {
            if (MainMenu.instance.shopUI.selectMenu - 100 == 0)
            {
                if (DataMng.instance.playData.gold < 100)
                    return;
                else
                {
                    SoundManager.instance.PlaySE("구매완료");
                    DataMng.instance.playData.gold -= 100;
                    for (int i = 0; i < 1; i++)
                        DataMng.instance.playData.packs.Add(new PlayData.Pack());
                }
            }

            if (MainMenu.instance.shopUI.selectMenu - 100 == 1)
            {
                if (DataMng.instance.playData.gold < 150)
                    return;
                else
                {
                    SoundManager.instance.PlaySE("구매완료");
                    DataMng.instance.playData.gold -= 150;
                    for (int i = 0; i < 2; i++)
                        DataMng.instance.playData.packs.Add(new PlayData.Pack());
                }
            }

            if (MainMenu.instance.shopUI.selectMenu - 100 == 2)
            {
                if (DataMng.instance.playData.gold < 500)
                    return;
                else
                {
                    SoundManager.instance.PlaySE("구매완료");
                    DataMng.instance.playData.gold -= 500;
                    for (int i = 0; i < 7; i++)
                        DataMng.instance.playData.packs.Add(new PlayData.Pack());
                }
            }

            if (MainMenu.instance.shopUI.selectMenu - 100 == 3)
            {
                if (DataMng.instance.playData.gold < 1000)
                    return;
                else
                {
                    SoundManager.instance.PlaySE("구매완료");
                    DataMng.instance.playData.gold -= 1000;
                    for (int i = 0; i < 15; i++)
                        DataMng.instance.playData.packs.Add(new PlayData.Pack());
                }
            }

            if (MainMenu.instance.shopUI.selectMenu - 100 == 4)
            {
                if (DataMng.instance.playData.gold < 2500)
                    return;
                else
                {
                    SoundManager.instance.PlaySE("구매완료");
                    DataMng.instance.playData.gold -= 2500;
                    for (int i = 0; i < 40; i++)
                        DataMng.instance.playData.packs.Add(new PlayData.Pack());
                }
            }

            DataMng.instance.SaveData();
          
        }
        MainMenu.instance.shopUI.checkBuy.SetBool("Open", flag);
    }
    #endregion
}
