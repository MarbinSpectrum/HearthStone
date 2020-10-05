using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Text;

public class DataMng : MonoBehaviour
{
    public enum TableType
    {
        모두 = -1,
        드루이드, 도적, 중립
    }

    public static DataMng instance;

    public Dictionary<TableType, LowBase> m_dic = new Dictionary<TableType, LowBase>();
    public Dictionary<string, Sprite> cardImg = new Dictionary<string, Sprite>();
    public Dictionary<string, Vector2> dragCardPos = new Dictionary<string, Vector2>();
    [HideInInspector]
    public PlayData playData;
    [HideInInspector]
    public Sprite[] num;
    public Animator loadingAni;

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
        if (SceneManager.GetActiveScene().name.Equals("Loading"))
            StartCoroutine(LoadData());
        else
            LoadDatas();

    }
    #endregion

    #region[OnApplicationQuit]
    private void OnApplicationQuit()
    {
        SaveData();
    }
    #endregion

    public void SaveData()
    {
        string jsonData = ObjectToJson(playData);
        var jtc2 = JsonToOject<PlayData>(jsonData);
        if (!Application.isEditor)
            CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
        else
            CreateJsonFile(Application.dataPath, "Resources/PlayData", jsonData);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[드래그 카드 이미지]
    public void SettingDragCardPos()
    {
        dragCardPos.Add("SI7 요원", new Vector2(90, -82));
        dragCardPos.Add("가시덤불 호랑이", new Vector2(62, -49));
        dragCardPos.Add("가시덩굴 사냥꾼", new Vector2(90, -95));
        dragCardPos.Add("가젯잔 경매인", new Vector2(90, -66));
        dragCardPos.Add("가혹한 하사관", new Vector2(85, -67));
        dragCardPos.Add("검 제작의 대가", new Vector2(85, -42.5f));
        dragCardPos.Add("검은무쇠 드워프", new Vector2(85, -91.4f));
        dragCardPos.Add("고대의 감시자", new Vector2(85, -108.9f));
        dragCardPos.Add("고대의 마법사", new Vector2(85, -58.6f));
        dragCardPos.Add("고대의 비밀", new Vector2(124.8f, -77.5f));
        dragCardPos.Add("고대의 양조사", new Vector2(110.9f, -67.5f));
        dragCardPos.Add("고대의 지식", new Vector2(88, -6.7f));
        dragCardPos.Add("곰 변신", new Vector2(133.8f, 12.2f));
        dragCardPos.Add("공격대장", new Vector2(95.9f, -47.6f));
        dragCardPos.Add("공포의 해적", new Vector2(107.9f, -80.5f));
        dragCardPos.Add("광기의 연금술사", new Vector2(107.9f, -48.6f));
        dragCardPos.Add("광포한 늑대 우두머리", new Vector2(121.8f, -84.5f));
        dragCardPos.Add("구루바시 광전사", new Vector2(85.9f, -103.4f));
        dragCardPos.Add("굶주린 식인 구울", new Vector2(92.9f, -77.5f));
        dragCardPos.Add("그룰", new Vector2(82.9f, -34.7f));
        dragCardPos.Add("그림자 밟기", new Vector2(120.7f, -70.6f));
        dragCardPos.Add("급속 성장", new Vector2(24, 126));
        dragCardPos.Add("기계용 정비사", new Vector2(110, -42));
        dragCardPos.Add("기습", new Vector2(55, -67));
        dragCardPos.Add("나 이런 사냥꾼이야", new Vector2(86, -83));
        dragCardPos.Add("나무정령(숲의영혼)", new Vector2(86, -57));
        dragCardPos.Add("나무정령(자연의 군대)", new Vector2(86, -57));
        dragCardPos.Add("나무정령(샨도의 가르침)", new Vector2(86, -57));
        dragCardPos.Add("날뛰는 코도", new Vector2(69, 29));
        dragCardPos.Add("남쪽바다 갑판원", new Vector2(109, -98));
        dragCardPos.Add("내트 페이글", new Vector2(70, -73));
        dragCardPos.Add("냉기정령", new Vector2(70, -77));
        dragCardPos.Add("냉혈", new Vector2(90, -89));
        dragCardPos.Add("넘치는마나", new Vector2(95, -28));
        dragCardPos.Add("노움 발명가", new Vector2(95, -1));
        dragCardPos.Add("늑대기수", new Vector2(120, -119));
        dragCardPos.Add("달빛 섬광(숲의 수호자)", new Vector2(103, -123));
        dragCardPos.Add("달빛 섬광", new Vector2(98, 42));
        dragCardPos.Add("대지 고리회 선견자", new Vector2(98, -57));
        dragCardPos.Add("데스윙", new Vector2(95, -57));
        dragCardPos.Add("데피아즈단 두목", new Vector2(98.5f, -78.4f));
        dragCardPos.Add("데피아즈단 무법자", new Vector2(92.3f, -117.6f));
        dragCardPos.Add("독칼", new Vector2(99.4f, -92.6f));
        dragCardPos.Add("돌주먹 오우거", new Vector2(99.4f, -77.4f));
        dragCardPos.Add("두터운 가죽", new Vector2(74.4f, -19.4f));
        dragCardPos.Add("리로이 젠킨스", new Vector2(132, -117));
        dragCardPos.Add("마음가짐", new Vector2(113.3f, -42.1f));
        dragCardPos.Add("말리고스", new Vector2(98.1f, -98.3f));
        dragCardPos.Add("맹독", new Vector2(37.6f, -3.8f));
        dragCardPos.Add("머리 후려치기", new Vector2(164.2f, -2));
        dragCardPos.Add("멧돼지", new Vector2(100, -9.1f));
        dragCardPos.Add("무리의 우두머리", new Vector2(36.7f, -89.4f));
        dragCardPos.Add("무쇠껍질 수호정령", new Vector2(80.4f, -45.7f));
        dragCardPos.Add("무쇠부리 올빼미", new Vector2(72.4f, -84.9f));
        dragCardPos.Add("무수한 성장", new Vector2(91.1f, -15.3f));
        dragCardPos.Add("무효화", new Vector2(37.6f, -50.1f));
        dragCardPos.Add("바인 블러드후프", new Vector2(83.9f, -61.7f));
        dragCardPos.Add("반신의 총애", new Vector2(157f, -74.2f));
        dragCardPos.Add("발톱의 드루이드(곰)", new Vector2(114.2f, 7.9f));
        dragCardPos.Add("발톱의 드루이드(표범)", new Vector2(128.5f, -15.3f));
        dragCardPos.Add("발톱의 드루이드", new Vector2(128.5f, -103.6f));
        dragCardPos.Add("배신", new Vector2(117, -54.6f));
        dragCardPos.Add("별똥별", new Vector2(132.2f, -0.2f));
        dragCardPos.Add("별빛섬광", new Vector2(171.4f, -28.7f));
        dragCardPos.Add("별의 군주", new Vector2(107.2f, -75.1f));
        dragCardPos.Add("보랏빛 수습생", new Vector2(107.2f, -101f));
        dragCardPos.Add("보랏빛 여교사", new Vector2(92.9f, -77.9f));
        dragCardPos.Add("불의 군주 라그나로스", new Vector2(86.7f, -52.9f));
        dragCardPos.Add("붉은해적단 약탈자", new Vector2(95.6f, -109.1f));
        dragCardPos.Add("뿌리 들기", new Vector2(8.2f, -101.1f));
        dragCardPos.Add("뿌리 박기", new Vector2(40.3f, -50.3f));
        dragCardPos.Add("사악한 일격", new Vector2(154.4f, 8.6f));
        dragCardPos.Add("새끼용", new Vector2(133.9f, -21.7f));
        dragCardPos.Add("샨도의 가르침", new Vector2(86, -57));
        dragCardPos.Add("서리바람 설인", new Vector2(107.2f, -67.2f));
        dragCardPos.Add("세나리우스", new Vector2(93.8f, -98.4f));
        dragCardPos.Add("센진 방패대가", new Vector2(100, -52.9f));
        dragCardPos.Add("소멸", new Vector2(142.7f, -33.2f));
        dragCardPos.Add("소형 기계용", new Vector2(47.3f, -51f));
        dragCardPos.Add("손상된 골렘", new Vector2(92.8f, -117));
        dragCardPos.Add("숲의 수호자", new Vector2(92.8f, -90.2f));
        dragCardPos.Add("숲의 영혼", new Vector2(36.6f, -75.9f));
        dragCardPos.Add("실바나스 윈드러너", new Vector2(91.9f, -89.3f));
        dragCardPos.Add("알렉스트라자", new Vector2(101.7f, -74.2f));
        dragCardPos.Add("암살", new Vector2(58, -56.4f));
        dragCardPos.Add("암살자의 검", new Vector2(92.8f, -12.7f));
        dragCardPos.Add("야생성", new Vector2(63.4f, -97.4f));
        dragCardPos.Add("야생의 징표", new Vector2(79.4f, 36.4f));
        dragCardPos.Add("야생의 포효", new Vector2(-1.7f, -15.3f));
        dragCardPos.Add("야생의 힘", new Vector2(191.4f, -65.7f));
        dragCardPos.Add("에드윈 밴클리프", new Vector2(101.3f, -102f));
        dragCardPos.Add("육성", new Vector2(113, 18.7f));
        dragCardPos.Add("은폐", new Vector2(136.3f, -78));
        dragCardPos.Add("자연의 격노", new Vector2(97, -78));
        dragCardPos.Add("자연의 군대", new Vector2(-21.5f, -73.7f));
        dragCardPos.Add("자연의 징표", new Vector2(29, -64));
        dragCardPos.Add("자연화", new Vector2(100, -30));
        dragCardPos.Add("전력 질주", new Vector2(153, -109));
        dragCardPos.Add("전리품 수집가", new Vector2(133, -36));
        dragCardPos.Add("전멸의 비수", new Vector2(109, -117));
        dragCardPos.Add("전쟁의 고대정령", new Vector2(104, -58));
        dragCardPos.Add("절개", new Vector2(104, 1));
        dragCardPos.Add("젊은 양조사", new Vector2(97, -59));
        dragCardPos.Add("정신 자극", new Vector2(90, -38));
        dragCardPos.Add("정신 지배 기술자", new Vector2(92, -31));
        dragCardPos.Add("지식의 고대정령", new Vector2(92, -74));
        dragCardPos.Add("천벌", new Vector2(87, -87));
        dragCardPos.Add("치유의 손길", new Vector2(81, -105));
        dragCardPos.Add("칼날 부채", new Vector2(11, -30));
        dragCardPos.Add("케른 블러드후프", new Vector2(58, -73));
        dragCardPos.Add("태양의 격노", new Vector2(111, -87));
        dragCardPos.Add("폭풍의 칼날", new Vector2(111, -48));
        dragCardPos.Add("표범 변신", new Vector2(190, -17));
        dragCardPos.Add("표범 소환", new Vector2(27, -49));
        dragCardPos.Add("표범", new Vector2(64, 0));
        dragCardPos.Add("풍요로움", new Vector2(57, -8));
        dragCardPos.Add("하늘빛 비룡", new Vector2(57, -109));
        dragCardPos.Add("할퀴기", new Vector2(104, -27));
        dragCardPos.Add("항성 이동", new Vector2(94, 2));
        dragCardPos.Add("허수아비골렘", new Vector2(94, -99));
        dragCardPos.Add("혈법사 탈노스", new Vector2(89, -89));
        dragCardPos.Add("호랑이의 분노", new Vector2(136, 13));
        dragCardPos.Add("혼절시키기", new Vector2(72, -26));
        dragCardPos.Add("휘둘러치기", new Vector2(131, -53));
        dragCardPos.Add("흑기사", new Vector2(111, -39));
    }
    #endregion

    #region[데이터 로드]
    public void LoadDatas()
    {
        num = Resources.LoadAll<Sprite>("Card/Number");
        Load(TableType.드루이드);
        Load(TableType.도적);
        Load(TableType.중립);
        for (int i = 0; i < 3; i++)
            for (int j = 1; j <= m_dic[(TableType)i].m_table.Count; j++)
            {
                string name = ToString((TableType)i, j, "카드이름");
                Texture2D temp = Resources.Load("CardImg/" + name) as Texture2D;
                if (temp)
                    cardImg[name] = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
            }
        SettingDragCardPos();
        LoadPlayData();

    }

    void LoadPlayData()
    {
        string filePath = Application.persistentDataPath + "/PlayData.json";
        if(Application.isEditor)
        {
            filePath = Application.dataPath + "/Resources/PlayData.json";
            if (File.Exists(filePath))
            {
                var jtc = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
                playData = jtc;
                playData.Print();
            }
            else
            {

                playData = new PlayData(true);
                string jsonData = ObjectToJson(playData);
                var jtc = JsonToOject<PlayData>(jsonData);
                CreateJsonFile(Application.dataPath, "Resources/PlayData", jsonData);
                jtc = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
                playData = jtc;
                playData.Print();
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                var jtc = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
                playData = jtc;
                playData.Print();
            }
            else
            {

                playData = new PlayData(true);
                string jsonData = ObjectToJson(playData);
                var jtc = JsonToOject<PlayData>(jsonData);
                CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
                jtc = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
                playData = jtc;
                playData.Print();
            }
        }
    }

    IEnumerator LoadData()
    {
        num = Resources.LoadAll<Sprite>("Card/Number");
        yield return new WaitForSeconds(0.01f);
        Load(TableType.드루이드);
        Debug.Log("드루이드 로드 완료");
        yield return new WaitForSeconds(0.01f);
        Load(TableType.도적);
        Debug.Log("도적 로드 완료");
        yield return new WaitForSeconds(0.01f);
        Load(TableType.중립);
        Debug.Log("중립 로드 완료");
        yield return new WaitForSeconds(0.01f);
        for (int i = 0; i < 3; i++)
            for (int j = 1; j <= m_dic[(TableType)i].m_table.Count; j++)
            {
                yield return new WaitForSeconds(0.01f);
                string name = ToString((TableType)i, j, "카드이름");
                Texture2D temp = Resources.Load("CardImg/" + name) as Texture2D;
                if (temp)
                    cardImg[name] = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
            }
        Debug.Log("카드 이미지 완료");
        yield return new WaitForSeconds(0.1f);
        SettingDragCardPos();
        Debug.Log("카드 위치 설정 롼료");
        yield return new WaitForSeconds(0.1f);
        LoadPlayData();
        Debug.Log("저장 기록 로드 완료");
        if (loadingAni)
            loadingAni.enabled = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayBGM("메인화면배경음");
        SoundManager.instance.PlaySE("게임인트로");
        yield return new WaitForSeconds(2f);
        Destroy(loadingAni.gameObject);
    }

    public void Load(TableType table)
    {
        if(!m_dic.ContainsKey(table))
        {
            LowBase lowBase = new LowBase();
            lowBase.Load("Table/" + table.ToString());
            m_dic.Add(table, lowBase);
        }
    }
    #endregion

    #region[데이터쌍 얻기]
    public Vector2 GetPairByName(string s)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 1; j <= m_dic[(TableType)i].m_table.Count; j++)
                if (ToString((TableType)i, j, "카드이름").Equals(s))
                    return new Vector2(i, j);
        return new Vector2(0, 0);
    }
    #endregion

    #region[값 얻기]
    public int ToInteger(TableType tableType, int mainKey, string subKey)
    {
        if (m_dic.ContainsKey(tableType))
            return m_dic[tableType].ToInteger(mainKey, subKey);
        return -1;
    }

    public float ToFloat(TableType tableType, int mainKey, string subKey)
    {
        if (m_dic.ContainsKey(tableType))
            return m_dic[tableType].ToFloat(mainKey, subKey);
        return -1;
    }

    public string ToString(TableType tableType, int mainKey, string subKey)
    {
        if (m_dic.ContainsKey(tableType))
            return m_dic[tableType].ToString(mainKey, subKey);
        return string.Empty;
    }
    #endregion

    #region[JSON 관리]
    string ObjectToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    T JsonToOject<T>(string jsonData)
    {
        return JsonUtility.FromJson<T>(jsonData);
    }

    void CreateJsonFile(string createPath, string fileName, string jsonData)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    T LoadJsonFile<T>(string loadPath, string fileName)
    {
        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", loadPath, fileName), FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(jsonData);
    }

    [System.Serializable]
    public class Serialization<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> keys;
        [SerializeField]
        List<TValue> values;

        Dictionary<TKey, TValue> target;
        public Dictionary<TKey, TValue> ToDictionary() { return target; }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = Mathf.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }

    [System.Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> target;
        public List<T> ToList() { return target; }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }
    #endregion
}
