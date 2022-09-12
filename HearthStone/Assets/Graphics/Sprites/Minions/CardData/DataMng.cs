using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public LowBase cardPer = new LowBase();

    [HideInInspector] public PlayData playData;
    [HideInInspector] public Sprite[] num;

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

    #region[OnApplicationQuit]
    private void OnApplicationQuit()
    {
        SaveData();
    }
    #endregion

    #region[SaveData]
    public void SaveData()
    {
        string jsonData = ObjectToJson(playData);
        var jtc2 = JsonToOject<PlayData>(jsonData);
        if (!Application.isEditor)
            CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
        else
            CreateJsonFile(Application.dataPath, "Resources/PlayData", jsonData);
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        //카드 숫자 폰트 로드
        num = Resources.LoadAll<Sprite>("Card/Number");

        //직업별 카드 데이터 로드
        yield return new WaitUntil(() => Load(TableType.드루이드));
        yield return new WaitUntil(() => Load(TableType.도적));
        yield return new WaitUntil(() => Load(TableType.중립));

        //카드 별 상세 확률 로드
        yield return new WaitUntil(() => LoadCarPercentData());

        //플레이어 데이터 로드
        yield return new WaitUntil(() => LoadPlayData());


        DataLoadSuccess = true;
    }

    private bool Load(TableType table)
    {
        if (!m_dic.ContainsKey(table))
        {
            //CSV파일로 되어있는 데이터를 로드
            LowBase lowBase = new LowBase();
            lowBase.Load("Table/" + table.ToString());
            m_dic.Add(table, lowBase);
        }

        for (int j = 1; j <= m_dic[table].m_table.Count; j++)
        {
            //인덱스에 해당하는 카드 이미지 로드
            string name = ToString(table, j, "카드이름");
            Texture2D temp = Resources.Load("CardImg/" + name) as Texture2D;
            if (temp)
            {
                //카드 이미지 등록
                cardImg[name] = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height),
                    new Vector2(0.5f, 0.5f));
            }

            //카드 피봇 등록(string으로 되어있는 숫자들을 파싱)
            string dragPivot = ToString(table, j, "DragPivot");
            dragPivot.Replace('\r', ' ');
            dragPivot.Trim();

            string[] pivotData = dragPivot.Split('[',']', '\r',' ');
            List<float> floatList = new List<float>();
            for (int i = 0; i < pivotData.Length; i++)
            {
                //Split한 데이터들을 확인
                if (!string.IsNullOrEmpty(pivotData[i]))
                {
                    //공백 데이터가 아니라면
                    float f = float.Parse(pivotData[i]);
                    floatList.Add(f);
                }
            }

            //피봇을 등록
            Vector2 pivot = new Vector2(floatList[0], floatList[1]);
            dragCardPos.Add(name, pivot);
        }
        return true;
    }

    private bool LoadCarPercentData()
    {
        cardPer.Load("Table/카드확률데이터");    
        return true;
    }
    #endregion

    #region[플레이더 데이터 로드]
    private bool LoadPlayData()
    {
        //json형식으로 저장된 데이터로드
        string filePath = Application.persistentDataPath + "/PlayData.json";
        if (Application.isEditor)
        {
            filePath = Application.dataPath + "/Resources/PlayData.json";
            if (File.Exists(filePath))
            {
                var jtc = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
                playData = jtc;
            }
            else
            {

                playData = new PlayData(true);
                string jsonData = ObjectToJson(playData);
                var jtc = JsonToOject<PlayData>(jsonData);
                CreateJsonFile(Application.dataPath, "Resources/PlayData", jsonData);
                jtc = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
                playData = jtc;
            }
        }
        else
        {
            if (File.Exists(filePath))
            {
                //저장된 데이터가 있다면 해당 데이터를 로드
                var jtc = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
                playData = jtc;
            }
            else
            {
                //데이터가 없다면 데이터를 새로 생성
                playData = new PlayData(true);
                string jsonData = ObjectToJson(playData);
                var jtc = JsonToOject<PlayData>(jsonData);

                CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
                jtc = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
                playData = jtc;
            }
        }
        return true;
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
