using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataMng : MonoBehaviour
{
    public enum TableType
    {
        중립, 도적, 드루이드
    }

    public static DataMng instance;

    private static DataMng m_instance;
    public Dictionary<TableType, LowBase> m_dic = new Dictionary<TableType, LowBase>();

    #region[Awake]
    public void Awake()
    {
        instance = this;
        LoadTable();
    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[데이터 로드]
    public void LoadTable()
    {
        Load(TableType.중립);
        Load(TableType.도적);
        Load(TableType.드루이드);
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
