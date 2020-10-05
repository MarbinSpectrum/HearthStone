using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public Animator mainMenuAni;
    public Animator outSideMenuAni;

    public GameObject battleMenuUI;
    public GameObject myCollectionsMenuUI;
    public GameObject openPackUI;
    public ShopScript shopUI;
    public Animator questUI;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private bool inMainMenu = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Awake]
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region[Update]
    private void Update()
    {
        CheckMainMenu();
    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        inMainMenu = false;
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[메인화면으로 이동했는지 체크]
    public void CheckMainMenu()
    {
        if (!mainMenuAni.GetCurrentAnimatorStateInfo(0).IsName("CenterUI") && !mainMenuAni.GetCurrentAnimatorStateInfo(0).IsName("StartTurn"))
        {
            inMainMenu = false;
            return;
        }

        if (!inMainMenu)
        {
            inMainMenu = true;
            Setting.instance.setting.SetActive(true);
            Setting.instance.settingBtn.SetActive(true);
            DataMng.instance.SaveData();
            SoundManager.instance.PlayBGM("메인화면배경음");
            Setting.instance.gameObject.SetActive(true);
        }
    }
    #endregion

    #region[보드열기]
    public void OpenBoard()
    {
        mainMenuAni.SetTrigger("Turn");
        StartCoroutine(ShowOutMenu(1, false));
    }
    #endregion

    #region[보드닫기]
    public void CloseBoard()
    {
        mainMenuAni.SetTrigger("ReTurn");
        StartCoroutine(ShowOutMenu(1,true));
    }
    #endregion

    #region[보드변경]
    public void ChangeBoard()
    {
        mainMenuAni.SetTrigger("Change");
    }
    #endregion

    #region[양쪽메뉴 보여주기]
    private IEnumerator ShowOutMenu(float waitTime,bool state)
    {
        yield return new WaitForSeconds(waitTime);
        outSideMenuAni.SetBool("In", state);
    }
    #endregion
}
