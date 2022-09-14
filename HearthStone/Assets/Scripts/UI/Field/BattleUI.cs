using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public static BattleUI instance;

    public MeshRenderer playerHeroPower;
    public Material druidMat;
    public Material rogueMat;

    public GameObject[] characterImg;
    public Text characterNameTxt;
    public Text jobNameTxt;

    public ShowPlayerText playerText;
    public ShowPlayerText enermyText;

    public Mulligan mulligan;

    public GameObject playerSetEffect;

    public GameObject cameraObject;

    public Animator[] enemyCardAni;
    public Animator[] playerCardAni;

    public Animator fieldShadowAni;

    public Animator grayFilterAni;
    public GameObject grayFilter;

    public RectTransform selectMinion;
    public Text selectMinionTxt;

    public Animator chooseOneDruid;
    public CardView []chooseCardView;

    public GameObject playerSpellPos;
    public GameObject enemySpellPos;
    public GameObject enemySpellViewPos;

    public GameObject exhaustionUI;
    public Text exhaustionText;

    public GameObject gameEndDontTouch;

    public GameObject gameDefeat;
    public Image gameDefeatImg;

    public GameObject gameWin;
    public Image gameWinImg;

    [HideInInspector] public bool gameStart;

    #region[Awake]
    public void Awake()
    {
        //if (MainMenu.instance)
        //    MainMenu.instance.gameObject.SetActive(false);
        //if (MyCollectionsMenu.instance)
        //    MyCollectionsMenu.instance.gameObject.SetActive(false);
        if (BattleMenu.instance)
            BattleMenu.instance.gameObject.SetActive(false);
        instance = this;
        gameStart = false;
        GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }
    #endregion

    #region[Start]
    private void Start()
    {
        StartCoroutine(PlayerSetEffect(7));
        StartCoroutine(CameraVibrationEffect(7, 12,0.5f));
        StartCoroutine(ShowEnermyText(7, "내가 대자연을 수호하겠다!"));
        StartCoroutine(PlayHeroSound(7, "말퓨리온", 영웅상태.게임시작_아군));
        mulligan.SetGoing(9.5f);

        int jobNum = (int)BattleMenu.instance.nowDeck.job;

        StartCoroutine(BattleVsSound(0.2f, "말퓨리온", jobNum == 0 ? "말퓨리온" : "발리라"));
        for (int i = 0; i < characterImg.Length; i++)
            characterImg[i].SetActive(false);
        characterImg[jobNum].SetActive(true);
        switch (jobNum)
        {
            case 0:
                characterNameTxt.text = "말퓨리온 스톰레이지";
                jobNameTxt.text = "드루이드";
                playerHeroPower.material = druidMat;
                HeroManager.instance.heroPowerManager.SetHeroPower("말퓨리온", false);
                HeroManager.instance.heroPowerManager.SetHeroPower("말퓨리온", true);
                StartCoroutine(ShowPlayerText(10, "자연이 그대를 거부하리라!"));
                StartCoroutine(PlayHeroSound(10, "말퓨리온", 영웅상태.게임시작_적군));
                break;
            case 1:
                characterNameTxt.text = "발리라 생귀나르";
                jobNameTxt.text = "도적";
                playerHeroPower.material = rogueMat;
                HeroManager.instance.heroPowerManager.SetHeroPower("발리라", false);
                HeroManager.instance.heroPowerManager.SetHeroPower("말퓨리온", true);
                StartCoroutine(ShowPlayerText(10, "등...뒤를... 조심해..."));
                StartCoroutine(PlayHeroSound(10, "발리라", 영웅상태.게임시작_아군));
                break;
        }
    }
    #endregion

    #region[대전자소개]
    private IEnumerator BattleVsSound(float waitTime, string name1, string name2)
    {
        yield return new WaitForSeconds(waitTime);
        SoundManager.instance.PlaySE(name1);
        yield return new WaitForSeconds(2);
        SoundManager.instance.PlaySE("그상대는");
        yield return new WaitForSeconds(2);
        SoundManager.instance.PlaySE(name2);
    }
    #endregion

    #region[등장대사]
    private IEnumerator PlayHeroSound(float waitTime, string name,영웅상태 state)
    {
        yield return new WaitForSeconds(waitTime);
        SoundManager.instance.PlayCharacterSE(name, state);
    }

    private IEnumerator ShowPlayerText(float waitTime,string s)
    {
        yield return new WaitForSeconds(waitTime);
        playerText.show = true;
        playerText.text = s;
    }

    private IEnumerator ShowEnermyText(float waitTime, string s)
    {
        yield return new WaitForSeconds(waitTime);
        enermyText.show = true;
        enermyText.text = s;
    }
    #endregion

    #region[플레이어 배치 이펙트]
    private IEnumerator PlayerSetEffect(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerSetEffect.SetActive(true);
    }
    #endregion

    #region[플레이어 배치 이펙트(진동)]
    private IEnumerator CameraVibrationEffect(float waitTime, int n, float power = 1)
    {
        yield return new WaitForSeconds(waitTime);
        Vector3 v = cameraObject.transform.position;
        for (int i = 0; i < n; i++)
        {
            cameraObject.transform.position = v + Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(1, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        cameraObject.transform.position = v;
    }
    #endregion

}
