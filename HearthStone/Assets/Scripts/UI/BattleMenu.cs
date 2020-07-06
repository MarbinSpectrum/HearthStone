using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleMenu : MonoBehaviour
{
    public static BattleMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HideInInspector] public bool battleCollections;

    public DeckBtn[] deckList = new DeckBtn[9];

    //덱선택
    public Animator jobSelectAni;
    public Image[] characterImg;
    public Text characterNameTxt;
    [HideInInspector] public int selectDeck = 0;

    //대전탐색
    public Animator findBattleAni;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[Awake]
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(this);
        }

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
        UpdateUI();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[UpdateUI]
    public void UpdateUI()
    {
        for(int i = 0; i < 9; i++)
        {
            if (DataMng.instance.playData.deck.Count <= i)
                deckList[i].hide = true;
            else
            {
                deckList[i].hide = false;
                deckList[i].deckNameTxt.text = DataMng.instance.playData.deck[i].name;
                deckList[i].nowCharacter = (int)DataMng.instance.playData.deck[i].job;

            }
        }
    }
    #endregion

    #region[덱선택확인]
    public void JobSelectCheck(int n = -1)
    {
        jobSelectAni.SetBool("Show", (n != -1));
        if (n != -1)
        {
            InGameDeck.nowDeck = n;
            int jobNum = (int)DataMng.instance.playData.deck[n].job;
            for (int i = 0; i < characterImg.Length; i++)
                characterImg[i].enabled = false;
            characterImg[jobNum].enabled = true;
            switch (jobNum)
            {
                case 0:
                    characterNameTxt.text = "말퓨리온 스톰레이지";
                    break;
                case 1:
                    characterNameTxt.text = "발리라 생귀나르";
                    break;
            }
        }
        selectDeck = n;
    }
    #endregion

    #region[대전탐색]
    public void FindBattle(bool b)
    {
        findBattleAni.SetBool("Find", b);

    }
    #endregion

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
