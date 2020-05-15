using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMng : MonoBehaviour
{
    public enum TableType
    {
        중립,드루이드, 도적
    }

    public static DataMng instance;

    private static DataMng m_instance;
    public Dictionary<TableType, LowBase> m_dic = new Dictionary<TableType, LowBase>();
    public Dictionary<string, Sprite> cardImg = new Dictionary<string, Sprite>();

    #region[Awake]
    public void Awake()
    {
        instance = this;
        LoadTable();
        LoadCardImage();
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
}
