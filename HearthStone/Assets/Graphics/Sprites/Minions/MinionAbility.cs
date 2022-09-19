using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum 입력 { 조건, 능력, 수치 };

[System.Serializable]
public class MinionAbility
{
    public MinionAbility() { }
    public MinionAbility(Condition condition_type, ParaData condition_data, Ability ability_type, ParaData ability_data)
    {
        Condition_type = condition_type;
        Condition_data = condition_data;
        Ability_type = ability_type;
        Ability_data = ability_data;
    }

    #region[발동 조건]
    public enum Condition
    {
        조건없음,
        전투의함성,
        죽음의메아리,
        선택,
        연계,
        주문시전,
        피해를_받을때마다,
        하수인이_죽을때마다,
        각턴_시작, 
        내턴의_시작, 
        상대턴의_시작,
        각턴_종료, 
        내_턴종료, 
        상대턴의_종료,
        무기장착시,
        조건을_만족하는_하수인선택,
        필드에_하수인이_일정수일때,
        버그
    }

    #region[해당 문자를 [조건데이터]로 바꿔주는 함수입니다.]
    /// <summary>
    /// 해당 문자를 [조건데이터]로 바꿔주는 함수입니다.
    /// </summary>
    public static Condition GetCondition(string str)
    {
        Condition condition = (Condition)System.Enum.Parse(typeof(Condition), str);
        return condition;
    }
    #endregion

    #region[데이터가 필요한 [조건데이터]인지 검사하는 함수입니다.]
    /// <summary>데이터가 필요한 [조건데이터]인지 검사하는 함수입니다. </summary>
    public static bool CheckDataCondition(Condition c)
    {
        return GetParameterNum(c) > 0;
    }
    public static int GetParameterNum(Condition c)
    {
        switch (c)
        {
            case Condition.선택:
                return 2;
            case Condition.조건을_만족하는_하수인선택:
            case Condition.필드에_하수인이_일정수일때:
                return 3;
        }
        return 0;
    }
    #endregion

    public Condition Condition_type;
    //상세 데이터
    public ParaData Condition_data;
    #endregion

    #region[능력 정보]
    public enum Ability
    {
        하수인에게_피해주기, 영웅에게_피해주기, 피해주기, 무작위_피해주기,
        하수인소환,
        돌진,
        도발,
        은신,
        빙결시키기,
        침묵시키기,
        죽음,
        공격불가,
        카드뽑기, 확률_카드뽑기,
        하수인의_생명력회복, 하수인의_생명력설정, 영웅의_생명력회복, 영웅의_생명력설정, 생명력회복,
        능력치를얻음, 능력치부여, 무작위_능력치부여, 해당턴동안_능력치부여,다른모두에게_능력치부여,
        대상의_공격력_생명력_교환,
        연계횟수만큼_능력치획득,
        무기의_공격력만큼_능력치획득,
        하수인처치, 모든하수인처치,
        아군하수인_주인의패로되돌리기, 적군하수인_주인의패로되돌리기, 모든_하수인_주인의패로되돌리기,
        무작위_패_버리기, 무작위_하수인뺏기,
        변신,
        전체_버프,양옆_버프,영구_버프,
        버그
    }

    public Ability Ability_type;
    //상세 데이터
    public ParaData Ability_data;

    #region[해당 문자를 [능력데이터]로 바꿔주는 함수입니다.]
    /// <summary>
    /// 해당 문자를 [능력데이터]로 바꿔주는 함수입니다.
    /// </summary>
    public static Ability GetAbility(string str)
    {
        Ability ability = (Ability)System.Enum.Parse(typeof(Ability), str);
        return ability;
    }
    #endregion

    #region[데이터가 필요한 [능력데이터]인지 검사하는 함수입니다.]
    /// <summary> 데이터가 필요한 [능력데이터]인지 검사하는 함수입니다.</summary>
    public static bool CheckDataAbility(Ability a)
    {
        return GetParameterNum(a) > 0;
    }

    public static int GetParameterNum(Ability a)
    {
        switch (a)
        {
            case Ability.하수인에게_피해주기:
            case Ability.영웅에게_피해주기:
            case Ability.피해주기:
            case Ability.무작위_피해주기:
            case Ability.하수인의_생명력회복:
            case Ability.하수인의_생명력설정:
            case Ability.영웅의_생명력회복:
            case Ability.영웅의_생명력설정:
            case Ability.생명력회복:
            case Ability.무작위_패_버리기:
                return 1;
            case Ability.카드뽑기:
            case Ability.변신:
                return 2;
            case Ability.하수인소환:
            case Ability.확률_카드뽑기:
            case Ability.능력치를얻음:
            case Ability.능력치부여:
            case Ability.무작위_능력치부여:
            case Ability.해당턴동안_능력치부여:
            case Ability.다른모두에게_능력치부여:
            case Ability.연계횟수만큼_능력치획득:
            case Ability.무기의_공격력만큼_능력치획득:
            case Ability.전체_버프:
            case Ability.양옆_버프:
            case Ability.영구_버프:
                return 3;
            case Ability.돌진:
            case Ability.도발:
            case Ability.은신:
            case Ability.빙결시키기:
            case Ability.침묵시키기:
            case Ability.죽음:
            case Ability.공격불가:
            case Ability.대상의_공격력_생명력_교환:
            case Ability.아군하수인_주인의패로되돌리기:
            case Ability.적군하수인_주인의패로되돌리기:
            case Ability.모든_하수인_주인의패로되돌리기:
            case Ability.하수인처치:
            case Ability.모든하수인처치:
            case Ability.무작위_하수인뺏기:
                return 0;
        }
        return 0;
    }
    #endregion


    #endregion
}
