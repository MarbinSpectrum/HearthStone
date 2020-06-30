using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAni : MonoBehaviour
{
    public GameObject fadeCursorObj;

    List<FadeCursor> fadeCursor = new List<FadeCursor>();

    float time = 0;

    public void Awake()
    {
        for (int i = 0; i < fadeCursorObj.transform.childCount; i++)
            fadeCursor.Add(fadeCursorObj.transform.GetChild(i).GetComponent<FadeCursor>());
    }

    private void Update()
    {
        for (int i = 0; i < fadeCursor.Count; i++)
            if (!fadeCursor[i].gameObject.activeSelf)
            {
                fadeCursor[i].gameObject.SetActive(true);
                fadeCursor[i].Act(transform.rotation);
                break;
            }
    }
}
