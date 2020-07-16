using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        피해받은상태, 피해를_받을때마다,
        하수인이_죽을때마다,
        전체_버프, 양옆_버프, 영구버프,
        각턴_시작, 내턴의_시작, 상대턴의_시작,
        각턴_종료, 자신의_턴종료, 상대턴의_종료,
        무기장착시,
        조건을만족하는하수인선택,
        상대필드에하수인이_일정수이상일때
    }

    public Condition type;
    //상세 데이터
    public Vector3 data;
    #endregion

    #region[능력 정보]
    [System.Serializable]
    public class Ability_Data
    {
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
            하수인의_생명력회복, 영웅의_생명력회복, 생명력회복,
            능력치부여, 무작위_능력치부여, 해당턴동안_능력치부여,
            공격력_생명력_교환,
            무기의_공격력만큼능력부여,
            하수인처치, 모든하수인처치,
            아군하수인_주인의패로되돌리기, 적군하수인_주인의패로되돌리기, 모든_하수인_주인의패로되돌리기,
            무작위_패_버리기
        }

        public Ability type;
        //상세 데이터
        public Vector3 data;

        public void ActAbillity()
        {
            switch (type)
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
                case Ability.공격력_생명력_교환:
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
        }
    }
    #endregion

    //발동될 능력들
    public List<Ability_Data> abilityList = new List<Ability_Data>();
}
