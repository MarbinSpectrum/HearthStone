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

    #region[������ �ε�]
    public void StartLoadData()
    {
        if (dataLoadSuccess)
        {
            //�̹� ������ �ε尡 ������.
            return;
        }
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        //���� ������ �ε�
        yield return new WaitUntil(() => LoadShopData());

        DataLoadSuccess = true;
    }

    private bool LoadShopData()
    {
        shopData.Load("Table/����������");
        return true;
    }
    #endregion
}
