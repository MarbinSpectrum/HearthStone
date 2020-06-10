using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
    public static BattleMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HideInInspector]
    public bool battleCollections;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Awake]
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        SoundManager.instance.PlayBGM("");
    }
    #endregion

    #region[Update]
    private void Update()
    {

    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[메인메뉴로 이동]
    public void GoToMain(float waitTime)
    {
        MainMenu.instance.CloseBoard();
        battleCollections = false;
        StartCoroutine(CloseBattleMenu(waitTime));
    }
    public IEnumerator CloseBattleMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }
    #endregion

    #region[나의 콜렉션으로 이동]
    public void GoToMyCollections(float waitTime1, float waitTime2)
    {
        MainMenu.instance.ChangeBoard();
        battleCollections = true;
        StartCoroutine(CloseBattleMenu(waitTime1));
        StartCoroutine(ShowMyCollectionsMenu(waitTime2));
    }

    public IEnumerator ShowMyCollectionsMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        MainMenu.instance.myCollectionsMenuUI.SetActive(true);
    }
    #endregion
}
