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
            //final_hp = nowHp;   //체력의 지속버프 처리라는게 이상함
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
                    minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                    minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.도발)
                    minionObject.taunt = true;
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                {
                    minionObject.canAttackNum = 1;
                    Debug.Log("돌진");
                }
                else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.은신)
                    minionObject.stealth = true;
            }
        }


    }
    #endregion

    #region[하수인 소환시 효과]

    public List<int> BattlecryEventList = new List<int>();

    public void SpawnMinionAbility(MinionObject minionObject)
    {
        BattlecryEventList.Clear();

        #region[대상선택이 필요한 이벤트]
        bool selectMinionEvent = false;
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성)
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
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                {
                    BattlecryEventList.Add(j);
                    //minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    //minionObject.baseAtk += (int)minionObject.abilityList[j].Ability_data.x;
                    //minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                    //minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                    //minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
                }
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성)
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                {
                    BattlecryEventList.Add(j);
                    //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                    //bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                    //string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                    //if (enemy)
                    //{
                    //    int spawnIndex = 0;
                    //    bool flag = false;
                    //    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    //        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                    //        {
                    //            flag = true;
                    //            spawnIndex = Mathf.Max(spawnIndex, EnemyMinionField.instance.minions[i].num);
                    //        }
                    //    if (flag)
                    //        spawnIndex++;
                    //    spawnIndex += minionNum;
                    //    EnemyMinionField.instance.AddMinion(spawnIndex, minion_name, false);
                    //}
                    //else
                    //{
                    //    int spawnIndex = minionObject.num + minionNum;
                    //    MinionField.instance.AddMinion(spawnIndex, minion_name, false);
                    //}
                    //minionNum++;
                }
            }
        }

        #endregion

        StartCoroutine(BattlecryEvent(minionObject));
    }
    #endregion

    #region[전투의 함성 이벤트]
    private IEnumerator BattlecryEvent(MinionObject minionObject)
    {
        int minionNum = 0;
        for (int i = 0; i < BattlecryEventList.Count; i++)
        {
            int j = BattlecryEventList[i];
            int NowEvent = 0;
            if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.빙결시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.아군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.영웅의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.적군하수인_주인의패로되돌리기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.침묵시키기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.해당턴동안_능력치부여)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인처치)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인의_생명력회복)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인에게_피해주기)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.대상의_공격력_생명력_교환)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치부여)
                NowEvent = 1;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                NowEvent = 2;
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;

            if (NowEvent == 0)
            {
                bool targetExistence = false;
                for (int m = 0; m < minionList.Count; m++)
                    if (minionList[m].gameObject.activeSelf && !minionList[m].Equals(minionObject))
                    {
                        targetExistence = true;
                        break;
                    }

                if (targetExistence)
                {
                    DragLineRenderer.instance.startPos = minionObject.transform.position;
                    BattleUI.instance.grayFilterAni.SetBool("On", true);
                    DragLineRenderer.instance.lineRenderer.enabled = true;
                    DragLineRenderer.instance.InitMask();
                    DragLineRenderer.instance.activeObj = minionObject.gameObject;
                    DragLineRenderer.instance.AddMask(타겟.적하수인);
                    DragLineRenderer.instance.AddMask(타겟.아군하수인);

                    GameEventManager.instance.EventSet(10000f);
                    while (GameEventManager.instance.GetEventValue() > 0.1f)
                        yield return new WaitForSeconds(0.001f);
                }
            }
            else if(NowEvent == 2)
            {
                minionObject.nowAtk += (int)minionObject.abilityList[j].Ability_data.x;
                minionObject.baseAtk += (int)minionObject.abilityList[j].Ability_data.x;
                minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
                minionObject.nowSpell += (int)minionObject.abilityList[j].Ability_data.z;
            }
            else if (NowEvent == 3)
            {
                //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                bool enemy = (int)minionObject.abilityList[j].Ability_data.z == 1 ? false : true;
                string minion_name = DataMng.instance.ToString((DataMng.TableType)minionObject.abilityList[j].Ability_data.x, (int)minionObject.abilityList[j].Ability_data.y, "카드이름");
                if (enemy)
                {
                    int spawnIndex = 0;
                    bool flag = false;
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            flag = true;
                            spawnIndex = Mathf.Max(spawnIndex, EnemyMinionField.instance.minions[m].num);
                        }
                    if (flag)
                        spawnIndex++;
                    spawnIndex += minionNum;
                    EnemyMinionField.instance.AddMinion(spawnIndex, minion_name, false);
                }
                else
                {
                    int spawnIndex = minionObject.num + minionNum;
                    MinionField.instance.AddMinion(spawnIndex, minion_name, false);
                }
                minionNum++;
            }
        }

        if(minionNum > 0)
            GameEventManager.instance.EventSet(2);
    }
    #endregion
}

