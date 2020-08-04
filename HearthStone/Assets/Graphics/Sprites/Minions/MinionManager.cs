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
        if (instance == null)
        {
            instance = this;
            for (int i = 0; i < minionMat.Count; i++)
                minionMaterial.Add(minionMat[i].name, minionMat[i]);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
    public void MinionsTurnStartTrigger()
    {
        for (int i = 0; i < minionList.Count; i++)
            if(minionList[i].gameObject.activeSelf)
                minionList[i].turnStartTrigger = true;
    }
    #endregion

    #region[미니언 턴 종료시 처리]
    public void MinionsTurnEndTrigger()
    {
        for (int i = 0; i < minionList.Count; i++)
            if (minionList[i].gameObject.activeSelf)
                minionList[i].turnEndTrigger = true;
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
                    minionObject.canAttackNum = 1;
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
                    if(DragLineRenderer.instance.CheckMask(타겟.아군영웅) || DragLineRenderer.instance.CheckMask(타겟.적영웅))
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
                    minionObject.canAttackNum = 1;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
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
                        {
                            for (int c = 0; c < BattleUI.instance.playerCardAni.Length; c++)
                            {
                                if (BattleUI.instance.playerCardAni[c].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                                {
                                    BattleUI.instance.playerCardAni[c].SetTrigger("Draw");
                                    CardHand.instance.DrawCard();
                                    string s = InGameDeck.instance.playDeck[0];
                                    InGameDeck.instance.playDeck.RemoveAt(0);
                                    CardHand.instance.CardMove(s, CardHand.instance.nowHandNum - 1, CardHand.instance.drawCardPos.transform.position, CardHand.instance.defaultSize, 0);
                                    CardViewManager.instance.UpdateCardView(0.001f);
                                    break;
                                }
                            }
                        }
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

                GameEventManager.instance.EventAdd(2.5f);

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

                        if(minionObject.enemy)
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

                        if(randomMinion.Count > 0)
                        {
                            int r = Random.Range(0, randomMinion.Count);
                            minion_name = randomMinion[r].minion_name;

                            MinionObject temp = randomMinion[r];
                            if (minionObject.enemy)
                            {
                                for (int m = randomMinion[r].num; m < MinionField.instance.minions.Length - 1; m++)
                                    MinionField.instance.minions[m] = MinionField.instance.minions[m + 1];
                                MinionField.instance.minionNum--;
                                MinionField.instance.minions[MinionField.instance.minions.Length - 1] = temp;
                            }
                            else
                            {
                                for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                                    EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                                EnemyMinionField.instance.minionNum--;
                                EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = temp;
                            }

                            if (minionObject.enemy)
                            {
                                int index = EnemyMinionField.instance.minionNum;
                                EnemyMinionField.instance.AddMinion(index, minion_name, false);
                                yield return new WaitForSeconds(0.5f);
                                MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                            }
                            else
                            {
                                int index = MinionField.instance.minionNum;
                                MinionField.instance.AddMinion(index, minion_name, false);
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
            GameEventManager.instance.EventAdd(2);
    }

    #region[대상 선택 취소]
    public void MinionSelectCancle()
    {
        if (!selectMinionEvent)
            return;
        selectMinionEvent = false;
        eventMininon.gotoHandTrigger = true;
        GameEventManager.instance.EventAdd(1.4f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(eventMininon.minion_name));
        int mana = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
        ManaManager.instance.playerNowMana += mana;
        CardHand.instance.useCardNum--;

    }
    #endregion

    #region[대상 선택시]
    public void MinionSelect(MinionObject minionObject)
    {
        if (!selectMinionEvent)
            return;
        selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (eventMininon.abilityList[eventNum].Ability_type)
        {
            case MinionAbility.Ability.빙결시키기:
                minionObject.freezeTrigger = true;
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                minionObject.gotoHandTrigger = true;
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                break;
            case MinionAbility.Ability.침묵시키기:
                minionObject.ActSilence();
                break;
            case MinionAbility.Ability.하수인처치:
                minionObject.MinionDeath();
                break;
            case MinionAbility.Ability.생명력회복:
            case MinionAbility.Ability.하수인의_생명력회복:
                minionObject.final_hp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                minionObject.final_hp = Mathf.Min(minionObject.final_hp, minionObject.baseHp);
                break;
            case MinionAbility.Ability.피해주기:
            case MinionAbility.Ability.하수인에게_피해주기:
                AttackManager.instance.AddDamageObj(minionObject.damageEffect, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                AttackManager.instance.AttackEffectRun();
                break;
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
                int temp = minionObject.final_hp;
                minionObject.baseHp = minionObject.final_atk;
                minionObject.final_hp = minionObject.final_atk;
                minionObject.baseAtk = temp;
                minionObject.nowAtk = temp;
                minionObject.final_atk = temp;
                for (int i = 0; i < minionObject.buffList.Count; i++)
                    minionObject.buffList[i] = new Vector4(0, minionObject.buffList[i].y, minionObject.buffList[i].z, minionObject.buffList[i].w);
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

    public void HeroSelect(bool enemy)
    {
        if (!selectMinionEvent)
            return;
        selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (eventMininon.abilityList[eventNum].Ability_type)
        {
            case MinionAbility.Ability.빙결시키기:
                HeroManager.instance.SetFreeze(enemy);
                break;
            case MinionAbility.Ability.피해주기:
            case MinionAbility.Ability.영웅에게_피해주기:
                if(enemy)
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                else
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)eventMininon.abilityList[eventNum].Ability_data.x);
                AttackManager.instance.AttackEffectRun();
                break;
            case MinionAbility.Ability.생명력회복:
            case MinionAbility.Ability.영웅의_생명력회복:
                if(enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp += (int)eventMininon.abilityList[eventNum].Ability_data.x;
                break;
            case MinionAbility.Ability.영웅의_생명력설정:
                if (enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp = (int)eventMininon.abilityList[eventNum].Ability_data.x;
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp = (int)eventMininon.abilityList[eventNum].Ability_data.x;
                break;
        }
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

    #endregion

    #region[선택(드루이드) 이벤트]
    [HideInInspector] public int selectChoose = -1;
    private IEnumerator ChooseOneEvent(MinionObject minionObject)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);

        BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

        List<Vector2> chooseOneList = new List<Vector2>();

        for(int i = 0; i < ChooseOneEventList.Count; i++)
        {
            int j = ChooseOneEventList[i];
            int chooseEventJobNum = (int)minionObject.abilityList[j].Condition_data.x;
            int chooseEventCardNum = (int)minionObject.abilityList[j].Condition_data.y;
            if (!chooseOneList.Contains(new Vector2(chooseEventJobNum, chooseEventCardNum)))
                chooseOneList.Add(new Vector2(chooseEventJobNum, chooseEventCardNum));
        }

        for (int i = 0; i < chooseOneList.Count; i++)
        {
            string ChooseName = DataMng.instance.ToString((DataMng.TableType)chooseOneList[i].x, (int)chooseOneList[i].y, "카드이름");
            CardViewManager.instance.CardShow(ref BattleUI.instance.chooseCardView[i], ChooseName);
            CardViewManager.instance.UpdateCardView(0.001f);
        }

        eventMininon = minionObject;

        selectChoose = -1;

        while (selectChoose == -1)
        {
            GameEventManager.instance.EventSet(1f);
            yield return new WaitForSeconds(0.001f);
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
                    minionObject.canAttackNum = 1;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                if ((enemy && !minionObject.enemy) || (!enemy && minionObject.enemy))
                {
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else
                {
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
                        {
                            for (int c = 0; c < BattleUI.instance.playerCardAni.Length; c++)
                            {
                                if (BattleUI.instance.playerCardAni[c].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                                {
                                    BattleUI.instance.playerCardAni[c].SetTrigger("Draw");
                                    CardHand.instance.DrawCard();
                                    string s = InGameDeck.instance.playDeck[0];
                                    InGameDeck.instance.playDeck.RemoveAt(0);
                                    CardHand.instance.CardMove(s, CardHand.instance.nowHandNum - 1, CardHand.instance.drawCardPos.transform.position, CardHand.instance.defaultSize, 0);
                                    CardViewManager.instance.UpdateCardView(0.001f);
                                    break;
                                }
                            }
                        }
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

                GameEventManager.instance.EventAdd(2.5f);

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
                                MinionField.instance.minionNum--;
                                MinionField.instance.minions[MinionField.instance.minions.Length - 1] = temp;
                            }
                            else
                            {
                                for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                                    EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                                EnemyMinionField.instance.minionNum--;
                                EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = temp;
                            }

                            if (minionObject.enemy)
                            {
                                int index = EnemyMinionField.instance.minionNum;
                                EnemyMinionField.instance.AddMinion(index, minion_name, false);
                                yield return new WaitForSeconds(0.5f);
                                MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                            }
                            else
                            {
                                int index = MinionField.instance.minionNum;
                                MinionField.instance.AddMinion(index, minion_name, false);
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
                if(minionObject.enemy)
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

        StartCoroutine(DeathrattleEvent(DeathrattleEventQueue, minionObject.enemy));
    }
    #endregion

    #region[죽음의 메아리 이벤트]
    private IEnumerator DeathrattleEvent(Queue<MinionAbility> deathrattleEventQueue, bool minion_enemy)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        int NowEvent = 0;
        int minionNum = 0;

        while (deathrattleEventQueue.Count > 0)
        {
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
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)temp.Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)temp.Ability_data.x, (int)temp.Ability_data.y, "카드이름");
                if ((enemy && !minion_enemy) || (!enemy && minion_enemy))
                {
                    EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                }
                else 
                {
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
                        {
                            for (int c = 0; c < BattleUI.instance.playerCardAni.Length; c++)
                            {
                                if (BattleUI.instance.playerCardAni[c].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                                {
                                    BattleUI.instance.playerCardAni[c].SetTrigger("Draw");
                                    CardHand.instance.DrawCard();
                                    string s = InGameDeck.instance.playDeck[0];
                                    InGameDeck.instance.playDeck.RemoveAt(0);
                                    CardHand.instance.CardMove(s, CardHand.instance.nowHandNum - 1, CardHand.instance.drawCardPos.transform.position, CardHand.instance.defaultSize, 0);
                                    CardViewManager.instance.UpdateCardView(0.001f);
                                    break;
                                }
                            }
                        }
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
                GameEventManager.instance.EventAdd(1f);
                yield return new WaitForSeconds(1f);
                List<MinionObject> randomMinion = new List<MinionObject>();

                if (minion_enemy)
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

                    MinionObject tempMinion = randomMinion[r];
                    if (minion_enemy)
                    {
                        for (int m = randomMinion[r].num; m < MinionField.instance.minions.Length - 1; m++)
                            MinionField.instance.minions[m] = MinionField.instance.minions[m + 1];
                        MinionField.instance.minionNum--;
                        MinionField.instance.minions[MinionField.instance.minions.Length - 1] = tempMinion;
                    }
                    else
                    {
                        for (int m = randomMinion[r].num; m < EnemyMinionField.instance.minions.Length - 1; m++)
                            EnemyMinionField.instance.minions[m] = EnemyMinionField.instance.minions[m + 1];
                        EnemyMinionField.instance.minionNum--;
                        EnemyMinionField.instance.minions[EnemyMinionField.instance.minions.Length - 1] = tempMinion;
                    }

                    if (minion_enemy)
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(index, minion_name, false);
                        yield return new WaitForSeconds(0.5f);
                        MinionCopy(randomMinion[r], EnemyMinionField.instance.minions[index]);
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                        yield return new WaitForSeconds(0.5f);
                        MinionCopy(randomMinion[r], MinionField.instance.minions[index]);
                    }
                    randomMinion[r].MinionRemoveProcess();
                    minionNum++;
                }
            }
            #endregion

        }

        if (minionNum > 0)
            GameEventManager.instance.EventAdd(1.5f);
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

}

