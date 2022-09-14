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
    [SerializeField] private GameObject cancleBtn;

    [Header("필터")]
    [SerializeField] private GameObject filterBtn;
    public Animator filterAni;
    [SerializeField] private GameObject costCancleObject;
    [SerializeField] private Image cancleCost;
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
    [SerializeField] private GameObject nextArrow;
    [SerializeField] private GameObject backArrow;
    [SerializeField] private Animator pageAni;

    [Header("직업버튼")]
    [SerializeField] private GameObject[] JobBtn;
    public Text selectjobText;
    public Animator jobListAni;

    [Header("현재페이지 카드")]
    public GameObject nowCard;
    public GameObject nowNotCard;
    private CardView[] nowCards = new CardView[8];
    [SerializeField] private GameObject[] pageCardNum_Obj = new GameObject[8];
    private Text[] pageCardNum_Txt = new Text[8];
    [SerializeField] private GameObject[] pageCardLock = new GameObject[8];
    private Text[] pageCardLock_Txt = new Text[8];
    [SerializeField] private GameObject[] pageCardLockFrame = new GameObject[8];
    private Image[] pageCardLockFrame_Img = new Image[8];
    [SerializeField] private Sprite lockFrame_M;
    [SerializeField] private Sprite lockFrame_S;

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
    CardData closeUpCardData;
    //string nowCloseUpcardName;
    //string nowCloseUpcardLevel;
    //string nowCloseUpcardType;
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

    [Header("덱 갯수 표시")]
    [SerializeField] private GameObject viewDeckCount;  //덱 갯수 표시 오브젝트
    [SerializeField] private Image hasDeckNum;          //덱 갯수 표시

    [Header("덱 리스트")]
    public RectTransform deckContext;
    public DeckBtn[] deckBtn;
    [SerializeField] private GameObject deckListView;
    [SerializeField] private GameObject deckCardView;
    [SerializeField] private GameObject deckBannerView;
    [SerializeField] private DeckBtn deckBannerBtn;
    [SerializeField] private RectTransform deckCardContext;
    public RectTransform newDeckPos;
    public RectTransform deckObject;
    [HideInInspector] public bool deckCardViewFlag;

    [Header("덱 세부 정보")]
    [HideInInspector] public int nowDeck = -1;  //현재 선택중인 덱
    [SerializeField] private CardDrag[] deckCardObject;
    private List<string> deckCardList = new List<string>();
    private float deckCardShow = 0;
    [SerializeField] private GameObject viewDeckCardCount;
    [SerializeField] private Image []hasDeckCardNum;
    [SerializeField] private GameObject dontClick;
    [SerializeField] private Animator checkDeckMakeAni;
    [SerializeField] private GameObject checkDeckMake;
    [SerializeField] private Animator deckSettingAni;
    public GameObject deckSetting;
    [SerializeField] private Image[] deckHeroPower;
    [SerializeField] private Text heroPowerName;
    [SerializeField] private Text heroPowerExplain;
    [SerializeField] private RectTransform[] deckCardCostNum;
    [SerializeField] private RectTransform[] deckCardCostBar;
    [SerializeField] private GameObject checkDeckDelete;
    [SerializeField] private Animator checkDeckDeleteAni;
    public GameObject newCreateDeck;
    [SerializeField] private DeckBtn newCreateDeckBtn;
    public GameObject dontClick_remove;
    private int deleteDeckNum = -1;
    private TouchScreenKeyboard keyboard;
    string keyboardText = "";
    public Animator reNameUI;
    public InputField reNameInput;
    string baseName;

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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        for (int i = 0; i < 8; i++)
        {
            pageCardNum_Txt[i] = pageCardNum_Obj[i].transform.Find("Text").GetComponent<Text>();
            pageCardLock_Txt[i] = pageCardLock[i].transform.Find("Text").GetComponent<Text>();
            pageCardLockFrame_Img[i] = pageCardLockFrame[i].GetComponent<Image>();

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

        gameObject.SetActive(false);
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        InitCollectMenu();
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

    #region[InitCollectMenu]
    private void InitCollectMenu()
    {
        StartCoroutine(CardCloseUpCard(0.1f));

        nowJobIndex = 0;
        nowCardIndex = 0;
        nowJob = DataMng.TableType.모두;
        SoundManager.instance.PlayBGM("수집함배경음");
        Setting.instance.OnOffSetting(false);
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[내수집품설정]
    public void CardDataInput(DataMng.TableType job = DataMng.TableType.모두,
        int cost = -1, string search = "",
        bool make = false)
    {
        //job : 직업, cost : 가격, search : 이름,make : 제작여부

        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
            return;

        PlayData playData = dataMng.playData;
        if (playData == null)
            return;

        for (int i = 0; i < 3; i++)
            cardDatas[i].Clear();

        for (int i = 0; i < 3; i++)
        {
            DataMng.TableType nowJobCard = (DataMng.TableType)i;
            LowBase cardTable = dataMng.m_dic[nowJobCard];

            for (int j = 1; j <= cardTable.m_table.Count; j++)
            {
                if(job != DataMng.TableType.모두)
                    if (nowJobCard != DataMng.TableType.중립)
                        if (job != nowJobCard)
                        {
                            //모두를 선택한 경우가 아니고
                            //중립카드가 아니며
                            //필터에서 찾고자 하는 직업군이 아닐때는
                            //표시할 카드군에서 제외한다.
                            continue;
                        }

                string cardLevel = cardTable.ToString(j, "등급");
                string cardName = cardTable.ToString(j, "카드이름");
                string cardExplain = cardTable.ToString(j, "카드설명");
                int cardCost = cardTable.ToInteger(j, "코스트");

                if (cardLevel.Equals("토큰"))
                {
                    //토큰 카드는 표시하지 않는다.
                    continue;
                }

                if(cost == -1)
                {
                    //cost = -1은
                    //코스트 상관없이 카드를 찾는다는 뜻
                }
                else if (cost == 7 && cardCost < cost)
                {
                    //cost = 7은
                    //7 코스트 이상은 모두 찾는다는 뜻
                    continue;
                }
                else if (cardCost != cost)
                {
                    //나머지는
                    //찾고자 하는 코스트가 아니라면 제외한다.
                    continue;
                }

                if(String.IsNullOrEmpty(search))
                {
                    //공백이라면
                    //검색하고자하는 내용이 없다는 뜻이다.
                }
                else if(cardName.Contains(search) == false &&
                        cardExplain.Contains(search) == false)
                {
                    //찾고자하는 내용이
                    //카드의 이름과 설명에 없다면
                    //해당 카드는 제외한다.
                    continue;
                }

                if (make == false && playData.GetCardNum(cardName) == 0)
                {
                    //제작모드가 아니고
                    //플레이어가 해당카드를 보유하지 않았다면
                    //해당 카드는 제외한다.
                    continue;
                }

                //위의 필터에 걸리지 않은 카드는 카드데이터에 등록한다.
                cardDatas[i].Add(GetCardData(j, (DataMng.TableType)i));
            }

            //카드를 코스트 별로 정렬한다.
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
    public CardData GetCardData(int num, DataMng.TableType job)
    {
        //해당 직업의 num번째에 해당하는 카드를 받아온다.
        DataMng dataMng = DataMng.instance;

        return new CardData(job,
            dataMng.ToString(job, num, "등급"),
            dataMng.ToString(job, num, "카드이름"),
            dataMng.ToString(job, num, "카드종류"),
            dataMng.ToInteger(job, num, "코스트"),
            dataMng.ToInteger(job, num, "공격력"),
            dataMng.ToInteger(job, num, "체력"),
            dataMng.ToString(job, num, "카드설명"),
            dataMng.ToInteger(job, num, "BuyPowder"),
            dataMng.ToInteger(job, num, "SellPowder"));
    }
    #endregion

    #region[UpdateUI]
    public void UpdateUI()
    {
        //카드리스트
        ShowCard();

        //플레이어 덱리스트
        DeckUpdate();

        //페이지가 넘어가는 애니메이션 진행중인지 확인
        AnimatorStateInfo paperAnimator = pageAni.GetCurrentAnimatorStateInfo(0);
        bool arrowFlag = (paperAnimator.IsName("PaperStop") && paperAnimator.normalizedTime >= 0.2f);

        //좌우 이동 화살표 활성화
        backArrow.SetActive(arrowFlag && hasBackPage);
        nextArrow.SetActive(arrowFlag && hasNextPage);

        //직업표시
        bool jobTextFlag = false;
        for (int i = 0; i < cardDatas.Length; i++)
        {
            //직업카드가 존재하는지 확인후
            //존재하면 직업카드가 존재함을 표시
            bool hasJobCard = cardDatas[i].Count > 0;
            jobTextFlag |= hasJobCard;
            JobBtn[i].gameObject.SetActive(hasJobCard);
        }
        //현재선택중인 직업을 UI에 표시
        DataMng.TableType tableName = (DataMng.TableType)(nowJobIndex);
        selectjobText.text = jobTextFlag ? tableName.ToString() : "";

        //삭제할 덱이 있다면 나오는 애니메이션
        UpdateDeckRemoveAni();

        //덱이 선택되었을때 애니메이션
        UpdateSelectDeckAni();
        
        if(nowDeck == -1)
        {
            //덱 미선택시, 덱 갯수를 표시
            viewDeckCardCount.SetActive(false);
            viewDeckCount.SetActive(true);
        }
        else
        {
            //덱 선택시, 덱의 카드 갯수를 표시
            viewDeckCardCount.SetActive(true);
            viewDeckCount.SetActive(false);
        }

        //덱 보여주기
        UpdatePlayerDeck();

        //덱이름 짓기 처리
        UpdateNamedDeck();
    }

    public void DeckUpdate()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        int deckNum = playData.deck.Count;
        hasDeckNum.sprite = dataMng.num[deckNum];
        deckContext.sizeDelta = new Vector2(deckContext.sizeDelta.x, 185.4f * Mathf.Min(deckNum + 1, 9));

        for (int i = 0; i < deckNum; i++)
        {
            //플레이어의 덱을 표시해준다.
            deckBtn[i].deckNameTxt.text = playData.deck[i].name;
            deckBtn[i].nowCharacter = (int)playData.deck[i].job;
        }

        for (int i = 0; i < 9; i++)
        {
            //덱 표시 오브젝트들을 업데이트한다.
            deckBtn[i].gameObject.SetActive(i < Mathf.Min(deckNum + 1, 9));
            deckBtn[i].hasDeck = (i < deckNum);
            deckBtn[i].ImageUpdate();
        }
    }

    public void DeckSort(int idx)
    {
        deckCardList.Clear();
        for (int i = 0; i < DataMng.instance.playData.deck[idx].card.Count; i++)
            deckCardList.Add(DataMng.instance.playData.deck[idx].card[i]);

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
    }
    public void DeckSort()
    {
        DeckSort(nowDeck);
    }
    #endregion

    #region[UpdateDeckRemoveAni]
    private void UpdateDeckRemoveAni()
    {
        //삭제할 덱이 있다
        if (deleteDeckNum != -1)
        {
            if (newCreateDeckBtn.characterDeckRect.anchoredPosition.y < 0)
            {
                if (DataMng.instance.playData.deck.Count == 9)
                    newCreateDeckBtn.hide = false;
                else
                    newCreateDeckBtn.hide = true;

                newCreateDeckBtn.characterDeckRect.anchoredPosition += new Vector2(0, Time.deltaTime) * 400;
                newCreateDeckBtn.newDeckRect.anchoredPosition += new Vector2(0, Time.deltaTime) * 400;
                for (int i = deleteDeckNum; i < 9; i++)
                {
                    deckBtn[i].characterDeckRect.anchoredPosition += new Vector2(0, Time.deltaTime) * 400;
                    deckBtn[i].newDeckRect.anchoredPosition += new Vector2(0, Time.deltaTime) * 400;
                }
            }
            else
            {
                Debug.Log("올라가는가기완료");
                newCreateDeckBtn.characterDeckRect.anchoredPosition = new Vector2(0, 0);
                newCreateDeckBtn.newDeckRect.anchoredPosition = new Vector2(0, 0);
                for (int i = deleteDeckNum; i < 9; i++)
                {
                    deckBtn[i].characterDeckRect.anchoredPosition = new Vector2(0, 0);
                    deckBtn[i].newDeckRect.anchoredPosition = new Vector2(0, 0);
                }

                dontClick_remove.SetActive(false);
                deleteDeckNum = -1;
            }
        }
        else
        {
            newCreateDeckBtn.hide = true;
        }
    }
    #endregion

    #region[UpdateSelectDeckAni]
    private void UpdateSelectDeckAni()
    {
        if (deckCardViewFlag)
        {
            if (deckObject.anchoredPosition.y < newDeckPos.anchoredPosition.y + 2.5f)
            {
                deckObject.anchoredPosition += new Vector2(0, Time.deltaTime * 7);
                dontClick.SetActive(true);
                deckCardShow = 0;
            }
            else if (deckObject.anchoredPosition.y < 0)
                deckObject.anchoredPosition += new Vector2(0, Mathf.Max(2, 
                    Time.deltaTime * Mathf.Pow(Mathf.Abs(deckObject.anchoredPosition.y), 1.4f)));
            else if (deckCardShow == 0)
            {
                ActCancleBtn(false);
                deckCardShow += Time.deltaTime * 10;
            }
            else
            {
                deckCardShow += Time.deltaTime * 10;
                deckCardShow = Mathf.Min(deckCardShow, 30);
                deckObject.anchoredPosition = new Vector2(deckObject.anchoredPosition.x, 0);
            }
            if (deckCardShow > 10)
                dontClick.SetActive(false);
        }
        else if (nowDeck != -1)
        {
            if (deckCardShow > 0)
            {
                deckCardShow = 0;
                dontClick.SetActive(true);
            }
            else if (nowJob != DataMng.TableType.모두)
            {
                int j = nowJobIndex;
                int c = nowCardIndex;
                ActCancleBtn(false);
                if (cardmakeFlag)
                    ChangeCardMakeMode();
                nowJob = DataMng.TableType.모두;
                CardDataInput(nowJob, nowCost, nowSearch, cardmakeFlag);
                nowJobIndex = j;
                nowCardIndex = c;
                deckCardList.Clear();
            }
            else if (deckObject.anchoredPosition.y > newDeckPos.anchoredPosition.y + 2.5f)
                deckObject.anchoredPosition -= new Vector2(0, Mathf.Max(2, Time.deltaTime *
                    Mathf.Pow(Mathf.Abs(newDeckPos.anchoredPosition.y - deckObject.anchoredPosition.y), 1.3f)));
            else if (deckObject.anchoredPosition.y > newDeckPos.anchoredPosition.y)
                deckObject.anchoredPosition -= new Vector2(0, 20 * Time.deltaTime);
            else
            {
                deckCardShow = 0;
                nowDeck = -1;
                deckObject.anchoredPosition = new Vector2(deckObject.anchoredPosition.x, newDeckPos.anchoredPosition.y);
                deckListView.SetActive(true);
                deckCardView.SetActive(false);
                deckBannerBtn.hide = true;
                dontClick.SetActive(false);
                cancleBtn.gameObject.SetActive(false);
                filterBtn.gameObject.SetActive(true);
            }
            CardViewManager.instance.UpdateCardView();
        }
    }
    #endregion

    #region[UpdatePlayerDeck]
    private void UpdatePlayerDeck()
    {
        if (nowDeck != -1)
        {
            DataMng dataMng = DataMng.instance;
            PlayData playData = dataMng.playData;

            if (deckCardList.Count != playData.deck[nowDeck].card.Count)
                DeckSort();

            deckBannerBtn.deckNameTxt.text = playData.deck[nowDeck].name;
            deckBannerBtn.nowCharacter = (int)playData.deck[nowDeck].job;

            for (int i = 0; i < Deck.MAX_DECK_CARD; i++)
            {
                //덱의 카드를 UI로 표시
                //PlayData.Deck.MAX_DECK_CARD = 30
                if (i < Mathf.Min((int)deckCardShow, playData.deck[nowDeck].card.Count))
                {
                    //카드를 표시할 슬롯에 
                    //정보를 기입한다.
                    string cardName = playData.GetCardName(deckCardList[i]);
                    int cardNum = playData.GetCardNumber(deckCardList[i]);
                    deckCardObject[i].cardName_Data = cardName;
                    deckCardObject[i].hasCardNum = cardNum;
                    deckCardObject[i].gameObject.SetActive(true);
                }
                else
                {
                    //남는 슬롯은 비활성화
                    deckCardObject[i].gameObject.SetActive(false);
                }
            }

            //덱의 카드 수만큼 UI조절
            int deckCardNum = 0;
            for (int i = 0; i < Deck.MAX_DECK_CARD; i++)
                if (deckCardObject[i].gameObject.activeSelf)
                    deckCardNum++;
            deckCardContext.sizeDelta = new Vector2(deckCardContext.sizeDelta.x, 86f * (deckCardNum + 1));

            int cardN = DataMng.instance.playData.deck[nowDeck].CountCardNum();
            if (cardN / 10 > 0)
            {
                hasDeckCardNum[0].sprite = DataMng.instance.num[cardN / 10];
                hasDeckCardNum[0].enabled = true;
            }
            else
                hasDeckCardNum[0].enabled = false;

            hasDeckCardNum[1].sprite = DataMng.instance.num[cardN % 10];
        }
    }
    #endregion

    #region[UpdateNamedDeck]
    private void UpdateNamedDeck()
    {
        if (TouchScreenKeyboard.visible == false && keyboard != null)
        {
            if (keyboard.done == true)
            {
                keyboardText = keyboard.text;
                deckBannerBtn.deckNameTxt.text = keyboardText;
                DataMng.instance.playData.deck[nowDeck].name = keyboardText;
                deckBtn[nowDeck].deckNameTxt.text = keyboardText;
                keyboard = null;
            }
        }
    }
    #endregion

    #region[카드들표시]
    public void ShowCard()
    {
        //다음 페이지가 있는지 검사
        hasNextPage = false;
        //현재 직업의 다음페이지에 해당하는 카드군이 존재하는지 확인
        hasNextPage = hasNextPage || (nowCardIndex + 8 < cardDatas[nowJobIndex].Count);
        for (int i = nowJobIndex + 1; i < 3; i++)
        {
            //현재의 다음순서 직업의 카드가 존재하는지 확인
            hasNextPage = hasNextPage || (cardDatas[i].Count > 0);
        }

        //이전 페이지가 있는지 검사
        hasBackPage = false;
        //현재 직업의 이전페이지에 해당하는 카드군이 존재하는지 확인
        hasBackPage = hasBackPage || (nowCardIndex > 0);
        for (int i = 0; i < nowJobIndex; i++)
        {
            //현재의 이전순서 직업의 카드가 존재하는지 확인
            hasBackPage = hasBackPage || (cardDatas[i].Count > 0);
        }

        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        #region[카드표시]
        for (int i = 0; i < 8; i++)
        {
            //카드는 한페이지당 8개씩 묶음으로 표시된다.
            //마지막 페이지는 남은 카드를 모두 표시해준다.
            int tempCardIndex = i + nowCardIndex;
            if (tempCardIndex < Mathf.Min(nowCardIndex + 8, cardDatas[nowJobIndex].Count))
            {
                //활성화해야하는 카드 객체인지 검사

                //카드 정보를 가져온다.
                CardData nowCardData = cardDatas[nowJobIndex][tempCardIndex];
                string nowCardName = nowCardData.cardName;
                string nowCardLevel = nowCardData.cardLevel;
                string nowCardType = nowCardData.cardType;

                //플레이어가 보유중인 해당카드의 갯수
                int cardNum = playData.GetCardNum(nowCardName);

                AnimatorStateInfo cardCloseUpAnimator = cardCloseUpAni.GetCurrentAnimatorStateInfo(0);
                if (cardCloseUpAnimator.IsName("CardCloseUpStop"))
                {
                    //카드를 확대해서 보고 있는 중이 아니고
                    //해당 카드를 한개 이상 보유중이라면
                    //카드 갯수 표시 UI를 활성화
                    pageCardNum_Obj[i].SetActive(cardNum > 0);
                }
                else
                {
                    //카드를 확대해서 보고 있는 중이고
                    //해당 카드를 한개 이상 보유중이라면
                    //현재 확대중인 카드를 제외한 객체들의
                    //카드 갯수 표시 UI를 활성화
                    pageCardNum_Obj[i].SetActive(cardNum > 0 && i != nowCloseUpCardView);
                }

                if (nowDeck == -1)
                {
                    //현재 덱을 선택중이 아니라면
                    //잠금표시(더이상 덱에 해당 카드를 추가할 수 없다는것을 알린다) UI를 비활성화
                    pageCardLock[i].SetActive(false);
                    pageCardLockFrame[i].SetActive(false);
                    pageCardNum_Txt[i].text = "X" + cardNum;
                }
                else
                {
                    //전설 등급카드는 덱에 종류별로 한장씩만 넣을 수 있다.
                    //다른 등급카드는 두장씩 넣을 수 있다.
                    int maxNum = nowCardLevel.Equals("전설") ? 1 : 2;

                    //덱에있는 카드수
                    int cardInDeckNum = playData.deck[nowDeck].HasCardNum(nowCardName);

                    if (Mathf.Min(maxNum, cardNum) - cardInDeckNum == 0 && cardInDeckNum != 0)
                    {
                        //덱에 있는 카드 갯수를 파악후
                        //덱에 넣을 수 있는지를 표시
                        //덱에 넣을 수 없다면
                        //잠금표시(더이상 덱에 해당 카드를 추가할 수 없다는것을 알린다) UI를 활성화
                        if (maxNum == 1)
                            pageCardLock_Txt[i].text = "덱 한도:1";
                        else if (maxNum == 2 && cardNum == 2)
                            pageCardLock_Txt[i].text = "덱 한도:2";
                        else if (maxNum == 2 && cardNum == 1)
                            pageCardLock_Txt[i].text = "카드없음";

                        //카드가 주문인지 하수인인지에 따라서
                        //잠금표시 UI의 이미지가 다르다.
                        //필요한 스프라이트를 골라서 배치한다.
                        Sprite lockSprite = nowCardType.Equals("주문") ? lockFrame_S : lockFrame_M;
                        pageCardLockFrame_Img[i].sprite = lockSprite;

                        if (cardCloseUpAnimator.IsName("CardCloseUpStop"))
                        {
                            //카드를 확대해서 보고 있는 중이 아니라면
                            //모든 객체에게 잠금표시 UI를 표시
                            pageCardLock[i].SetActive(true);
                            pageCardLockFrame[i].SetActive(true);
                        }
                        else
                        {
                            //카드를 확대해서 보고 있는 중이라면
                            //현재 확대중인 카드를 제외한 객체들의
                            //잠금표시 UI를 표시
                            pageCardLock[i].SetActive(i != nowCloseUpCardView);
                            pageCardLockFrame[i].SetActive(i != nowCloseUpCardView);
                        }
                    }
                    else
                    {
                        //덱에 넣을 수 있는 상태라면
                        //현재 수집함에 남은 카드 수를 표시해준다.
                        pageCardNum_Txt[i].text = "X" + (cardNum - cardInDeckNum);
                        pageCardLock[i].SetActive(false);
                        pageCardLockFrame[i].SetActive(false);
                    }
                }

                //카드 오브젝트를 카드데이터에 따라서 갱신
                CardShow(ref nowCards[i], nowCardData);
            }
            else
            {
                //카드 오브젝트 비활성화
                //아마 마지막 페이지에서만 활성화 될것이다.
                pageCardNum_Obj[i].SetActive(false);
                nowCards[i].gameObject.SetActive(false);
                pageCardLock[i].SetActive(false);
                pageCardLockFrame[i].SetActive(false);
            }
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
    public void CardShow(ref CardView card, CardData cardData)
    {
        if (card == null)
            return;
        if (cardData == null)
            return;
        card.gameObject.SetActive(true);
        string cardType = cardData.cardType;

        if (cardType.Equals("하수인"))
        {
            //하수운카드 표시
            card.cardType = CardType.하수인;
            card.MinionsCostData = cardData.cardCost;
            card.MinionsAttackData = cardData.cardAttack;
            card.MinionsHpData = cardData.cardHp;
            card.MinionsCardNameData = cardData.cardName;
            card.MinionsCardExplainData = cardData.cardExplain;
        }
        else if (cardType.Equals("주문"))
        {
            //주문카드 표시
            card.cardType = CardType.주문;
            card.SpellCostData = cardData.cardCost;
            card.SpellCardNameData = cardData.cardName;
            card.SpellCardExplainData = cardData.cardExplain;
        }
        else if (cardType.Equals("무기"))
        {
            //무기카드 표시
            card.cardType = CardType.무기;
            card.WeaponCostData = cardData.cardCost;
            card.WeaponAttackData = cardData.cardAttack;
            card.WeaponHpData = cardData.cardHp;
            card.WeaponCardNameData = cardData.cardName;
            card.WeaponCardExplainData = cardData.cardExplain;
        }

        //등급
        card.cardLevel = cardData.cardLevel;

        //직업
        DataMng.TableType tableName = (DataMng.TableType)(nowJobIndex);
        card.cardJob = tableName.ToString();
    }

    public void CardShow(ref CardView card,int nowJobIndex,int cardIndex)
    {
        CardData cardData = cardDatas[nowJobIndex][cardIndex];
        CardShow(ref card, cardData);       
    }
    #endregion

    #region[카드페이지 넘기기]
    public void NextPage()
    {
        AnimatorStateInfo paperAnimator = pageAni.GetCurrentAnimatorStateInfo(0);
        if (paperAnimator.IsName("PaperStop"))
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
        AnimatorStateInfo paperAnimator = pageAni.GetCurrentAnimatorStateInfo(0);
        if (paperAnimator.IsName("PaperStop"))
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
        AnimatorStateInfo paperAnimator = pageAni.GetCurrentAnimatorStateInfo(0);
        if (paperAnimator.IsName("PaperStop"))
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

        MainMenu mainMenu = MainMenu.instance;
        mainMenu.BattleMenu(true);
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
        int j = nowJobIndex;
        if (!act)
        {
            if (String.IsNullOrWhiteSpace(searchText.text))
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
                CardViewManager.instance.UpdateCardView();
            }
        }
        nowJobIndex = j;
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
            int j = nowJobIndex;
            CardDataInput(nowJob , nowCost, nowSearch, cardmakeFlag);
            nowJobIndex = j;
        }
        filterBtn.gameObject.SetActive(!act);
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드 클로우즈업]
    public void CardCloseUp(int n)
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        CardViewManager.instance.UpdateCardView();
        for(int i = 0; i < 8; i++)
            closeUpCards[i].gameObject.SetActive(false);
        cardCloseUpAni.SetTrigger("Show");
        SoundManager.instance.PlaySE("수집함카드선택");

        CardShow(ref closeUpCards[n], nowJobIndex, nowCardIndex + n);
        closeUpCardData = cardDatas[nowJobIndex][nowCardIndex + n];

        int cardNum = playData.GetCardNum(closeUpCardData.cardName);
        hasCardNumText.text = cardNum.ToString();

        int nowM = playData.magicPowder;
        for(int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowMagicPowderNumMakeUI[i].sprite = DataMng.instance.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }

        int temp;
        temp = closeUpCardData.cardSellPowder;
        for (int i = 0; i < 4; i++)
        {
            GetMagicPowderNum[i].sprite = DataMng.instance.num[temp % 10];
            GetMagicPowderNum[i].gameObject.SetActive((temp != 0));
            temp /= 10;
        }

        temp = closeUpCardData.cardBuyPowder;
        for (int i = 0; i < 4; i++)
        {
            CostMagicPowderNum[i].sprite = DataMng.instance.num[temp % 10];
            CostMagicPowderNum[i].gameObject.SetActive((temp != 0));
            temp /= 10;
        }

        powderUI.SetActive(closeUpCardData.cardLevel.Equals("기본") == false);
        basicCardUI.SetActive(closeUpCardData.cardLevel.Equals("기본"));
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
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

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

        int cardNum = playData.GetCardNum(closeUpCardData.cardName);
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
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        int cardNum = playData.GetCardNum(closeUpCardData.cardName);
        if (cardNum == CardData.MAX_CARD_NUM)
        {
            //카드 최대 소지갯수는 9개
            //MAX_CARD_NUM = 9
            return;
        }

        if (closeUpCardData.cardLevel.Equals("토큰") ||
            closeUpCardData.cardLevel.Equals("기본"))
        {
            //토큰이나 기본 카드는 만들수 없다.
            return;
        }

        if (closeUpCardData.cardBuyPowder > playData.magicPowder)
        {
            //매직파우더가 부족하다.
            return;
        }

        SoundManager.instance.PlaySE("카드생성");
        StartCoroutine(MakeCard(cardNum, 1.5f));
        StartCoroutine(ShowCardNum(cardNum + 1, 1.5f));
    }

    private IEnumerator MakeCard(int cardNum, float waitTime)
    {
        //카드 종류에 따른 이펙트
        if(closeUpCardData.cardType.Equals("하수인"))
            makeEffectAni.SetTrigger("M_CardCreate");
        else if (closeUpCardData.cardType.Equals("주문"))
            makeEffectAni.SetTrigger("S_CardCreate");
        else if (closeUpCardData.cardType.Equals("무기"))
            makeEffectAni.SetTrigger("W_CardCreate");

        yield return new WaitForSeconds(waitTime);

        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        playData.magicPowder -= closeUpCardData.cardBuyPowder;
        playData.AddCard(closeUpCardData.cardName, 1);

        int nowM = playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = dataMng.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].sprite = dataMng.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        hasCardNumText.text = (cardNum + 1).ToString();

        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드제거]
    public void RemoveCard()
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        int cardNum = playData.GetCardNum(closeUpCardData.cardName);
        if (cardNum == 0)
        {
            //카드가 0개면 제거할 수 없다.
            return;
        }
        if (closeUpCardData.cardLevel.Equals("토큰") || 
            closeUpCardData.cardLevel.Equals("기본"))
        {
            //토큰이나 기본카드는 제거할 수 없다.
            return;
        }

        for(int i = 0; i < playData.deck.Count; i++)
        {
            //해당 카드가 덱에 존재한다면
            if(playData.deck[i].HasCardNum(closeUpCardData.cardName) > 0)
            {
                //해당 카드를 덱에서 제거한다.
                playData.deck[i].PopCard(closeUpCardData.cardName);

                //덱리스트 정렬
                DeckSort(i);
            }
        }

        SoundManager.instance.PlaySE("카드삭제");
        StartCoroutine(RemoveCard(cardNum, 0.5f));
        StartCoroutine(ShowCardNum(cardNum - 1, 0.5f));
    }

    private IEnumerator RemoveCard(int cardNum, float waitTime)
    {
        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        removeEffectAni.SetTrigger("CardRemove");

        yield return new WaitForSeconds(waitTime);

        playData.magicPowder += closeUpCardData.cardSellPowder;
        playData.RemoveCard(closeUpCardData.cardName, 1);

        int nowM = playData.magicPowder;
        for (int i = 0; i < 6; i++)
        {
            nowMagicPowderNum[i].sprite = dataMng.num[nowM % 10];
            nowMagicPowderNum[i].gameObject.SetActive((nowM != 0));
            nowMagicPowderNumMakeUI[i].sprite = dataMng.num[nowM % 10];
            nowMagicPowderNumMakeUI[i].gameObject.SetActive((nowM != 0));
            nowM /= 10;
        }
        hasCardNumText.text = (cardNum - 1).ToString();

        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[코스트로 분류버튼]
    public void ActFilterCostBtn(int n)
    {
        nowSearch = "";
        searchText.text = "";
        int j = nowJobIndex;
        CardDataInput(nowJob,n, nowSearch, cardmakeFlag);
        nowJobIndex = j;
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드제작상태로 변경]
    public void ChangeCardMakeMode()
    {
        int j = nowJobIndex;
        cardmakeFlag = !cardmakeFlag;
        CardDataInput(nowJob, nowCost, nowSearch, cardmakeFlag);
        nowJobIndex = j;
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

    public void SelectCharacterOK(bool b,int job = -1)
    {
        selectCharacterOKAni.SetBool("Show", b);

        if(job != -1)
        {
            for (int i = 0; i < characterImg.Length; i++)
                characterImg[i].enabled = false;
            characterImg[job].enabled = true;
            nowJob = (DataMng.TableType)job;
            switch (nowJob)
            {
                case DataMng.TableType.드루이드:
                    characterNameTxt.text = "말퓨리온 스톰레이지";
                    break;
                case DataMng.TableType.도적:
                    characterNameTxt.text = "발리라 생귀나르";
                    break;
            }
        }
    }

    #endregion

    #region[덱 카드 확인]
    public void DeckCardView(int n)
    {
        SelectCharacter(false);
        SelectCharacterOK(false);
        SoundManager.instance.PlaySE("덱펼치기");
       // deckListView.SetActive(false);
        deckCardView.SetActive(true);
        deckBannerBtn.hide = false;
        deckBannerBtn.hide = false;
        ActCancleBtn(false);
        deckCardContext.anchoredPosition = new Vector2(deckCardContext.anchoredPosition.x, 0);
        deckObject.anchoredPosition = new Vector2(newDeckPos.anchoredPosition.x, newDeckPos.anchoredPosition.y);
        deckCardViewFlag = true;
        nowDeck = n;
        deckCardShow = 0;
        if (cardmakeFlag)
            ChangeCardMakeMode();
    }
    #endregion

    #region[덱 카드 검사]
    public void DeckMakeCheck(bool b)
    {
        if(b)
        {
            if (DataMng.instance.playData.deck[nowDeck].CountCardNum() == 30)
                DeckCardFade();
            else
            {
                checkDeckMakeAni.SetBool("Show", true);
                checkDeckMake.SetActive(b);
            }
        }
        else
        {
            checkDeckMakeAni.SetBool("Show", false);
            checkDeckMake.SetActive(b);
        }
    }
    #endregion

    #region[덱 자동 생성]
    public void DeckAutoCreate()
    {
        deckCardShow = DataMng.instance.playData.deck[nowDeck].CountCardNum();
        int inputNum = 0;
        bool exFlag = false;
        for (int i = 0; i < cardDatas.Length; i++)
        {
            if (i == (int)DataMng.TableType.중립 || i == (int)DataMng.instance.playData.deck[nowDeck].job)
                for (int j = 0; j < cardDatas[i].Count; j++)
                {
                    if (DataMng.instance.playData.deck[nowDeck].CountCardNum() == 30)
                        exFlag = true;
                    if (exFlag)
                        break;
                    DataMng.instance.playData.deck[nowDeck].AddCard(cardDatas[i][j].cardName);
                    inputNum++;
                    DeckSort();
                }
            if (exFlag)
                break;
        }
        StartCoroutine(DeckAutoCreateCor(inputNum));
    }

    private IEnumerator DeckAutoCreateCor(int n)
    {
        n /= 4;
        n = Mathf.Max(1, n);
        for (int i = 0; i < n; i++)
        {
            SoundManager.instance.PlaySE("덱에카드넣기");
            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

    #region[덱 카드 접기]
    public void DeckCardFade()
    {
        SoundManager.instance.PlaySE("덱펼치기");
        deckCardViewFlag = false;
        int deckCardNum = 0;
        for (int i = 0; i < 30; i++)
            if (deckCardObject[i].gameObject.activeSelf)
                deckCardNum++;
        deckCardShow = deckCardNum;
    }
    #endregion

    #region[덱 설정확인]
    public void DeckSetting()
    {
        if(!deckSetting.activeSelf)
        {
            deckSetting.SetActive(true);
            deckSettingAni.SetBool("Show", true);
            for (int i = 0; i < deckHeroPower.Length; i++)
                deckHeroPower[i].enabled = false;
            deckHeroPower[(int)nowJob].enabled = true;
            switch(nowJob)
            {
                case DataMng.TableType.도적:
                    heroPowerName.text = "단검의 대가";
                    heroPowerExplain.text = "1/2 단검을\n장착합니다.";
                    break;
                case DataMng.TableType.드루이드:
                    heroPowerName.text = "변신";
                    heroPowerExplain.text = "방어도를 +1 얻고, 이번 턴에 공격력을 +1 얻습니다.";
                    break;
            }
            for(int i = 0; i <= 7; i++)
            {
                int n = 0;
                for (int j = 0; j < DataMng.instance.playData.deck[nowDeck].card.Count; j++)
                {
                    string name = DataMng.instance.playData.GetCardName(DataMng.instance.playData.deck[nowDeck].card[j]);
                    Vector2 pair = DataMng.instance.GetPairByName(name);
                    int cardC = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
                    int cardN = DataMng.instance.playData.GetCardNumber(DataMng.instance.playData.deck[nowDeck].card[j]);
                    if (cardC == i)
                        n += cardN;
                    else if(i == 7 && cardC >= 7)
                        n += cardN;
                }

                deckCardCostBar[i].sizeDelta = new Vector2(44, 258 * Mathf.Min(n / 7f, 1));
                int[] newY = new int[] { -108, -74, -34, 0, 36, 75, 110, 145 };
                deckCardCostNum[i].anchoredPosition = new Vector2(deckCardCostNum[i].anchoredPosition.x, newY[Mathf.Min(n, 7)]);

                Image T = deckCardCostNum[i].transform.GetChild(0).GetComponent<Image>();
                Image O = deckCardCostNum[i].transform.GetChild(1).GetComponent<Image>();
                T.gameObject.SetActive(true);
                if (n / 10 != 0)
                    T.sprite = DataMng.instance.num[n / 10];
                else
                    T.gameObject.SetActive(false);
                O.sprite = DataMng.instance.num[n % 10];
            }
        }
        else if (deckSettingAni.GetBool("Show"))
        {
            deckSettingAni.SetBool("Show", false);
            StartCoroutine(DeckSetting(0.1f, false));
        }
    }

    private IEnumerator DeckSetting(float waitTime,bool b)
    {
        yield return new WaitForSeconds(waitTime);
        deckSetting.SetActive(b);
    }
    #endregion

    #region[덱 삭제검사]
    public void DeckDeleteCheck(bool b)
    {
        if(b)
        {
            checkDeckDelete.SetActive(true);
            checkDeckDeleteAni.SetBool("Show", true);
        }
        else
        {
            checkDeckDeleteAni.SetBool("Show", false);
            StartCoroutine(DeckDelete(0.1f, false));
        }

    }

    private IEnumerator DeckDelete(float waitTime, bool b)
    {
        yield return new WaitForSeconds(waitTime);
        checkDeckDelete.SetActive(b);
    }
    #endregion

    #region[덱 이름설정]
    public void DeckReName()
    {
        if (Application.platform == RuntimePlatform.Android)
            keyboard = TouchScreenKeyboard.Open(deckBannerBtn.deckNameTxt.text, 
                TouchScreenKeyboardType.Default, false, false, false, false);
        else
        {
            reNameUI.SetBool("Show", true);
            baseName = deckBannerBtn.deckNameTxt.text;
            reNameInput.text = deckBannerBtn.deckNameTxt.text;
        }
    }

    public void DeckReNameOK()
    {
        reNameUI.SetBool("Show", false);
        deckBannerBtn.deckNameTxt.text = reNameInput.text;
        DataMng.instance.playData.deck[nowDeck].name = reNameInput.text;
        deckBtn[nowDeck].deckNameTxt.text = reNameInput.text;
    }

    public void DeckReNameCancle()
    {
        reNameUI.SetBool("Show", false);
        deckBannerBtn.deckNameTxt.text = baseName;
    }
    #endregion

    #region[덱삭제]
    public void DeckDeleteAct(int n)
    {
        dontClick_remove.SetActive(true);
        StartCoroutine(DeckDeleteActCor(n));
    }

    private IEnumerator DeckDeleteActCor(int n)
    {
        yield return new WaitForSeconds(1);
        //deckBtn[n].hide = true;
        SoundManager.instance.PlaySE("덱삭제");
        DataMng.instance.playData.deck.RemoveAt(n);
        DeckUpdate();
        //yield return new WaitForSeconds(0.1f);
        newCreateDeckBtn.characterDeckRect.anchoredPosition = new Vector2(0, -189);
        newCreateDeckBtn.newDeckRect.anchoredPosition = new Vector2(0, -189);
        for (int i = n; i < 9; i++)
        {
            deckBtn[i].characterDeckRect.anchoredPosition = new Vector2(0, -189);
            deckBtn[i].newDeckRect.anchoredPosition = new Vector2(0, -189);
        }
        yield return new WaitForSeconds(0.2f);
        deleteDeckNum = n;
    }
    #endregion

}


