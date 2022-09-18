using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellAbility
{
    public SpellAbility() { }

    public SpellAbility(Condition conditionType, ParaData conditionData, 
        Ability abilityType, ParaData abilityData)
    {
        ConditionType = conditionType;
        ConditionData = conditionData;
        AbilityType = abilityType;
        AbilityData = abilityData;
    }

    public enum Condition
    {
        조건없음,
        선택,
        //data1 직업데이터(0 드루이드 1 도적 2 중립)
        //data2 선택카드번호
        연계,       
        연계시_작동안함,
        피해입지않은하수인,
        버그
    }

    public static Condition GetCondition(string str)
    {
        Condition condition = (Condition)System.Enum.Parse(typeof(Condition), str);
        return condition;
    }

    public static int GetParameterNum(Condition a)
    {
        switch (a)
        {
            case Condition.선택:
                return 2;
        }
        return 0;
    }

    public static bool CheckDataCondition(Condition a)
    {
        return GetParameterNum(a) > 0;
    }

    public Condition ConditionType;
    //상세 데이터
    public ParaData ConditionData;

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public enum Ability
    {
        하수인에게_피해주기, 
        영웅에게_피해주기, 
        피해주기, 
        영웅의공격력만큼_피해주기, 
        피해받지않은하수인에게_피해주기, 
        모든_하수인_피해주기,
        적영웅에게_피해주기,
        다른모든_적군하수인_피해주기, 
        모든_적군하수인_피해주기, 
        다른모든_적군에게_피해주기,
        적에게피해주기,
        모든_아군하수인_피해주기,

        돌진부여,
        도발부여,
        은신부여,
        빙결시키기,
        침묵시키기,
        능력부여,
        대상이_양옆하수인을_공격,
        하수인소환,
        모든하수인에게_은신부여,
        모든하수인에게_능력부여,
        카드뽑기,//카드뽑는수 //뽑는대상(자신(0),적(1))
        방어도얻기,
        영웅공격력얻기,
        무기에_공격력부여,
        무기장착,//무기의번호
        다음카드비용감소, 
        다음주문카드비용감소,
        하수인의_생명력회복,
        하수인의_생명력설정, 
        영웅의_생명력회복, 
        영웅의_생명력설정, 
        생명력회복,
        능력치부여, 
        모든하수인에게_능력치부여,
        해당턴동안_능력치부여, 
        모든하수인에게_해당턴동안_능력치부여,
        대상의_공격력_생명력_교환,
        무기의_공격력만큼능력부여,
        무기의_공격력만큼_모든_적군에게피해,
        무기파괴,
        하수인처치, 
        모든하수인처치,
        아군하수인_주인의패로되돌리기, 
        적군하수인_주인의패로되돌리기, 
        모든_하수인_주인의패로되돌리기,
        하수인_주인의패로되돌리면서_비용감소,
        무작위_패_버리기, 
        무작위_하수인뺏기,
        마나수정획득,
        마나획득,
        다음턴에다시가져오기,
        내손으로다시가져오기,
        버그
    }

    public Ability AbilityType;
    //상세 데이터
    public ParaData AbilityData;

    public static Ability GetAbility(string str)
    {
        Ability ability = (Ability)System.Enum.Parse(typeof(Ability), str);
        return ability;
    }

    public static int GetParameterNum(Ability a)
    {
        switch (a)
        {
            case Ability.하수인에게_피해주기:
            case Ability.적영웅에게_피해주기:
            case Ability.영웅에게_피해주기:
            case Ability.피해주기:
            case Ability.다른모든_적군하수인_피해주기:
            case Ability.모든_적군하수인_피해주기:
            case Ability.모든_아군하수인_피해주기:
            case Ability.모든_하수인_피해주기:
            case Ability.방어도얻기:
            case Ability.영웅공격력얻기:
            case Ability.생명력회복:
            case Ability.하수인의_생명력회복:
            case Ability.하수인의_생명력설정:
            case Ability.영웅의_생명력회복:
            case Ability.영웅의_생명력설정:
            case Ability.다른모든_적군에게_피해주기:
            case Ability.적에게피해주기:
            case Ability.무기에_공격력부여:
            case Ability.다음카드비용감소:
            case Ability.다음주문카드비용감소:
            case Ability.마나획득:
            case Ability.마나수정획득:
            case Ability.하수인_주인의패로되돌리면서_비용감소:
                return 1;
            case Ability.능력부여:
            case Ability.모든하수인에게_능력부여:
            case Ability.카드뽑기:
            case Ability.무기장착:
                return 2;
            case Ability.능력치부여:
            case Ability.해당턴동안_능력치부여:
            case Ability.모든하수인에게_능력치부여:
            case Ability.모든하수인에게_해당턴동안_능력치부여:
            case Ability.하수인소환:
                return 3;
            case Ability.도발부여:
            case Ability.하수인처치:
            case Ability.모든_하수인_주인의패로되돌리기:
            case Ability.다음턴에다시가져오기:
            case Ability.모든하수인에게_은신부여:
            case Ability.무기파괴:
            case Ability.무기의_공격력만큼_모든_적군에게피해:
            case Ability.적군하수인_주인의패로되돌리기:
            case Ability.내손으로다시가져오기:
            case Ability.영웅의공격력만큼_피해주기:
            case Ability.대상이_양옆하수인을_공격:
            case Ability.무작위_패_버리기:
                return 0;
            default:
                Debug.Log(a.ToString() + " : 설정값에 등록이안됨!!");
                return -1;
        }
        return -1;
    }

    public static bool CheckDataAbility(Ability a)
    {
        return GetParameterNum(a) > 0;
    }
}
