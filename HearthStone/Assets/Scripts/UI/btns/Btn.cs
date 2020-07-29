using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Btn : MonoBehaviour
{
    protected enum ButtonState { 보통, 누름, 선택 }
    public Animator btnAni;
    public Image btnImg;
    protected EventTrigger.Entry pEnter;
    protected EventTrigger.Entry pDown;
    protected EventTrigger.Entry pExit;
    protected EventTrigger.Entry pClick;
    public EventTrigger btnEvent;
    public Sprite[] btnSprites;

    #region[AddEvent]
    protected void AddEvent()
    {
        pEnter = new EventTrigger.Entry();
        pEnter.eventID = EventTriggerType.PointerEnter;
        pEnter.callback.AddListener((data) =>
        {
            pointerEnter();
        });
        btnEvent.triggers.Add(pEnter);

        pDown = new EventTrigger.Entry();
        pDown.eventID = EventTriggerType.PointerDown;
        pDown.callback.AddListener((data) =>
        {
            pointerDown();
        });
        btnEvent.triggers.Add(pDown);

        pExit = new EventTrigger.Entry();
        pExit.eventID = EventTriggerType.PointerExit;
        pExit.callback.AddListener((data) =>
        {
            pointerExit();
        });
        btnEvent.triggers.Add(pExit);

        pClick = new EventTrigger.Entry();
        pClick.eventID = EventTriggerType.PointerClick;
        pClick.callback.AddListener((data) =>
        {
            pointerClick();
        });
        btnEvent.triggers.Add(pClick);
    }
    #endregion

    public abstract void Awake();
    public abstract void Update();
    public abstract void pointerEnter();
    public abstract void pointerDown();
    public abstract void pointerExit();
    public abstract void pointerClick();
    public abstract void ActBtn();
}

