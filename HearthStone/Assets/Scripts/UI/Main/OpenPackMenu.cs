using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPackMenu : MonoBehaviour
{
    public static OpenPackMenu instance;

    [SerializeField] private GameObject numFlag;
    [SerializeField] private Text packNum;
    [SerializeField] private GameObject packObj;
    [SerializeField] private Animator packAni;
    public Animator dragObj;
    public Animator packOpenAni;
    public CardView[] packCardView = new CardView[5];
    public OpenPack[] openPackBtn = new OpenPack[5];
    public GameObject openCheckBtn;

    [HideInInspector] public int dragPackNum = 0;
    [HideInInspector] public int cardOpenNum = 0;

    private float dragtime = 0;

    #region[Awake]
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region[Update]
    public void Update()
    {
        UpdatePackActive();
        UpdatePackAnimation();
    }
    #endregion

    #region[메인메뉴로 이동]
    public void GoToMain(float waitTime)
    {
        MainMenu.instance.CloseBoard();
        StartCoroutine(CloseOpenPackMenu(waitTime));
    }
    public IEnumerator CloseOpenPackMenu(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }
    #endregion

    #region[UpdatePackActive]
    private void UpdatePackActive()
    {
        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
            return;

        PlayData playData = dataMng.playData;
        if (playData == null)
            return;

        int packN = playData.packs.Count;

        dragPackNum = dragObj.gameObject.activeSelf ? 1 : 0;

        packN -= dragPackNum;

        if (packN <= 0)
        {
            packObj.SetActive(false);
            numFlag.SetActive(false);
        }
        else if (packN == 1)
        {
            packObj.SetActive(true);
            numFlag.SetActive(false);
        }
        else
        {
            packObj.SetActive(true);
            numFlag.SetActive(true);
            packNum.text = packN.ToString();
        }
    }
    #endregion

    #region[UpdatePackAnimation]
    private void UpdatePackAnimation()
    {
        if (dragObj.gameObject.activeSelf)
        {
            if (Mathf.Abs(dragObj.transform.position.x - Input.mousePosition.x) > 0.01f)
            {
                if (dragObj.transform.position.x < Input.mousePosition.x)
                {
                    if (dragObj.GetCurrentAnimatorStateInfo(0).IsName("PackRight"))
                        dragtime = 0.015f;
                    else
                        dragtime = 0.005f;
                    dragObj.SetInteger("Dic", +1);
                }
                else if (dragObj.transform.position.x > Input.mousePosition.x)
                {
                    if (dragObj.GetCurrentAnimatorStateInfo(0).IsName("PackLeft"))
                        dragtime = 0.015f;
                    else
                        dragtime = 0.005f;
                    dragObj.SetInteger("Dic", -1);
                }
            }
            else
            {
                dragtime -= Time.deltaTime;
                if (dragtime < 0)
                    dragObj.SetInteger("Dic", 0);
            }

            dragObj.transform.position =
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragObj.transform.position.z);
        }
    }
    #endregion

    #region[OpenCardPack]
    public void OpenCardPack()
    {
        DataMng dataMng = DataMng.instance;
        if (dataMng == null)
            return;

        PlayData playData = dataMng.playData;
        if (playData == null)
            return;

        CardViewManager cardViewManager = CardViewManager.instance;
        if (cardViewManager == null)
            return;

        //팩 개봉 애니메이션 및 효과음 출력
        packOpenAni.SetBool("Light", false);
        packAni.SetBool("Open", true);
        SoundManager.instance.PlaySE("팩개봉");

        for (int i = 0; i < packCardView.Length; i++)
        {
            //카드팩의 카드 정보를 토대로 카드를 만든다.
            string cardName = playData.packs[0].card[i];

            //CardShow에서 카드이름을 토대로 
            //이미지,이름,텍스트를 작성해서 진짜카드를 만든다.
            cardViewManager.CardShow(ref packCardView[i], cardName);

            //플레이어에게 카드를 추가한다.
            playData.AddCard(cardName, 1);
        }

        //개봉한 카드를 제거
        playData.packs.RemoveAt(0);

        //현재상태를 저장
        dataMng.SaveData();
    }
    #endregion

    public void ActOpenCheck(float waitTime)
    {
        StartCoroutine(ActOpenCheckBtn(1.5f));
    }

    public IEnumerator ActOpenCheckBtn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        openCheckBtn.SetActive(true);
    }


}
