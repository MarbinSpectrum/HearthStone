using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HideInInspector]
    public Animator mainMenuAni;

    [HideInInspector]
    public Animator outSideMenuAni;

    Animator battleAni;
    Animator adventureAni;
    Animator meleeAni;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    EventTrigger.Entry pointerEnter;
    EventTrigger.Entry pointerDown;
    EventTrigger.Entry pointerExit;
    EventTrigger.Entry pointerClick;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject battleMenuUI;
    public GameObject myCollectionsMenuUI;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Image battleImg;
    Image adventureImg;
    Image meleeImg;

    Image questGlowImg;
    Image openPacksGlowImg;
    Image shopGlowImg;
    Image myCollectionsGlowImg;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum ButtonState { 보통, 누름, 선택 }

    public Sprite[] battleSprites;
    public Sprite[] adventureSprites;
    public Sprite[] meleeSprites;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Awake]
    private void Awake()
    {
        instance = this;

        mainMenuAni = GetComponent<Animator>();

        battleAni = transform.Find("LeftBoard").Find("Center").Find("대전").GetComponent<Animator>();
        adventureAni = transform.Find("LeftBoard").Find("Center").Find("모험").GetComponent<Animator>();
        meleeAni = transform.Find("LeftBoard").Find("Center").Find("난투").GetComponent<Animator>();

        battleImg = transform.Find("LeftBoard").Find("Center").Find("대전").GetChild(0).GetComponent<Image>();
        adventureImg = transform.Find("LeftBoard").Find("Center").Find("모험").GetChild(0).GetComponent<Image>();
        meleeImg = transform.Find("LeftBoard").Find("Center").Find("난투").GetChild(0).GetComponent<Image>();

        #region[battleTrigger]
        EventTrigger battleTrigger = transform.Find("LeftBoard").Find("Center").Find("대전").GetChild(0).GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
            {
                battleAni.SetTrigger("Shake");
                battleImg.sprite = battleSprites[(int)ButtonState.선택];
            }
        });
        battleTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                battleImg.sprite = battleSprites[(int)ButtonState.누름];
        });
        battleTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            battleImg.sprite = battleSprites[(int)ButtonState.보통];
        });
        battleTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActBattleBtn();
        });
        battleTrigger.triggers.Add(pointerClick);
        #endregion

        #region[adventureTrigger]
        EventTrigger adventureTrigger = transform.Find("LeftBoard").Find("Center").Find("모험").GetChild(0).GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
            {
                adventureAni.SetTrigger("Shake");
                adventureImg.sprite = adventureSprites[(int)ButtonState.선택];
            }
        });
        adventureTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                adventureImg.sprite = adventureSprites[(int)ButtonState.누름];
        });
        adventureTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            adventureImg.sprite = adventureSprites[(int)ButtonState.보통];
        });
        adventureTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActAdventureBtn();
        });
        adventureTrigger.triggers.Add(pointerClick);
        #endregion

        #region[meleeTrigger]
        EventTrigger meleeTrigger = transform.Find("LeftBoard").Find("Center").Find("난투").GetChild(0).GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
            {
                meleeAni.SetTrigger("Shake");
                meleeImg.sprite = meleeSprites[(int)ButtonState.선택];
            }
        });
        meleeTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                meleeImg.sprite = meleeSprites[(int)ButtonState.누름];
        });
        meleeTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            meleeImg.sprite = meleeSprites[(int)ButtonState.보통];
        });
        meleeTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActMeleeBtn();
        });
        meleeTrigger.triggers.Add(pointerClick);
        #endregion

        outSideMenuAni = transform.Find("OutSideMenu").GetComponent<Animator>();

        questGlowImg = transform.Find("OutSideMenu").Find("퀘스트").Find("Glow").GetComponent<Image>();
        openPacksGlowImg = transform.Find("OutSideMenu").Find("팩개봉").Find("Glow").GetComponent<Image>();
        shopGlowImg = transform.Find("OutSideMenu").Find("상점").Find("Glow").GetComponent<Image>();
        myCollectionsGlowImg = transform.Find("OutSideMenu").Find("수집품").Find("Glow").GetComponent<Image>();

        #region[questTrigger]
        EventTrigger questTrigger = transform.Find("OutSideMenu").Find("퀘스트").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                questGlowImg.enabled = true;
        });
        questTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                questGlowImg.enabled = true;
        });
        questTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            questGlowImg.enabled = false;
        });
        questTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActQuestBtn();
        });
        questTrigger.triggers.Add(pointerClick);
        #endregion

        #region[openPacksTrigger]
        EventTrigger openPacksTrigger = transform.Find("OutSideMenu").Find("팩개봉").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                openPacksGlowImg.enabled = true;
        });
        openPacksTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                openPacksGlowImg.enabled = true;
        });
        openPacksTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            openPacksGlowImg.enabled = false;
        });
        openPacksTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActOpenPacksBtn();
        });
        openPacksTrigger.triggers.Add(pointerClick);
        #endregion

        #region[shopTrigger]
        EventTrigger shopTrigger = transform.Find("OutSideMenu").Find("상점").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                shopGlowImg.enabled = true;
        });
        shopTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                shopGlowImg.enabled = true;
        });
        shopTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            shopGlowImg.enabled = false;
        });
        shopTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActShopBtn();
        });
        shopTrigger.triggers.Add(pointerClick);
        #endregion

        #region[myCollectionsTrigger]
        EventTrigger myCollectionsTrigger = transform.Find("OutSideMenu").Find("수집품").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                myCollectionsGlowImg.enabled = true;
        });
        myCollectionsTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                myCollectionsGlowImg.enabled = true;
        });
        myCollectionsTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            myCollectionsGlowImg.enabled = false;
        });
        myCollectionsTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActmyCollectionsBtn();
        });
        myCollectionsTrigger.triggers.Add(pointerClick);
        #endregion

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

    #region[UpdateUI]
    public void UpdateUI()
    {
        if (Input.GetMouseButtonUp(0))
        {
            battleImg.sprite = battleSprites[(int)ButtonState.보통];
            adventureImg.sprite = adventureSprites[(int)ButtonState.보통];
            meleeImg.sprite = meleeSprites[(int)ButtonState.보통];

            questGlowImg.enabled = false;
            openPacksGlowImg.enabled = false;
            shopGlowImg.enabled = false;
            myCollectionsGlowImg.enabled = false;
        }
    }
    #endregion

    public void ActBattleBtn()
    {
        Debug.Log("대전");
        OpenBoard();
        battleMenuUI.SetActive(true);
    }

    public void ActAdventureBtn()
    {
        Debug.Log("모험");
        OpenBoard();
    }

    public void ActMeleeBtn()
    {
        Debug.Log("난투");
        OpenBoard();
    }

    public void ActQuestBtn()
    {
        Debug.Log("퀘스트");
        OpenBoard();
    }

    public void ActOpenPacksBtn()
    {
        Debug.Log("팩 개봉");
        OpenBoard();
    }

    public void ActShopBtn()
    {
        Debug.Log("상점");
        OpenBoard();
    }

    public void ActmyCollectionsBtn()
    {
        Debug.Log("수집품");
        OpenBoard();
        myCollectionsMenuUI.SetActive(true);
    }

    public void OpenBoard()
    {
        mainMenuAni.SetTrigger("Turn");
        StartCoroutine(ShowOutMenu(1, false));
    }

    public void CloseBoard()
    {
        mainMenuAni.SetTrigger("ReTurn");
        StartCoroutine(ShowOutMenu(1,true));
    }

    private IEnumerator ShowOutMenu(float waitTime,bool state)
    {
        yield return new WaitForSeconds(waitTime);
        outSideMenuAni.SetBool("In", state);
    }
}
