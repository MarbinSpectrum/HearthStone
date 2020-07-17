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
    void Awake()
    {
        List<Dropdown.OptionData> temp = new List<Dropdown.OptionData>();
        for (MinionAbility.Condition i = MinionAbility.Condition.전투의함성; i <= MinionAbility.Condition.버그; i++)
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

    void Update()
    {
        MinionAbility.Condition c = (MinionAbility.Condition)condition.value;
        condition_data.SetActive(MinionAbility.CheckDataCondition(c));

        MinionAbility.Ability a = (MinionAbility.Ability)ability.value;
        ability_data.SetActive(MinionAbility.CheckDataAbility(a));

        string s = "";
        s += "[" + c.ToString() + "]";
        if(condition_data.activeSelf)
        {
            s += "[" + c_data1.text + "]";
            s += "[" + c_data2.text + "]";
            s += "[" + c_data3.text + "]";
        }
        s += "[" + a.ToString() + "]";
        if (ability_data.activeSelf)
        {
            s += "[" + a_data1.text + "]";
            s += "[" + a_data2.text + "]";
            s += "[" + a_data3.text + "]";
        }
        outData.text = s;
    }
}
