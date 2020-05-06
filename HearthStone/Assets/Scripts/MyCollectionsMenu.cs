using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyCollectionsMenu : MonoBehaviour
{
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum ButtonState { 보통, 누름 }

    public Sprite[] goBackSprites;
    public Sprite[] selectJobSprites;
    public Sprite[] filterSprites;
    public Sprite[] makingSprites;

    #region[Awake]
    private void Awake()
    {
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
            ActSelectJobBtn();
        });
        selectJobTrigger.triggers.Add(pointerClick);
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
            ActFilterJobBtn();
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
    }
    #endregion

    public void ActGoBackBtn()
    {
        MainMenu.instance.CloseBoard();
        StartCoroutine(CloseBattleMenu(1));
        Debug.Log("뒤로가기");
    }

    private IEnumerator CloseBattleMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }

    public void ActSelectJobBtn()
    {
        Debug.Log("직업");
    }

    public void ActFilterJobBtn()
    {
        Debug.Log("필터");
    }

    public void ActMakingBtn()
    {
        Debug.Log("제작");
    }
}
