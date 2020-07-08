using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mulligan : MonoBehaviour
{
    //1 후공   2 선공
    int r = 2;

    public CardView[] cardView;
    public Animator cardAnimator;
    public GameObject[] cardGlow;
    public GameObject mulliganUI;

    public Animator coinAnimator;
    public Material coinMat;
    public Animator createCoinCard;
    public Transform coinCardPos;

    public CardChangeBtn[] cardChangeBtns;

    public Animator firstCardAni;
    public CardView[] firstTurnCardView;

    public Animator secondCardAni;
    public CardView[] secondTurnCardView;

    public void Update()
    {

    }

    public void MulliganCardSet()
    {
        mulliganUI.SetActive(false);

        //0 아무상태아님 //1 카드를 넣음 // 2카드를 뽑음
        if (r == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                secondTurnCardView[i].gameObject.SetActive(cardChangeBtns[i].change);
                CardViewManager.instance.CardShow(ref secondTurnCardView[i], cardView[i]);
                cardView[i].gameObject.SetActive(!cardChangeBtns[i].change);
                cardChangeBtns[i].btnImg.enabled = false;
                cardChangeBtns[i].enabled = false;
                cardChangeBtns[i].change = true;
            }
            secondCardAni.gameObject.SetActive(true);
            secondCardAni.SetInteger("State",1);
        }
        else if (r == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                firstTurnCardView[i].gameObject.SetActive(cardChangeBtns[i].change);
                CardViewManager.instance.CardShow(ref firstTurnCardView[i], cardView[i]);
                cardView[i].gameObject.SetActive(!cardChangeBtns[i].change);
                cardChangeBtns[i].btnImg.enabled = false;
                cardChangeBtns[i].enabled = false;
                cardChangeBtns[i].change = true;
            }
            firstCardAni.gameObject.SetActive(true);
            firstCardAni.SetInteger("State", 1);
        }
        StartCoroutine(MulliganDrawCard(1.5f));
        CardViewManager.instance.UpdateCardView();
    }

    private IEnumerator MulliganDrawCard(float waitTime)
    {
        StartCoroutine(InputCard(0.1f));

        yield return new WaitForSeconds(waitTime);

        StartCoroutine(DrawCard(0.1f));
        bool changeCard = false;
        if (r == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                if (secondTurnCardView[i].gameObject.activeSelf)
                {
                    changeCard = true;
                    string name = "";
                    if (secondTurnCardView[i].cardType == CardType.무기)
                        name = secondTurnCardView[i].WeaponCardNameData;
                    else if (secondTurnCardView[i].cardType == CardType.주문)
                        name = secondTurnCardView[i].SpellCardNameData;
                    else if (secondTurnCardView[i].cardType == CardType.하수인)
                        name = secondTurnCardView[i].MinionsCardNameData;
                    InGameDeck.instance.playDeck.Add(name);
                    CardViewManager.instance.CardShow(ref secondTurnCardView[i], InGameDeck.instance.playDeck[0]);
                    CardViewManager.instance.UpdateCardView();
                    InGameDeck.instance.playDeck.RemoveAt(0);
                    InGameDeck.instance.Shuffle(1000);
                }
            }
            secondCardAni.SetInteger("State", 2);
            yield return new WaitForSeconds(changeCard ? 1.5f : 0.1f);
            CardHand.instance.nowHandNum = 4;
            for (int i = 0; i < 4; i++)
                CardHand.instance.CardMove(secondTurnCardView[i], i, cardView[i].transform.position, new Vector2(10.685f, 13.714f), 0);
            yield return new WaitForSeconds(0.001f);
            CardViewManager.instance.UpdateCardView();
            for (int i = 0; i < cardView.Length; i++)
                cardView[i].gameObject.SetActive(false);
            firstCardAni.gameObject.SetActive(false);
            secondCardAni.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            coinAnimator.SetInteger("State", 3);
            yield return new WaitForSeconds(1f);
            createCoinCard.SetBool("Hide", false);
            yield return new WaitForSeconds(1.1f);
            createCoinCard.SetBool("Hide", true);
            BattleUI.instance.fieldShadowAni.SetInteger("State", 1);
            CardHand.instance.nowHandNum++;
            CardHand.instance.CardMove("동전 한 닢", 4, coinCardPos.position, new Vector2(10.685f, 13.714f), 0);
            yield return new WaitForSeconds(0.001f);
            CardViewManager.instance.UpdateCardView();
        }
        else if (r == 2)
        {
            for (int i = 0; i < 3; i++)
            {
                if (firstTurnCardView[i].gameObject.activeSelf)
                {
                    changeCard = true;
                    string name = "";
                    if (firstTurnCardView[i].cardType == CardType.무기)
                        name = firstTurnCardView[i].WeaponCardNameData;
                    else if (firstTurnCardView[i].cardType == CardType.주문)
                        name = firstTurnCardView[i].SpellCardNameData;
                    else if (firstTurnCardView[i].cardType == CardType.하수인)
                        name = firstTurnCardView[i].MinionsCardNameData;
                    InGameDeck.instance.playDeck.Add(name);
                    CardViewManager.instance.CardShow(ref firstTurnCardView[i], InGameDeck.instance.playDeck[0]);
                    CardViewManager.instance.UpdateCardView();
                    InGameDeck.instance.playDeck.RemoveAt(0);
                    InGameDeck.instance.Shuffle(1000);
                }
            }
            firstCardAni.SetInteger("State", 2);
            yield return new WaitForSeconds(changeCard ? 1.5f : 0.1f);
            CardHand.instance.nowHandNum = 3;
            for (int i = 0; i < 3; i++)
                CardHand.instance.CardMove(firstTurnCardView[i],i, cardView[i].transform.position, new Vector2(10.685f, 13.714f), 0);
            yield return new WaitForSeconds(0.001f);
            CardViewManager.instance.UpdateCardView();
            for (int i = 0; i < cardView.Length; i++)
                cardView[i].gameObject.SetActive(false);
            firstCardAni.gameObject.SetActive(false);
            secondCardAni.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            coinAnimator.SetInteger("State", 3);
            yield return new WaitForSeconds(1f);
            BattleUI.instance.fieldShadowAni.SetInteger("State", 1);
        }
        yield return new WaitForSeconds(1f);
        BattleUI.instance.gameStart = true;
    }

    #region[세팅]
    public void SetGoing(float n)
    {
        StartCoroutine(DrawCard(n - 1f));
        StartCoroutine(ShowMulligan(n));

        r = Random.Range(0, 100) > 50 ? 1 : 2;

        SetMulligan(r);
        StartCoroutine(SetCoin(n + 1.5f, r));
        StartCoroutine(CardGlow(n + (r == 2 ? 4.5f : 4f), r));
    }
    #endregion

    #region[멀리건]
    public void SetMulligan(int n)
    {
        for (int i = 0; i < (n == 1 ? 4 : 3); i++)
        {
            CardViewManager.instance.CardShow(ref cardView[i], InGameDeck.instance.playDeck[0]);
            CardViewManager.instance.UpdateCardView();
            //Debug.Log(InGameDeck.instance.playDeck[0] + ":" + cardView[i].cardN);
            InGameDeck.instance.playDeck.RemoveAt(0);
        }
        InGameDeck.instance.Shuffle(1000);

    }

    private IEnumerator ShowMulligan(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cardAnimator.SetInteger("State", 1);
    }

    private IEnumerator DrawCard(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        BattleUI.instance.playerCardAni[24].SetTrigger("Draw");
        yield return new WaitForSeconds(0.5f);
        BattleUI.instance.playerCardAni[12].SetTrigger("Draw");
        yield return new WaitForSeconds(0.5f);
        BattleUI.instance.playerCardAni[0].SetTrigger("Draw");
    }

    private IEnumerator InputCard(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        BattleUI.instance.playerCardAni[24].SetTrigger("Input");
        yield return new WaitForSeconds(0.5f);
        BattleUI.instance.playerCardAni[12].SetTrigger("Input");
        yield return new WaitForSeconds(0.5f);
        BattleUI.instance.playerCardAni[0].SetTrigger("Input");
    }
    #endregion

    #region[코인]
    private IEnumerator SetCoin(float waitTime,int n)
    {
        yield return new WaitForSeconds(waitTime);
        cardAnimator.SetInteger("State", n);
        coinAnimator.SetInteger("State", n);
    }
    #endregion

    #region[카드 글로우]
    private IEnumerator CardGlow(float waitTime, int n)
    {
        yield return new WaitForSeconds(waitTime);
        cardGlow[n - 1].SetActive(true);
        mulliganUI.SetActive(true);
    }
    #endregion
}
