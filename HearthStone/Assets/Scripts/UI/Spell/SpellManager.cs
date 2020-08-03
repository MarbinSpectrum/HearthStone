using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager instance;

    public void Awake()
    {
        instance = this;
    }

    public void RunSpell(string name)
    {
        Queue<SpellAbility> spellQueue = new Queue<SpellAbility>();

        List<SpellAbility> abilityList = new List<SpellAbility>();
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(name));
        string ability_string = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");

        #region[스펠 능력 파싱]
        char[] splitChar = { '[', ']' };
        string[] ability_Split = ability_string.Split(splitChar);
        List<string> ability_Data = new List<string>();
        for (int i = 0; i < ability_Split.Length; i++)
            if (ability_Split[i].Length != 0)
                ability_Data.Add(ability_Split[i]);

        입력 inputType = 입력.조건;
        SpellAbility temp = null;
        int dataNum = 0;
        for (int i = 0; i < ability_Data.Count; i++)
        {
            if (inputType == 입력.조건)
            {
                temp = new SpellAbility();

                SpellAbility.Condition condition = SpellAbility.GetCondition(ability_Data[i]);
                temp.Condition_type = condition;

                if (SpellAbility.CheckDataCondition(condition))
                {
                    inputType = 입력.수치;
                    dataNum = 6;
                }
                else
                    inputType = 입력.능력;
            }
            else if (inputType == 입력.능력)
            {
                SpellAbility.Ability ability = SpellAbility.GetAbility(ability_Data[i]);
                temp.Ability_type = ability;

                if (SpellAbility.CheckDataAbility(ability))
                {
                    inputType = 입력.수치;
                    dataNum = 3;
                }
                else
                {
                    inputType = 입력.조건;
                    abilityList.Add(temp);
                }
            }
            else if (inputType == 입력.수치)
            {
                int value = 0;
                int.TryParse(ability_Data[i], out value);
                switch (dataNum)
                {
                    case 1:
                        temp.Ability_data = new Vector3(temp.Ability_data.x, temp.Ability_data.y, value);
                        break;
                    case 2:
                        temp.Ability_data = new Vector3(temp.Ability_data.x, value, temp.Ability_data.z);
                        break;
                    case 3:
                        temp.Ability_data = new Vector3(value, temp.Ability_data.y, temp.Ability_data.z);
                        break;
                    case 4:
                        temp.Condition_data = new Vector3(temp.Condition_data.x, temp.Condition_data.y, value);
                        break;
                    case 5:
                        temp.Condition_data = new Vector3(temp.Condition_data.x, value, temp.Condition_data.z);
                        break;
                    case 6:
                        temp.Condition_data = new Vector3(value, temp.Condition_data.y, temp.Condition_data.z);
                        break;
                }
                dataNum--;
                if (dataNum == 0)
                {
                    inputType = 입력.조건;
                    abilityList.Add(temp);
                }
                else if (dataNum == 3)
                    inputType = 입력.능력;
            }
        }

        #endregion

        for (int i = 0; i < abilityList.Count; i++)
            spellQueue.Enqueue(abilityList[i]);



    }

}
