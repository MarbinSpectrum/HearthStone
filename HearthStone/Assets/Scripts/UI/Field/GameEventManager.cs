using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance;

    bool Event;
    float time = 0;

    void Awake()
    {
        instance = this;
        Event = false;
    }

    public float GetEventValue()
    {
        return time;
    }

    public bool EventCheck()
    {
        return Event;
    }

    public void EventAdd(float t)
    {
        time += t;
    }

    public void EventSub(float t)
    {
        time -= t;
    }

    public void EventStop()
    {
        time = 0;
    }

    public void EventSet(float t)
    {
        time = t;
    }

    private void Update()
    {
        Event = (time > 0);
        if (time < 0)
            time = 0;
        else
            time -= Time.deltaTime;
    }
}
