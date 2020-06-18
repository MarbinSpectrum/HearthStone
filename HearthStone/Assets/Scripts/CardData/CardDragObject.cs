using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragObject : MonoBehaviour
{
    public static CardDragObject instance;

    [HideInInspector] public bool isDrag;
    [HideInInspector] public bool inDeck;
    public CardView cardView;
    public CardDrag cardDrag;
    public ParticleSystem viewCardEffect;
    public ParticleSystem dragCardEffect;

    public Transform changeDragObject;
    IEnumerator showCardView;
    bool viewFlag = false;
    IEnumerator showCardDrag;
    bool dragFlag = false;

    #region[Awake]
    public void Awake()
    {
        instance = this;
        viewCardEffect.Stop();
        dragCardEffect.Stop();
    }
    #endregion

    #region[Update]
    public void Update()
    {
        UiUpdate();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[UiUpdate]

    public void UiUpdate()
    {
        inDeck = !(transform.position.x < changeDragObject.transform.position.x);
        transform.position = Input.mousePosition;
        if (isDrag && MyCollectionsMenu.instance.deckCardViewFlag)
        {
            if(!inDeck)
            {
                if (!viewFlag)
                {
                    viewFlag = true;
                    dragFlag = false;
                    if (showCardDrag != null)
                        StopCoroutine(showCardDrag);
                    showCardView = ShowCardView(0.2f);
                    StartCoroutine(showCardView);
                }
            }
            else
            {
                if (!dragFlag)
                {
                    dragFlag = true;
                    viewFlag = false;
                    if (showCardView != null)
                        StopCoroutine(showCardView);
                    showCardDrag = ShowCardDrag(0.2f);
                    StartCoroutine(showCardDrag);
                }
            }
        }
        else
        {
            viewFlag = false;
            dragFlag = false;
            cardView.enabled = false;
            cardDrag.enabled = false;
            cardView.gameObject.SetActive(false);
            cardDrag.gameObject.SetActive(false);
        }

        if (isDrag && inDeck)
        {
            if (Input.GetMouseButtonUp(0))
            {
               // Debug.LogError("!!");
                string cardName = cardDrag.cardName_Data;
                DataMng.instance.playData.deck[MyCollectionsMenu.instance.nowDeck].AddCard(cardName);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            viewCardEffect.Stop();
            dragCardEffect.Stop();
            isDrag = false;
        }

    }
    #endregion

    #region[카드보여주기]
    private IEnumerator ShowCardView(float waitTime)
    {
        viewCardEffect.Play();
        cardDrag.enabled = false;
        cardDrag.gameObject.SetActive(false);
        yield return new WaitForSeconds(waitTime);
        cardView.enabled = true;
        cardView.gameObject.SetActive(true);
    }

    private IEnumerator ShowCardDrag(float waitTime)
    {
        dragCardEffect.Play();
        cardView.enabled = false;
        cardView.gameObject.SetActive(false);
        yield return new WaitForSeconds(waitTime);
        cardDrag.enabled = true;
        cardDrag.gameObject.SetActive(true);
    }
    #endregion
}
