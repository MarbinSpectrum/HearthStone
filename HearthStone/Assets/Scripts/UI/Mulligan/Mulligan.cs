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

    //멀리건 관련
    [Header("멀리건")]
    [SerializeField] private ChangeCardGlow[] mulliganGlow;
    [SerializeField] private CardView[] mulliganCardView;
    [SerializeField] private Animator mulliganAnimator;
    [SerializeField] private GameObject mulliganUI;
    private bool[] changeIdx = new bool[4];
    private string[] mulliganCard = new string[4];

    //코인관련
    [Header("코인")]
    [SerializeField] private Animator coinAnimator;
    [SerializeField] private Material coinMat;
    [SerializeField] private Animator createCoinCard;
    [SerializeField] private Transform coinCardPos;

    //카드교환 버튼
    [Header("카드교환 버튼")]
    [SerializeField] private CardChangeBtn[] cardChangeBtns;

    //선공 애니메이션
    [Header("선공")]
    [SerializeField] private Animator firstCardAni;
    [SerializeField] private CardView[] firstTurnCardView;

    //후공 애니메이션
    [Header("후공")]
    [SerializeField] private Animator secondCardAni;
    [SerializeField] private CardView[] secondTurnCardView;

    public void ChangeMulligan(int idx, bool state)
    {
        changeIdx[idx] = state;
        mulliganGlow[idx].isRun = !state;
    }

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
            cardViews[i].gameObject.SetActive(changeIdx[i]);
            CardViewManager.instance.CardShow(ref cardViews[i], mulliganCardView[i]);

            mulliganCardView[i].gameObject.SetActive(changeIdx[i] == false);
            cardChangeBtns[i].btnImg.enabled = false;
            cardChangeBtns[i].enabled = false;
            mulliganGlow[i].isRun = false;
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

        //선후공에 따라 사용하는 카드 객체와
        //카드 애니메이션이 다르기 때문에 알맞는것을 선택한다.
        CardView[] cardViews = (drawType == DrawType.후공) ?
            secondTurnCardView : firstTurnCardView;
        Animator cardAni = (drawType == DrawType.후공) ?
            secondCardAni : firstCardAni;

        //선후공에 따라 뽑는 카드의 수도 다르다.
        int playerDrawNum = drawNum[(int)drawType];
        int enemyDrawNum = (drawType == DrawType.후공) ?
            drawNum[(int)DrawType.선공] : drawNum[(int)DrawType.후공];

        float cardPushDelay = 0.1f;
        for (int i = 0; i < playerDrawNum; i++)
        {
            if (changeIdx[i])
            {
                //해당 위치의 패가 교환된다면

                cardPushDelay = 1.5f;

                //해당 위치의 카드를 덱에 다시 넣는다.
                InGameDeck.instance.PushCard(mulliganCard[i]);

                //덱 맨위의 카드를 해당 위치에 추가한다.
                string topCard = InGameDeck.instance.GetTopCard();
                InGameDeck.instance.PopTopCard();
                mulliganCard[i] = topCard;

                //카드 객체로 해당카드를 표시한다.
                CardViewManager.instance.CardShow(ref mulliganCardView[i], topCard);
                CardViewManager.instance.CardShow(ref cardViews[i], topCard);
                CardViewManager.instance.UpdateCardView();

                //덱을 섞는다.
                InGameDeck.instance.Shuffle(1000);
            }
        }

        for (int i = 0; i < playerDrawNum; i++)
        {
            //결정된 손패를 적용한다.
            CardHand.instance.SetCardHand(i, mulliganCard[i]);
        }

        cardAni.SetInteger("State", 2);
        StartCoroutine(EnemyDrawCard(enemyDrawNum));

        yield return new WaitForSeconds(cardPushDelay);
        CardHand.instance.nowHandNum = playerDrawNum;
        for (int i = 0; i < playerDrawNum; i++)
            CardHand.instance.SetCardHand(i, mulliganCardView[i].transform.position,
                CardHand.handCardSize, 0);

        yield return new WaitForSeconds(0.001f);
        CardViewManager.instance.UpdateCardView();
        for (int i = 0; i < mulliganCardView.Length; i++)
            mulliganCardView[i].gameObject.SetActive(false);
        firstCardAni.gameObject.SetActive(false);
        secondCardAni.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        coinAnimator.SetInteger("State", 3);

        if (drawType == DrawType.후공)
        {
            //코인 카드 생성 애니메이션 실행
            yield return new WaitForSeconds(1f);
            createCoinCard.SetBool("Hide", false);
            SoundManager.instance.PlaySE("코인얻기");

            yield return new WaitForSeconds(2f);
            createCoinCard.SetBool("Hide", true);
            BattleUI.instance.fieldShadowAni.SetInteger("State", 1);
            CardHand.instance.DrawCard();
            CardHand.instance.SetCardHand("동전 한 닢", 4, coinCardPos.position,
                CardHand.handCardSize, 0);

            yield return new WaitForSeconds(0.001f);
            CardViewManager.instance.UpdateCardView();
        }
        else if (drawType == DrawType.선공)
        {
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
        BattleUI.instance.RunShowCharacterAni(false);
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
            mulliganCard[i] = topCard;

            //카드객체로 해당 카드를 표시한다.
            CardViewManager.instance.CardShow(ref mulliganCardView[i], topCard);
            CardViewManager.instance.UpdateCardView();
        }

        //덱을 섞는다.
        InGameDeck.instance.Shuffle(1000);
    }

    private IEnumerator ShowMulligan(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        mulliganAnimator.SetInteger("State", 1);
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
        mulliganAnimator.SetInteger("State", (int)draw);
        coinAnimator.SetInteger("State", (int)draw);
    }
    #endregion

    #region[카드 글로우]
    private IEnumerator CardGlow(float waitTime, DrawType draw)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < mulliganGlow.Length; i++)
            mulliganGlow[i].isRun = true;
        mulliganUI.SetActive(true);
    }
    #endregion
}
