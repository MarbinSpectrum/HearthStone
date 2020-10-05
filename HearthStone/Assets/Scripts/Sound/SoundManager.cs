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

    public Dictionary<string, MinionSound> minionSound = new Dictionary<string, MinionSound>();
    public Dictionary<string, SpellSound> spellSound = new Dictionary<string, SpellSound>();
    public Dictionary<string, CharacterSound> characterSound = new Dictionary<string, CharacterSound>();

    AudioClip gameIntro;

    AudioClip mainMenuBGM;
    AudioClip myCollectMenuBGM;
    AudioClip battleMapBGM;
    AudioClip[] findBattle = new AudioClip[6];

    AudioClip findBattleSlotEnd;
    AudioClip selectMulliganFirst;
    AudioClip selectMulliganSecond;
    AudioClip selectMulligan;

    AudioClip createCard;
    AudioClip deleteCard;
    AudioClip drawCard;
    AudioClip deckDelete;
    AudioClip deckOpen;
    AudioClip deckInputCard;

    AudioClip tinyBtn;
    AudioClip clickBtn;
    AudioClip paperFlip;
    AudioClip checkCard;
    AudioClip getCoin;
    AudioClip turnEndPlz;

    AudioClip battleVs;
    AudioClip battleMalfurion;
    AudioClip battleValeera;

    AudioClip turnStart;
    AudioClip playerTurnBtnDown;
    AudioClip AITurnBtnDown;

    AudioClip equitWeapon;
    AudioClip openWeapon;
    AudioClip closeWeapon;

    AudioClip heroAttackStart;
    AudioClip heroAttackEnd;

    AudioClip spawnMininon_normal;
    AudioClip attackMinion_normal;
    AudioClip attackMinion_middle;
    AudioClip attackMinion_hard;

    AudioClip victory;
    AudioClip defeat;
    AudioClip victoryCheer;
    AudioClip victoryFirecracker;
    AudioClip heroCrash;
    AudioClip heroExplode;

    AudioClip questClear;

    AudioClip buyEnd;
    AudioClip packArea;
    AudioClip packOpen;
    AudioClip packDown;

    AudioClip getRare;
    AudioClip getSpecial;
    AudioClip getLegend;

    AudioClip[] cheerSmall = new AudioClip[5];
    AudioClip[] cheerNormal = new AudioClip[5];
    AudioClip[] cheerBig = new AudioClip[5];

    [Range(0, 1)]
    public float maxBGM = 1;
    [Range(0, 1)]
    public float maxSE = 1;

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

        gameIntro = Resources.Load("Sound/여관주인 게임시작시") as AudioClip;

        mainMenuBGM = Resources.Load("Sound/메인화면배경음") as AudioClip;
        myCollectMenuBGM = Resources.Load("Sound/수집함배경음") as AudioClip;
        battleMapBGM = Resources.Load("Sound/대전맵배경음") as AudioClip;
        packArea = Resources.Load("Sound/팩개봉시설") as AudioClip;

        for (int i = 0; i < 6; i++)
            findBattle[i] = Resources.Load("Sound/대전상대찾기" + (i + 1)) as AudioClip;
        findBattleSlotEnd = Resources.Load("Sound/대전상대찾기슬롯멈춤") as AudioClip;

        selectMulliganFirst = Resources.Load("Sound/멀리건선택선공") as AudioClip;
        selectMulliganSecond = Resources.Load("Sound/멀리건선택후공") as AudioClip;
        selectMulligan = Resources.Load("Sound/멀리건선택") as AudioClip;

        createCard = Resources.Load("Sound/카드생성") as AudioClip;
        deleteCard = Resources.Load("Sound/카드삭제") as AudioClip;
        drawCard = Resources.Load("Sound/카드드로우") as AudioClip;
        deckDelete = Resources.Load("Sound/덱삭제") as AudioClip;
        deckOpen  = Resources.Load("Sound/덱펼치기") as AudioClip;
        deckInputCard = Resources.Load("Sound/덱에카드넣기") as AudioClip;

        tinyBtn = Resources.Load("Sound/작은버튼") as AudioClip;
        clickBtn = Resources.Load("Sound/버튼클릭") as AudioClip;
        paperFlip = Resources.Load("Sound/페이지넘기기") as AudioClip;
        checkCard = Resources.Load("Sound/수집함카드선택") as AudioClip;
        getCoin = Resources.Load("Sound/코인얻기") as AudioClip;
        turnEndPlz = Resources.Load("Sound/해당턴에할게없습니다") as AudioClip;
        battleVs = Resources.Load("Sound/대전자 소개/그 상대는") as AudioClip;
        battleMalfurion = Resources.Load("Sound/대전자 소개/여관주인_말퓨리온") as AudioClip;
        battleValeera = Resources.Load("Sound/대전자 소개/여관주인_발리라") as AudioClip;

        turnStart = Resources.Load("Sound/턴시작") as AudioClip;
        playerTurnBtnDown = Resources.Load("Sound/턴종료버튼누름") as AudioClip;
        AITurnBtnDown = Resources.Load("Sound/상대가턴종료버튼누름") as AudioClip;

        equitWeapon = Resources.Load("Sound/무기장착") as AudioClip;
        openWeapon = Resources.Load("Sound/무기열기") as AudioClip;
        closeWeapon = Resources.Load("Sound/무기닫기") as AudioClip;

        heroAttackStart = Resources.Load("Sound/영웅공격시작") as AudioClip;
        heroAttackEnd = Resources.Load("Sound/영웅공격끝") as AudioClip;

        spawnMininon_normal = Resources.Load("Sound/미니언소환_일반") as AudioClip;
        attackMinion_normal = Resources.Load("Sound/약한공격") as AudioClip;
        attackMinion_middle = Resources.Load("Sound/중간공격") as AudioClip;
        attackMinion_hard = Resources.Load("Sound/강한공격") as AudioClip;

        victory = Resources.Load("Sound/승리시") as AudioClip;
        victoryCheer = Resources.Load("Sound/승리의환호") as AudioClip;
        victoryFirecracker = Resources.Load("Sound/승리의폭죽") as AudioClip;
        defeat = Resources.Load("Sound/패배시") as AudioClip;
        heroCrash = Resources.Load("Sound/영웅깨짐") as AudioClip;
        heroExplode = Resources.Load("Sound/영웅폭발") as AudioClip;

        questClear = Resources.Load("Sound/퀘스트클리어") as AudioClip;
        buyEnd = Resources.Load("Sound/구매완료") as AudioClip;

        packOpen = Resources.Load("Sound/팩개봉") as AudioClip;
        packDown = Resources.Load("Sound/팩내려놓기") as AudioClip;

        getRare = Resources.Load("Sound/희귀카드") as AudioClip;
        getSpecial = Resources.Load("Sound/특급카드") as AudioClip;
        getLegend = Resources.Load("Sound/전설카드") as AudioClip;

        for (int i = 0; i < 5; i++)
            cheerSmall[i] = Resources.Load("Sound/관객반응/환호_작음" + (i + 1)) as AudioClip;
        for (int i = 0; i < 5; i++)
            cheerNormal[i] = Resources.Load("Sound/관객반응/환호_보통" + (i + 1)) as AudioClip;
        for (int i = 0; i < 5; i++)
            cheerBig[i] = Resources.Load("Sound/관객반응/환호_큼" + (i + 1)) as AudioClip;

        GetMinionSound();
        GetSpellSound();
        GetHeroSound();
    }
    #endregion

    private void GetMinionSound()
    {
        Transform sounds = transform.Find("MinionSounds");
        for (int i = 0; i < sounds.childCount; i++)
            minionSound.Add(sounds.GetChild(i).transform.name, sounds.GetChild(i).GetComponent<MinionSound>());
    }

    private void GetSpellSound()
    {
        Transform sounds = transform.Find("SpellSounds");
        for (int i = 0; i < sounds.childCount; i++)
            spellSound.Add(sounds.GetChild(i).transform.name, sounds.GetChild(i).GetComponent<SpellSound>());
    }

    private void GetHeroSound()
    {
        Transform sounds = transform.Find("CharacterSounds");
        for (int i = 0; i < sounds.childCount; i++)
            characterSound.Add(sounds.GetChild(i).transform.name, sounds.GetChild(i).GetComponent<CharacterSound>());
    }

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

    private IEnumerator PlayNewBGM(string s)
    {
        bool isPlayNow = false;
        AudioClip newBGM = null;

        while (BGM.volume < maxBGM)
            yield return new WaitForSeconds(0.001f);

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
            case "대전상대찾기2":
            case "대전상대찾기3":
            case "대전상대찾기4":
            case "대전상대찾기5":
            case "대전상대찾기6":
                int n = s[6] - '0';
                n--;
                if (BGM.clip == findBattle[n])
                    isPlayNow = true;
                else
                    newBGM = findBattle[n];
                break;
            case "대전상대찾기":
                n = Random.Range(0, 6);
                if (BGM.clip == findBattle[n])
                    isPlayNow = true;
                else
                    newBGM = findBattle[n];
                break;

            case "대전맵배경음":
                if (BGM.clip == battleMapBGM)
                    isPlayNow = true;
                else
                    newBGM = battleMapBGM;
                break;

            case "팩개봉시설":
                if (BGM.clip == packArea)
                    isPlayNow = true;
                else
                    newBGM = packArea;
                break;

            default:
                newBGM = null;
                break;

        }

        if (!isPlayNow)
        {
            //float v = BGM.volume * maxBGM;
            //for (int i = 0; i < 50; i++)
            //{
            //    yield return new WaitForSeconds(0.002f);
            //    BGM.volume -= 0.02f * maxBGM;
            //}
            BGM.clip = null;
            BGM.clip = newBGM;
            //BGM.volume = v;
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

    public void PlaySE(string s)
    {
        switch (s)
        {
            case "게임인트로":
                PlaySE(gameIntro);
                break;
            case "작은버튼":
                PlaySE(tinyBtn);
                break;
            case "버튼클릭":
                PlaySE(clickBtn);
                break;
            case "페이지넘기기":
                PlaySE(paperFlip);
                break;
            case "수집함카드선택":
                PlaySE(checkCard);
                break;
            case "코인얻기":
                PlaySE(getCoin);
                break;
            case "해당턴에할게없습니다":
                PlaySE(turnEndPlz);
                break;
            case "그상대는":
                PlaySE(battleVs);
                break;
            case "말퓨리온":
                PlaySE(battleMalfurion);
                break;
            case "발리라":
                PlaySE(battleValeera);
                break;
            case "카드생성":
                PlaySE(createCard);
                break;
            case "카드삭제":
                PlaySE(deleteCard);
                break;
            case "카드드로우":
                PlaySE(drawCard);
                break;
            case "덱펼치기":
                PlaySE(deckOpen);
                break;
            case "덱삭제":
                PlaySE(deckDelete);
                break;
            case "덱에카드넣기":
                PlaySE(deckInputCard);
                break;
            case "대전상대찾기슬롯멈춤":
                PlaySE(findBattleSlotEnd);
                break;
            case "멀리건선택선공":
                PlaySE(selectMulliganFirst);
                break;
            case "멀리건선택후공":
                PlaySE(selectMulliganSecond);
                break;
            case "멀리건선택":
                PlaySE(selectMulligan);
                break;
            case "턴시작":
                PlaySE(turnStart);
                break;
            case "무기장착":
                PlaySE(equitWeapon);
                break;
            case "무기열기":
                PlaySE(openWeapon);
                break;
            case "무기닫기":
                PlaySE(closeWeapon);
                break;
            case "영웅깨짐":
                PlaySE(heroCrash);
                break;
            case "영웅폭발":
                PlaySE(heroExplode);
                break;
            case "승리시":
                PlaySE(victory);
                break;
            case "승리의환호":
                PlaySE(victoryCheer);
                break;
            case "승리의폭죽":
                PlaySE(victoryFirecracker);
                break;
            case "패배시":
                PlaySE(defeat);
                break;
            case "미니언소환일반":
                PlaySE(spawnMininon_normal);
                break;
            case "약한공격":
                PlaySE(attackMinion_normal);
                break;
            case "중간공격":
                PlaySE(attackMinion_middle);
                break;
            case "강한공격":
                PlaySE(attackMinion_hard);
                break;
            case "상대가턴종료버튼누름":
                PlaySE(AITurnBtnDown);
                break;
            case "턴종료버튼누름":
                PlaySE(playerTurnBtnDown);
                break;
            case "영웅공격시작":
                PlaySE(heroAttackStart);
                break;
            case "영웅공격끝":
                PlaySE(heroAttackEnd);
                break;
            case "환호작음":
                PlaySE(cheerSmall[Random.Range(0, cheerSmall.Length)]);
                break;
            case "환호보통":
                PlaySE(cheerNormal[Random.Range(0, cheerNormal.Length)]);
                break;
            case "환호큼":
                PlaySE(cheerBig[Random.Range(0, cheerBig.Length)]);
                break;
            case "퀘스트클리어":
                PlaySE(questClear);
                break;
            case "구매완료":
                PlaySE(buyEnd);
                break;
            case "팩개봉":
                PlaySE(packOpen);
                break;
            case "팩내려놓기":
                PlaySE(packDown);
                break;
            case "희귀카드":
                PlaySE(getRare);
                break;
            case "특급카드":
                PlaySE(getSpecial);
                break;
            case "전설카드":
                PlaySE(getLegend);
                break;
        }
    }
}
