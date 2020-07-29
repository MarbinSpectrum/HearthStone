using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource BGM;
    public AudioSource SE;

    AudioClip mainMenuBGM;
    AudioClip myCollectMenuBGM;
    AudioClip[] findBattle = new AudioClip[6];

    AudioClip tinyBtn;
    AudioClip clickBtn;
    AudioClip paperFlip;
    AudioClip checkCard;

    #region[Awake]
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainMenuBGM = Resources.Load("Sound/메인화면배경음") as AudioClip;
        myCollectMenuBGM = Resources.Load("Sound/수집함배경음") as AudioClip;
        for (int i = 0; i < 6; i++)
            findBattle[i] = Resources.Load("Sound/대전상대찾기" + (i + 1)) as AudioClip;

        tinyBtn = Resources.Load("Sound/작은버튼") as AudioClip;
        clickBtn = Resources.Load("Sound/버튼클릭") as AudioClip;
        paperFlip = Resources.Load("Sound/페이지넘기기") as AudioClip;
        checkCard = Resources.Load("Sound/수집함카드선택") as AudioClip;
    }
    #endregion

    public void PlayBGM(string s)
    {
        StartCoroutine(PlayNewBGM(s));
    }

    public void StopBGM()
    {
        BGM.clip = null;
        BGM.Stop();
    }


    private IEnumerator PlayNewBGM(string s)
    {
        bool isPlayNow = false;
        AudioClip newBGM = null;
        switch (s)
        {
            case "메인화면배경음":
                if (BGM.clip == mainMenuBGM)
                    isPlayNow = true;
                else
                    newBGM = mainMenuBGM;
                break;
            case "수집함배경음":
                if (BGM.clip == myCollectMenuBGM)
                    isPlayNow = true;
                else
                    newBGM = myCollectMenuBGM;
                break;
            case "대전상대찾기1":
                if (BGM.clip == findBattle[0])
                    isPlayNow = true;
                else
                    newBGM = findBattle[0];
                break;
        }

        if (isPlayNow)
        {

        }
        else
        {
            float v = BGM.volume;
            for (int i = 0; i < 100; i++)
            {
                yield return new WaitForSeconds(0.005f);
                BGM.volume -= 0.01f;
            }
            BGM.clip = null;
            BGM.clip = newBGM;
            BGM.volume = v;
            BGM.Play();
        }
    }

    public void PlaySE(string s)
    {
        switch (s)
        {
            case "작은버튼":
                SE.PlayOneShot(tinyBtn);
                break;
            case "버튼클릭":
                SE.PlayOneShot(clickBtn);
                break;
            case "페이지넘기기":
                SE.PlayOneShot(paperFlip);
                break;
            case "수집함카드선택":
                SE.PlayOneShot(checkCard);
                break;
        }
    }
}
