using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum 미니언상태 { 소환, 공격, 죽음, 효과 };
public enum 주문상태 { 발동,효과 };

public enum 영웅상태 { 게임시작_아군,게임시작_적군,덱이적다,덱이없다,공격시,죽을시 };
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource BGM;
    public AudioSource SE;

    [Range(0, 1)] public float maxBGM = 1;
    [Range(0, 1)] public float maxSE = 1;

    private Dictionary<string, MinionSoundObj> minionSound = new Dictionary<string, MinionSoundObj>();
    private Dictionary<string, SpellSoundObj> spellSound = new Dictionary<string, SpellSoundObj>();
    private Dictionary<string, CharacterSoundObj> characterSound = new Dictionary<string, CharacterSoundObj>();
    private Dictionary<string, AudioClip> etcSound = new Dictionary<string, AudioClip>();

    private bool DataLoadSuccess;
    public bool dataLoadSuccess
    {
        get { return DataLoadSuccess; }
    }

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
        yield return new WaitUntil(() => LoadEtcSound());
        yield return new WaitUntil(() => LoadMinionSound());
        yield return new WaitUntil(() => LoadSpellSound());
        yield return new WaitUntil(() => LoadCharacterSound());

        DataLoadSuccess = true;
    }

    private bool LoadEtcSound()
    {
        //CSV파일로 되어있는 데이터를 로드(Key,Path)
        LowBase lowBase = new LowBase();
        lowBase.Load("Table/기타음향");

        for (int i = 1; i <= lowBase.m_table.Count; i++)
        {
            //키에 해당 하는 음향파일 로드
            string name = lowBase.ToString(i, "Key");
            string path = lowBase.ToString(i, "Path");

            //사운드를 등록
            etcSound[name] = Resources.Load("Sound/" + path) as AudioClip;
        }
        return true;
    }
    private bool LoadMinionSound()
    {
        //스크립터블 오브젝트로 등록되어있는 미니언 사운드를 로드
        MinionSoundObj[] minionSounds = Resources.LoadAll<MinionSoundObj>("Sound/미니언");
        for (int i = 0; i < minionSounds.Length; i++)
        {
            //미니언 사운드 오브젝트를 등록
            minionSound.Add(minionSounds[i].name, minionSounds[i]);
        }
        return true;
    }

    private bool LoadSpellSound()
    {
        //스크립터블 오브젝트로 등록되어있는 주문 사운드를 로드
        SpellSoundObj[] spellSounds = Resources.LoadAll<SpellSoundObj>("Sound/주문");
        for (int i = 0; i < spellSounds.Length; i++)
        {
            //주문 사운드 오브젝트를 등록
            spellSound.Add(spellSounds[i].name, spellSounds[i]);
        }
        return true;
    }
    private bool LoadCharacterSound()
    {
        //스크립터블 오브젝트로 등록되어있는 캐릭터 사운드를 로드
        CharacterSoundObj[] characterSounds = Resources.LoadAll<CharacterSoundObj>("Sound/캐릭터");
        for (int i = 0; i < characterSounds.Length; i++)
        {
            //캐릭터 사운드 오브젝트를 등록
            characterSound.Add(characterSounds[i].name, characterSounds[i]);
        }
        return true;
    }
    #endregion

    public void PlayBGM()
    {
        BGM.Play();
    }
    public void PlayBGM(string s)
    {
        StartCoroutine(PlayNewBGM(s));
    }

    public void StopBGM()
    {
        BGM.clip = null;
        BGM.Stop();
    }


    public void MuteBGM(bool mute)
    {
        BGM.mute = mute;
    }
    public void PauseBGM()
    {
        BGM.Pause();
    }
    public void PauseBGM(float time,bool flag = false)
    {
        StartCoroutine(PauseNowBGM(time, flag));
    }
    private IEnumerator PauseNowBGM(float time, bool flag = false)
    {
        BGM.Pause();
        yield return new WaitForSeconds(time);
        if(!flag)
            BGM.Play();
    }

    public void DownBGM(float time,float volume)
    {
        StartCoroutine(DownNowBGM(time, volume));
    }

    private IEnumerator DownNowBGM(float time,float volume)
    {
        float v = BGM.volume;
        BGM.volume = volume;
        yield return new WaitForSeconds(time);
        BGM.volume = v;
        v /= 50f;
        for (int i = 0; i < 50; i++)
        {
            yield return new WaitForSeconds(0.004f);
            BGM.volume += v;
        }
        BGM.volume = maxBGM;
    }

    private IEnumerator PlayNewBGM(string sound)
    {
        bool isPlayNow = false;
        AudioClip newBGM = null;

        while (BGM.volume < maxBGM)
            yield return new WaitForSeconds(0.001f);

        if(sound == "대전상대찾기")
        {
            int n = Random.Range(0, 6) + 1;
            sound += n.ToString();
        }

        if (etcSound.ContainsKey(sound) == false)
            yield break;

        if (BGM.clip == etcSound[sound])
            isPlayNow = true;
        else
            newBGM = etcSound[sound];

        if (!isPlayNow)
        {
            BGM.clip = null;
            BGM.clip = newBGM;
            BGM.Play();
        }
    }

    public void PlaySE(AudioClip c)
    {
        SE.volume = maxSE;
        SE.PlayOneShot(c);
    }

    public void PlayMinionSE(string minionName, 미니언상태 state)
    {
        try
        {
            if (state == 미니언상태.소환)
            PlaySE(minionSound[minionName].spawnSound);
        else if (state == 미니언상태.공격)
            PlaySE(minionSound[minionName].attackSound);
        else if (state == 미니언상태.죽음)
            PlaySE(minionSound[minionName].deathSound);
        else if (state == 미니언상태.효과)
            PlaySE(minionSound[minionName].effectSound);
        }
        catch { }
    }

    public void PlaySpellSE(string spellName, 주문상태 state)
    {
        try
        {
            if (state == 주문상태.발동)
                PlaySE(spellSound[spellName].playSound);
            else if (state == 주문상태.효과)
                PlaySE(spellSound[spellName].effectSound);
        }
        catch { }
    }

    public void PlayCharacterSE(string characterName, 영웅상태 state)
    {
        try
        {
            if (state == 영웅상태.게임시작_아군)
                PlaySE(characterSound[characterName].battleStart_Player);
            else if (state == 영웅상태.게임시작_적군)
                PlaySE(characterSound[characterName].battleStart_Enemy);
            else if (state == 영웅상태.공격시)
                PlaySE(characterSound[characterName].attack);
            else if (state == 영웅상태.덱이없다)
                PlaySE(characterSound[characterName].deckEmpty);
            else if (state == 영웅상태.덱이적다)
                PlaySE(characterSound[characterName].deckLittle);
            else if (state == 영웅상태.죽을시)
                PlaySE(characterSound[characterName].death);
        }
        catch { }
    }

    public void PlaySE(string sound)
    {
        if(sound == "환호작음")
        {
            int n = Random.Range(0, 5) + 1;
            sound += n.ToString();
        }
        else if (sound == "환호보통")
        {
            int n = Random.Range(0, 5) + 1;
            sound += n.ToString();
        }
        else if (sound == "환호큼")
        {
            int n = Random.Range(0, 5) + 1;
            sound += n.ToString();
        }

        if (etcSound.ContainsKey(sound) == false)
            return;

        PlaySE(etcSound[sound]);
    }
}
