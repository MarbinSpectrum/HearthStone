using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    EventTrigger.Entry pointerEnter;
    EventTrigger.Entry pointerDown;
    EventTrigger.Entry pointerExit;
    EventTrigger.Entry pointerClick;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    Image goBackImg;
    Image myCollectionsImg;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum ButtonState { 보통, 누름 }

    public Sprite[] goBackSprites;
    public Sprite[] myCollectionsSprites;

    #region[Awake]
    private void Awake()
    {
        goBackImg = transform.Find("뒤로").GetComponent<Image>();
        myCollectionsImg = transform.Find("수집품").GetComponent<Image>();

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

        #region[myCollectionsTrigger]
        EventTrigger myCollectionsTrigger = transform.Find("수집품").GetComponent<EventTrigger>();
        pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) =>
        {
            if (Input.GetMouseButton(0))
                myCollectionsImg.sprite = myCollectionsSprites[(int)ButtonState.누름];
        });
        myCollectionsTrigger.triggers.Add(pointerEnter);

        pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) =>
        {
            if (Input.GetMouseButtonDown(0))
                myCollectionsImg.sprite = myCollectionsSprites[(int)ButtonState.누름];
        });
        myCollectionsTrigger.triggers.Add(pointerDown);

        pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) =>
        {
            myCollectionsImg.sprite = myCollectionsSprites[(int)ButtonState.보통];
        });
        myCollectionsTrigger.triggers.Add(pointerExit);

        pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((data) =>
        {
            ActMyCollectionsBtn();
        });
        myCollectionsTrigger.triggers.Add(pointerClick);
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
            myCollectionsImg.sprite = myCollectionsSprites[(int)ButtonState.보통];
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

    public void ActMyCollectionsBtn()
    {
        Debug.Log("수집품");
    }

}
