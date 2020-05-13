using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowBase
{
    Dictionary<int, Dictionary<string, string>> m_table = new Dictionary<int, Dictionary<string, string>>();

    public void Load(string str)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(str);
        if (textAsset == null) return;

        string[] rows = textAsset.text.Split('\n');
        List<string> rowList = new List<string>();
        for(int i = 0; i < rows.Length; i++)
            if (!string.IsNullOrEmpty(rows[i]))
            {
                string row = rows[i].Replace('\r', ' ');
                row = row.Trim();
                rowList.Add(rows[i]);
            }


        string[] subjects = rowList[0].Split(',');

        for (int r = 1; r < rowList.Count; r++)
        {
            string[] values = rowList[r].Split(',');
            int tableID = 0;
            int.TryParse(values[0], out tableID);

            if (!m_table.ContainsKey(tableID))
            {
                m_table.Add(tableID, new Dictionary<string, string>());

                for(int c = 1; c < values.Length; c++)
                    if (!m_table[tableID].ContainsKey(subjects[c]))
                        m_table[tableID].Add(subjects[c], values[c]);
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
