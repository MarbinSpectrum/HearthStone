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

    [HideInInspector]
    public PlayData playData;
    [HideInInspector]
    public Sprite[] num;

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
        DontDestroyOnLoad(gameObject);
        LoadTable();
        LoadCardImage();

        try
        {
#if UNITY_EDITOR
            var jtc2 = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
            playData = jtc2;
            playData.Print();
#else
            var jtc = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
                        playData = jtc;
            playData.Print();
#endif

        }
        catch
        {
            playData = new PlayData(true);
            string jsonData = ObjectToJson(playData);
            var jtc2 = JsonToOject<PlayData>(jsonData);
#if UNITY_EDITOR
            CreateJsonFile(Application.dataPath, "Resources/PlayData", jsonData);
            jtc2 = LoadJsonFile<PlayData>(Application.dataPath, "Resources/PlayData");
#else
            CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
            jtc2 = LoadJsonFile<PlayData>(Application.persistentDataPath, "PlayData");
#endif
            playData = jtc2;
            playData.Print();
        }

        num = Resources.LoadAll<Sprite>("Card/Number");
    }
    #endregion

    #region[OnApplicationQuit]
    private void OnApplicationQuit()
    {
        string jsonData = ObjectToJson(playData);
        JsonUtility.ToJson(new Serialization<PlayData.Deck>(playData.deck));
        CreateJsonFile(Application.persistentDataPath, "PlayData", jsonData);
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[카드 이미지 데이터 로드]
    public void LoadCardImage()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 1; j <= m_dic[(TableType)i].m_table.Count; j++)
            {
                string name = ToString((TableType)i, j, "카드이름");
                Texture2D temp = Resources.Load("CardImg/" + name) as Texture2D;
                if (temp)
                {
                    cardImg[name] = Sprite.Create(temp, new Rect(0, 0, temp.width, temp.height), new Vector2(0.5f, 0.5f));
                }
            }
    }

    #endregion

    #region[데이터 로드]
    public void LoadTable()
    {
        Load(TableType.드루이드);
        Load(TableType.도적);
        Load(TableType.중립);
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
