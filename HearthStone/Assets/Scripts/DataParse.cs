using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataParse
{
    #region[ī���̸� ���]
    public static string GetCardName(string s)
    {
        string cardName = "";
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '~')
                break;
            else
                cardName += s[i];
        }
        return cardName;
    }
    #endregion

    #region[ī�尹�� ���]
    public static int GetCardNumber(string s)
    {
        string cardN = "";
        bool flag = false;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '~')
                flag = true;
            else if (flag)
                cardN += s[i];
        }
        try
        {
            int R = 0;
            int.TryParse(cardN, out R);
            return R;
        }
        catch { return 0; }
    }
    #endregion

    public static string GetParseData(string name, int num)
    {
        string data = name + "~" + num.ToString();

        return data;
    }
}
