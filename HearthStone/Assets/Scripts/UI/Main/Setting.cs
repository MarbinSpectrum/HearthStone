using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    public static Setting instance;

    #region[Awake]
    private void Awake()
    {
        instance = this;
        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
    #endregion
}
