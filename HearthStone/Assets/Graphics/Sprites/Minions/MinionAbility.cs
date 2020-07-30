using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum 입력 { 조건, 능력, 수치 };

[System.Serializable]
public class MinionAbility
{
    #region[발동 조건]
    public enum Condition
    {
        전투의함성,
        죽음의메아리,
        선택,
        연계,
        주문시전,
        피해를_받을때마다,
        하수인이_죽을때마다,
        전체_버프, 양옆_버프, 영구버프,
        각턴_시작, 내턴의_시작, 상대턴의_시작,
        각턴_종료, 자신의_턴종료, 상대턴의_종료,
        무기장착시,
        조건없음,
        조건을_만족하는_하수인선택,
        //data3이 1이면 공격력 -1이면 체력
        //data2이 -1이면 이하   1이면 이상  0이면 같을때
        //data1이 판별수치로 사용됨 //(data1 = 7 data2 = 1 data3 = 1) ==> 공격력 7이상 하수인 선택
        필드에_하수인이_일정수일때,
        //data3이 1이면 자신필드 //  -1이면 상대필드 // 0이면 전체필드
        //data2이 -1이면 이하 //  1이면 이상 // 0이면 같을때
        //data1이 판별수치로 사용됨 //(data1 = 4 data2 = 1 data3 = -1) ==> 상대필드에 하수인이 4이상일때
        버그
    }

    #region[해당 문자를 [조건데이터]로 바꿔주는 함수입니다.]
    /// <summary>
    /// 해당 문자를 [조건데이터]로 바꿔주는 함수입니다.
    /// </summary>
    public static Condition GetCondition(string s)
    {
        for (Condition i = Condition.전투의함성; i <= Condition.버그; i++)
        {
            if (i.ToString().Equals(s))
                return i;
        }
        return Condition.버그;
    }
    #endregion

    #region[데이터가 필요한 [조건데이터]인지 검사하는 함수입니다.]
    /// <summary>
    /// 데이터가 필요한 [조건데이터]인지 검사하는 함수입니다.
    /// </summary>
    public static bool CheckDataCondition(Condition c)
    {
        if (c == Condition.조건을_만족하는_하수인선택)
            return true;
        if (c == Condition.필드에_하수인이_일정수일때)
            return true;
        return false;
    }
    #endregion

    public Condition Condition_type;
    //상세 데이터
    public Vector3 Condition_data;
    #endregion

    #region[능력 정보]
    public enum Ability
    {
        하수인에게_피해주기, 영웅에게_피해주기, 피해주기,
        하수인소환,
        돌진,
        도발,
        은신,
        빙결시키기,
        침묵시키기,
        공격불가,
        카드뽑기, 확률_카드뽑기,
        하수인의_생명력회복, 하수인의_생명력설정, 영웅의_생명력회복, 영웅의_생명력설정, 생명력회복,
        능력치를얻음, 능력치부여, 무작위_능력치부여, 해당턴동안_능력치부여,
        대상의_공격력_생명력_교환,
        무기의_공격력만큼능력부여,
        하수인처치, 모든하수인처치,
        아군하수인_주인의패로되돌리기, 적군하수인_주인의패로되돌리기, 모든_하수인_주인의패로되돌리기,
        무작위_패_버리기,
        버그
    }

    public Ability Ability_type;
    //상세 데이터
    public Vector3 Ability_data;

    #region[해당 문자를 [능력데이터]로 바꿔주는 함수입니다.]
    /// <summary>
    /// 해당 문자를 [능력데이터]로 바꿔주는 함수입니다.
    /// </summary>
    public static Ability GetAbility(string s)
    {
        for (Ability i = Ability.하수인에게_피해주기; i <= Ability.버그; i++)
        {
            if (i.ToString().Equals(s))
                return i;
        }
        return Ability.버그;
    }
    #endregion

