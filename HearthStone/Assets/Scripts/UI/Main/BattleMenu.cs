﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleMenu : MonoBehaviour
{
    public static BattleMenu instance;

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [HideInInspector] public bool battleCollections;

    [HideInInspector] public List<Deck> decks = new List<Deck>();
    public DeckBtn[] deckList = new DeckBtn[9];

    //덱선택
    public Animator jobSelectAni;
    public Image[] characterImg;
    public Text characterNameTxt;
    [HideInInspector] public Deck nowDeck;

    //대전탐색
    public Animator findBattleZoomAni;
    public Animator findBattleAni;

    //대전씬으로
    public Animator gotoBattleFadeAni;

    bool findBattleFalg = false;

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
            Destroy(this.gameObject);
        }

    }
    #endregion

    #region[OnEnable]
    void OnEnable()
    {
        Init();
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

    #region[Init]
    private void Init()
    {
        SoundManager.instance.PlayBGM("");

        DataMng dataMng = DataMng.instance;
        PlayData playData = dataMng.playData;

        //덱리스트 초기화
        decks.Clear();

        for (int i = 0; i < playData.deck.Count; i++)
        {
            //플레이어의 덱들을 검사
            if (playData.deck[i].IsEffective() == false)
            {
                //해당 덱은 유효화지 않다. 패스
                continue;
            }
            //전투에 사용할 덱리스트에 추가
            decks.Add(playData.deck[i]);
        }
    }
    #endregion

    #region[UpdateUI]
    public void UpdateUI()
    {
        for (int i = 0; i < Deck.MAX_DECK_NUM; i++)
        {
            if (decks.Count <= i)
            {
                //필요 없는 슬롯 비활성화
                deckList[i].hide = true;
            }
            else
            {
                //필요한 슬롯은 덱이름과 덱직업을 설정
                deckList[i].hide = false;
                deckList[i].deckNameTxt.text = decks[i].name;
                deckList[i].nowCharacter = (int)decks[i].job;
            }
        }

        if (findBattleZoomAni.GetBool("Find"))
        {
            if (findBattleAni.GetCurrentAnimatorStateInfo(0).IsName("FindBattle") && findBattleAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.977f)
            {
                if (gotoBattleFadeAni.GetCurrentAnimatorStateInfo(0).IsName("GotoBattle_Fade_Not"))
                    gotoBattleFadeAni.SetTrigger("Fade");
            }
            if (findBattleAni.GetCurrentAnimatorStateInfo(0).IsName("FindBattle") && findBattleAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7 && !findBattleFalg)
            {
                findBattleFalg = true;
                SoundManager.instance.PlaySE("대전상대찾기슬롯멈춤");
                SoundManager.instance.StopBGM();
            }
            if (findBattleAni.GetCurrentAnimatorStateInfo(0).IsName("FindBattle") && findBattleAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                findBattleAni.gameObject.SetActive(false);
                SceneManager.LoadScene("Battle");
            }
        }
    }
    #endregion

    #region[덱선택확인]
    public void JobSelectCheck(int deckNum = -1)
    {
        //deckNum : 선택한 덱 번호, -1이면 덱을 선택하지 않겠다는 것

        jobSelectAni.SetBool("Show", (deckNum != -1));
        if (deckNum != -1)
        {
            int jobNum = (int)decks[deckNum].job;
            for (int i = 0; i < characterImg.Length; i++)
            {
                //영웅에 해당하는 초상화 이미지만 활성화
                characterImg[i].enabled = (jobNum == i);
            }

            switch (decks[deckNum].job)
            {
                //영웅 이름 표시
                case Job.드루이드:
                    characterNameTxt.text = "말퓨리온 스톰레이지";
                    break;
                case Job.도적:
                    characterNameTxt.text = "발리라 생귀나르";
                    break;
            }

            //배틀에 사용할 덱 결정
            nowDeck = decks[deckNum];
        }
    }
    #endregion

    #region[대전탐색]
    public void FindBattle(bool b)
    {
        findBattleZoomAni.SetBool("Find", b);
        findBattleFalg = false;
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

        MainMenu mainMenu = MainMenu.instance;
        mainMenu.CollectMenu(true);
    }
    #endregion
}
