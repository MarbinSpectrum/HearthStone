using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyCollectionsMenu : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Animator jobListAni;
    Animator filterAni;
    Animator[] costBtnAni;
    Animator pageAni;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    EventTrigger.Entry pointerEnter;
    EventTrigger.Entry pointerDown;
    EventTrigger.Entry pointerExit;
    EventTrigger.Entry pointerClick;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Image goBackImg;
    Image selectJobImg;
    Image filterImg;
    Image makingImg;

    Image completeImg;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum ButtonState { 보통, 누름 }

    public Sprite[] goBackSprites;
    public Sprite[] selectJobSprites;
    public Sprite[] filterSprites;
    public Sprite[] makingSprites;

    public Sprite[] completeSprites;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

    public CardView[] nowCards = new CardView[8];
    public CardView[] nextCards = new CardView[8];
    int nowJobIndex = 0;
    int nowCardIndex = 0;
    bool nextPageFlag = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    #region[Awake]
    private void Awake()
    {
        jobListAni = transform.Find("JobList").GetComponent<Animator>();
        filterAni = transform.Find("Filter").GetComponent<Animator>();
        pageAni = transform.Find("Page").GetComponent<Animator>();

        goBackImg = transform.Find("뒤로").GetComponent<Image>();
        selectJobImg = transform.Find("직업").GetComponent<Image>();
        filterImg = transform.Find("필터").GetComponent<Image>();
        makingImg = transform.Find("제작").GetComponent<Image>();

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

        for(int i = 0; i < cardDatas.Length; i++)
            cardDatas[i] = new List<CardData>();
        CardDataInput();
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {

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
    public void CardDataInput()
    {
        for (int i = 0; i < 3; i++)
            cardDatas[i].Clear();

        for (int i = 0; i < 3; i++)
            for (int j = 1; j <= DataMng.instance.m_dic[(DataMng.TableType)i].m_table.Count; j++)
            {
                cardDatas[i].Add(GetCardData(j, (DataMng.TableType)i));
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
            makingImg.sprite = makingSprites[(int)ButtonState.보통];
        }
        ShowCard();
    }
    #endregion

    #region[카드표시]
    public void ShowCard()
    {
        if (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperNext") && pageAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f && nextPageFlag)
        {
            nextPageFlag = false;
            nowCardIndex += 8;
        }
        else if (pageAni.GetCurrentAnimatorStateInfo(0).IsName("PaperStop") && !nextPageFlag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                nextPageFlag = true;
                pageAni.SetTrigger("Next");
            }

            int nextCardIndex = nowCardIndex + 8;

            for (int i = 0; i < 8; i++)
            {
                if (cardDatas[nowJobIndex].Count <= i + nextCardIndex || i + nextCardIndex < 0)
                {
                    nextCards[i].gameObject.SetActive(false);
                }
                else
                {
                    nextCards[i].gameObject.SetActive(true);
                    if (cardDatas[nowJobIndex][i + nextCardIndex].cardType.Equals("하수인"))
                    {
                        nextCards[i].cardType = CardType.하수인;
                        nextCards[i].MinionsCostData = cardDatas[nowJobIndex][i + nextCardIndex].cardCost;
                        nextCards[i].MinionsAttackData = cardDatas[nowJobIndex][i + nextCardIndex].cardAttack;
                        nextCards[i].MinionsHpData = cardDatas[nowJobIndex][i + nextCardIndex].cardHp;
                        nextCards[i].MinionsCardNameData = cardDatas[nowJobIndex][i + nextCardIndex].cardName;
                        nextCards[i].MinionsCardExplainData = cardDatas[nowJobIndex][i + nextCardIndex].cardExplain;
                    }
                }
            }
        }


        for (int i = 0; i < 8; i++)
        {
            if (cardDatas[nowJobIndex].Count <= i + nowCardIndex || i + nowCardIndex < 0)
            {
                nowCards[i].gameObject.SetActive(false);
            }
            else
            {
                nowCards[i].gameObject.SetActive(true);
                if (cardDatas[nowJobIndex][i + nowCardIndex].cardType.Equals("하수인"))
                {
                    nowCards[i].cardType = CardType.하수인;
                    nowCards[i].MinionsCostData = cardDatas[nowJobIndex][i + nowCardIndex].cardCost;
                    nowCards[i].MinionsAttackData = cardDatas[nowJobIndex][i + nowCardIndex].cardAttack;
                    nowCards[i].MinionsHpData = cardDatas[nowJobIndex][i + nowCardIndex].cardHp;
                    nowCards[i].MinionsCardNameData = cardDatas[nowJobIndex][i + nowCardIndex].cardName;
                    nowCards[i].MinionsCardExplainData = cardDatas[nowJobIndex][i + nowCardIndex].cardExplain;
                }
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
        Debug.Log("필터");
        filterAni.SetBool("Show", act);
    }
    #endregion

    public void ActFilterCostBtn(int n)
    {

    }

    public void ActMakingBtn()
    {
        Debug.Log("제작");
    }
}
