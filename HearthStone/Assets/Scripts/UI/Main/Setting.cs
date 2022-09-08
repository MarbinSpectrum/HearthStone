﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public static Setting instance;

    public GameObject settingBtn;
    public GameObject setting;

    #region[Awake]
    private void Awake()
    {
        instance = this;
        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
    #endregion

    #region[Update]
    private void Update()
    {
        if(BattleUI.instance)
        {
            settingBtn.SetActive(BattleUI.instance.gameStart);
            setting.SetActive(BattleUI.instance.gameStart);

        }
    }
    #endregion
}
