using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] private Animation loadingAni;

    private void Start()
    {
        if (DataMng.instance != null &&
            DataMng.instance.dataLoadSuccess)
        {
            //�̹� ������ �ε尡 ������.
            return;
        }
        DontDestroyOnLoad(this);

        StartCoroutine(LoadingData());
    }

    private IEnumerator LoadingData()
    {
        yield return new WaitUntil(() => (DataMng.instance != null));
        yield return new WaitUntil(() => (SoundManager.instance != null));
        yield return new WaitUntil(() => (QuestManager.instance != null));
        yield return new WaitUntil(() => (ShopManager.instance != null));

        //������ ī�� ������ �ε�
        DataMng.instance.StartLoadData();
        yield return new WaitUntil(() => DataMng.instance.dataLoadSuccess);

        //���� ���� ������ �ε�
        SoundManager.instance.StartLoadData();
        yield return new WaitUntil(() => SoundManager.instance.dataLoadSuccess);

        //����Ʈ ������ �ε�
        QuestManager.instance.StartLoadData();
        yield return new WaitUntil(() => QuestManager.instance.dataLoadSuccess);

        //���� ������ �ε�
        ShopManager.instance.StartLoadData();
        yield return new WaitUntil(() => ShopManager.instance.dataLoadSuccess);

        //�ε��Ϸ� �ִϸ��̼� ����
        loadingAni.Play();
        yield return new WaitForSeconds(1f);

        //���̵�
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(1f);

        //�������� ȿ���� ����
        SoundManager.instance.PlayBGM("����ȭ������");
        SoundManager.instance.PlaySE("������Ʈ��");
    }

}
