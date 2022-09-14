using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mulligan : MonoBehaviour
{
    public enum DrawType
    {
        없음 = 0,
        후공 = 1,
        선공 = 2
    }
    DrawType drawType;
    private int[] drawNum = { 0, 4, 3 };

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

    public Animator showCharacterAni;

    #region[카드세트]
    public void MulliganCardSet()
    {
        mulliganUI.SetActive(false);

        CardView[] cardViews = (drawType == DrawType.후공) ?
            secondTurnCardView : firstTurnCardView;
        Animator cardAni = (drawType == DrawType.후공) ?
            secondCardAni : firstCardAni;

        int drawCardNum = drawNum[(int)drawType];
        for (int i = 0; i < drawCardNum; i++)
        {
            cardViews[i].gameObject.SetActive(cardChangeBtns[i].change);
            CardViewManager.instance.CardShow(ref cardViews[i], cardView[i]);
            cardView[i].gameObject.SetActive(!cardChangeBtns[i].change);
            cardChangeBtns[i].btnImg.enabled = false;
            cardChangeBtns[i].enabled = false;
            cardChangeBtns[i].change = true;
        }
        cardAni.gameObject.SetActive(true);
        cardAni.SetInteger("State", 1);

        StartCoroutine(MulliganDrawCard(1.5f));
        CardViewManager.instance.UpdateCardView();
    }

    private IEnumerator EnemyDrawCard(int n,bool coin = false)
    {
        for (int i = 0; i < n; i++)
        {
            EnemyCardHand.instance.DrawCard();
            yield return new WaitForSeconds(0.5f);
        }
        if(coin)
            EnemyCardHand.instance.AddCard("동전 한 닢");
    }

    private IEnumerator MulliganDrawCard(float waitTime)
    {
        StartCoroutine(InputCard(0.1f));

        yield return new WaitForSeconds(waitTime);

        StartCoroutine(DrawCard(0.1f));
        bool changeCard = false;
        if (drawType == DrawType.후공)
        {
            StartCoroutine(EnemyDrawCard(drawNum[(int)DrawType.선공]));
            for (int i = 0; i < drawNum[(int)drawType]; i++)
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
                    InGameDeck.instance.PushCard(name);
                    string topCard = InGameDeck.instance.GetTopCard();
                    CardViewManager.instance.CardShow(ref secondTurnCardView[i], topCard);
                    CardViewManager.instance.UpdateCardView();
                    InGameDeck.instance.PopTopCard();
                    InGameDeck.instance.Shuffle(1000);
                }
            }
            secondCardAni.SetInteger("State", 2);
            yield return new WaitForSeconds(changeCard ? 1.5f : 0.1f);
            CardHand.instance.nowHandNum = drawNum[(int)drawType];
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
            SoundManager.instance.PlaySE("코인얻기");
            yield return new WaitForSeconds(2f);
            createCoinCard.SetBool("Hide", true);
            BattleUI.instance.fieldShadowAni.SetInteger("State", 1);
            CardHand.instance.DrawCard();
            CardHand.instance.CardMove("동전 한 닢", 4, coinCardPos.position, new Vector2(10.685f, 13.714f), 0);
            yield return new WaitForSeconds(0.001f);
            CardViewManager.instance.UpdateCardView();
        }
        else if (drawType == DrawType.선공)
        {
            StartCoroutine(EnemyDrawCard(drawNum[(int)DrawType.후공], true));
            for (int i = 0; i < drawNum[(int)drawType]; i++)
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
                    InGameDeck.instance.PushCard(name);
                    string topCard = InGameDeck.instance.GetTopCard();
                    CardViewManager.instance.CardShow(ref firstTurnCardView[i], topCard);
                    CardViewManager.instance.UpdateCardView();
                    InGameDeck.instance.PopTopCard();
                    InGameDeck.instance.Shuffle(1000);
                }
            }
            firstCardAni.SetInteger("State", 2);
            yield return new WaitForSeconds(changeCard ? 1.5f : 0.1f);
            CardHand.instance.nowHandNum = drawNum[(int)drawType];
            for (int i = 0; i < drawNum[(int)drawType]; i++)
                CardHand.instance.CardMove(firstTurnCardView[i], i, cardView[i].transform.position, new Vector2(10.685f, 13.714f), 0);
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
        HeroManager.instance.playerHero.gameObject.SetActive(true);
        HeroManager.instance.enemyHero.gameObject.SetActive(true);
        HeroManager.instance.heroHpManager.playerHp.SetActive(true);
        HeroManager.instance.heroHpManager.enemyHp.SetActive(true);
        TurnManager.instance.turnBtn.SetActive(true);
        if (drawType == DrawType.후공)
            TurnManager.instance.turnBtnAni.SetTrigger("상대턴");      

        BattleUI.instance.gameStart = true;
        SoundManager.instance.PlayBGM("대전맵배경음");
        TurnManager.instance.turnEndTrigger = true;
        showCharacterAni.enabled = false;
        HeroManager.instance.heroAtkManager.playerObjectAni.enabled = true;
        HeroManager.instance.heroAtkManager.enemyObjectAni.enabled = true;
    }

    public void SetGoing(float n)
    {
        StartCoroutine(DrawCard(n - 1f));
        StartCoroutine(ShowMulligan(n));

        if(Random.Range(0, 100) < 50)
        {
            //선공
            drawType = DrawType.선공;
            TurnManager.instance.FirstTurnPlayer(Turn.플레이어);
            SoundManager.instance.PlaySE("멀리건선택선공");
        }
        else
        {
            //후공
            drawType = DrawType.후공;
            TurnManager.instance.FirstTurnPlayer(Turn.상대방);
            SoundManager.instance.PlaySE("멀리건선택후공");
        }

        SetMulligan(drawType);
        StartCoroutine(SetCoin(n + 1.5f, drawType));
        StartCoroutine(CardGlow(n + (drawType == DrawType.선공 ? 4.5f : 4f), drawType));
    }
    #endregion

    #region[멀리건]
    public void SetMulligan(DrawType draw)
    {
        //선공이면 3장 후공이면 4장 받는다.
        int drawCardNum = drawNum[(int)draw];

        for (int i = 0; i < drawCardNum; i++)
        {
            //아래의 과정을 뽑는 카드 수 만큼 반복한다.

            //덱 맨위의 카드를 뽑는다.
            string topCard = InGameDeck.instance.GetTopCard();
            InGameDeck.instance.PopTopCard();

            //카드객체로 해당 카드를 표시한다.
            CardViewManager.instance.CardShow(ref cardView[i], topCard);
            CardViewManager.instance.UpdateCardView();
        }

        //덱을 섞는다.
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
        yield return new WaitForSeconds(0.5f);
        if (drawType == DrawType.후공)
            BattleUI.instance.playerCardAni[6].SetTrigger("Draw");

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
    private IEnumerator SetCoin(float waitTime, DrawType draw)
    {
        yield return new WaitForSeconds(waitTime);
        cardAnimator.SetInteger("State", (int)draw);
        coinAnimator.SetInteger("State", (int)draw);
    }
    #endregion

    #region[카드 글로우]
    private IEnumerator CardGlow(float waitTime, DrawType draw)
    {
        yield return new WaitForSeconds(waitTime);
        cardGlow[(int)draw - 1].SetActive(true);
        mulliganUI.SetActive(true);
    }
    #endregion
}
