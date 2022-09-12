using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<Animator> packAni = new List<Animator>();
    [SerializeField] private Text packNumText;
    [SerializeField] private Text priceText;
    [SerializeField] private Image []selectImg = new Image[5];
    [SerializeField] private Animator checkBuy;

    [HideInInspector] public int selectMenu = 0;
    public void OnEnable()
    {
        MenuSelect(0);
    }

    private void MenuSelect(int n)
    {
        //n번째 메뉴를 선택
        selectMenu = n;

        ShopManager shopManager = ShopManager.instance;
        LowBase data = shopManager.shopData;

        //n번째 메뉴에 해당하는 (표기이름,가격,팩갯수)를 가져온다.
        string shopText = data.ToString(n + 1, "표기이름");
        int price = data.ToInteger(n + 1, "가격");
        int cnt = data.ToInteger(n + 1, "팩갯수");

        //팩이 쌓이는 애니메이션 실행
        SetAnimation(cnt);

        //UI에 팩 정보를 표시
        packNumText.text = shopText;
        priceText.text = price.ToString();
        for (int i = 0; i < selectImg.Length; i++)
            selectImg[i].enabled = false;
        selectImg[n].enabled = true;
    }

    public void SetShopState(bool state)
    {
        bool aniState = animator.GetBool("Run");
        if (aniState == state)
            return;
        gameObject.SetActive(state);
        animator.SetBool("Run", state);
    }

    public void SetSelectMenu(int n)
    {
        MenuSelect(n);
    }

    public void BuySelectMenu(bool state)
    {
        ShopManager shopManager = ShopManager.instance;
        if (shopManager == null)
            return;

        if(state)
        {
            //팩을 구매
            shopManager.BuyPack(selectMenu);
        }

        //state상태에 따른 구매완료 메세지 출력 
        checkBuy.SetBool("Open", state);
    }

    #region[팩배치 보여주기]
    private void SetAnimation(int n)
    {
        StartCoroutine(SetPack(n));
    }

    private IEnumerator SetPack(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if(i % 8 == 0)
                SoundManager.instance.PlaySE("덱에카드넣기");
            packAni[i].SetBool("Up", false);
            yield return new WaitForSeconds(0.001f);
        }
        for (int i = n; i < packAni.Count; i++)
        {
            packAni[i].SetBool("Up", true);
            yield return new WaitForSeconds(0.001f);
        }
    }
    #endregion


}
