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

    #region[����]
    public void BuyPack(int selectMenu)
    {
        DataMng dataMng = DataMng.instance;

        if (dataMng == null)
            return;

        //�޴��� ���� ��ǰ������ �����´�.
        string shopText = shopData.ToString(selectMenu + 1, "ǥ���̸�");
        int price = shopData.ToInteger(selectMenu + 1, "����");
        int cnt = shopData.ToInteger(selectMenu + 1, "�Ѱ���");

        PlayData playData = dataMng.playData;

        if (playData.gold < price)
        {
            //���ݺ��� ���� ��尡 ���� ������ ���� ����
            return;
        }
        else
        {
            //���ݸ�ŭ ��带 �Ҹ��ϰ�
            playData.gold -= price;
            for (int i = 0; i < cnt; i++)
            {
                //���� ������ŭ �÷��̾�� ���� ���� �־��ش�.
                playData.packs.Add(new Pack());
            }
            //���ԿϷ� ���� ȣ��
            SoundManager.instance.PlaySE("���ſϷ�");

            //�÷��̾� �����͸� ���� �����ͷ� ����
            dataMng.SaveData();
        }
    }
    #endregion
}
