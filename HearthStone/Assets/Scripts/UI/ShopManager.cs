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
}
