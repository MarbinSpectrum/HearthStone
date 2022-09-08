using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour
{
    public Animator animator;
    public List<Animator> packAni = new List<Animator>();
    public Text packNumText;
    public Text priceText;
    public Image []selectImg = new Image[5];
    public Animator checkBuy;

    [HideInInspector] public int selectMenu = 0;
    public void OnEnable()
    {
        MenuSelect(0);
    }

    private void MenuSelect(int n)
    {
        selectMenu = n;

        ShopManager shopManager = ShopManager.instance;
        LowBase data = shopManager.shopData;

        string shopText = data.ToString(n + 1, "표기이름");
        int price = data.ToInteger(n + 1, "가격");
        int cnt = data.ToInteger(n + 1, "팩갯수");

        StartCoroutine(SetPack(cnt));
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
        if(state)
        {
            DataMng dataMng = DataMng.instance;
            LowBase data = dataMng.shopData;
            string shopText = data.ToString(selectMenu + 1, "표기이름");
            int price = data.ToInteger(selectMenu + 1, "가격");
            int cnt = data.ToInteger(selectMenu + 1, "팩갯수");

            PlayData playData = dataMng.playData;

            if (playData.gold < price)
                return;
            else
            {
                SoundManager.instance.PlaySE("구매완료");
                playData.gold -= price;
                for (int i = 0; i < cnt; i++)
                    DataMng.instance.playData.packs.Add(new PlayData.Pack());
            }

            DataMng.instance.SaveData();
        }

        checkBuy.SetBool("Open", state);
    }

    #region[팩배치 보여주기]
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
