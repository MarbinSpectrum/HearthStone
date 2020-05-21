using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class MyCollectionsMenu : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Animator jobListAni;
    Animator filterAni;
    Animator[] costBtnAni;
    Animator pageAni;

    Animator cardCloseUpAni;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Button nextArrow;
    Button backArrow;

    Button[] jobButton;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    EventTrigger.Entry pointerEnter;
    EventTrigger.Entry pointerDown;
    EventTrigger.Entry pointerExit;
    EventTrigger.Entry pointerClick;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Image goBackImg;
    Image selectJobImg;
    Image filterImg;
    Image cancleImg;
    Image makingImg;

    Image completeImg;

    Image cancleCost;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    InputField searchText;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum ButtonState { 보통, 누름 }

    public Sprite[] goBackSprites;
    public Sprite[] selectJobSprites;
    public Sprite[] filterSprites;
    public Sprite[] makingSprites;

    public Sprite[] completeSprites;

    Sprite[] num;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Text selectjobText;
    Text searchCancleText;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CardData
    {
        public DataMng.TableType cardJob; 
        public string cardLevel;
        public string cardName;
        public string cardType;
        public int cardCost;
        public int cardAttack;
        public int cardHp;
        public string cardExplain;

        public CardData(DataMng.TableType acardJob, string acardLevel ,string acardName, string acardType, int acardCost, int acardAttack, int acardHp, string acardExplain)
        {
            cardJob = acardJob;
            cardLevel = acardLevel;
            cardName = acardName;
            cardType = acardType;
            cardCost = acardCost;
            cardAttack = acardAttack;
            cardHp = acardHp;
            cardExplain = acardExplain;
        }
    }

    List<CardData>[] cardDatas = new List<CardData>[3];

    CardView[] nowCards = new CardView[8];
    GameObject nowNotCard;
    CardView[] backCards = new CardView[8];
    GameObject backNotCard;
    CardView[] nextCards = new CardView[8];
    GameObject nextNotCard;
    CardView[] closeUpCards = new CardView[8];

    GameObject costCancleObject;
    GameObject stringCancleObject;
    GameObject cardCloseUp;

    int nowJobIndex = 0;
    int nowCardIndex = 0;

    int nowCost = -1;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    bool hasBackPage = false;
    bool hasNextPage = false;
    bool nextPageFlag;
    bool backPageFlag;

    #region[Awake]
    private void Awake()
    {
        num = Resources.LoadAll<Sprite>("Card/Number");

        Transform closeUpCard = transform.Find("CardCloseUp");
        for (int i = 0; i < 8; i++)
            closeUpCards[i] = closeUpCard.Find("Card" + i).GetComponent<CardView>();

        Transform nowCard = transform.Find("Page").Find("NowCards");
        for (int i = 0; i < 8; i++)
        {
            nowCards[i] = nowCard.Find("Card" + i).GetComponent<CardView>();
            Button temp = nowCards[i].transform.Find("EventButton").GetComponent<Button>();
            int n = i;
            temp.onClick.AddListener(() =>
            {
                CardCloseUp(n);
            });

        }

        #region[closeUpExitTrigger]
        EventTrigger closeUpExitTrigger = closeUpCard.Find("Blur").GetComponent<EventTrigger>();
        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            CardCloseOut();
        });
        closeUpExitTrigger.triggers.Add(pointerDown);
        #endregion

        nowNotCard = nowCard.Find("NotCard").gameObject;

        Transform backEventCard = transform.Find("Page").Find("BackEventCards");
        for (int i = 0; i < 8; i++)
            backCards[i] = backEventCard.Find("Card" + i).GetComponent<CardView>();
        backNotCard = backEventCard.Find("NotCard").gameObject;

        Transform nextCard = transform.Find("Page").Find("NextEventCards").Find("NextCard").Find("NextCardPage");
        for (int i = 0; i < 8; i++)
            nextCards[i] = nextCard.Find("Card" + i).GetComponent<CardView>();
        nextNotCard = nextCard.Find("NotCard").gameObject;

        jobListAni = transform.Find("JobList").GetComponent<Animator>();
        filterAni = transform.Find("Filter").GetComponent<Animator>();
        pageAni = transform.Find("Page").GetComponent<Animator>();
        cardCloseUpAni = transform.Find("CardCloseUp").GetComponent<Animator>();


        nextArrow = transform.Find("CardArrow").Find("Next").GetComponent<Button>();
        backArrow = transform.Find("CardArrow").Find("Back").GetComponent<Button>();

        cardCloseUp = transform.Find("CardCloseUp").gameObject;

        #region[nextArrow]
        nextArrow.onClick.AddListener(() =>
        {
            NextPage();
        });
        #endregion
        #region[backArrow]
        backArrow.onClick.AddListener(() =>
        {
            BackPage();
        });
        #endregion

        Transform jobList = transform.Find("JobList");
        int jobButtonCount = jobList.Find("JobBoard").childCount;
        jobButton = new Button[jobButtonCount];

        for (int i = 0; i < jobButtonCount; i++)
        {
            int n = i;
            jobButton[n] = jobList.Find("JobBoard").GetChild(n).GetComponent<Button>();
            jobButton[n].onClick.AddListener(() =>
            {
                MovePage(n);
                ActSelectJobBtn(false);
            });
        }

        goBackImg = transform.Find("뒤로").GetComponent<Image>();
        selectJobImg = transform.Find("직업").GetComponent<Image>();
        filterImg = transform.Find("필터").GetComponent<Image>();
        cancleImg = transform.Find("취소").GetComponent<Image>();
        makingImg = transform.Find("제작").GetComponent<Image>();

        searchText = transform.Find("Filter").Find("CardType").Find("입력").GetComponent<InputField>();

        #region[goBackTrigger]
        EventTrigger goBackTrigger = transform.Find("뒤로").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                goBackImg.sprite = goBackSprites[(int)ButtonState.누름];
        });
        goBackTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                goBackImg.sprite = goBackSprites[(int)ButtonState.누름];
        });
        goBackTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            goBackImg.sprite = goBackSprites[(int)ButtonState.보통];
        });
        goBackTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActGoBackBtn();
        });
        goBackTrigger.triggers.Add(pointerClick);
        #endregion

        #region[selectJobTrigger]
        EventTrigger selectJobTrigger = transform.Find("직업").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                selectJobImg.sprite = selectJobSprites[(int)ButtonState.누름];
        });
        selectJobTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                selectJobImg.sprite = selectJobSprites[(int)ButtonState.누름];
        });
        selectJobTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            selectJobImg.sprite = selectJobSprites[(int)ButtonState.보통];
        });
        selectJobTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActSelectJobBtn(true);
        });
        selectJobTrigger.triggers.Add(pointerClick);
        #endregion
        #region[selectJobExitTrigger]
        EventTrigger selectJobExitTrigger = transform.Find("JobList").Find("Blur").GetComponent<EventTrigger>();
        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            ActSelectJobBtn(false);
        });
        selectJobExitTrigger.triggers.Add(pointerDown);
        #endregion

        #region[filterTrigger]
        EventTrigger filterTrigger = transform.Find("필터").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                filterImg.sprite = filterSprites[(int)ButtonState.누름];
        });
        filterTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                filterImg.sprite = filterSprites[(int)ButtonState.누름];
        });
        filterTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            filterImg.sprite = filterSprites[(int)ButtonState.보통];
        });
        filterTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActFilterJobBtn(true);
        });
        filterTrigger.triggers.Add(pointerClick);
        #endregion

        #region[cancleTrigger]
        EventTrigger cancleTrigger = transform.Find("취소").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                cancleImg.sprite = filterSprites[(int)ButtonState.누름];
        });
        cancleTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                cancleImg.sprite = filterSprites[(int)ButtonState.누름];
        });
        cancleTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            cancleImg.sprite = filterSprites[(int)ButtonState.보통];
        });
        cancleTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActCancleBtn(false);
        });
        cancleTrigger.triggers.Add(pointerClick);
        #endregion

        #region[makingTrigger]
        EventTrigger makingTrigger = transform.Find("제작").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                makingImg.sprite = makingSprites[(int)ButtonState.누름];
        });
        makingTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                makingImg.sprite = makingSprites[(int)ButtonState.누름];
        });
        makingTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            makingImg.sprite = makingSprites[(int)ButtonState.보통];
        });
        makingTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActMakingBtn();
        });
        makingTrigger.triggers.Add(pointerClick);
        #endregion

        completeImg = transform.Find("Filter").Find("CardType").Find("완료").GetComponent<Image>();

        #region[completeTrigger]
        EventTrigger completeTrigger = transform.Find("Filter").Find("CardType").Find("완료").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                completeImg.sprite = completeSprites[(int)ButtonState.누름];
        });
        completeTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                completeImg.sprite = completeSprites[(int)ButtonState.누름];
        });
        completeTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            completeImg.sprite = completeSprites[(int)ButtonState.보통];
        });
        completeTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActFilterJobBtn(false);
        });
        completeTrigger.triggers.Add(pointerClick);
        #endregion

        #region[costBtnTrigger]
        costBtnAni = new Animator[transform.Find("Filter").Find("CostList").childCount];
        for (int i = 0; i < costBtnAni.Length; i++)
        {
            int n = i;
            costBtnAni[i] = transform.Find("Filter").Find("CostList").GetChild(i).GetComponent<Animator>();
            EventTrigger costBtnTrigger = transform.Find("Filter").Find("CostList").GetChild(i).GetComponent<EventTrigger>();
            pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) =>
            {
                if (Input.GetMouseButton(0))
                    costBtnAni[n].SetBool("Glow", true);
            });
            costBtnTrigger.triggers.Add(pointerEnter);

            pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) =>
            {
                if (Input.GetMouseButtonDown(0))
                    costBtnAni[n].SetBool("Glow", true);
            });
            costBtnTrigger.triggers.Add(pointerDown);

            pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) =>
            {
                costBtnAni[n].SetBool("Glow", false);
            });
            costBtnTrigger.triggers.Add(pointerExit);

            pointerClick = new EventTrigger.Entry();
            pointerClick.eventID = EventTriggerType.PointerClick;
            pointerClick.callback.AddListener((data) =>
            {
                ActFilterCostBtn(n);
            });
            costBtnTrigger.triggers.Add(pointerClick);
        }
        #endregion

        selectjobText = selectJobImg.transform.Find("Text").GetComponent<Text>();

        for (int i = 0; i < cardDatas.Length; i++)
            cardDatas[i] = new List<CardData>();
        CardDataInput();

        stringCancleObject = transform.Find("취소").Find("SearchText").gameObject;
        searchCancleText = stringCancleObject.transform.Find("Text").GetComponent<Text>();

        costCancleObject = transform.Find("취소").Find("SearchCost").gameObject;
        cancleCost = costCancleObject.transform.Find("num").GetComponent<Image>();
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        StartCoroutine(CardCloseUnCard(0.1f));
        nowJobIndex = 0;
        nowCardIndex = 0;
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
    public void CardDataInput(int cost = -1, string search = "")
    {
        for (int i = 0; i < 3; i++)
            cardDatas[i].Clear();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j <= DataMng.instance.m_dic[(DataMng.TableType)i].m_table.Count; j++)
                if (!DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "등급").Equals("토큰"))
                    if (cost == -1 || DataMng.instance.m_dic[(DataMng.TableType)i].ToInteger(j, "코스트") == cost ||
                        (cost == 7 && DataMng.instance.m_dic[(DataMng.TableType)i].ToInteger(j, "코스트") >= cost))
                        if (search.Equals("") || 
                            DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드이름").Contains(search) ||
                            DataMng.instance.m_dic[(DataMng.TableType)i].ToString(j, "카드설명").Contains(search))
                            cardDatas[i].Add(GetCardData(j, (DataMng.TableType)i));

            cardDatas[i].Sort(delegate(CardData A,CardData B)
            {
                if (A.cardCost > B.cardCost)
                    return 1;
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
        if (Input.GetMouseButtonUp(0))
        {
            goBackImg.sprite = goBackSprites[(int)ButtonState.보통];
            selectJobImg.sprite = selectJobSprites[(int)ButtonState.보통];
            filterImg.sprite = filterSprites[(int)ButtonState.보통];
            cancleImg.sprite = filterSprites[(int)ButtonState.보통];
            makingImg.sprite = makingSprites[(int)ButtonState.보통];
        }

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
                jobButton[i].gameObject.SetActive(true);
            }
            else
                jobButton[i].gameObject.SetActive(false);

        //직업선택
        DataMng.TableType tableName = (DataMng.TableType)(nowJobIndex);
        selectjobText.text = jobTextFlag ? tableName.ToString() : "";

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
        }
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
        }
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
            }
            else
            {
                #region[카드인덱스 변경]
                nowJobIndex = n;
                nowCardIndex = 0;
                #endregion
            }
        }
    }
    #endregion

    #region[뒤로가기버튼]
    public void ActGoBackBtn()
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
        Debug.Log("뒤로가기");
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
                CardDataInput(nowCost, "");
            else
                CardDataInput(-1, searchText.text);

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
                    cancleCost.sprite = num[nowCost];
                    costCancleObject.SetActive(true);
                    stringCancleObject.SetActive(false);
                }
                ActCancleBtn(true);
            }

        }
    }
    #endregion

    #region[취소버튼]
    public void ActCancleBtn(bool act)
    {
        cancleImg.gameObject.SetActive(act);
        if (!act)
            CardDataInput();
        filterImg.gameObject.SetActive(!act);
    }
    #endregion

    void CardCloseUp(int n)
    {
        for(int i = 0; i < 8; i++)
            closeUpCards[i].gameObject.SetActive(false);
        cardCloseUpAni.SetTrigger("Show");
        CardShow(ref closeUpCards[n], nowJobIndex, nowCardIndex + n);
        closeUpCards[n].gameObject.SetActive(true);
    }

    void CardCloseOut()
    {
        cardCloseUpAni.SetTrigger("Close");
        StartCoroutine(CardCloseUnCard(0.3f));
    }

    private IEnumerator CardCloseUnCard(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < 8; i++)
            closeUpCards[i].gameObject.SetActive(false);
    }

    public void ActFilterCostBtn(int n)
    {
        CardDataInput(n);
    }

    public void ActMakingBtn()
    {
        Debug.Log("제작");
    }
}
