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
            //이미 데이터 로드가 끝났다.
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

        //직업별 카드 데이터 로드
        DataMng.instance.StartLoadData();
        yield return new WaitUntil(() => DataMng.instance.dataLoadSuccess);

        //게임 사운드 데이터 로드
        SoundManager.instance.StartLoadData();
        yield return new WaitUntil(() => SoundManager.instance.dataLoadSuccess);

        //퀘스트 데이터 로드
        QuestManager.instance.StartLoadData();
        yield return new WaitUntil(() => QuestManager.instance.dataLoadSuccess);

        //상점 데이터 로드
        ShopManager.instance.StartLoadData();
        yield return new WaitUntil(() => ShopManager.instance.dataLoadSuccess);

        //로딩완료 애니메이션 실행
        loadingAni.Play();
        yield return new WaitForSeconds(1f);

        //씬이동
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(1f);

        //게임진입 효과음 실행
        SoundManager.instance.PlayBGM("메인화면배경음");
        SoundManager.instance.PlaySE("게임인트로");
    }

}
