using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckCardDragBtn : MonoBehaviour
{
    public CardDrag cardDrag;
    public GameObject dragEffect;
    public BoxCollider2D collider2D;
    public Transform changeDragObject;
    public ScrollRect scrollRect;
    string cardName;
    bool inMouse = false;
    bool drag = false;
    public void Update()
    {
        if (MyCollectionsMenu.instance.deckSetting.activeSelf)
            return;
        if (MyCollectionsMenu.instance.makeAni.GetBool("Show"))
            return;
        if (MyCollectionsMenu.instance.filterAni.GetBool("Show"))
            return;
        InMouse();
        Drag();
    }

    private void OnEnable()
    {
        drag = false;
        inMouse = false;
    }

    public void InMouse()
    {
        Vector2 v = (Vector2)transform.position + collider2D.offset;
        Vector2 mouse = Input.mousePosition;
        if (v.x + collider2D.size.x / 2 < mouse.x || v.x - collider2D.size.x / 2 > mouse.x || v.y + collider2D.size.y / 2 < mouse.y || v.y - collider2D.size.y / 2 > mouse.y)
            inMouse = false;
        else
            inMouse = true;
    }

    public void Drag()
    {
        dragEffect.SetActive(drag);
        if (inMouse && Input.GetMouseButtonDown(0))
            drag = true;

        if(drag && Input.mousePosition.x < changeDragObject.position.x)
        {
            scrollRect.enabled = false;
            scrollRect.enabled = true;
            drag = false;

            cardName = cardDrag.cardName_Data;
            DataMng.instance.playData.deck[MyCollectionsMenu.instance.nowDeck].PopCard(cardName);
            CardDragObject.instance.isDrag = true;
            for (int i = 0; i < MyCollectionsMenu.instance.cardDatas.Length; i++)
                for (int j = 0; j < MyCollectionsMenu.instance.cardDatas[i].Count; j++)
                    if (MyCollectionsMenu.instance.cardDatas[i][j].cardName == cardName)
                        MyCollectionsMenu.instance.CardShow(ref CardDragObject.instance.cardView, i, j);
            CardDragObject.instance.cardView.gameObject.SetActive(false);
            CardDragObject.instance.cardDrag.cardName_Data = cardName;

            CardViewManager.instance.UpdateCardView();
        }

        if (Input.GetMouseButtonUp(0))
            drag = false;
    }
}
