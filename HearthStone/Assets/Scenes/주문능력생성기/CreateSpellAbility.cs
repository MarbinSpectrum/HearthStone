using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateSpellAbility : MonoBehaviour
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
        for (SpellAbility.Condition i = SpellAbility.Condition.조건없음; i <= SpellAbility.Condition.버그; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            temp.Add(data);
        }
        condition.AddOptions(temp);

        temp.Clear();
        for (SpellAbility.Ability i = SpellAbility.Ability.하수인에게_피해주기; i <= SpellAbility.Ability.버그; i++)
        {
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = i.ToString();
            temp.Add(data);
        }
        ability.AddOptions(temp);
    }

    private void Update()
    {
        SpellAbility.Condition c = (SpellAbility.Condition)condition.value;
        condition_data.SetActive(SpellAbility.CheckDataCondition(c));
        int cnum = SpellAbility.GetParameterNum(c);
        c_data1.gameObject.SetActive(cnum >= 1);
        c_data2.gameObject.SetActive(cnum >= 2);
        c_data3.gameObject.SetActive(cnum >= 3);


        SpellAbility.Ability a = (SpellAbility.Ability)ability.value;
        ability_data.SetActive(SpellAbility.CheckDataAbility(a));
        int anum = SpellAbility.GetParameterNum(a);
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
