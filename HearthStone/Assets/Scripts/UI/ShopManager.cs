using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public LowBase shopData = new LowBase();

    private bool DataLoadSuccess;
    public bool dataLoadSuccess
    {
        get { return DataLoadSuccess; }
    }

    #region[Awake]
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    #region[데이터 로드]
    public void StartLoadData()
    {
        if (dataLoadSuccess)
        {
            //이미 데이터 로드가 끝났다.
            return;
        }
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        //사운드 데이터 로드
        yield return new WaitUntil(() => LoadShopData());

        DataLoadSuccess = true;
    }

    private bool LoadShopData()
    {
        shopData.Load("Table/상점데이터");
        return true;
    }
    #endregion

    #region[구매]
    public void BuyPack(int selectMenu)
    {
        DataMng dataMng = DataMng.instance;

        if (dataMng == null)
            return;

        //메뉴에 따른 상품정보를 가져온다.
        string shopText = shopData.ToString(selectMenu + 1, "표기이름");
        int price = shopData.ToInteger(selectMenu + 1, "가격");
        int cnt = shopData.ToInteger(selectMenu + 1, "팩갯수");

        PlayData playData = dataMng.playData;

        if (playData.gold < price)
        {
            //가격보다 보유 골드가 낮기 때문에 구입 실패
            return;
        }
        else
        {
            //가격만큼 골드를 소모하고
            playData.gold -= price;
            for (int i = 0; i < cnt; i++)
            {
                //팩의 갯수만큼 플레이어에게 팩을 만들어서 넣어준다.
                playData.packs.Add(new Pack());
            }
            //구입완료 사운드 호출
            SoundManager.instance.PlaySE("구매완료");

            //플레이어 데이터를 현재 데이터로 갱신
            dataMng.SaveData();
        }
    }
    #endregion
}
