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

    private void OnEnable()
    {
        viewCardEffect.Stop();
        dragCardEffect.Stop();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[UiUpdate]

    public void UiUpdate()
    {
        MyCollectionsMenu myCollectionsMenu = MyCollectionsMenu.instance;
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        inDeck = !(transform.position.x < changeDragObject.transform.position.x);
        transform.position = Input.mousePosition;
        if (isDrag && myCollectionsMenu.deckCardViewFlag)
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
            //드래그 중이고 
            //드래그 객체가 덱 위치인 경우
            if (Input.GetMouseButtonUp(0))
            {
                //마우스 포인터를 땟을때
                if (playData.deck[myCollectionsMenu.nowDeck].CountCardNum() < 30)
                {
                    //덱의 카드 갯수가 30개 미만일때
                    //카드를 덱에 넣는다.
                    string cardName = cardDrag.cardName_Data;
                    playData.deck[myCollectionsMenu.nowDeck].AddCard(cardName);
                    SoundManager.instance.PlaySE("덱에카드넣기");
                    myCollectionsMenu.DeckSort();
                }
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
