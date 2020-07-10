using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragCardObject : MonoBehaviour
{
    public static DragCardObject instance;

    RectTransform rectTransform;

    public CardView dragCardView;
    public GameObject dragPoint;

    public int dragCardNum = 0;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        instance = this;
        dragCardView.hide = true;
    }

    public void Update()
    {
        if (!Input.GetMouseButton(0))
            HideDragCard();
        dragPoint.SetActive(!dragCardView.hide);
        rectTransform.anchoredPosition = Input.mousePosition;
    }

    public void HideDragCard()
    {
        dragCardView.hide = true;
    }

    public void DragAndDropCard()
    {
        if (CardHand.instance.canUse[dragCardNum])
            CardHand.instance.UseCard(dragCardNum);
        HideDragCard();
    }

    public void ShowDragCard(CardView cardView)
    {
        dragCardView.hide = false;
        CardViewManager.instance.CardShow(ref dragCardView, cardView);
        CardViewManager.instance.UpdateCardView(0.001f);
    }
}
