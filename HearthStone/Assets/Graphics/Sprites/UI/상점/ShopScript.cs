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
        selectMenu = 0;
    }

    public void Update()
    {
        if(selectMenu == 0)
        {
            selectMenu = 100;
            StartCoroutine(SetPack(1));
            packNumText.text = "고급 팩 1 개";
            priceText.text = "100";
            for (int i = 0; i < selectImg.Length; i++)
                selectImg[i].enabled = false;
            selectImg[0].enabled = true;
        }
        else if (selectMenu == 1)
        {
            selectMenu = 101;
            StartCoroutine(SetPack(2));
            packNumText.text = "고급 팩 2 개";
            priceText.text = "150";
            for (int i = 0; i < selectImg.Length; i++)
                selectImg[i].enabled = false;
            selectImg[1].enabled = true;
        }
        else if (selectMenu == 2)
        {
            selectMenu = 102;
            StartCoroutine(SetPack(7));
            packNumText.text = "고급 팩 7 개";
            priceText.text = "500";
            for (int i = 0; i < selectImg.Length; i++)
                selectImg[i].enabled = false;
            selectImg[2].enabled = true;
        }
        else if (selectMenu == 3)
        {
            selectMenu = 103;
            StartCoroutine(SetPack(15));
            packNumText.text = "고급 팩 15 개";
            priceText.text = "1000";
            for (int i = 0; i < selectImg.Length; i++)
                selectImg[i].enabled = false;
            selectImg[3].enabled = true;
        }
        else if (selectMenu == 4)
        {
            selectMenu = 104;
            StartCoroutine(SetPack(40));
            packNumText.text = "고급 팩 40 개";
            priceText.text = "2500";
            for (int i = 0; i < selectImg.Length; i++)
                selectImg[i].enabled = false;
            selectImg[4].enabled = true;
        }
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
