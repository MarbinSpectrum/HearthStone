using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int questNum;
    public int value;
    public Quest()
    {
        questNum = Random.Range(0, 6);
        value = 0;
    }

    public Quest(int n)
    {
        questNum = n;
        value = 0;
    }
}