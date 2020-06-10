using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour
{
    public string cardName_Data;
    public RectTransform imgRect;
    public Image cardImg;
    public Image cardCost;
    public Text cardName;
    public Image deckCardNum;
    public bool isDrag = false;

    #region[Update]
    private void Update()
    {
        CardImg();
        CardName();
        CardCost();
        DeckCardNum();
    }
    #endregion

    #region[카드이미지]
    public void CardImg()
    {
        cardImg.sprite = DataMng.instance.cardImg[cardName_Data];
        imgRect.anchoredPosition = DataMng.instance.dragCardPos[cardName_Data];
    }
    #endregion

    #region[카드이름]
    public void CardName()
    {
        string temp = "";
        for (int i = 0; i < cardName_Data.Length; i++)
        {
            if (cardName_Data[i].Equals('('))
                break;
            else
                temp += cardName_Data[i];
        }
        cardName.text = cardName_Data;
    }
    #endregion

    #region[카드코스트]
    public void CardCost()
    {
        cardCost.sprite = DataMng.instance.num[DataMng.instance.playData.GetCardNum(cardName_Data)];
    }
    #endregion

    #region[덱에 있는 카드의 갯수]
    public void DeckCardNum()
    {
        if (isDrag)
        {
            deckCardNum.enabled = false;
        }
        else
            deckCardNum.enabled = true;
    }
    #endregion

}
