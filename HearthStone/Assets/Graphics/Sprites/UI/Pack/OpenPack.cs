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
        UpdatePackGlow();
    }
    #endregion

    #region[Update Pack Glow]
    private void UpdatePackGlow()
    {
        OpenPackMenu openPackMenu = OpenPackMenu.instance;
        if (openPackMenu == null)
            return;

        if (flag)
            value += Time.deltaTime;
        else
            value -= Time.deltaTime;

        value = Mathf.Max(value, 0);
        value = Mathf.Min(value, 172 / 255f);

        Color newColor = new Color();
        if (openPackMenu.packCardView[cardNum].cardLevel == "전설")
            newColor = new Color(1, 172 / 255f, 0);
        else if (openPackMenu.packCardView[cardNum].cardLevel == "특급")
            newColor = new Color(164 / 255f, 0, 149 / 255f);
        else if (openPackMenu.packCardView[cardNum].cardLevel == "희귀")
            newColor = new Color(0, 85 / 255f, 164 / 255f);
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
        OpenPackMenu openPackMenu = OpenPackMenu.instance;
        if (openPackMenu == null)
            return;

        CardView cardData = openPackMenu.packCardView[cardNum];
        if (cardData == null)
            return;

        SoundManager soundManager = SoundManager.instance;
        if (soundManager == null)
            return;

        //카드 확인 애니메이션 실행
        btnAni.SetBool("Open", true);

        //카드 정보 갱신
        cardData.updateCard = true;

        //카드의 등급에 따라 효과음 결정
        if (cardData.cardLevel == "전설")
            soundManager.PlaySE("전설카드");
        else if (cardData.cardLevel == "특급")
            soundManager.PlaySE("특급카드");
        else if (cardData.cardLevel == "희귀")
            soundManager.PlaySE("희귀카드");

        //확인한 카드수 갱신
        openPackMenu.cardOpenNum++;
        value = 172 / 255f;

        for (int i = 0; i < glowImages.Length; i++)
            glowImages[i].color = new Color(glowImages[i].color.r, glowImages[i].color.g, glowImages[i].color.b, value);
        if (openPackMenu.cardOpenNum >= 5)
        {
            openPackMenu.cardOpenNum = 0;
            openPackMenu.ActOpenCheck(1.5f);
        }
    }
    #endregion
}
