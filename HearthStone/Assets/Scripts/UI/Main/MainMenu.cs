using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [SerializeField] private Animator mainMenuAni;
    [SerializeField] private Animator outSideMenuAni;

    [SerializeField] private GameObject battleMenuUI;
    [SerializeField] private GameObject myCollectionsMenuUI;
    [SerializeField] private GameObject openPackUI;
    [SerializeField] private ShopScript shopUI;
    [SerializeField] private Animator questUI;

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
        AnimatorStateInfo stateInfo = mainMenuAni.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("CenterUI") && !stateInfo.IsName("StartTurn"))
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

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    public void BattleMenu(bool state)
    {
        //BattleMenu 활성/비활성 설정
        battleMenuUI.SetActive(state);
    }

    public void CollectMenu(bool state)
    {
        //CollectMenu 활성/비활성 설정
        myCollectionsMenuUI.SetActive(state);
    }

    public void PackOpenMenu(bool state)
    {
        openPackUI.SetActive(state);
    }

    public void ShopMenu(bool state)
    {
        shopUI.SetShopState(state);
    }

    public void ShopSelectMenu(int n)
    {
        shopUI.SetSelectMenu(n);
    }

    public void BuySelectMenu(bool state)
    {
        shopUI.BuySelectMenu(state);
    }

    public void QuestMenu(bool state)
    {
        questUI.SetBool("Open", state);
    }
}