    #region[데이터가 필요한 [능력데이터]인지 검사하는 함수입니다.]
    /// <summary>
    /// 데이터가 필요한 [능력데이터]인지 검사하는 함수입니다.
    /// </summary>
    public static bool CheckDataAbility(Ability a)
    {
        if (a == Ability.하수인에게_피해주기)
            return true;
        if (a == Ability.영웅에게_피해주기)
            return true;
        if (a == Ability.피해주기)
            return true;
        if (a == Ability.하수인소환)
            return true;
        if (a == Ability.카드뽑기)
            return true;
        if (a == Ability.확률_카드뽑기)
            return true;
        if (a == Ability.무작위_패_버리기)
            return true;
        if (a == Ability.하수인의_생명력회복)
            return true;
        if (a == Ability.하수인의_생명력설정)
            return true;
        if (a == Ability.영웅의_생명력회복)
            return true;
        if (a == Ability.영웅의_생명력설정)
            return true;
        if (a == Ability.생명력회복)
            return true;
        if (a == Ability.하수인에게_피해주기)
            return true;
        if (a == Ability.능력치를얻음)
            return true;
        if (a == Ability.능력치부여)
            return true;
        if (a == Ability.무작위_능력치부여)
            return true;
        if (a == Ability.해당턴동안_능력치부여)
            return true;
        return false;
    }
    #endregion

    #region[ActAbillity]
    public void ActAbillity()
    {
        switch (Ability_type)
        {
            case Ability.하수인에게_피해주기:
                //하수인선택로직
                //data1만큼피해
                break;
            case Ability.영웅에게_피해주기:
                //영웅선택로직
                //data1만큼피해
                break;
            case Ability.피해주기:
                //대상선택로직
                //data1만큼피해
                break;
            case Ability.하수인소환:
                //하수인소환로직
                //data1테이블의 data2번 하수인소환
                break;
            case Ability.돌진:
                //돌진로직
                break;
            case Ability.도발:
                //도발로직
                break;
            case Ability.은신:
                //은신로직
                break;
            case Ability.빙결시키기:
                //대상선택로직
                //빙결로직
                break;
            case Ability.침묵시키기:
                //대상선택로직
                //침묵로직
                break;
            case Ability.공격불가:
                //공격불가로직
                break;
            case Ability.카드뽑기:
                //data1만큼 카드를 뽑음
                break;
            case Ability.확률_카드뽑기:
                //data1의 확률로
                //data2만큼 카드를 뽑음
                break;
            case Ability.하수인의_생명력회복:
                //하수인선택로직
                //data1만큼 회복
                break;
            case Ability.영웅의_생명력회복:
                //영웅선택로직
                //data1만큼 회복
                break;
            case Ability.생명력회복:
                //대상선택로직
                //data1만큼 회복
                break;
            case Ability.능력치를얻음:
                //data1의 체력부여
                //data2의 공격력부여
                //data3의 주문공격력부여
                break;
            case Ability.능력치부여:
                //하수인선택로직
                //data1의 체력부여
                //data2의 공격력부여
                //data3의 주문공격력부여
                break;
            case Ability.무작위_능력치부여:
                //무작위하수인선택로직
                //data1의 체력부여
                //data2의 공격력부여
                //data3의 주문공격력부여
                break;
            case Ability.해당턴동안_능력치부여:
                //하수인선택로직
                //data1의 체력부여
                //data2의 공격력부여
                //data3의 주문공격력부여
                //턴종료시 능력치부여한게 사라지게 설정
                break;
            case Ability.대상의_공격력_생명력_교환:
                //하수인선택로직
                //공격력_생명력_교환
                break;
            case Ability.무기의_공격력만큼능력부여:
                //하수인선택로직
                //무기의 공격력만큼 공격력부여
                break;
            case Ability.아군하수인_주인의패로되돌리기:
                //아군하수인선택로직
                //주인손으로돌아가는로직
                break;
            case Ability.적군하수인_주인의패로되돌리기:
                //적군하수인선택로직
                //주인손으로돌아가는로직
                break;
            case Ability.모든_하수인_주인의패로되돌리기:
                //모든하수인선택로직
                //주인손으로돌아가는로직
                break;
            case Ability.하수인처치:
                //하수인선택로직
                //하수인처치로직
                break;
            case Ability.모든하수인처치:
                //모든하수인선택로직
                //하수인처치로직
                break;
            case Ability.무작위_패_버리기:
                //무작위패를 data1만큼선택
                //패버리기로직
                break;
        }
        #endregion

    }
    #endregion
}
