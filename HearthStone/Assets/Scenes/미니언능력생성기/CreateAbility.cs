using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : MonoBehaviour
{
    public Dropdown condition;
    public GameObject condition_data;
    public InputField c_data1;
    public InputField c_data2;
    public InputField c_data3;
    public Dropdown ability;
    public GameObject ability_data;
    public InputField a_data1;
    public InputField a_data2;
    public InputField a_data3;
    public InputField outData;
    private void Awake()
    {
        List<Dropdown.OptionData> temp = new List<Dropdown.OptionData>();
        for (MinionAbility.Condition i = MinionAbility.Condition.조건없음; i <= MinionAbility.Condition.버그; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            temp.Add(data);
        }
        condition.AddOptions(temp);

        temp.Clear();
        for (MinionAbility.Ability i = MinionAbility.Ability.하수인에게_피해주기; i <= MinionAbility.Ability.버그; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            temp.Add(data);
        }
        ability.AddOptions(temp);
    }

    private void Update()
    {
        MinionAbility.Condition c = (MinionAbility.Condition)condition.value;
        condition_data.SetActive(MinionAbility.CheckDataCondition(c));
        int cnum = MinionAbility.GetParameterNum(c);
        c_data1.gameObject.SetActive(cnum >= 1);
        c_data2.gameObject.SetActive(cnum >= 2);
        c_data3.gameObject.SetActive(cnum >= 3);

        MinionAbility.Ability a = (MinionAbility.Ability)ability.value;
        ability_data.SetActive(MinionAbility.CheckDataAbility(a));
        int anum = MinionAbility.GetParameterNum(a);
        a_data1.gameObject.SetActive(anum >= 1);
        a_data2.gameObject.SetActive(anum >= 2);
        a_data3.gameObject.SetActive(anum >= 3);

        string s = "";
        s += "[" + c.ToString() + "]";
        if (condition_data.activeSelf)
        {
            if (c_data1.gameObject.activeSelf)
                s += "[" + c_data1.text + "]";
            if (c_data2.gameObject.activeSelf)
                s += "[" + c_data2.text + "]";
            if (c_data3.gameObject.activeSelf)
                s += "[" + c_data3.text + "]";
        }

        s += "[" + a.ToString() + "]";
        if (ability_data.activeSelf)
        {
            if (a_data1.gameObject.activeSelf)
                s += "[" + a_data1.text + "]";
            if (a_data2.gameObject.activeSelf)
                s += "[" + a_data2.text + "]";
            if (a_data3.gameObject.activeSelf)
                s += "[" + a_data3.text + "]";
        }
        outData.text = s;
    }
}
