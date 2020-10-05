using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    public static MinionManager instance;

    [HideInInspector] public List<MinionObject> minionList = new List<MinionObject>();

    public List<Material> minionMat = new List<Material>();
    public Dictionary<string, Material> minionMaterial = new Dictionary<string, Material>(); 

    #region[Awake]
    void Awake()
    {
        //minionList.Clear();
        if (instance == null)
        {
            instance = this;
            for (int i = 0; i < minionMat.Count; i++)
                minionMaterial.Add(minionMat[i].name, minionMat[i]);
            //DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
    }
    #endregion

    #region[Update]
    public void Update()
    {
        MinionsBuffUpdate();
    }
    #endregion

    #region[하수인 능력 파싱]
    public List<MinionAbility> MinionAbilityParsing(string ability_string)
    {
        List<MinionAbility> abilityList = new List<MinionAbility>();
        char[] splitChar = { '[', ']' };
        string[] ability_Split = ability_string.Split(splitChar);
        List<string> ability_Data = new List<string>();
        for (int i = 0; i < ability_Split.Length; i++)
            if (ability_Split[i].Length != 0)
                ability_Data.Add(ability_Split[i]);

        입력 inputType = 입력.조건;
        MinionAbility temp = null;
        int dataNum = 0;
        for (int i = 0; i < ability_Data.Count; i++)
        {
            if (inputType == 입력.조건)
            {
                temp = new MinionAbility();

                MinionAbility.Condition condition = MinionAbility.GetCondition(ability_Data[i]);
                temp.Condition_type = condition;

                if (MinionAbility.CheckDataCondition(condition))
                {
                    inputType = 입력.수치;
                    dataNum = 6;
                }
                else
                    inputType = 입력.능력;
            }
            else if (inputType == 입력.능력)
            {
                MinionAbility.Ability ability = MinionAbility.GetAbility(ability_Data[i]);
                temp.Ability_type = ability;

                if (MinionAbility.CheckDataAbility(ability))
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

        return abilityList;
    }
    #endregion

    #region[미니언 턴 시작시 처리]
    public void MinionsTurnStartTrigger(bool enemy)
    {
        for (int i = 0; i < minionList.Count; i++)
            if(minionList[i].gameObject.activeSelf && minionList[i].enemy == enemy)
                minionList[i].turnStartTrigger = true;

        for (int i = 0; i < minionList.Count; i++)
            if (minionList[i].gameObject.activeSelf)
                TurnStartMinionAbility(minionList[i], enemy);
    }
    #endregion

    #region[미니언 턴 종료시 처리]
    public void MinionsTurnEndTrigger(bool enemy)
    {
        for (int i = 0; i < minionList.Count; i++)
            if (minionList[i].gameObject.activeSelf && minionList[i].enemy == enemy)
                minionList[i].turnEndTrigger = true;

        for (int i = 0; i < minionList.Count; i++)
            if (minionList[i].gameObject.activeSelf)
                TurnEndMinionAbility(minionList[i], enemy);
    }
    #endregion

    #region[도발하수인 검사]
    public bool CheckTaunt(MinionObject minionObject)
    {
        //본인이 도발 하수인이면 공격가능
        if (minionObject.taunt)
            return true;

        //본인이 도발 하수인이 아닌데 필드에 도발 하수인이 있어서 공격불가능
        for (int k = 0; k < minionList.Count; k++)
            if (minionList[k].enemy == minionObject.enemy)
                if(minionObject.num != minionList[k].num)
                    if (minionList[k].taunt)
                        return false;

        //본인이 도발 하수인이 아니고 필드에 도발 하수인이 없기에 공격가능
        return true;
    }

    public bool CheckTaunt(bool enemy)
    {
        //도발 하수인이 있어서 공격불가능
        for (int k = 0; k < minionList.Count; k++)
            if (minionList[k].enemy == enemy)
                    if (minionList[k].taunt)
                        return false;

        //도발 하수인이 없기에 공격가능
        return true;
    }
    #endregion

    #region[버프를 주는 하수인 처리(최종적인 스텟을 만들어주니 가장 마지막에 두세요)]
    public void MinionsBuffUpdate()
    {
        for (int k = 0; k < minionList.Count; k++)
        {
            Vector3 AddStat = Vector3.zero;
            for (int i = 0; i < minionList[k].buffList.Count; i++)
                AddStat += (Vector3)minionList[k].buffList[i];

            for (int i = 0; i < minionList.Count; i++)
            {
                if (minionList[k].enemy == minionList[i].enemy)
                {
                    for (int j = 0; j < minionList[i].abilityList.Count; j++)
                    {
                        if (minionList[i].abilityList[j].Condition_type == MinionAbility.Condition.양옆_버프)
                        {
                            if (minionList[i].abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                            {
                                int buffObjectIndex = minionList[i].num;
                                if (buffObjectIndex - 1 == minionList[k].num || buffObjectIndex + 1 == minionList[k].num)
                                    AddStat += minionList[i].abilityList[j].Ability_data;
                            }
                        }
                        else if (minionList[i].abilityList[j].Condition_type == MinionAbility.Condition.전체_버프)
                        {
                            if (minionList[i].abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                            {
                                int buffObjectIndex = minionList[i].num;
                                if (buffObjectIndex != minionList[k].num)
                                    AddStat += minionList[i].abilityList[j].Ability_data;
                            }
                        }
                    }
                }
            }
            minionList[k].final_atk = minionList[k].nowAtk + (int)AddStat.x;
           // minionList[k].final_hp = minionList[k].nowHp + (int)AddStat.y;
            minionList[k].final_spellAtk = minionList[k].nowSpell + (int)AddStat.z;
        }
    }
    #endregion

    #region[기본 하수인 효과]
    public void BaseMinionAbility(MinionObject minionObject)
    {
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.조건없음)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.공격불가)
                    minionObject.canAttack = false;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.baseAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    minionObject.taunt = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    minionObject.sleep = false;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
            }
        }


    }
    #endregion

    #region[하수인 소환시 효과]
    public List<int> BattlecryEventList = new List<int>();
    [HideInInspector] public MinionObject eventMininon;
    [HideInInspector] public int eventNum;

    public List<int> ChooseOneEventList = new List<int>();

    public void SpawnMinionAbility(MinionObject minionObject)
    {
        //전투의 함성 및 연계

        BattlecryEventList.Clear();

        #region[대상선택이 필요한 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 ||
                minionObject.abilityList[j].Condition_type == MinionAbility.Condition.조건을_만족하는_하수인선택 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.빙결시키기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.생명력회복)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.아군하수인_주인의패로되돌리기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅에게_피해주기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력회복)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력설정)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.적군하수인_주인의패로되돌리기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.침묵시키기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.해당턴동안_능력치부여)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인처치)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력회복)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력설정)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.피해주기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인에게_피해주기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.대상의_공격력_생명력_교환)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                    BattlecryEventList.Add(j);
            }
        }
        #endregion

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 ||
                minionObject.abilityList[j].Condition_type == MinionAbility.Condition.필드에_하수인이_일정수일때 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.연계횟수만큼능력치부여)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.모든하수인처치)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.변신)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.다른모두에게_능력치부여)
                    BattlecryEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무기의_공격력만큼능력부여)
                    BattlecryEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 || 
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    BattlecryEventList.Add(j);
            }
        }

        #endregion

        StartCoroutine(BattlecryEvent(minionObject));

        ChooseOneEventList.Clear();

        #region[대상선택이 필요한 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.선택)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.빙결시키기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.생명력회복)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.아군하수인_주인의패로되돌리기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅에게_피해주기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력회복)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력설정)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.적군하수인_주인의패로되돌리기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.침묵시키기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.해당턴동안_능력치부여)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인처치)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력회복)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력설정)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.피해주기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인에게_피해주기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.대상의_공격력_생명력_교환)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                    ChooseOneEventList.Add(j);
            }
        }
        #endregion

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.선택)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.모든하수인처치)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.변신)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    ChooseOneEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.다른모두에게_능력치부여)
                    ChooseOneEventList.Add(j);

            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.선택)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    ChooseOneEventList.Add(j);
            }
        }

        #endregion
        
        if(ChooseOneEventList.Count > 0)
            StartCoroutine(ChooseOneEvent(minionObject));
    }
    #endregion

    #region[연계인지 검사]
    bool CheckCombo()
    {
        return CardHand.instance.useCardNum > 1;
    }
    #endregion

    #region[조건을 만족하는 하수인인지 검사]
    public bool CheckConditionMinion(MinionObject targetMinion,MinionObject conditionMinion,int abilityNum)
    {
        if (conditionMinion.abilityList[abilityNum].Condition_type != MinionAbility.Condition.조건을_만족하는_하수인선택)
            return true;

        #region[공격력이 조건을 만족하는지 검사]
        if (conditionMinion.abilityList[abilityNum].Condition_data.z == 1)
        {
            if (conditionMinion.abilityList[abilityNum].Condition_data.y > 0 && targetMinion.final_atk >= conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
            else if (conditionMinion.abilityList[abilityNum].Condition_data.y < 0 && targetMinion.final_atk <= conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
            else if (conditionMinion.abilityList[abilityNum].Condition_data.y == 0 && targetMinion.final_atk == conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
        }
        #endregion

        #region[생명력이 조건을 만족하는지 검사]
        else if (conditionMinion.abilityList[abilityNum].Condition_data.z == 2)
        {
            if (conditionMinion.abilityList[abilityNum].Condition_data.y > 0 && targetMinion.final_hp >= conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
            else if (conditionMinion.abilityList[abilityNum].Condition_data.y < 0 && targetMinion.final_hp <= conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
            else if (conditionMinion.abilityList[abilityNum].Condition_data.y == 0 && targetMinion.final_hp == conditionMinion.abilityList[abilityNum].Condition_data.x)
                return true;
        }
        #endregion

        #region[도발하수인인지 검사]
        else if (conditionMinion.abilityList[abilityNum].Condition_data.z == 3)
        {
            if (targetMinion.taunt)
                return true;
        }
        #endregion

        return false;
    }
    #endregion

    [HideInInspector] public bool selectMinionEvent;

    #region[전투의 함성 이벤트]
    private IEnumerator BattlecryEvent(MinionObject minionObject)
    {
        int minionNum = 0;
        for (int i = 0; i < BattlecryEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = BattlecryEventList[i];
            int NowEvent = 0;

            //이벤트 분류

            #region[대상선택 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.빙결시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.아군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.적군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력설정)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.침묵시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.해당턴동안_능력치부여)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인처치)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력설정)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.대상의_공격력_생명력_교환)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                NowEvent = 1;
            #endregion

            #region[자가버프 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무기의_공격력만큼능력부여)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.연계횟수만큼능력치부여)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                NowEvent = 4;
            #endregion

            #region[필드하수인 모두 파괴]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.모든하수인처치)
                NowEvent = 5;
            #endregion

            #region[무작위 하수인뺏기]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                NowEvent = 6;
            #endregion

            #region[변신]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.변신)
                NowEvent = 7;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                SetSelectMask(minionObject.abilityList[j].Ability_type);

                bool targetExistence = false;

                for (int m = 0; m < minionList.Count; m++)
                    if ((DragLineRenderer.instance.CheckMask(타겟.아군하수인) && !minionList[m].enemy) ||
                        (DragLineRenderer.instance.CheckMask(타겟.적하수인) && minionList[m].enemy))
                        if (minionList[m].gameObject.activeSelf && !minionList[m].Equals(minionObject))
                            targetExistence = targetExistence || CheckConditionMinion(minionList[m], minionObject, j);

                if (DragLineRenderer.instance.CheckMask(타겟.아군영웅))
                    targetExistence = true;
                if (DragLineRenderer.instance.CheckMask(타겟.적영웅))
                    targetExistence = true;
                if (DragLineRenderer.instance.CheckMask(타겟.실행주체))
                    targetExistence = true;

                if (targetExistence)
                {
                    if(minionObject.enemy)
                    {
                        eventMininon = minionObject;
                        eventNum = j;
                        DruidAI.instance.AI_Select(eventMininon.abilityList[eventNum].Ability_type);
                    }
                    else
                    {
                        SoundManager.instance.MuteBGM(true);

                        if (DragLineRenderer.instance.CheckMask(타겟.아군영웅) || DragLineRenderer.instance.CheckMask(타겟.적영웅))
                            CardHand.instance.handAni.SetTrigger("축소");
                        selectMinionEvent = true;
                        eventMininon = minionObject;
                        eventNum = j;
                        BattleUI.instance.grayFilterAni.SetBool("On", true);
                        BattleUI.instance.selectMinion.gameObject.SetActive(true);
                        DragLineRenderer.instance.activeObj = minionObject.gameObject;
                        BattleUI.instance.selectMinionTxt.text = GetText(minionObject.abilityList[j].Ability_type);

                        while (selectMinionEvent)
                        {
                            GameEventManager.instance.EventSet(1f);
                            yield return new WaitForSeconds(0.001f);
                        }
                    }
                }
            }
            else if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무기의_공격력만큼능력부여)
                {
                    if (minionObject.enemy)
                        minionObject.nowAtk += HeroManager.instance.heroAtkManager.enemyWeaponAtk;
                    else
                        minionObject.nowAtk += HeroManager.instance.heroAtkManager.playerWeaponAtk;
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    minionObject.taunt = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    minionObject.sleep = false;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.연계횟수만큼능력치부여)
                {   
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x * (CardHand.instance.useCardNum - 1);
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y * (CardHand.instance.useCardNum - 1);
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z * (CardHand.instance.useCardNum - 1);
                }
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if(minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for (int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                {
                    for (int c = 0; c < minionObject.abilityList[j].Ability_data.x; c++)
                    {
                        if (!minionObject.enemy)
                        {
                            if (CardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, CardHand.instance.nowHandNum);
                            CardHand.instance.CardRemove(r);
                        }
                        else
                        {
                            if (EnemyCardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, EnemyCardHand.instance.nowHandNum);
                            EnemyCardHand.instance.CardRemove(r);
                        }
                    }
                }
            }
            else if (NowEvent == 5)
            {
                for (int m = 0; m < minionList.Count; m++)
                    if (minionList[m].gameObject.activeSelf && !minionList[m].Equals(minionObject))
                        minionList[m].animator.SetTrigger("Death");

                yield return new WaitForSeconds(1.25f);

                GameEventManager.instance.EventSet(2.5f);

                #region[아군하수인효과]
                if (!minionObject.enemy)
                {
                    #region[아군필드상황정리]
                    MinionObject temp = minionObject;
                    List<MinionObject> removeMinionList = new List<MinionObject>();
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].Equals(minionObject))
                            removeMinionList.Add(MinionField.instance.minions[m]);
                    MinionField.instance.minions[0] = temp;

                    int pos = 1;
                    for (int m = 0; m < removeMinionList.Count; m++)
                    {
                        MinionField.instance.minions[pos] = removeMinionList[m];
                        DeathMinionAbility(MinionField.instance.minions[pos]);
                        MinionField.instance.minions[pos].MinionRemoveProcess();
                        pos++;
                    }
                    MinionField.instance.minionNum = 1;
                    removeMinionList.Clear();
                    #endregion

                    #region[적군필드상황정리]
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                    {
                        DeathMinionAbility(EnemyMinionField.instance.minions[m]);
                        EnemyMinionField.instance.minions[m].MinionRemoveProcess();
                    }
                    EnemyMinionField.instance.minionNum = 0;
                    #endregion
                }
                #endregion

                #region[적군하수인효과]
                else
                {
                    #region[적군필드상황정리]
                    MinionObject temp = minionObject;
                    List<MinionObject> removeMinionList = new List<MinionObject>();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].Equals(minionObject))
                            removeMinionList.Add(EnemyMinionField.instance.minions[m]);
                    EnemyMinionField.instance.minions[0] = temp;

                    int pos = 1;
                    for (int m = 0; m < removeMinionList.Count; m++)
                    {
                        EnemyMinionField.instance.minions[pos] = removeMinionList[m];
                        DeathMinionAbility(EnemyMinionField.instance.minions[pos]);
                        EnemyMinionField.instance.minions[pos].MinionRemoveProcess();
                        pos++;
                    }
                    EnemyMinionField.instance.minionNum = 1;
                    removeMinionList.Clear();
                    #endregion

                    #region[아군필드상황정리]
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                    {
                        DeathMinionAbility(MinionField.instance.minions[m]);
                        MinionField.instance.minions[m].MinionRemoveProcess();
                    }
                    MinionField.instance.minionNum = 0;
                    #endregion
                }
                #endregion



            }
            else if (NowEvent == 6)
            {
                if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.필드에_하수인이_일정수일때)
                {
                    bool conditionCheck = false;
                    int targetMinionNum = 0;
                    for (int m = 0; m < minionList.Count; m++)
                    {
                        if(minionList[m].gameObject.activeSelf)
                        {
                            if (minionList[m].enemy && (minionObject.abilityList[j].Condition_data.z == 0 || minionObject.abilityList[j].Condition_data.z == -1))
                                targetMinionNum++;
                            else if (!minionList[m].enemy && (minionObject.abilityList[j].Condition_data.z == 0 || minionObject.abilityList[j].Condition_data.z == 1))
                                targetMinionNum++;
                        }
                    }

                    if (minionObject.abilityList[j].Condition_data.y > 0 && targetMinionNum >= minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;
                    else if (minionObject.abilityList[j].Condition_data.y < 0 && targetMinionNum <= minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;
                    else if (minionObject.abilityList[j].Condition_data.y == 0 && targetMinionNum == minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;

                    if(conditionCheck)
                    {
                        string minion_name = "데스윙";

                        List<MinionObject> randomMinion = new List<MinionObject>();

                        //적군미니언효과이면
                        if (minionObject.enemy)
                        {
                            for (int m = 0; m < MinionField.instance.minions.Length; m++)
                                if (MinionField.instance.minions[m].gameObject.activeSelf && MinionField.instance.minions[m].final_hp > 0)
                                    randomMinion.Add(MinionField.instance.minions[m]);
                            if (EnemyMinionField.instance.minionNum >= 7)
                                randomMinion.Clear();
                        }
                        //아군미니언효과이면
                        else
                        {
                            for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                                if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && EnemyMinionField.instance.minions[m].final_hp > 0)
                                    randomMinion.Add(EnemyMinionField.instance.minions[m]);
                            if (MinionField.instance.minionNum >= 7)
                                randomMinion.Clear();
                        }
                        if (randomMinion.Count > 0)
                        {
                            yield return new WaitForSeconds(1f);
                            if (minionObject.enemy)
                            {
                                if (MinionField.instance.minionNum <= 0)
                                    randomMinion.Clear();
                            }
                            else
                            {
                                if (EnemyMinionField.instance.minionNum <= 0)
                                    randomMinion.Clear();
                            }

                        }

                        if (randomMinion.Count > 0)
                        {
                            GameEventManager.instance.EventSet(1.5f);
                            int r = Random.Range(0, randomMinion.Count);
                            minion_name = randomMinion[r].minion_name;

                            MinionObject tempMinion = randomMinion[r];
                            Vector2 pos = tempMinion.transform.position;

                            if (minionObject.enemy)
                            {
                                int index = EnemyMinionField.instance.minionNum;
                                EnemyMinionField.instance.AddMinion(index, minion_name, false, 1);
                                MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                                MinionField.instance.setMinionPos = false;
                                EnemyMinionField.instance.setMinionPos = false;
                                EnemyMinionField.instance.minions[index].transform.position = pos;
                            }
                            else
                            {
                                int index = MinionField.instance.minionNum;
                                MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false, 1);
                                MinionCopy(randomMinion[r], MinionField.instance.minions[index]);
                                MinionField.instance.setMinionPos = false;
                                EnemyMinionField.instance.setMinionPos = false;
                                MinionField.instance.minions[index].transform.position = pos;
                            }

                            yield return new WaitForSeconds(1.5f);

                            if (minionObject.enemy)
                            {
                                for (int m = randomMinion[r].num; m < MinionField.instance.minions.Length - 1; m++)
                                    MinionField.instance.minions[m] = MinionField.instance.minions[m + 1];
                                MinionField.instance.minions[MinionField.instance.minions.Length - 1] = tempMinion;
                                MinionField.instance.minionNum--;
                            }
                            else
                            {
                                for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                                    EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                                EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = tempMinion;
                                EnemyMinionField.instance.minionNum--;
                            }

                            randomMinion[r].MinionRemoveProcess();

                            MinionField.instance.setMinionPos = true;
                            EnemyMinionField.instance.setMinionPos = true;

                            minionNum++;
                        }
                    }
                }

            }
            else if (NowEvent == 7)
            {
                SetSelectMask(minionObject.abilityList[j].Ability_type);

                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                minionObject.minion_name = minion_name;
                minionObject.InitTrigger = true;
            }
            else if (NowEvent == 8)
            {
                if (minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].Equals(minionObject) && EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            EnemyMinionField.instance.minions[m].nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                            EnemyMinionField.instance.minions[m].final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                            EnemyMinionField.instance.minions[m].nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                        }
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].Equals(minionObject) && MinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            MinionField.instance.minions[m].nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                            MinionField.instance.minions[m].final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                            MinionField.instance.minions[m].nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                        }
                }
            }
            #endregion
        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);
    }
    #endregion

    #region[대상 선택 취소]
    public void MinionSelectCancle()
    {
        if (!selectMinionEvent)
            return;
        SoundManager.instance.MuteBGM(false);
        selectMinionEvent = false;
        eventMininon.gotoHandTrigger = true;
        GameEventManager.instance.EventAdd(1.4f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(eventMininon.minion_name));
        int mana = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
        mana += CardHand.instance.removeCostOffset;
        mana = mana < 0 ? 0 : mana;
        ManaManager.instance.playerNowMana += mana;
        CardHand.instance.handCostOffset[CardHand.instance.nowHandNum] = CardHand.instance.removeCostOffset;
        CardHand.instance.useCardNum--;

    }
    #endregion

    #region[대상 선택시]
    public void MinionSelect(MinionObject minionObject)
    {
        if (!selectMinionEvent)
            return;
        SoundManager.instance.MuteBGM(false);
        selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (eventMininon.abilityList[eventNum].Ability_type)
        {
            case MinionAbility.Ability.빙결시키기:
                invokeMinion = minionObject;
                EffectManager.instance.IceBallEffect(eventMininon.transform.position, invokeMinion.transform.position);
                Invoke("FreezeMinion", 1.25f);
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                minionObject.gotoHandTrigger = true;
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                break;
            case MinionAbility.Ability.침묵시키기:
                invokeMinion = minionObject;
                EffectManager.instance.MagicEffect(invokeMinion.transform.position);
                Invoke("SilenceMinion", 1f);
                break;
            case MinionAbility.Ability.하수인처치:
                if (eventMininon.minion_name == "나 이런 사냥꾼이야")
                {
                    invokeMinion = minionObject;
                    EffectManager.instance.ExplodeEffect(minionObject.transform.position);
                    Invoke("DeathMinion", 0.5f);
                }
                else if (eventMininon.minion_name == "날뛰는 코도")
                {
                    invokeMinion = minionObject;
                    EffectManager.instance.EnergyEffect(eventMininon.transform.position, invokeMinion.transform.position);
                    Invoke("DeathMinion", 0.5f);
                }
                else if (eventMininon.minion_name == "흑기사")
                {
                    invokeMinion = minionObject;
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(-1, 1));
                    Invoke("DeathMinion", 1f);
                }
                else
                    minionObject.MinionDeath();
                break;
            case MinionAbility.Ability.생명력회복:
            case MinionAbility.Ability.하수인의_생명력회복:
                SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
                minionObject.final_hp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                minionObject.final_hp = Mathf.Min(minionObject.final_hp, minionObject.baseHp);
                EffectManager.instance.HealEffect(minionObject.transform.position, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                break;
            case MinionAbility.Ability.피해주기:
            case MinionAbility.Ability.하수인에게_피해주기:
                if (eventMininon.minion_name == "SI7 요원")
                {
                    invokeMinion = minionObject;
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    Invoke("DamageMinion", 1f);
                }
                else
                {
                    invokeMinion = minionObject;
                    DamageMinion();
                }
                break;
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
                invokeMinion = minionObject;
                if (eventMininon.minion_name == "광기의 연금술사")
                {
                    EffectManager.instance.PortionEffect(eventMininon.transform.position, minionObject.transform.position);
                    Invoke("ChangeHpAtk", 1.5f);
                }
                else
                    ChangeHpAtk();
                break;
            case MinionAbility.Ability.해당턴동안_능력치부여:
            case MinionAbility.Ability.능력치부여:
                Vector4 buff = new Vector4((int)eventMininon.abilityList[eventNum].Ability_data.x, (int)eventMininon.abilityList[eventNum].Ability_data.y, (int)eventMininon.abilityList[eventNum].Ability_data.z, 1);
                if(eventMininon.abilityList[eventNum].Ability_type == MinionAbility.Ability.능력치부여)
                    buff -= new Vector4(0, 0, 0, 1);
                minionObject.buffList.Add(buff);
                minionObject.final_hp += (int)eventMininon.abilityList[eventNum].Ability_data.y;
                break;
        }
    }

    MinionObject invokeMinion;

    public void FreezeMinion()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        invokeMinion.freezeTrigger = true;
        invokeMinion = null;
    }

    public void DamageMinion()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        AttackManager.instance.PopAllDamageObj();
        AttackManager.instance.AddDamageObj(invokeMinion.damageEffect, (int)eventMininon.abilityList[eventNum].Ability_data.x);
        AttackManager.instance.AttackEffectRun();
        invokeMinion = null;
    }

    public void SilenceMinion()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        invokeMinion.ActSilence();
        invokeMinion = null;
    }

    public void DeathMinion()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        invokeMinion.MinionDeath();
        invokeMinion = null;
    }

    public void ChangeHpAtk()
    {
        if (invokeMinion == null)
            return;
        int temp = invokeMinion.final_hp;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        invokeMinion.baseHp = invokeMinion.final_atk;
        invokeMinion.final_hp = invokeMinion.final_atk;
        invokeMinion.baseAtk = temp;
        invokeMinion.nowAtk = temp;
        invokeMinion.final_atk = temp;
        for (int i = 0; i < invokeMinion.buffList.Count; i++)
            invokeMinion.buffList[i] = new Vector4(0, invokeMinion.buffList[i].y, invokeMinion.buffList[i].z, invokeMinion.buffList[i].w);
        invokeMinion = null;
    }



    public void HeroSelect(bool enemy)
    {
        if (!selectMinionEvent)
            return;
        SoundManager.instance.MuteBGM(false);
        selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (eventMininon.abilityList[eventNum].Ability_type)
        {
            case MinionAbility.Ability.빙결시키기:
                //1아군 2적
                invokeHero = enemy ? 2 : 1;
                if(enemy)
                    EffectManager.instance.IceBallEffect(eventMininon.transform.position, HeroManager.instance.enemyHero.transform.position);
                else
                    EffectManager.instance.IceBallEffect(eventMininon.transform.position, HeroManager.instance.playerHero.transform.position);
                Invoke("FreezeHero", 1.25f);
                break;
            case MinionAbility.Ability.피해주기:
            case MinionAbility.Ability.영웅에게_피해주기:
                AttackManager.instance.PopAllDamageObj();
                invokeHero = enemy ? 2 : 1;
                if (enemy)
                    EffectManager.instance.CutEffect(HeroManager.instance.enemyHero.transform.position, new Vector2(+1, 1));
                else
                    EffectManager.instance.CutEffect(HeroManager.instance.playerHero.transform.position, new Vector2(+1, 1));
                Invoke("DamageHero", 1f);
                break;
            case MinionAbility.Ability.생명력회복:
            case MinionAbility.Ability.영웅의_생명력회복:
                SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
                if (enemy)
                {
                    HeroManager.instance.heroHpManager.nowEnemyHp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                    EffectManager.instance.HealEffect(HeroManager.instance.enemyHero.transform.position, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                }
                else
                {
                    HeroManager.instance.heroHpManager.nowPlayerHp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                    EffectManager.instance.HealEffect(HeroManager.instance.playerHero.transform.position, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                }
                break;
            case MinionAbility.Ability.영웅의_생명력설정:
                if (enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp = (int)eventMininon.abilityList[eventNum].Ability_data.x;
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp = (int)eventMininon.abilityList[eventNum].Ability_data.x;
                if (eventMininon.minion_name.Equals("알렉스트라자"))
                {
                    List<List<float>> angleList = new List<List<float>>();
                    angleList.Add(new List<float>());
                    angleList.Add(new List<float>() { 0 });
                    angleList.Add(new List<float>() { -10, +10 });
                    angleList.Add(new List<float>() { -20, 0, +20 });
                    angleList.Add(new List<float>() { -30, -10, +10, +30 });
                    angleList.Add(new List<float>() { -40, -20, 0, +20, +40 });
                    angleList.Add(new List<float>() { -45, -30, -10, +10, +30, +45 });
                    angleList.Add(new List<float>() { -40, -20, 0, +20, +40 });
                    float angle;
   
                    if (!enemy)
                    {
                        if (eventMininon.enemy)
                            angle = angleList[EnemyMinionField.instance.minionNum][EnemyMinionField.instance.minionNum - eventMininon.num - 1];
                        else
                            angle = angleList[MinionField.instance.minionNum][MinionField.instance.minionNum - eventMininon.num - 1];
                        angle -= 180;
                    }
                    else
                    {
                        if (eventMininon.enemy)
                            angle = angleList[EnemyMinionField.instance.minionNum][eventMininon.num];
                        else
                            angle = angleList[MinionField.instance.minionNum][eventMininon.num];
                    }
                    EffectManager.instance.DragonBreath(eventMininon.transform.position, angle);
                }
                break;
        }
    }

    int invokeHero = 0;
    public void FreezeHero()
    {
        if (invokeHero == 0)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        if (invokeHero == 1)
            HeroManager.instance.SetFreeze(false);
        else if (invokeHero == 2)
            HeroManager.instance.SetFreeze(true);
        invokeHero = 0;
    }

    public void DamageHero()
    {
        if (invokeHero == 0)
            return;
        SoundManager.instance.PlayMinionSE(eventMininon.minion_name, 미니언상태.효과);
        AttackManager.instance.PopAllDamageObj();
        if (invokeHero == 2)
            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)eventMininon.abilityList[eventNum].Ability_data.x);
        else if (invokeHero == 1)
            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)eventMininon.abilityList[eventNum].Ability_data.x);
        AttackManager.instance.AttackEffectRun();
        invokeHero = 0;
    }

    #endregion

    #region[선택할 객체 설정]
    public void SetSelectMask(MinionAbility.Ability ability)
    {
        switch (ability)
        {
            case MinionAbility.Ability.영웅에게_피해주기:
            case MinionAbility.Ability.영웅의_생명력회복:
            case MinionAbility.Ability.영웅의_생명력설정:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                break;
            case MinionAbility.Ability.빙결시키기:
            case MinionAbility.Ability.생명력회복:
            case MinionAbility.Ability.피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.침묵시키기:
            case MinionAbility.Ability.하수인의_생명력회복:
            case MinionAbility.Ability.하수인에게_피해주기:
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
            case MinionAbility.Ability.해당턴동안_능력치부여:
            case MinionAbility.Ability.능력치부여:
            case MinionAbility.Ability.하수인처치:
            case MinionAbility.Ability.하수인의_생명력설정:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
        }
    }
    #endregion

    #region[하수인 선택 안내문구]
    string GetText(MinionAbility.Ability ability)
    {
        switch(ability)
        {
            case MinionAbility.Ability.빙결시키기:
                return "빙결시킬 하수인 선택";
            case MinionAbility.Ability.생명력회복:
                return "회복시킬 대상 선택";
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                return "패로 되돌리고 싶은 아군하수인 선택";
            case MinionAbility.Ability.영웅에게_피해주기:
                return "피해를 줄 영웅을 선택";
            case MinionAbility.Ability.영웅의_생명력회복:
                return "회복시킬 영웅을 선택";
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                return "패로 되돌리고 싶은 적군하수인 선택";
            case MinionAbility.Ability.침묵시키기:
                return "침묵시킬 하수인 선택";
            case MinionAbility.Ability.하수인의_생명력회복:
                return "회복시킬 하수인 선택";
            case MinionAbility.Ability.하수인에게_피해주기:
                return "피해를 줄 하수인 선택";
            case MinionAbility.Ability.해당턴동안_능력치부여:
            case MinionAbility.Ability.능력치부여:
                return "능력치를 부여할 하수인 선택";
        }
        return "대상선택";
    }
    #endregion


    #region[선택(드루이드) 이벤트]
    [HideInInspector] public int selectChoose = -1;
    private IEnumerator ChooseOneEvent(MinionObject minionObject)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);


        List<Vector2> chooseOneList = new List<Vector2>();

        for (int i = 0; i < ChooseOneEventList.Count; i++)
        {
            int j = ChooseOneEventList[i];
            int chooseEventJobNum = (int)minionObject.abilityList[j].Condition_data.x;
            int chooseEventCardNum = (int)minionObject.abilityList[j].Condition_data.y;
            if (!chooseOneList.Contains(new Vector2(chooseEventJobNum, chooseEventCardNum)))
                chooseOneList.Add(new Vector2(chooseEventJobNum, chooseEventCardNum));
        }

        if (minionObject.enemy)
        {
            selectChoose = DruidAI.instance.AI_ChoiceSelect(minionObject);
        }
        else
        {
            for (int i = 0; i < chooseOneList.Count; i++)
            {
                string ChooseName = DataMng.instance.ToString((DataMng.TableType)chooseOneList[i].x, (int)chooseOneList[i].y, "카드이름");
                CardViewManager.instance.CardShow(ref BattleUI.instance.chooseCardView[i], ChooseName);
                CardViewManager.instance.UpdateCardView(0.001f);
            }

            BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

            eventMininon = minionObject;

            selectChoose = -1;

            while (selectChoose == -1)
            {
                GameEventManager.instance.EventSet(1f);
                yield return new WaitForSeconds(0.001f);
            }
        }
        int minionNum = 0;

        for (int i = 0; i < ChooseOneEventList.Count; i++)
        {
            int nowChoose = (int)chooseOneList[selectChoose].y;
            int NowEvent = 0;
            int j = ChooseOneEventList[i]; 
            
            if (nowChoose != minionObject.abilityList[j].Condition_data.y)
                continue;

            #region[대상선택 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.빙결시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.아군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.적군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력설정)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.침묵시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.해당턴동안_능력치부여)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인처치)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력설정)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.대상의_공격력_생명력_교환)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                NowEvent = 1;
            #endregion

            #region[자가버프 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                NowEvent = 4;
            #endregion

            #region[필드하수인 모두 파괴]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.모든하수인처치)
                NowEvent = 5;
            #endregion

            #region[무작위 하수인뺏기]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                NowEvent = 6;
            #endregion

            #region[변신]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.변신)
                NowEvent = 7;
            #endregion

            #region[다른모두에게 능력치 부여]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.다른모두에게_능력치부여)
                NowEvent = 8;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                SetSelectMask(minionObject.abilityList[j].Ability_type);

                bool targetExistence = false;

                for (int m = 0; m < minionList.Count; m++)
                    if ((DragLineRenderer.instance.CheckMask(타겟.아군하수인) && !minionList[m].enemy) ||
                        (DragLineRenderer.instance.CheckMask(타겟.적하수인) && minionList[m].enemy))
                        if (minionList[m].gameObject.activeSelf && !minionList[m].Equals(minionObject))
                            targetExistence = targetExistence || CheckConditionMinion(minionList[m], minionObject, j);

                if (DragLineRenderer.instance.CheckMask(타겟.아군영웅))
                    targetExistence = true;
                if (DragLineRenderer.instance.CheckMask(타겟.적영웅))
                    targetExistence = true;
                if (DragLineRenderer.instance.CheckMask(타겟.실행주체))
                    targetExistence = true;

                if (targetExistence)
                {
                    if (minionObject.enemy)
                    {
                        eventMininon = minionObject;
                        eventNum = j;
                        DruidAI.instance.AI_Select(eventMininon.abilityList[eventNum].Ability_type);
                    }
                    else
                    {
                        if (DragLineRenderer.instance.CheckMask(타겟.아군영웅) || DragLineRenderer.instance.CheckMask(타겟.적영웅))
                            CardHand.instance.handAni.SetTrigger("축소");
                        selectMinionEvent = true;
                        eventMininon = minionObject;
                        eventNum = j;
                        BattleUI.instance.grayFilterAni.SetBool("On", true);
                        BattleUI.instance.selectMinion.gameObject.SetActive(true);
                        DragLineRenderer.instance.activeObj = minionObject.gameObject;
                        BattleUI.instance.selectMinionTxt.text = GetText(minionObject.abilityList[j].Ability_type);

                        while (selectMinionEvent)
                        {
                            GameEventManager.instance.EventSet(1f);
                            yield return new WaitForSeconds(0.001f);
                        }
                    }
                }
            }
            else if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    minionObject.taunt = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    minionObject.sleep = false;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for(int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_패_버리기)
                {
                    for (int c = 0; c < minionObject.abilityList[j].Ability_data.x; c++)
                    {
                        if (!minionObject.enemy)
                        {
                            if (CardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, CardHand.instance.nowHandNum);
                            CardHand.instance.CardRemove(r);
                        }
                        else
                        {
                            if (EnemyCardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, EnemyCardHand.instance.nowHandNum);
                            EnemyCardHand.instance.CardRemove(r);
                        }
                    }
                }
            }
            else if (NowEvent == 5)
            {
                for (int m = 0; m < minionList.Count; m++)
                    if (minionList[m].gameObject.activeSelf && !minionList[m].Equals(minionObject))
                        minionList[m].animator.SetTrigger("Death");

                yield return new WaitForSeconds(1.25f);

                GameEventManager.instance.EventSet(2.5f);

                #region[아군하수인효과]
                if (!minionObject.enemy)
                {
                    #region[아군필드상황정리]
                    MinionObject temp = minionObject;
                    List<MinionObject> removeMinionList = new List<MinionObject>();
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].Equals(minionObject))
                            removeMinionList.Add(MinionField.instance.minions[m]);
                    MinionField.instance.minions[0] = temp;

                    int pos = 1;
                    for (int m = 0; m < removeMinionList.Count; m++)
                    {
                        MinionField.instance.minions[pos] = removeMinionList[m];
                        DeathMinionAbility(MinionField.instance.minions[pos]);
                        //MinionField.instance.minions[pos].MinionRemoveProcess();
                        pos++;
                    }
                    MinionField.instance.minionNum = 1;
                    removeMinionList.Clear();
                    #endregion

                    #region[적군필드상황정리]
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                    {
                        DeathMinionAbility(EnemyMinionField.instance.minions[m]);
                        EnemyMinionField.instance.minions[m].MinionRemoveProcess();
                    }
                    EnemyMinionField.instance.minionNum = 0;
                    #endregion
                }
                #endregion

                #region[적군하수인효과]
                else
                {
                    #region[적군필드상황정리]
                    MinionObject temp = minionObject;
                    List<MinionObject> removeMinionList = new List<MinionObject>();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].Equals(minionObject))
                            removeMinionList.Add(EnemyMinionField.instance.minions[m]);
                    EnemyMinionField.instance.minions[0] = temp;

                    int pos = 1;
                    for (int m = 0; m < removeMinionList.Count; m++)
                    {
                        EnemyMinionField.instance.minions[pos] = removeMinionList[m];
                        DeathMinionAbility(EnemyMinionField.instance.minions[pos]);
                        EnemyMinionField.instance.minions[pos].MinionRemoveProcess();
                        pos++;
                    }
                    EnemyMinionField.instance.minionNum = 1;
                    removeMinionList.Clear();
                    #endregion

                    #region[아군필드상황정리]
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                    {
                        DeathMinionAbility(MinionField.instance.minions[m]);
                        MinionField.instance.minions[m].MinionRemoveProcess();
                    }
                    MinionField.instance.minionNum = 0;
                    #endregion
                }
                #endregion

            }
            else if (NowEvent == 6)
            {
                if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.필드에_하수인이_일정수일때)
                {
                    bool conditionCheck = false;
                    int targetMinionNum = 0;
                    for (int m = 0; m < minionList.Count; m++)
                    {
                        if (minionList[m].gameObject.activeSelf)
                        {
                            if (minionList[m].enemy && (minionObject.abilityList[j].Condition_data.z == 0 || minionObject.abilityList[j].Condition_data.z == -1))
                                targetMinionNum++;
                            else if (!minionList[m].enemy && (minionObject.abilityList[j].Condition_data.z == 0 || minionObject.abilityList[j].Condition_data.z == 1))
                                targetMinionNum++;
                        }
                    }

                    if (minionObject.abilityList[j].Condition_data.y > 0 && targetMinionNum >= minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;
                    else if (minionObject.abilityList[j].Condition_data.y < 0 && targetMinionNum <= minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;
                    else if (minionObject.abilityList[j].Condition_data.y == 0 && targetMinionNum == minionObject.abilityList[j].Condition_data.x)
                        conditionCheck = true;

                    if (conditionCheck)
                    {
                        string minion_name = "데스윙";

                        List<MinionObject> randomMinion = new List<MinionObject>();

                        if (minionObject.enemy)
                        {
                            for (int m = 0; m < MinionField.instance.minions.Length; m++)
                                if (MinionField.instance.minions[m].gameObject.activeSelf)
                                    randomMinion.Add(MinionField.instance.minions[m]);
                            if (EnemyMinionField.instance.minionNum >= 7)
                                randomMinion.Clear();
                        }
                        else
                        {
                            for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                                if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                    randomMinion.Add(EnemyMinionField.instance.minions[m]);
                            if (MinionField.instance.minionNum >= 7)
                                randomMinion.Clear();
                        }

                        if (randomMinion.Count > 0)
                        {
                            int r = Random.Range(0, randomMinion.Count);
                            minion_name = randomMinion[r].minion_name;
                            MinionObject temp = randomMinion[r];

                            if (minionObject.enemy)
                            {
                                for (int m = randomMinion[r].num; m < MinionField.instance.minions.Length - 1; m++)
                                    MinionField.instance.minions[m] = MinionField.instance.minions[m + 1];
                                MinionField.instance.minions[MinionField.instance.minions.Length - 1] = temp;
                                MinionField.instance.minionNum--;
                            }
                            else
                            {
                                for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                                    EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                                EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = temp;
                                EnemyMinionField.instance.minionNum--;
                            }

                            if (minionObject.enemy)
                            {
                                int index = EnemyMinionField.instance.minionNum;
                                EnemyMinionField.instance.AddMinion(index, minion_name, false, 1);
                                yield return new WaitForSeconds(0.5f);
                                MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                            }
                            else
                            {
                                int index = MinionField.instance.minionNum;
                                MinionField.instance.AddMinion(index, minion_name, false, 1);
                                yield return new WaitForSeconds(0.5f);
                                MinionCopy(randomMinion[r], MinionField.instance.minions[index]);
                            }
                            minionNum++;
                            randomMinion[r].MinionRemoveProcess();
                        }
                    }
                }

            }
            else if (NowEvent == 7)
            {
                SetSelectMask(minionObject.abilityList[j].Ability_type);

                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                minionObject.minion_name = minion_name;
                minionObject.InitTrigger = true;
            }
            else if (NowEvent == 8)
            {
                if (minionObject.enemy)
                {
                    for(int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if(!EnemyMinionField.instance.minions[m].Equals(minionObject) && EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            EnemyMinionField.instance.minions[m].nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                            EnemyMinionField.instance.minions[m].final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                            EnemyMinionField.instance.minions[m].nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                        }
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].Equals(minionObject) && MinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            MinionField.instance.minions[m].nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                            MinionField.instance.minions[m].final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                            MinionField.instance.minions[m].nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                        }
                }
            }
            #endregion

        }
    }
    #endregion

    #region[하수인 제거시 효과]
    public void DeathMinionAbility(MinionObject minionObject)
    {
        Queue<MinionAbility> DeathrattleEventQueue = new Queue<MinionAbility>();

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.죽음의메아리)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                {
                    MinionAbility minionAbility = new MinionAbility(minionObject.abilityList[j].Condition_type, minionObject.abilityList[j].Condition_data, minionObject.abilityList[j].Ability_type, minionObject.abilityList[j].Ability_data);
                    DeathrattleEventQueue.Enqueue(minionAbility);
                }
            }
        }
        #endregion

        #region[먼저처리되는이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.죽음의메아리)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    MinionAbility minionAbility = new MinionAbility(minionObject.abilityList[j].Condition_type, minionObject.abilityList[j].Condition_data, minionObject.abilityList[j].Ability_type, minionObject.abilityList[j].Ability_data);
                    DeathrattleEventQueue.Enqueue(minionAbility);
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                {
                    MinionAbility minionAbility = new MinionAbility(minionObject.abilityList[j].Condition_type, minionObject.abilityList[j].Condition_data, minionObject.abilityList[j].Ability_type, minionObject.abilityList[j].Ability_data);
                    DeathrattleEventQueue.Enqueue(minionAbility);
                }
            }
        }
        #endregion

        StartCoroutine(DeathrattleEvent(DeathrattleEventQueue, minionObject.enemy, minionObject.transform.position));

        for (int i = 0; i < minionList.Count; i++)
            if (minionList[i].gameObject.activeSelf)
                MinionDeadMinionAbility(minionList[i]);
    }
    #endregion

    #region[죽음의 메아리 이벤트]
    private IEnumerator DeathrattleEvent(Queue<MinionAbility> deathrattleEventQueue, bool minion_enemy,Vector2 actPos)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);

        int minionNum = 0;

        while (deathrattleEventQueue.Count > 0)
        {
            int NowEvent = 0;

            MinionAbility temp = deathrattleEventQueue.Dequeue();

            #region[하수인 소환 이벤트]
            if (temp.Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (temp.Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            else if (temp.Ability_type == MinionAbility.Ability.무작위_패_버리기)
                NowEvent = 4;
            #endregion

            #region[무작위 하수인뺏기]
            else if (temp.Ability_type == MinionAbility.Ability.무작위_하수인뺏기)
                NowEvent = 6;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 3)
            {
                GameEventManager.instance.EventSet(0.5f);
                yield return new WaitForSeconds(0.5f);

                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)temp.Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)temp.Ability_data.x, (int)temp.Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)temp.Ability_data.x, (int)temp.Ability_data.y, "명령어");
                if ((enemy && !minion_enemy) || (!enemy && minion_enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else 
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if (temp.Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for (int draw = 0; draw < temp.Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minion_enemy && (temp.Ability_data.y == 0)) || (!minion_enemy && (temp.Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                    }
                }
                else if (temp.Ability_type == MinionAbility.Ability.무작위_패_버리기)
                {
                    for (int c = 0; c < temp.Ability_data.x; c++)
                    {
                        if (!minion_enemy)
                        {
                            if (CardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, CardHand.instance.nowHandNum);
                            CardHand.instance.CardRemove(r);
                        }
                        else
                        {
                            if (EnemyCardHand.instance.nowHandNum <= 0)
                                break;
                            int r = Random.Range(0, EnemyCardHand.instance.nowHandNum);
                            EnemyCardHand.instance.CardRemove(r);
                        }
                    }
                }
            }
            else if (NowEvent == 6)
            {
                string minion_name = "데스윙";

                List<MinionObject> randomMinion = new List<MinionObject>();

                //적군미니언효과이면
                if (minion_enemy)
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf && MinionField.instance.minions[m].final_hp > 0)
                            randomMinion.Add(MinionField.instance.minions[m]);
                    if (EnemyMinionField.instance.minionNum >= 7)
                        randomMinion.Clear();
                }
                //아군미니언효과이면
                else
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && EnemyMinionField.instance.minions[m].final_hp > 0)
                            randomMinion.Add(EnemyMinionField.instance.minions[m]);
                    if (MinionField.instance.minionNum >= 7)
                        randomMinion.Clear();
                }         
                if (randomMinion.Count > 0)
                {
                    yield return new WaitForSeconds(1f);
                    if (minion_enemy)
                    {
                        if (MinionField.instance.minionNum <= 0)
                            randomMinion.Clear();
                    }
                    else
                    {
                        if (EnemyMinionField.instance.minionNum <= 0)
                            randomMinion.Clear();
                    }

                }

                if (randomMinion.Count > 0)
                {
                    GameEventManager.instance.EventSet(1.5f);
                    int r = Random.Range(0, randomMinion.Count);
                    minion_name = randomMinion[r].minion_name;

                    MinionObject tempMinion = randomMinion[r];
                    Vector2 pos = tempMinion.transform.position;

                    EffectManager.instance.CurseEffect(actPos, pos);

                    yield return new WaitForSeconds(1f);

                    if (minion_enemy)
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(index, minion_name, false, 1);
                        MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                        MinionField.instance.setMinionPos = false;
                        EnemyMinionField.instance.setMinionPos = false;
                        EnemyMinionField.instance.minions[index].transform.position = pos;
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false, 1);
                        MinionCopy(randomMinion[r], MinionField.instance.minions[index]);
                        MinionField.instance.setMinionPos = false;
                        EnemyMinionField.instance.setMinionPos = false;
                        MinionField.instance.minions[index].transform.position = pos;
                    }

                    yield return new WaitForSeconds(1.5f);

                    if (minion_enemy)
                    {
                        for (int m = randomMinion[r].num; m < MinionField.instance.minions.Length - 1; m++)
                            MinionField.instance.minions[m] = MinionField.instance.minions[m + 1];
                        MinionField.instance.minions[MinionField.instance.minions.Length - 1] = tempMinion;
                        MinionField.instance.minionNum--;
                    }
                    else
                    {
                        for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                            EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                        EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = tempMinion;
                        EnemyMinionField.instance.minionNum--;
                    }

                    randomMinion[r].MinionRemoveProcess();

                    MinionField.instance.setMinionPos = true;
                    EnemyMinionField.instance.setMinionPos = true;

                    minionNum++;
                }
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);
    }

    #endregion

    #region[미니언 상태 복사]
    public void MinionCopy(MinionObject copyMinionObject, MinionObject pasteMinionObject)
    {
        pasteMinionObject.final_hp = copyMinionObject.final_hp;
        pasteMinionObject.baseHp = copyMinionObject.baseHp;
        pasteMinionObject.nowAtk = copyMinionObject.nowAtk;
        pasteMinionObject.final_spellAtk = copyMinionObject.final_spellAtk;
        pasteMinionObject.nowSpell = copyMinionObject.nowSpell;
        pasteMinionObject.legend = copyMinionObject.legend;
        pasteMinionObject.silence = copyMinionObject.silence;
        pasteMinionObject.freezeCount = copyMinionObject.freezeCount;
        pasteMinionObject.sleep = copyMinionObject.sleep;
        pasteMinionObject.stealth = copyMinionObject.stealth;
        pasteMinionObject.taunt = copyMinionObject.taunt;
        pasteMinionObject.abilityList.Clear();

        for(int i = 0; i < copyMinionObject.abilityList.Count; i++)
        {
            MinionAbility minionAbility = new MinionAbility(copyMinionObject.abilityList[i].Condition_type, copyMinionObject.abilityList[i].Condition_data, copyMinionObject.abilityList[i].Ability_type, copyMinionObject.abilityList[i].Ability_data);
            pasteMinionObject.abilityList.Add(minionAbility);
        }
        pasteMinionObject.buffList.Clear();
        for (int i = 0; i < copyMinionObject.buffList.Count; i++)
            pasteMinionObject.buffList.Add(copyMinionObject.buffList[i]);

    }
    #endregion

    #region[주문사용시]
    public void SpellRunMinionAbility(MinionObject minionObject)
    {
        List<int> SpellRunEventList = new List<int>();

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.주문시전)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    SpellRunEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    SpellRunEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.주문시전)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    SpellRunEventList.Add(j);
            }
        }
        #endregion

        if (SpellRunEventList.Count > 0)
            StartCoroutine(SpellRunMinionAbilityRun(minionObject, SpellRunEventList));
    }

    private IEnumerator SpellRunMinionAbilityRun(MinionObject minionObject,List<int> SpellRunEventList)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int minionNum = 0;
        for (int i = 0; i < SpellRunEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = SpellRunEventList[i];

            int NowEvent = 0;

            #region[자가버프 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for (int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        if (minionObject.minion_name == "가젯잔 경매인")
                        {
                            EffectManager.instance.CoinEffect(minionObject.transform.position);
                            SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                        }
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);

    }
    #endregion

    #region[턴시작시]
    public void TurnStartMinionAbility(MinionObject minionObject, bool turnStartHero)
    {
        //turnStartHero == true 면 적군 아니면 아군
        List<int> TurnStartEventList = new List<int>();

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.각턴_시작 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.상대턴의_시작 && minionObject.enemy != turnStartHero) ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.내턴의_시작 && minionObject.enemy == turnStartHero))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    TurnStartEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    TurnStartEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.확률_카드뽑기)
                    TurnStartEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                    TurnStartEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.각턴_시작 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.상대턴의_시작 && minionObject.enemy != turnStartHero) ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.내턴의_시작 && minionObject.enemy == turnStartHero))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    TurnStartEventList.Add(j);
            }
        }
        #endregion

        if (TurnStartEventList.Count > 0)
            StartCoroutine(TurnStartMinionAbility(minionObject, TurnStartEventList));
    }

    private IEnumerator TurnStartMinionAbility(MinionObject minionObject, List<int> TurnStartEventList)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int minionNum = 0;
        for (int i = 0; i < TurnStartEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = TurnStartEventList[i];

            int NowEvent = 0;

            #region[무작위능력치부여 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                NowEvent = 1;
            #endregion

            #region[자가버프 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.확률_카드뽑기)
                NowEvent = 4;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                List<MinionObject> minionList = new List<MinionObject>();
                if (minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && !EnemyMinionField.instance.minions[m].Equals(minionObject))
                            minionList.Add(EnemyMinionField.instance.minions[m]);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf && !MinionField.instance.minions[m].Equals(minionObject))
                            minionList.Add(MinionField.instance.minions[m]);
                }

                if (minionList.Count > 0)
                {
                    MinionObject targetMinion = minionList[Random.Range(0, minionList.Count)];
                    targetMinion.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    targetMinion.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    targetMinion.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                int r = 100;
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.확률_카드뽑기)
                    r = Random.Range(0,100);
                if (r > minionObject.abilityList[j].Ability_data.z)
                {
                    for (int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                        {
                            SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                            if (minionObject.minion_name == "내트 페이글")
                                EffectManager.instance.WaterEffect(EffectManager.instance.enemyDeckPos.position);
                            EnemyCardHand.instance.DrawCard();
                        }
                        else
                        {
                            SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                            if (minionObject.minion_name == "내트 페이글")
                                EffectManager.instance.WaterEffect(EffectManager.instance.playerDeckPos.position);
                            CardHand.instance.CardDrawAct();
                        }
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);

    }
    #endregion

    #region[턴종료시]
    public void TurnEndMinionAbility(MinionObject minionObject,bool turnEndHero)
    {
        //turnEndHero == true 면 적군 아니면 아군
        List<int> TurnEndEventList = new List<int>();

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.각턴_종료 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.상대턴의_종료 && minionObject.enemy != turnEndHero) ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.자신의_턴종료 && minionObject.enemy == turnEndHero))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    TurnEndEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    TurnEndEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                    TurnEndEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_피해주기)
                    TurnEndEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.각턴_종료 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.상대턴의_종료 && minionObject.enemy != turnEndHero) ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.자신의_턴종료 && minionObject.enemy == turnEndHero))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    TurnEndEventList.Add(j);
            }
        }
        #endregion

        #region[죽음 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.각턴_종료 ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.상대턴의_종료 && minionObject.enemy != turnEndHero) ||
                (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.자신의_턴종료 && minionObject.enemy == turnEndHero))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.죽음)
                    TurnEndEventList.Add(j);
            }
        }
        #endregion

        if (TurnEndEventList.Count > 0)
            StartCoroutine(TurnEndMinionAbility(minionObject, TurnEndEventList));
    }

    private IEnumerator TurnEndMinionAbility(MinionObject minionObject, List<int> TurnEndEventList)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int minionNum = 0;
        for (int i = 0; i < TurnEndEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = TurnEndEventList[i];

            int NowEvent = 0;

            #region[무작위능력치부여 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                NowEvent = 1;
            #endregion

            #region[자가버프 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            #endregion

            #region[무작위 피해 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_피해주기)
                NowEvent = 5;
            #endregion

            #region[죽음 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.죽음)
                NowEvent = 6;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                List<MinionObject> targetList = new List<MinionObject>();
                if(minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && !EnemyMinionField.instance.minions[m].Equals(minionObject))
                            targetList.Add(EnemyMinionField.instance.minions[m]);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf && !MinionField.instance.minions[m].Equals(minionObject))
                            targetList.Add(MinionField.instance.minions[m]);
                }

                if(targetList.Count > 0)
                {
                    MinionObject targetMinion = targetList[Random.Range(0, targetList.Count)];
                    SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                    if (minionObject.minion_name == "검 제작의 대가")
                        EffectManager.instance.SwordEffect(minionObject.transform.position, targetMinion.transform.position);
                    yield return new WaitForSeconds(1.3f);
                    targetMinion.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    targetMinion.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    targetMinion.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for (int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            else if (NowEvent == 5)
            {
                GameEventManager.instance.EventSet(1f);
                List<int> targetList = new List<int>();
                if (!minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && EnemyMinionField.instance.minions[m].final_hp > 0)
                            targetList.Add(m);
                    //영웅번호
                    targetList.Add(987654321);
                    if (targetList.Count > 0)
                    {
                        int r = Random.Range(0, targetList.Count);
                        if(targetList[r] == 987654321)
                        {
                            if (minionObject.minion_name == "불의 군주 라그나로스")
                            {
                                SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                                EffectManager.instance.FireBallEffect(minionObject.transform.position, HeroManager.instance.enemyHero.transform.position);
                                GameEventManager.instance.EventAdd(1f);
                                yield return new WaitForSeconds(1f);
                            }
                            AttackManager.instance.PopAllDamageObj();
                            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)minionObject.abilityList[j].Ability_data.x);
                            AttackManager.instance.AttackEffectRun();
                        }
                        else
                        {
                            if (minionObject.minion_name == "불의 군주 라그나로스")
                            {
                                SoundManager.instance.PlayMinionSE(minionObject.minion_name, 미니언상태.효과);
                                EffectManager.instance.FireBallEffect(minionObject.transform.position, EnemyMinionField.instance.minions[targetList[r]].transform.position);
                                GameEventManager.instance.EventAdd(1f);
                                yield return new WaitForSeconds(1f);
                            }
                            AttackManager.instance.PopAllDamageObj();
                            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[targetList[r]].damageEffect, (int)minionObject.abilityList[j].Ability_data.x);
                            AttackManager.instance.AttackEffectRun();
                        }
                       
                    }
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf && MinionField.instance.minions[m].final_hp > 0)
                            targetList.Add(m);
                    //영웅번호
                    targetList.Add(987654321);
                    if (targetList.Count > 0)
                    {
                        int r = Random.Range(0, targetList.Count);
                        if (targetList[r] == 987654321)
                        {
                            if (minionObject.minion_name == "불의 군주 라그나로스")
                            {
                                EffectManager.instance.FireBallEffect(minionObject.transform.position, HeroManager.instance.playerHero.transform.position);
                                GameEventManager.instance.EventAdd(1f);
                                yield return new WaitForSeconds(1f);
                            }
                            AttackManager.instance.PopAllDamageObj();
                            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)minionObject.abilityList[j].Ability_data.x);
                            AttackManager.instance.AttackEffectRun();
                        }
                        else
                        {
                            if (minionObject.minion_name == "불의 군주 라그나로스")
                            {
                                EffectManager.instance.FireBallEffect(minionObject.transform.position, MinionField.instance.minions[targetList[r]].transform.position);
                                GameEventManager.instance.EventAdd(1f);
                                yield return new WaitForSeconds(1f);
                            }
                            AttackManager.instance.PopAllDamageObj();
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[targetList[r]].damageEffect, (int)minionObject.abilityList[j].Ability_data.x);
                            AttackManager.instance.AttackEffectRun();
                        }

                    }
                }
            }
            else if (NowEvent == 6)
            {
                minionObject.MinionDeath();
                minionNum++;
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);

    }
    #endregion

    #region[데미지받을때마다]
    public void DamageMinionAbility(MinionObject minionObject)
    {
        //turnStartHero == true 면 적군 아니면 아군
        List<int> DamageEventList = new List<int>();

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.피해를_받을때마다)
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    DamageEventList.Add(j);
        }
        #endregion

        if (DamageEventList.Count > 0)
            StartCoroutine(DamaeMinionAbility(minionObject, DamageEventList));
    }

    private IEnumerator DamaeMinionAbility(MinionObject minionObject, List<int> DamageEventList)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int minionNum = 0;
        for (int i = 0; i < DamageEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = DamageEventList[i];

            int NowEvent = 0;

            #region[자가버프 이벤트]
           if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }          
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);

    }
    #endregion

    #region[하수인이 죽을때 마다]
    public void MinionDeadMinionAbility(MinionObject minionObject)
    {
        //turnEndHero == true 면 적군 아니면 아군
        List<int> MinionDeadEventList = new List<int>();

        #region[대상선택이 필요 없는 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.하수인이_죽을때마다)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    MinionDeadEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    MinionDeadEventList.Add(j);
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                    MinionDeadEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.하수인이_죽을때마다)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    MinionDeadEventList.Add(j);
            }
        }
        #endregion

        if (MinionDeadEventList.Count > 0)
            StartCoroutine(MinionDeadMinionAbility(minionObject, MinionDeadEventList));
    }

    private IEnumerator MinionDeadMinionAbility(MinionObject minionObject, List<int> MinionDeadEventList)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int minionNum = 0;
        for (int i = 0; i < MinionDeadEventList.Count; i++)
        {
            if (minionObject.abilityList.Count <= 0)
                break;

            int j = MinionDeadEventList[i];
            int NowEvent = 0;

            #region[무작위능력치부여 이벤트]
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.무작위_능력치부여)
                NowEvent = 1;
            #endregion

            #region[자가버프 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드관련 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                List<MinionObject> minionList = new List<MinionObject>();
                if (minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf && !EnemyMinionField.instance.minions[m].Equals(minionObject))
                            minionList.Add(EnemyMinionField.instance.minions[m]);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf && !MinionField.instance.minions[m].Equals(minionObject))
                            minionList.Add(MinionField.instance.minions[m]);
                }

                if (minionList.Count > 0)
                {
                    MinionObject targetMinion = minionList[Random.Range(0, minionList.Count)];
                    targetMinion.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    targetMinion.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    targetMinion.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 2)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    minionObject.final_hp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                string minion_ability = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "명령어");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    int index = EnemyMinionField.instance.minionNum;
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
                    int index = MinionField.instance.minionNum;
                    MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                }
                minionNum++;
            }
            else if (NowEvent == 4)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                {
                    for (int draw = 0; draw < minionObject.abilityList[j].Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 0)) || (!minionObject.enemy && (minionObject.abilityList[j].Ability_data.y == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventSet(1.5f);

    }
    #endregion

    #region[무기장착중이면]
    public void EquipWeaponMinionAbility(MinionObject minionObject)
    {
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.무기장착시)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    minionObject.sleep = false;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    minionObject.taunt = true;
            }
        }
    }
    #endregion
}

