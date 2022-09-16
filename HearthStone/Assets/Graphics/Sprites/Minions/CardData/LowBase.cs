using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBase
{
    public Dictionary<int, Dictionary<string, string>> m_table = new Dictionary<int, Dictionary<string, string>>();

    public void Load(string str)
    {
        //,자 형식으로 저장된 csv파일을 읽는다.
        TextAsset textAsset = Resources.Load<TextAsset>(str);
        if (textAsset == null) 
            return;

        //줄을 나눈다.
        string[] rows = textAsset.text.Split('\n');
        List<string> rowList = new List<string>();
        for(int i = 0; i < rows.Length; i++)
        {
            if (string.IsNullOrEmpty(rows[i]))
            {
                //아무것도 없는 객체다.
                continue;
            }
            string row = rows[i].Replace("\r", String.Empty);
            row = row.Trim();
            rowList.Add(rows[i]);
        }

        //제목줄
        string[] subjects = rowList[0].Split(',');

        for (int r = 1; r < rowList.Count; r++)
        {
            //해당 줄부터 데이터다.
            string[] values = rowList[r].Split(',');

            //ID부터 등록
            int tableID = 0;
            int.TryParse(values[0], out tableID);

            if(m_table.ContainsKey(tableID))
            {
                //해당키의 존재한다. 
                continue;
            }

            m_table.Add(tableID, new Dictionary<string, string>());

            for (int c = 1; c < values.Length; c++)
            {
                if (m_table[tableID].ContainsKey(subjects[c]))
                {
                    //같은 형식의 데이터가 존재한다.
                    continue;
                }
                string temp = subjects[c].Replace('\r', ' ').Trim();
                values[c] = values[c].Replace('\r', ' ').Trim();
                m_table[tableID].Add(temp, values[c]);
            }
        }
    }

    public int ToInteger(int mainKey,string subKey)
    {
        if (!m_table.ContainsKey(mainKey))
            return -1;

        if (!m_table[mainKey].ContainsKey(subKey))
            return -1;

        int findValue = 0;
        int.TryParse(m_table[mainKey][subKey], out findValue);
        return findValue;
    }

    public float ToFloat(int mainKey, string subKey)
    {
        if (!m_table.ContainsKey(mainKey))
            return -1;

        if (!m_table[mainKey].ContainsKey(subKey))
            return -1;

        float findValue = 0;
        float.TryParse(m_table[mainKey][subKey], out findValue);
        return findValue;
    }

    public string ToString(int mainKey, string subKey)
    {
        if (!m_table.ContainsKey(mainKey))
            return string.Empty;

        if (!m_table[mainKey].ContainsKey(subKey))
            return string.Empty;

        return m_table[mainKey][subKey];
    }
}
