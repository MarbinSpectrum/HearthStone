using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CopyAct : MonoBehaviour
{
    public GameObject act_copy;
    public GameObject act_paste;

    // Update is called once per frame
    void Update()
    {
        if(act_copy && act_paste)
            act_paste.SetActive(act_copy.activeSelf);
    }
}
