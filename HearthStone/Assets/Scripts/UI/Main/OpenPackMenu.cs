using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenPackMenu : MonoBehaviour
{
    public static OpenPackMenu instance;

    public GameObject numFlag;
    public Text packNum;
    public GameObject packObj;
    public Animator dragObj;
    public Animator packOpenAni;
    public Animator packAni;
    public CardView []packCardView = new CardView[5];
    public OpenPack[] openPackBtn = new OpenPack[5];
    public GameObject openCheckBtn;

    [HideInInspector] public int dragPackNum = 0;
    [HideInInspector] public int cardOpenNum = 0;

    float dragtime = 0;

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

    #region[OnEnable]
    void OnEnable()
    {

    }
    #endregion

    #region[Update]
    public void Update()
    {
        if (DataMng.instance)
        {
            int packN = DataMng.instance.playData.packs.Count;

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

        if(dragObj.gameObject.activeSelf)
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

            dragObj.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragObj.transform.position.z);
        }
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

    public void OpenCardPack()
    {
        for(int i = 0; i < packCardView.Length; i++)
        {
            string cardName = DataMng.instance.playData.packs[0].card[i];
            CardViewManager.instance.CardShow(ref packCardView[i], cardName);
            int cardNum = DataMng.instance.playData.GetCardNum(cardName);
            DataMng.instance.playData.SetCardNum(cardName, cardNum + 1);
        }
        DataMng.instance.playData.packs.RemoveAt(0);
        DataMng.instance.SaveData();
    }

    public void ActOpenCheck(float waitTime)
    {
        StartCoroutine(ActOpenCheckBtn(1.5f));
    }

    public IEnumerator ActOpenCheckBtn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OpenPackMenu.instance.openCheckBtn.SetActive(true);
    }


}
