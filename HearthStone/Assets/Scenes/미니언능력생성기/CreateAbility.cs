using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAbility : MonoBehaviour
{
    [SerializeField] private Dropdown condition;
    [SerializeField] private GameObject condition_data;
    [SerializeField] private InputField c_data1;
    [SerializeField] private InputField c_data2;
    [SerializeField] private InputField c_data3;
    [SerializeField] private Dropdown ability;
    [SerializeField] private GameObject ability_data;
    [SerializeField] private InputField a_data1;
    [SerializeField] private InputField a_data2;
    [SerializeField] private InputField a_data3;
    [SerializeField] private InputField outData;
    [SerializeField] private Text explain;
    [SerializeField] private DataMng dataMng;

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

        dataMng.StartLoadData();
    }

    private void Update()
    {
        if (dataMng.dataLoadSuccess == false)
            return;

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

        explain.text = GetEx(c,a);
    }

    public void Change()
    {
        c_data1.text = "0";
        c_data2.text = "0";
        c_data3.text = "0";
        a_data1.text = "0";
        a_data2.text = "0";
        a_data3.text = "0";
    }

    private string GetEx(MinionAbility.Condition condition, MinionAbility.Ability ability)
    {
        string res = "";
        switch (condition)
        {
            case MinionAbility.Condition.조건없음:
                break;
            case MinionAbility.Condition.전투의함성:
                res += "몬스터 소환시";
                break;
            case MinionAbility.Condition.죽음의메아리:
                res += "몬스터 사망시";
                break;
            case MinionAbility.Condition.선택:
                {
                    int data0 = int.Parse(c_data1.text);
                    int data1 = int.Parse(c_data2.text);
                    if(data0 >= dataMng.m_dic.Count)
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
   
                    string v = dataMng.ToString(data0, data1, "카드이름");
                    if (v == string.Empty)
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
                    res += "선택지중 하나입니다.\n";
                    res += ((DataMng.TableType)data0).ToString() + "의 ";
                    res += "\"" + v + "\" 카드가 선택지로 나옵니다.";
                }
                break;
            case MinionAbility.Condition.연계:
                res += "앞서 낸카드가 있을경우";
                break;
            case MinionAbility.Condition.주문시전:
                res += "주문을 시전할때 마다";
                break;
            case MinionAbility.Condition.피해를_받을때마다:
                res += "피해를 받을때 마다";
                break;
            case MinionAbility.Condition.각턴_시작:
                res += "각 플레이어의 턴이 시작할때마다";
                break;
            case MinionAbility.Condition.내턴의_시작:
                res += "플레이어의 턴이 시작할때마다";
                break;
            case MinionAbility.Condition.상대턴의_시작:
                res += "상대 플레이어의 턴이 시작할때마다";
                break;
            case MinionAbility.Condition.각턴_종료:
                res += "각 플레이어의 턴이 종료될때마다";
                break;
            case MinionAbility.Condition.내_턴종료:
                res += "플레이어의 턴이 종료될때마다";
                break;
            case MinionAbility.Condition.상대턴의_종료:
                res += "상대 플레이어의 턴이 종료될때마다";
                break;
            case MinionAbility.Condition.무기장착시:
                res += "무기 장착마다";
                break;
            case MinionAbility.Condition.조건을_만족하는_하수인선택:
                {
                    int data0 = int.Parse(c_data1.text);
                    int data1 = int.Parse(c_data2.text);
                    int data2 = int.Parse(c_data3.text);
                    if(data2 == 1)
                    {
                        res += "공격력이 "; 
                    }
                    else if (data2 == 2)
                    {
                        res += "체력이 ";
                    }
                    else if (data2 == 3)
                    {
                        res += "도발능력이 있는 하수인만  선택합니다.";
                        break;
                    }
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
                    if (data1 == -1)
                        res += data0 + " 이하인 하수인을 선택합니다.";
                    else if (data1 == 0)
                        res += data0 + "인 하수인을 선택합니다.";
                    else if (data1 == +1)
                        res += data0 + " 이상인 하수인을 선택합니다.";
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
                }
                break;
            case MinionAbility.Condition.필드에_하수인이_일정수일때:
                {
                    int data0 = int.Parse(c_data1.text);
                    int data1 = int.Parse(c_data2.text);
                    int data2 = int.Parse(c_data3.text);
                    if (data2 == -1)
                    {
                        res += "상대필드의 ";
                    }
                    else if (data2 == 0)
                    {
                        res += "전체필드의 ";
                    }
                    else if (data2 == 1)
                    {
                        res += "자신필드의 ";
                    }
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    if (data1 == -1)
                    {
                        res += "하수인 수가" + data0 + " 이하인 경우";
                    }
                    else if (data1 == 0)
                    {
                        res += "하수인 수가 " + data0 + "인 경우";
                    }
                    else if (data1 == +1)
                    {
                        res += "하수인 수가 " + data0 + " 이상인 경우";
                    }
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
                }
                break;
        }
        if(condition != MinionAbility.Condition.조건없음)
        res += "\n";

        switch (ability)
        {
            case MinionAbility.Ability.하수인에게_피해주기:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "하수인을 선택하고 " + data0 + "만큼 대상에게 피해";
                }
                break;
            case MinionAbility.Ability.영웅에게_피해주기:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "영웅을 선택하고 " + data0 + "만큼 대상에게 피해";
                }
                break;
            case MinionAbility.Ability.피해주기:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "대상을 선택하고 " + data0 + "만큼 대상에게 피해";
                }
                break;
            case MinionAbility.Ability.무작위_피해주기:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "무작위 적에게 " + data0 + "만큼 피해";
                }
                break;
            case MinionAbility.Ability.하수인소환:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    if(data2 == 0)
                    {
                        res += "상대필드에 ";
                    }
                    else if (data2 == 0)
                    {
                        res += "자신필드에 ";
                    }
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    string v = dataMng.ToString(data0, data1, "카드이름");
                    string t = dataMng.ToString(data0, data1, "카드종류");
                    if (v == string.Empty || t != "하수인")
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }
 
                    res += "\"" + v + "\" 카드가 소환됩니다.";
                }
                break;
            case MinionAbility.Ability.돌진:
                {
                    res += "돌진을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.도발:
                {
                    res += "도발을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.은신:
                {
                    res += "은신을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.빙결시키기:
                {
                    res += "대상을 선택후 선택대상을 빙결시킵니다.";
                }
                break;
            case MinionAbility.Ability.침묵시키기:
                {
                    res += "대상을 선택후 선택대상을 침묵시킵니다.";
                }
                break;
            case MinionAbility.Ability.죽음:
                {
                    res += "파괴됩니다.";
                }
                break;
            case MinionAbility.Ability.공격불가:
                {
                    res += "공격할 수 없는 상태가 됩니다.";
                }
                break;
            case MinionAbility.Ability.카드뽑기:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    if (data1 == 0)
                        res += "플레이어가 ";
                    else if (data1 == 1)
                        res += "상대플레이어가 ";
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    res += data0 + "만큼 카드를 뽑습니다.";             
                }
                break;
            case MinionAbility.Ability.확률_카드뽑기:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    if (data1 == 0)
                        res += "플레이어가 ";
                    else if (data1 == 1)
                        res += "상대플레이어가 ";
                    else
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    if(data2 < 0 || data2 > 100)
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    res += data2 + "% 확률로 " + data0 + "만큼 카드를 뽑습니다.";
                }
                break;
            case MinionAbility.Ability.하수인의_생명력회복:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "하수인을 선택후 " + data0 + "만큼 체력을 회복시킵니다.";
                }
                break;
            case MinionAbility.Ability.하수인의_생명력설정:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "하수인을 선택후 " + data0 + "로 체력을 설정합니다.";
                }
                break;
            case MinionAbility.Ability.영웅의_생명력회복:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "영웅을 선택후 " + data0 + "만큼 체력을 회복시킵니다.";
                }
                break;
            case MinionAbility.Ability.영웅의_생명력설정:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "영웅을 선택후 " + data0 + "로 체력을 설정합니다.";
                }
                break;
            case MinionAbility.Ability.생명력회복:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "대상을 선택후 " + data0 + "만큼 체력을 회복시킵니다.";
                }
                break;
            case MinionAbility.Ability.능력치를얻음:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.능력치부여:
                {
                    res += "대상을 선택후 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 부여합니다.";
                }
                break;
            case MinionAbility.Ability.무작위_능력치부여:
                {
                    res += "무작위 아군 하수인에게 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 부여합니다.";
                }
                break;
            case MinionAbility.Ability.해당턴동안_능력치부여:
                {
                    res += "대상을 선택후 해당턴동안 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 부여합니다.";
                }
                break;
            case MinionAbility.Ability.다른모두에게_능력치부여:
                {
                    res += "대상을 선택후 해당 대상을 제외한 모든 하수인에게 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 부여합니다.";
                }
                break;
            case MinionAbility.Ability.연계횟수만큼_능력치획득:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += "(" + data0 + "*연계횟수)만큼 공격력, ";
                    res += "(" + data1 + "*연계횟수)만큼 체력, ";
                    res += "(" + data2 + "*연계횟수)만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
                {
                    res += "대상을 선택후 해당 대상의 공격력과 체력을 바꿉니다.";
                }
                break;
            case MinionAbility.Ability.무기의_공격력만큼_능력치획득:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += "(" + data0 + "*무기공격력)만큼 공격력, ";
                    res += "(" + data1 + "*무기공격력)만큼 체력, ";
                    res += "(" + data2 + "*무기공격력)만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                {
                    res += "아군 하수인을 선택후 주인의 패로 되돌립니다.";
                }
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                {
                    res += "아군 하수인을 선택후 주인의 패로 되돌립니다.";
                }
                break;
            case MinionAbility.Ability.모든_하수인_주인의패로되돌리기:
                {
                    res += "모든 하수인을 주인의 패로 되돌립니다.";
                }
                break;
            case MinionAbility.Ability.하수인처치:
                {
                    res += "선택한 하수인을 처치합니다.";
                }
                break;
            case MinionAbility.Ability.모든하수인처치:
                {
                    res += "모든 하수인을 처치합니다.";
                }
                break;
            case MinionAbility.Ability.무작위_패_버리기:
                {
                    int data0 = int.Parse(a_data1.text);
                    res += "자신의 무작위 패를 " + data0 + "만큼 버립니다.";
                }
                break;
            case MinionAbility.Ability.무작위_하수인뺏기:
                {
                    res += "상대의 하수인을 뺏습니다.";
                }
                break;
            case MinionAbility.Ability.전체_버프:
                {
                    res += "해당 하수인이 존재하는한 아군 하수인들이 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.양옆_버프:
                {
                    res += "해당 하수인이 존재하는한 하수인의 양옆 하수인들이 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.영구_버프:
                {
                    res += "아군 하수인들이 ";
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);
                    int data2 = int.Parse(a_data3.text);
                    res += data0 + "만큼 공격력, ";
                    res += data1 + "만큼 체력, ";
                    res += data2 + "만큼 주문공격력을 얻습니다.";
                }
                break;
            case MinionAbility.Ability.변신:
                {
                    int data0 = int.Parse(a_data1.text);
                    int data1 = int.Parse(a_data2.text);

                    string v = dataMng.ToString(data0, data1, "카드이름");
                    string t = dataMng.ToString(data0, data1, "카드종류");
                    if (v == string.Empty || t != "하수인")
                    {
                        res = "잘못된 데이터가 입력되었습니다.";
                        return res;
                    }

                    res += "\"" + v + "\" 로 변십합니다.";
                }
                break;
        }

        return res;
    }
}
