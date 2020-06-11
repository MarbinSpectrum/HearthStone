using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class MyCollectionsMenu : MonoBehaviour
{
    public static MyCollectionsMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [Header("취소")]
    public GameObject cancleBtn;

    [Header("필터")]
    public GameObject filterBtn;
    public Animator filterAni;
    public GameObject costCancleObject;
    public Image cancleCost;
    public GameObject stringCancleObject;
    public InputField searchText;
    public Text searchCancleText;
    [HideInInspector] public DataMng.TableType nowJob;
    int nowCost = -1;
    string nowSearch = "";

    [Header("제작")]
    public Animator makeAni;
    public GameObject selectMake;
    public GameObject nowMagicPowderMakeUI;
    public Animator makeEffectAni;
    public Animator removeEffectAni;
    Image[] nowMagicPowderNumMakeUI;

    [Header("페이지넘기기")]
    public GameObject nextArrow;
    public GameObject backArrow;
    public Animator pageAni;

    [Header("직업버튼")]
    public GameObject[] JobBtn;
    public Text selectjobText;
    public Animator jobListAni;

    [Header("현재페이지 카드")]
    public GameObject nowCard;
    public GameObject nowNotCard;
    CardView[] nowCards = new CardView[8];

    [Header("뒷페이지 카드")]
    public GameObject backCard;
    public GameObject backNotCard;
    CardView[] backCards = new CardView[8];

    [Header("앞페이지 카드")]
    public GameObject nextCard;
    public GameObject nextNotCard;
    CardView[] nextCards = new CardView[8];

    [Header("클로우즈업 카드")]
    public GameObject closeUpCard;
    public GameObject basicCardUI;
    public GameObject powderUI;
    public Animator cardCloseUpAni;
    CardView[] closeUpCards = new CardView[8];
    string nowCloseUpcardName;
    string nowCloseUpcardLevel;
    string nowCloseUpcardType;
    public GameObject hasCardNumUI;
    public Text hasCardNumText;
    int nowCloseUpCardView;
    IEnumerator showCardNum;

    [Header("현재 마법가루")]
    public GameObject nowMagicPowder;
    Image[] nowMagicPowderNum;

    [Header("획득할 마법가루")]
    public GameObject GetMagicPowder;
    Image[] GetMagicPowderNum;

    [Header("소비할 마법가루")]
    public GameObject CostMagicPowder;
    Image[] CostMagicPowderNum;

    [Header("덱리스트")]
    public Image hasDeckNum;
    public RectTransform deckContext;
    public DeckBtn[] deckBtn;
    public GameObject deckListView;
    public GameObject deckCardView;
    public GameObject deckBannerView;
    public DeckBtn deckBannerBtn;
    public RectTransform deckCardContext;
    [HideInInspector] public RectTransform newDeckPos;
    public RectTransform deckObject;
    [HideInInspector] public bool deckCardViewFlag;
    public CardDrag[] deckCardObject;
    [HideInInspector] public int nowDeck = -1;
    List<string> deckCardList = new List<string>();

    [Header("캐릭터선택")]
    public Animator selectCharacterAni;
    public Animator selectCharacterOKAni;
    public Image[] characterImg;
    public Text characterNameTxt;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //카드목록표시를 위한데이터
    [HideInInspector] public List<CardData>[] cardDatas = new List<CardData>[3];

    [HideInInspector] public int nowJobIndex = 0;
    [HideInInspector] public int nowCardIndex = 0;

    //카드제조플레그
    bool cardmakeFlag = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    bool hasBackPage = false;
    bool hasNextPage = false;
    bool nextPageFlag = false;
    bool backPageFlag = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Awake]
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 8; i++)
        {
            closeUpCards[i] = closeUpCard.transform.Find("Card" + i).GetComponent<CardView>();
            nowCards[i] = nowCard.transform.Find("Card" + i).GetComponent<CardView>();
            backCards[i] = backCard.transform.Find("Card" + i).GetComponent<CardView>();
            nextCards[i] = nextCard.transform.Find("Card" + i).GetComponent<CardView>();
        }

        nowMagicPowderNum = new Image[6];
        for (int i = 0; i < nowMagicPowderNum.Length; i++)
            nowMagicPowderNum[i] = nowMagicPowder.transform.Find(Mathf.Pow(10,i).ToString()).GetComponent<Image>();

        nowMagicPowderNumMakeUI = new Image[6];
        for (int i = 0; i < nowMagicPowderNumMakeUI.Length; i++)
            nowMagicPowderNumMakeUI[i] = nowMagicPowderMakeUI.transform.Find(Mathf.Pow(10, i).ToString()).GetComponent<Image>();

        GetMagicPowderNum = new Image[4];
        for (int i = 0; i < GetMagicPowderNum.Length; i++)
            GetMagicPowderNum[i] = GetMagicPowder.transform.Find(Mathf.Pow(10, i).ToString()).GetComponent<Image>();

        CostMagicPowderNum = new Image[4];
        for (int i = 0; i < CostMagicPowderNum.Length; i++)
            CostMagicPowderNum[i] = CostMagicPowder.transform.Find(Mathf.Pow(10, i).ToString()).GetComponent<Image>();

        for (int i = 0; i < cardDatas.Length; i++)
            cardDatas[i] = new List<CardData>();

        CardDataInput();
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        StartCoroutine(CardCloseUpCard(0.1f));
        nowJobIndex = 0;
        nowCardIndex = 0;
        nowJob = DataMng.TableType.모두;
        SoundManager.instance.PlayBGM("수집함배경음");
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[Update]
    private void Update()
    {
        UpdateUI();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[내수집품설정]
    public void CardDataInput(DataMng.TableType job = DataMng.TableType.모두,int cost = -1, string search = "",bool make = false)
    {
        for (int i = 0; i < 3; i++)
            cardDatas[i].Clear();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j <= DataMng.instance.m_dic[(DataMng.TableType)i].m_table.Count; j++)
            {
                if(job != DataMng.TableType.모두)
                {
                    if ((DataMng.TableType)(i) != DataMng.TableType.중립)
                        if (job != (DataMng.TableType)(i))
                            continue;
                }

                if (DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "등급").Equals("토큰"))
                    continue;

                if (cost != -1)
                {
                    if (cost == 7)
                    {
                        if (DataMng.instance.m_dic[(DataMng.TableType)i].ToInteger(j, "코스트") < cost)
                            continue;
                    }
                    else if (DataMng.instance.m_dic[(DataMng.TableType)i].ToInteger(j, "코스트") != cost)
                        continue;
                }

                if (!search.Equals(""))
                    if (!DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드이름").Contains(search) &&
                        !DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드설명").Contains(search))
                        continue;
                string cardName = DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드이름");
                if (!make && DataMng.instance.playData != null && DataMng.instance.playData.GetCardNum(cardName) == 0)
                    continue;

                cardDatas[i].Add(GetCardData(j, (DataMng.TableType)i));

            }


            cardDatas[i].Sort(delegate(CardData A,CardData B)
            {
                if (A.cardCost > B.cardCost)
                    return 1;
                if (A.cardCost == B.cardCost)
                    return A.cardName.CompareTo(B.cardName);
                return -1;

            });
        }
        nowCost = cost;

        for (int i = 0; i < 3; i++)
            if (cardDatas[i].Count > 0)
            {
                nowJobIndex = i;
                nowCardIndex = 0;
                break;
            }
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드데이터받기]
    public CardData GetCardData(int num,DataMng.TableType job)
    {
        DataMng dataMng = DataMng.instance;

        return new CardData
            (
            job,
            dataMng.ToString(job, num, "등급"),
            dataMng.ToString(job, num, "카드이름"),
            dataMng.ToString(job, num, "카드종류"),
            dataMng.ToInteger(job, num, "코스트"),
            dataMng.ToInteger(job, num, "공격력"),
            dataMng.ToInteger(job, num, "체력"),
            dataMng.ToString(job, num, "카드설명")
            );
    }
    #endregion

    #region[UpdateUI]
    public void UpdateUI()
    {
        //카드리스트
        ShowCard();

        bool arrowFlag = (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperStop") &&  pageAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
        backArrow.gameObject.SetActive(arrowFlag && hasBackPage);
        nextArrow.gameObject.SetActive(arrowFlag && hasNextPage);


        //직업표시
        bool jobTextFlag = false;
        for (int i = 0; i < cardDatas.Length; i++)
            if (cardDatas[i].Count > 0)
            {
                jobTextFlag = true;
                JobBtn[i].gameObject.SetActive(true);
            }
            else
                JobBtn[i].gameObject.SetActive(false);

        //직업선택
        DataMng.TableType tableName = (DataMng.TableType)(nowJobIndex);
        selectjobText.text = jobTextFlag ? tableName.ToString() : "";

        //덱의 수만큼 UI조절
        int deckNum = DataMng.instance.playData.deck.Count;
        hasDeckNum.sprite = DataMng.instance.num[deckNum];
        deckContext.sizeDelta = new Vector2(deckContext.sizeDelta.x, 185.4f * Mathf.Min(deckNum + 1,9));

        //덱조절
        for (int i = 0; i < 9; i++)
        {
            deckBtn[i].gameObject.SetActive(i < Mathf.Min(deckNum + 1, 9));
            deckBtn[i].hasDeck = (i < deckNum);
        }
        for (int i = 0; i < deckNum; i++)
        {
            deckBtn[i].deckNameTxt.text = DataMng.instance.playData.deck[i].name;
            deckBtn[i].nowCharacter = (int)DataMng.instance.playData.deck[i].job;
        }

        //덱이 선택되었을때 애니메이션
        if(deckCardViewFlag)
        {
            if(deckObject.anchoredPosition.y < newDeckPos .anchoredPosition.y + 2.5f)
                deckObject.anchoredPosition += new Vector2(0, Time.deltaTime * 7);
            else if (deckObject.anchoredPosition.y < 0)
                deckObject.anchoredPosition += new Vector2(0, Time.deltaTime* Mathf.Pow(Mathf.Abs(deckObject.anchoredPosition.y),1.2f));
            else
                deckObject.anchoredPosition = new Vector2(deckObject.anchoredPosition.x,0);
        }

        //덱보여주기
        if(nowDeck != -1)
        {
            deckCardList.Clear();
            for (int i = 0; i < DataMng.instance.playData.deck[nowDeck].card.Count; i++)
                deckCardList.Add(DataMng.instance.playData.deck[nowDeck].card[i]);

            deckCardList.Sort(delegate (string A, string B)
            {
                int costA = 0;
                int costB = 0;

                Vector2 pairA = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(A));
                Vector2 pairB = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(B));
               // Debug.Log(pairA == pairB);
                costA = DataMng.instance.ToInteger((DataMng.TableType)pairA.x, (int)pairA.y, "코스트");
                costB = DataMng.instance.ToInteger((DataMng.TableType)pairB.x, (int)pairB.y, "코스트");
              //  Debug.Log(costA == costB);
                if (costA > costB)
                    return 1;
                if (costA == costB)
                    return A.CompareTo(B);
                return -1;

            });

            deckBannerBtn.deckNameTxt.text = DataMng.instance.playData.deck[nowDeck].name;
            deckBannerBtn.nowCharacter = (int)DataMng.instance.playData.deck[nowDeck].job;

            for (int i = 0; i < 30; i++)
            {
                if(i < DataMng.instance.playData.deck[nowDeck].card.Count)
                {
                    deckCardObject[i].cardName_Data = DataMng.instance.playData.GetCardName(deckCardList[i]);
                    deckCardObject[i].hasCardNum = DataMng.instance.playData.GetCardNumber(deckCardList[i]);
                    deckCardObject[i].gameObject.SetActive(true);
                }
                else
                    deckCardObject[i].gameObject.SetActive(false);
            }

            //덱의 카드 수만큼 UI조절
            int deckCardNum = 0;
            for (int i = 0; i < 30; i++)
                if (deckCardObject[i].gameObject.activeSelf)
                    deckCardNum++;
            deckCardContext.sizeDelta = new Vector2(deckCardContext.sizeDelta.x, 85.39f * (deckCardNum+1));
        }
    }
    #endregion

    #region[카드들표시]
    public void ShowCard()
    {
        hasBackPage = false;
        for (int i = 0; i < nowJobIndex; i++)
            hasBackPage = hasBackPage || (cardDatas[i].Count > 0);
        hasBackPage = hasBackPage || (nowCardIndex > 0);

        hasNextPage = false;
        for (int i = nowJobIndex + 1; i < 3; i++)
            hasNextPage = hasNextPage || (cardDatas[i].Count > 0);
        hasNextPage = hasNextPage || (nowCardIndex + 8 < cardDatas[nowJobIndex].Count);

        #region[카드표시]
        for (int i = 0; i < 8; i++)
        {
            int tempCardIndex = i + nowCardIndex;
            if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                nowCards[i].gameObject.SetActive(false);
            else
                CardShow(ref nowCards[i], nowJobIndex, tempCardIndex);
        }
        #endregion

        #region[카드가 존재하지 않습니다]
        nowNotCard.SetActive(true);
        backNotCard.SetActive(true);
        nextNotCard.SetActive(true);

        for (int i = 0; i < 8; i++)
            if (nowCards[i].gameObject.activeSelf)
            {
                nowNotCard.SetActive(false);
                break;
            }
        for (int i = 0; i < 8; i++)
            if (backCards[i].gameObject.activeSelf)
            {
                backNotCard.SetActive(false);
                break;
            }
        for (int i = 0; i < 8; i++)
            if (nextCards[i].gameObject.activeSelf)
            {
                nextNotCard.SetActive(false);
                break;
            }

        #endregion

    }
    #endregion

    #region[카드표시]
    public void CardShow(ref CardView card,int nowJobIndex,int cardIndex)
    {
        card.gameObject.SetActive(true);
        if (cardDatas[nowJobIndex][cardIndex].cardType.Equals("하수인"))
        {
            card.cardType = CardType.하수인;
            card.MinionsCostData = cardDatas[nowJobIndex][cardIndex].cardCost;
            card.MinionsAttackData = cardDatas[nowJobIndex][cardIndex].cardAttack;
            card.MinionsHpData = cardDatas[nowJobIndex][cardIndex].cardHp;
            card.MinionsCardNameData = cardDatas[nowJobIndex][cardIndex].cardName;
            card.MinionsCardExplainData = cardDatas[nowJobIndex][cardIndex].cardExplain;
        }
        else if (cardDatas[nowJobIndex][cardIndex].cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SpellCostData = cardDatas[nowJobIndex][cardIndex].cardCost;
            card.SpellCardNameData = cardDatas[nowJobIndex][cardIndex].cardName;
            card.SpellCardExplainData = cardDatas[nowJobIndex][cardIndex].cardExplain;
        }
        else if (cardDatas[nowJobIndex][cardIndex].cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.WeaponCostData = cardDatas[nowJobIndex][cardIndex].cardCost;
            card.WeaponAttackData = cardDatas[nowJobIndex][cardIndex].cardAttack;
            card.WeaponHpData = cardDatas[nowJobIndex][cardIndex].cardHp;
            card.WeaponCardNameData = cardDatas[nowJobIndex][cardIndex].cardName;
            card.WeaponCardExplainData = cardDatas[nowJobIndex][cardIndex].cardExplain;
        }
        card.cardLevel = cardDatas[nowJobIndex][cardIndex].cardLevel;
        DataMng.TableType tableName = (DataMng.TableType)(nowJobIndex);
        card.cardJob = tableName.ToString();
    }
    #endregion

    #region[카드페이지 넘기기]
    public void NextPage()
    {
        if (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperStop"))
        {
            nextPageFlag = true;

            #region[카드이미지처리]
            for (int i = 0; i < 8; i++)
            {
                int tempCardIndex = i + nowCardIndex;
                if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                    nextCards[i].gameObject.SetActive(false);
                else
                    CardShow(ref nextCards[i], nowJobIndex, tempCardIndex);
            }
            #endregion

            #region[카드인덱스 변경]
            nowCardIndex += 8;
            if (nowCardIndex >= cardDatas[nowJobIndex].Count)
            {
                nowCardIndex = 0;
                for (int i = nowJobIndex + 1; i < cardDatas.Length; i++)
                    if (cardDatas[i].Count > 0)
                    {
                        nowJobIndex = i;
                        break;
                    }
            }
            #endregion
        
            pageAni.SetTrigger("Next");
            SoundManager.instance.PlaySE("페이지넘기기");
        }
        CardViewManager.instance.UpdateCardView();
    }

    public void BackPage()
    {
        if (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperStop"))
        {
            backPageFlag = true;

            #region[카드이미지처리]
            for (int i = 0; i < 8; i++)
            {
                int tempCardIndex = i + nowCardIndex;
                if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                    backCards[i].gameObject.SetActive(false);
                else
                    CardShow(ref backCards[i], nowJobIndex, tempCardIndex);
            }
            #endregion

            #region[카드인덱스 변경]
            nowCardIndex -= 8;
            if (nowCardIndex < 0)
            {
                for (int i = nowJobIndex - 1; i >= 0; i--)
                    if (cardDatas[i].Count > 0)
                    {
                        nowJobIndex = i;
                        break;
                    }
                nowCardIndex = ((int)Mathf.Ceil((float)cardDatas[nowJobIndex].Count / 8f) - 1) * 8;
            }
            #endregion

            #region[카드이미지처리]
            for (int i = 0; i < 8; i++)
            {
                int tempCardIndex = i + nowCardIndex;
                if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                    nextCards[i].gameObject.SetActive(false);
                else
                    CardShow(ref nextCards[i], nowJobIndex, tempCardIndex);
            }
            #endregion

            pageAni.SetTrigger("Back");
            SoundManager.instance.PlaySE("페이지넘기기");
        }
        CardViewManager.instance.UpdateCardView();
    }

    public void MovePage(int n)
    {
        if (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperStop"))
        {
            if (nowJobIndex < n)
            {
                nextPageFlag = true;

                #region[카드이미지처리]
                for (int i = 0; i < 8; i++)
                {
                    int tempCardIndex = i + nowCardIndex;
                    if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                        nextCards[i].gameObject.SetActive(false);
                    else
                        CardShow(ref nextCards[i], nowJobIndex, tempCardIndex);
                }
                #endregion

                #region[카드인덱스 변경]
                nowCardIndex = 0;
                nowJobIndex = n;
                #endregion

                pageAni.SetTrigger("Next");
                SoundManager.instance.PlaySE("페이지넘기기");
            }
            else if (nowJobIndex > n)
            {
                backPageFlag = true;

                #region[카드이미지처리]
                for (int i = 0; i < 8; i++)
                {
                    int tempCardIndex = i + nowCardIndex;
                    if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                        backCards[i].gameObject.SetActive(false);
                    else
                        CardShow(ref backCards[i], nowJobIndex, tempCardIndex);
                }
                #endregion

                #region[카드인덱스 변경]
                nowJobIndex = n;
                nowCardIndex = 0;
                #endregion

                #region[카드이미지처리]
                for (int i = 0; i < 8; i++)
                {
                    int tempCardIndex = i + nowCardIndex;
                    if (tempCardIndex >= Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
                        nextCards[i].gameObject.SetActive(false);
                    else
                        CardShow(ref nextCards[i], nowJobIndex, tempCardIndex);
                }
                #endregion

                pageAni.SetTrigger("Back");
                SoundManager.instance.PlaySE("페이지넘기기");
            }
            else
            {
                #region[카드인덱스 변경]
                nowJobIndex = n;
                nowCardIndex = 0;
                #endregion
            }
        }
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[뒤로가기]
    public void ActGoBack()
    {
        if(BattleMenu.instance && BattleMenu.instance.battleCollections)
        {
            MainMenu.instance.ChangeBoard();
            StartCoroutine(CloseCollectionsMenu(1));
            StartCoroutine(ShowBattleMenu(0.5f));
            BattleMenu.instance.battleCollections = false;
        }
        else
        {
            MainMenu.instance.CloseBoard();
            StartCoroutine(CloseCollectionsMenu(1));
        }
    }

    private IEnumerator CloseCollectionsMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }

    private IEnumerator ShowBattleMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        MainMenu.instance.battleMenuUI.SetActive(true);
    }
    #endregion

    #region[직업선택버튼]
    public void ActSelectJobBtn(bool act)
    {
        Debug.Log("직업");
        jobListAni.SetBool("Show", act);
    }
    #endregion

    #region[필터버튼]
    public void ActFilterJobBtn(bool act)
    {
        filterAni.SetBool("Show", act);
        if (!act)
        {
            if(String.IsNullOrWhiteSpace(searchText.text))
            {
                nowSearch = "";
                CardDataInput(nowJob ,nowCost, nowSearch, cardmakeFlag);
            }
            else
            {
                nowSearch = searchText.text;
                CardDataInput(nowJob ,-1, nowSearch, cardmakeFlag);
            }

            if (nowCost != -1 || !String.IsNullOrWhiteSpace(searchText.text))
            {
                if (!String.IsNullOrWhiteSpace(searchText.text))
                {
                    searchCancleText.text = searchText.text;
                    costCancleObject.SetActive(false);
                    stringCancleObject.SetActive(true);
                }
                else if(nowCost != -1)
                {
                    searchCancleText.text = "";
                    cancleCost.sprite = DataMng.instance.num[nowCost];
                    costCancleObject.SetActive(true);
                    stringCancleObject.SetActive(false);
                }
                ActCancleBtn(true);
            }
        }

        CardViewManager.instance.UpdateCardView();


    }
    #endregion

    #region[취소버튼]
    public void ActCancleBtn(bool act)
    {
        cancleBtn.gameObject.SetActive(act);
        if (!act)
        {
            nowCost = -1;
            nowSearch = "";
            searchText.text = "";
            CardDataInput(nowJob , nowCost, nowSearch, cardmakeFlag);
        }
        filterBtn.gameObject.SetActive(!act);
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드 클로우즈업]
    public void CardCloseUp(int n)
    {
        CardViewManager.instance.UpdateCardView();
        for(int i = 0; i < 8; i++)
            closeUpCards[i].gameObject.SetActive(false);
        cardCloseUpAni.SetTrigger("Show");
        SoundManager.instance.PlaySE("수집함카드선택");

        CardShow(ref closeUpCards[n], nowJobIndex, nowCardIndex + n);
        nowCloseUpcardName = cardDatas[nowJobIndex][nowCardIndex + n].cardName;
        nowCloseUpcardType = cardDatas[nowJobIndex][nowCardIndex + n].cardType;
        int cardNum = DataMng.instance.playData.GetCardNum(nowCloseUpcardName);
        hasCardNumText.text = cardNum.ToString();

        int nowM = DataMng.instance.playData.magicPowder;
        for(int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNumMakeUI[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        int temp;
        nowCloseUpcardLevel = cardDatas[nowJobIndex][nowCardIndex + n].cardLevel;
        temp = CardData.GetCardPowder(nowCloseUpcardLevel);
        for (int i = 0; i < 4; i++)
        {
            GetMagicPowderNum[i].sprite = DataMng.instance.num[temp % 10];
            GetMagicPowderNum[i].gameObject.SetActive((temp != 0));
            temp /= 10;
        }

        temp = CardData.CostCardPowder(nowCloseUpcardLevel);
        for (int i = 0; i < 4; i++)
        {
            CostMagicPowderNum[i].sprite = DataMng.instance.num[temp % 10];
            CostMagicPowderNum[i].gameObject.SetActive((temp != 0));
            temp /= 10;
        }
        powderUI.SetActive(!nowCloseUpcardLevel.Equals("기본"));
        basicCardUI.SetActive(nowCloseUpcardLevel.Equals("기본"));
        closeUpCards[n].gameObject.SetActive(true);
        nowCloseUpCardView = n;
        nowCards[nowCloseUpCardView].hide = true;
        showCardNum = ShowCardNum(cardNum, 0.3f);
        StartCoroutine(showCardNum);
        CardViewManager.instance.UpdateCardView();

    }

    private IEnumerator ShowCardNum(int cardNum, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        hasCardNumUI.SetActive(cardNum != 0);
    }

    public void CardCloseOut()
    {
        hasCardNumUI.SetActive(false);
        StopCoroutine(showCardNum);
        cardCloseUpAni.SetTrigger("Close");
        StartCoroutine(CardCloseUpCard(0.3f));
        int jobtemp = nowJobIndex;
        int cardtemp = nowCardIndex;
        if (String.IsNullOrWhiteSpace(searchText.text))
            CardDataInput(nowJob,nowCost, "", cardmakeFlag);
        else
            CardDataInput(nowJob ,- 1, searchText.text, cardmakeFlag);
        nowJobIndex = jobtemp;
        nowCardIndex = cardtemp;

        int cardNum = DataMng.instance.playData.GetCardNum(nowCloseUpcardName);
        if (cardNum == 0 && !cardmakeFlag)
            for (int i = 0; i < 8; i++)
                closeUpCards[i].gameObject.SetActive(false);
    }

    private IEnumerator CardCloseUpCard(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < 8; i++)
            closeUpCards[i].gameObject.SetActive(false);
        nowCards[nowCloseUpCardView].hide = false;
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드제조]
    public void MakeCard()
    {
        int cardNum = DataMng.instance.playData.GetCardNum(nowCloseUpcardName);
        if (cardNum == 9)
            return;
        if (nowCloseUpcardLevel.Equals("토큰") || nowCloseUpcardLevel.Equals("기본"))
            return;
        if (CardData.CostCardPowder(nowCloseUpcardLevel) > DataMng.instance.playData.magicPowder)
            return;

        StartCoroutine(MakeCardEffect(cardNum,1.5f));
        StartCoroutine(ShowCardNum(cardNum + 1, 1.5f));
    }

    private IEnumerator MakeCardEffect(int cardNum, float waitTime)
    {
        if(nowCloseUpcardType.Equals("하수인"))
            makeEffectAni.SetTrigger("M_CardCreate");
        else if (nowCloseUpcardType.Equals("주문"))
            makeEffectAni.SetTrigger("S_CardCreate");
        else if (nowCloseUpcardType.Equals("무기"))
            makeEffectAni.SetTrigger("W_CardCreate");
        yield return new WaitForSeconds(waitTime);
        DataMng.instance.playData.magicPowder -= CardData.CostCardPowder(nowCloseUpcardLevel);
        int nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNumMakeUI[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        DataMng.instance.playData.SetCardNum(nowCloseUpcardName, cardNum + 1);
        hasCardNumText.text = (cardNum + 1).ToString();
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드제거]
    public void RemoveCard()
    {
        int cardNum = DataMng.instance.playData.GetCardNum(nowCloseUpcardName);
        if (cardNum == 0)
            return;
        if (nowCloseUpcardLevel.Equals("토큰") || nowCloseUpcardLevel.Equals("기본"))
            return;
        StartCoroutine(RemoveCardEffect(cardNum, 0.5f));
        StartCoroutine(ShowCardNum(cardNum - 1, 0.5f));
    }

    private IEnumerator RemoveCardEffect(int cardNum, float waitTime)
    {
        removeEffectAni.SetTrigger("CardRemove");
        yield return new WaitForSeconds(waitTime);
        DataMng.instance.playData.magicPowder += CardData.GetCardPowder(nowCloseUpcardLevel);
        int nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNumMakeUI[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        DataMng.instance.playData.SetCardNum(nowCloseUpcardName, cardNum - 1);
        hasCardNumText.text = (cardNum - 1).ToString();
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[코스트로 분류버튼]
    public void ActFilterCostBtn(int n)
    {
        nowSearch = "";
        searchText.text = "";
        CardDataInput(nowJob,n, nowSearch, cardmakeFlag);
    }
    #endregion

    #region[카드제작상태로 변경]
    public void ChangeCardMakeMode()
    {
        cardmakeFlag = !cardmakeFlag;
        CardDataInput(nowJob, nowCost, nowSearch, cardmakeFlag);

        makeAni.SetBool("Show", cardmakeFlag);
        selectMake.SetActive(cardmakeFlag);
        int nowM = DataMng.instance.playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNumMakeUI[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
    }
    #endregion

    #region[캐릭터 선택]
    public void SelectCharacter(bool b)
    {
        selectCharacterAni.SetBool("Show", b);
    }

    public void SelectCharacterOK(bool b,int n = -1)
    {
        selectCharacterOKAni.SetBool("Show", b);

        if(n != -1)
        {
            for (int i = 0; i < characterImg.Length; i++)
                characterImg[i].enabled = false;
            characterImg[n].enabled = true;
            switch (n)
            {
                case 0:
                    characterNameTxt.text = "말퓨리온 스톰레이지";
                    break;
                case 1:
                    characterNameTxt.text = "발리라 생귀나르";
                    break;
            }
            nowJob = (DataMng.TableType)n;
        }
    }

    #endregion

    #region[덱 카드 확인]
    public void DeckCardView(int n)
    {
        SelectCharacter(false);
        SelectCharacterOK(false);
        deckListView.SetActive(false);
        deckCardView.SetActive(true);
        deckBannerView.SetActive(true);
        deckCardContext.anchoredPosition = new Vector2(deckCardContext.anchoredPosition.x, 0);
        deckObject.anchoredPosition = new Vector2(newDeckPos.anchoredPosition.x, newDeckPos.anchoredPosition.y + deckContext.anchoredPosition.y);
        deckCardViewFlag = true;
        ActCancleBtn(false);
        nowDeck = n;
        if(cardmakeFlag)
            ChangeCardMakeMode();
    }
    #endregion
}
