using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPack : Btn
{
    public int cardNum;

    public float value;
    public bool flag = false;
    public Image[] glowImages = new Image[3];

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        if(flag)
            value += Time.deltaTime;
        else
            value -= Time.deltaTime;

        value = Mathf.Max(value, 0);
        value = Mathf.Min(value, 172 / 255f);

        Color newColor = new Color();
        if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "전설")
            newColor = new Color(1, 172 / 255f, 0);
        else if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "특급")
            newColor = new Color(164/255f, 0, 149/255f);
        else if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "희귀")
            newColor = new Color(0, 85/255f, 164 / 255f);
        else
            newColor = new Color(0, 0, 0);

        for (int i = 0; i < glowImages.Length; i++)
            glowImages[i].color = new Color(newColor.r, newColor.g, newColor.b, value);
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        flag = true;
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
        if (!btnAni.GetBool("Open"))
            flag = false;
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
        btnAni.SetBool("Open", true);
        OpenPackMenu.instance.packCardView[cardNum].updateCard = true;
        OpenPackMenu.instance.cardOpenNum++;
        value = 172 / 255f;

        if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "전설")
            SoundManager.instance.PlaySE("전설카드");
        else if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "특급")
            SoundManager.instance.PlaySE("특급카드");
        else if (OpenPackMenu.instance.packCardView[cardNum].cardLevel == "희귀")
            SoundManager.instance.PlaySE("희귀카드");

        for (int i = 0; i < glowImages.Length; i++)
            glowImages[i].color = new Color(glowImages[i].color.r, glowImages[i].color.g, glowImages[i].color.b, value);
        if (OpenPackMenu.instance.cardOpenNum >= 5)
        {
            OpenPackMenu.instance.cardOpenNum = 0;
            OpenPackMenu.instance.ActOpenCheck(1.5f);
        }
    }
    #endregion
}
