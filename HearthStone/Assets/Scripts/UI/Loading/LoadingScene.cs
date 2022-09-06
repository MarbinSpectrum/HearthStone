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

        //������ �Ŵ����� �غ������ ������ �ε� ����
        DataMng.instance.StartLoadData();

        //������ ī�� ������ �ε�
        yield return new WaitUntil(() => DataMng.instance.dataLoadSuccess);

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
