﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager instance;

    public enum EventType
    {
        없음 = -1,
        대상선택,
        카드뽑기,
        적군광역피해,아군광역피해,광역피해,
        모든하수인주인의패로,모든하수인처치,
        하수인소환,
        하수인들에게_은신부여,
        하수인들에게_능력부여,
        하수인들에게_능력치부여,
        마나획득,마나수정획득,
        공격력획득,방어도획득
    }

    public void Awake()
    {
        instance = this;
    }

    #region[스펠 능력 파싱]
    List<SpellAbility> SpellParsing(string ability_string)
    {
        List<SpellAbility> abilityList = new List<SpellAbility>();

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


        return abilityList;
    }
    #endregion

    public void RunSpell(string name,bool enemy = false)
    {
        targetMinion = null;
        targetHero = -1;
        spellSelectCancle = false;
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(name));
        string ability_string = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");
        List<SpellAbility> spellList = SpellParsing(ability_string);
        nowSpellName = name;
        StartCoroutine(SpellEvent(spellList, enemy));
    }

    #region[연계인지 검사]
    bool CheckCombo()
    {
        return CardHand.instance.useCardNum > 1;
    }
    #endregion

    #region[이벤트 타입]
    EventType CheckEvent(SpellAbility spellAbility)
    {
        switch(spellAbility.Ability_type)
        {
            case SpellAbility.Ability.하수인에게_피해주기:
            case SpellAbility.Ability.영웅에게_피해주기:
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
            case SpellAbility.Ability.돌진부여:
            case SpellAbility.Ability.도발부여:
            case SpellAbility.Ability.은신부여:
            case SpellAbility.Ability.빙결시키기:
            case SpellAbility.Ability.침묵시키기:
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
            case SpellAbility.Ability.능력부여:
            case SpellAbility.Ability.하수인의_생명력회복:
            case SpellAbility.Ability.하수인의_생명력설정:
            case SpellAbility.Ability.영웅의_생명력회복:
            case SpellAbility.Ability.영웅의_생명력설정:
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.능력치부여:
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
            case SpellAbility.Ability.하수인처치:
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.하수인_주인의패로되돌리면서_비용감소:
            case SpellAbility.Ability.다른모든_적군하수인_피해주기:
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
            case SpellAbility.Ability.적에게피해주기:
                return EventType.대상선택;
            case SpellAbility.Ability.모든_적군하수인_피해주기:
                return EventType.적군광역피해;
            case SpellAbility.Ability.모든_아군하수인_피해주기:
                return EventType.아군광역피해;
            case SpellAbility.Ability.모든_하수인_피해주기:
                return EventType.광역피해;
            case SpellAbility.Ability.모든_하수인_주인의패로되돌리기:
                return EventType.모든하수인주인의패로;
            case SpellAbility.Ability.모든하수인처치:
                return EventType.모든하수인처치;
            case SpellAbility.Ability.하수인소환:
                return EventType.하수인소환;
            case SpellAbility.Ability.모든하수인에게_은신부여:
                return EventType.하수인들에게_은신부여;
            case SpellAbility.Ability.모든하수인에게_능력부여:
                return EventType.하수인들에게_능력부여;
            case SpellAbility.Ability.모든하수인에게_능력치부여:
                return EventType.하수인들에게_능력치부여;
            case SpellAbility.Ability.카드뽑기:
                return EventType.카드뽑기;
            case SpellAbility.Ability.마나수정획득:
                return EventType.마나수정획득;
            case SpellAbility.Ability.마나획득:
                return EventType.마나획득;
            case SpellAbility.Ability.방어도얻기:
                return EventType.방어도획득;
            case SpellAbility.Ability.영웅공격력얻기:
                return EventType.공격력획득;
            case SpellAbility.Ability.무기에_공격력부여:
            case SpellAbility.Ability.다음카드비용감소:
            case SpellAbility.Ability.다음주문카드비용감소:
            case SpellAbility.Ability.무작위_패_버리기:
            case SpellAbility.Ability.무작위_하수인뺏기:
            case SpellAbility.Ability.다음턴에다시가져오기:
                return EventType.없음;
            default:
                Debug.Log("이벤트 타입을 지정해주세요");
                return EventType.없음;
        }
    }
    #endregion

    #region[선택할 객체 설정]
    public void SetSelectMask(SpellAbility.Ability ability)
    {
        switch (ability)
        {
            case SpellAbility.Ability.돌진부여:
            case SpellAbility.Ability.도발부여:
            case SpellAbility.Ability.은신부여:
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.하수인_주인의패로되돌리면서_비용감소:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.다른모든_적군하수인_피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                break;
            case SpellAbility.Ability.적에게피해주기:
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                break;
            case SpellAbility.Ability.하수인에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
            case SpellAbility.Ability.빙결시키기:
            case SpellAbility.Ability.침묵시키기:
            case SpellAbility.Ability.능력부여:
            case SpellAbility.Ability.하수인의_생명력회복:
            case SpellAbility.Ability.하수인의_생명력설정:
            case SpellAbility.Ability.능력치부여:
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
            case SpellAbility.Ability.하수인처치:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case SpellAbility.Ability.영웅에게_피해주기:
            case SpellAbility.Ability.영웅의_생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
            case SpellAbility.Ability.영웅의_생명력설정:
            case SpellAbility.Ability.생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            default:
                Debug.Log("선택갤체를 지정해주세요");
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
        }
    }
    #endregion

    #region[조건을 만족하는 하수인인지 검사]
    public bool CheckConditionMinion(MinionObject targetMinion,SpellAbility spellAbility)
    {
        if (spellAbility.Condition_type != SpellAbility.Condition.피해입지않은하수인)
            return true;

        #region[피해를 입지않은 하수인인지 검사]
        if (targetMinion.baseHp > targetMinion.final_hp)
        {
            return false;
        }
        #endregion

        return true;
    }
    #endregion

    #region[하수인 선택 안내문구]
    string GetText(SpellAbility.Ability ability)
    {
        switch (ability)
        {
            case SpellAbility.Ability.돌진부여:
            case SpellAbility.Ability.도발부여:
            case SpellAbility.Ability.은신부여:
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.하수인에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
            case SpellAbility.Ability.빙결시키기:
            case SpellAbility.Ability.침묵시키기:
            case SpellAbility.Ability.능력부여:
            case SpellAbility.Ability.하수인의_생명력회복:
            case SpellAbility.Ability.하수인의_생명력설정:
            case SpellAbility.Ability.능력치부여:
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
            case SpellAbility.Ability.하수인처치:
            case SpellAbility.Ability.다른모든_적군하수인_피해주기:
                return "대상 하수인 선택";
            case SpellAbility.Ability.영웅에게_피해주기:
            case SpellAbility.Ability.영웅의_생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                return "대상 영웅 선택";
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
            case SpellAbility.Ability.영웅의_생명력설정:
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.하수인_주인의패로되돌리면서_비용감소:
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
            case SpellAbility.Ability.적에게피해주기:
                return "대상 선택";
            default:
                Debug.Log("선택갤체를 지정해주세요");
                break;
        }
        return "대상선택";
    }
    #endregion

    #region[대상 선택시]
    MinionObject targetMinion = null;
    int targetHero = -1;
    List<MinionObject> deathList = new List<MinionObject>();
    List<MinionObject> survivalList = new List<MinionObject>();
    List<MinionObject> emptyList = new List<MinionObject>();

    public void MinionSelect(MinionObject minionObject)
    {
        if (!MinionManager.instance.selectMinionEvent)
            return;
        targetMinion = minionObject;
        selectSpellEvent = false;
        MinionManager.instance.selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (nowSpellAbility.Ability_type)
        {
            case SpellAbility.Ability.빙결시키기:
                minionObject.freezeTrigger = true;
                break;
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
                minionObject.gotoHandTrigger = true;
                break;
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
                break;
            case SpellAbility.Ability.돌진부여:
                minionObject.canAttackNum = 1;
                break;
            case SpellAbility.Ability.도발부여:
                minionObject.taunt = true;
                break;
            case SpellAbility.Ability.은신부여:
                minionObject.stealth = true;
                break;
            case SpellAbility.Ability.침묵시키기:
                minionObject.ActSilence();
                break;
            case SpellAbility.Ability.하수인처치:
                minionObject.MinionDeath();
                break;
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.하수인의_생명력회복:
                minionObject.final_hp += (int)nowSpellAbility.Ability_data.x;
                minionObject.final_hp = Mathf.Min(minionObject.final_hp, minionObject.baseHp);
                break;
            case SpellAbility.Ability.하수인의_생명력설정:
                minionObject.final_hp = (int)nowSpellAbility.Ability_data.x;
                minionObject.baseHp = (int)nowSpellAbility.Ability_data.x;
                break;
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                if (minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= (int)nowSpellAbility.Ability_data.x && !EnemyMinionField.instance.minions[m].Equals(minionObject))
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                            if (!EnemyMinionField.instance.minions[i].Equals(minionObject))
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[i].damageEffect, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();

                    EnemyMinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = true;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(MinionField.instance.minions[m]);
                        else if (MinionField.instance.minions[m].final_hp <= (int)nowSpellAbility.Ability_data.x && !MinionField.instance.minions[m].Equals(minionObject))
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                            if (!MinionField.instance.minions[i].Equals(minionObject))
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[i].damageEffect, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();

                    MinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                break;
            case SpellAbility.Ability.적에게피해주기:
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.하수인에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
                AttackManager.instance.PopAllDamageObj();
                AttackManager.instance.AddDamageObj(minionObject.damageEffect, (int)nowSpellAbility.Ability_data.x);
                AttackManager.instance.AttackEffectRun();
                break;
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
                break;
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
                int temp = minionObject.final_hp;
                minionObject.baseHp = minionObject.final_atk;
                minionObject.final_hp = minionObject.final_atk;
                minionObject.baseAtk = temp;
                minionObject.nowAtk = temp;
                minionObject.final_atk = temp;
                for (int i = 0; i < minionObject.buffList.Count; i++)
                    minionObject.buffList[i] = new Vector4(0, minionObject.buffList[i].y, minionObject.buffList[i].z, minionObject.buffList[i].w);
                break;
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
                break;
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.능력치부여:
                Vector4 buff = new Vector4((int)nowSpellAbility.Ability_data.x, (int)nowSpellAbility.Ability_data.y, (int)nowSpellAbility.Ability_data.z, 1);
                if (nowSpellAbility.Ability_type == SpellAbility.Ability.능력치부여)
                    buff -= new Vector4(0, 0, 0, 1);
                minionObject.buffList.Add(buff);
                minionObject.final_hp += (int)nowSpellAbility.Ability_data.y;
                break;
            case SpellAbility.Ability.능력부여:
                string ability_string = DataMng.instance.ToString((DataMng.TableType)nowSpellAbility.Ability_data.x, (int)nowSpellAbility.Ability_data.y, "명령어");
                List<MinionAbility> abilityList = MinionManager.instance.MinionAbilityParsing(ability_string);
                for (int i = 0; i < abilityList.Count; i++)
                    minionObject.abilityList.Add(abilityList[i]);
                break;
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                int left = minionObject.num - 1;
                int right = minionObject.num + 1;
                if (minionObject.enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (((EnemyMinionField.instance.minions[m].num == left && left != -1) ||
                            (EnemyMinionField.instance.minions[m].num == right && right < 7)) &&
                            EnemyMinionField.instance.minions[m].final_hp <= (int)minionObject.final_atk)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                            if ((EnemyMinionField.instance.minions[i].num == left && left != -1) || (EnemyMinionField.instance.minions[i].num == right && right < 7))
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[i].damageEffect, (int)minionObject.final_atk);
                    AttackManager.instance.AttackEffectRun();

                    EnemyMinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = true;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(MinionField.instance.minions[m]);
                        else if (((MinionField.instance.minions[m].num == left && left != -1) || 
                            (MinionField.instance.minions[m].num == right && right < 7)) &&
                            MinionField.instance.minions[m].final_hp <= (int)minionObject.final_atk)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                            if ((MinionField.instance.minions[i].num == left && left != -1) || (MinionField.instance.minions[i].num == right && right < 7))
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[i].damageEffect, (int)minionObject.final_atk);
                    AttackManager.instance.AttackEffectRun();

                    MinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                break;
        }
    }

    #region[미니언위치 세팅]
    bool reArrangementEnemy = false;
    public void SetMinionPos()
    {
        if (reArrangementEnemy)
            EnemyMinionField.instance.setMinionPos = true;
        else
            MinionField.instance.setMinionPos = true;
    }
    public void ReArrangement()
    {
        int pos = 0;
        //survivalList.Sort((a, b) => (a.num > b.num) ? +1 : -1);
        if (reArrangementEnemy)
        {
            for (int m = 0; m < survivalList.Count; m++)
            {
                EnemyMinionField.instance.minions[pos] = survivalList[m];
                pos++;
            }
            for (int m = 0; m < deathList.Count; m++)
            {
                EnemyMinionField.instance.minions[pos] = deathList[m];
                pos++;
            }
            for (int m = 0; m < emptyList.Count; m++)
            {
                EnemyMinionField.instance.minions[pos] = emptyList[m];
                pos++;
            }
        }
        else
        {
            for (int m = 0; m < survivalList.Count; m++)
            {
                MinionField.instance.minions[pos] = survivalList[m];
                pos++;
            }
            for (int m = 0; m < deathList.Count; m++)
            {
                MinionField.instance.minions[pos] = deathList[m];
                pos++;
            }
            for (int m = 0; m < emptyList.Count; m++)
            {
                MinionField.instance.minions[pos] = emptyList[m];
                pos++;
            }
        }
    }
    #endregion

    public void HeroSelect(bool enemy)
    {
        if (!MinionManager.instance.selectMinionEvent)
            return;
        selectSpellEvent = false;
        targetHero = enemy ? 2 : 1;
        MinionManager.instance.selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (nowSpellAbility.Ability_type)
        {
            case SpellAbility.Ability.빙결시키기:
                HeroManager.instance.SetFreeze(enemy);
                break;
            case SpellAbility.Ability.적에게피해주기:
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
                if (enemy)
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)nowSpellAbility.Ability_data.x);
                else
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)nowSpellAbility.Ability_data.x);
                AttackManager.instance.AttackEffectRun();
                break;
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
                break;
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.영웅의_생명력회복:
                if (enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp += (int)nowSpellAbility.Ability_data.x;
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp += (int)nowSpellAbility.Ability_data.x;
                break;
            case SpellAbility.Ability.영웅의_생명력설정:
                if (enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp = (int)nowSpellAbility.Ability_data.x;
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp = (int)nowSpellAbility.Ability_data.x;
                break;
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                if (enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= (int)nowSpellAbility.Ability_data.x)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);
                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[i].damageEffect, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();

                    EnemyMinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = true;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                else
                {
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(MinionField.instance.minions[m]);
                        else if (MinionField.instance.minions[m].final_hp <= (int)nowSpellAbility.Ability_data.x)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);
                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[i].damageEffect, (int)nowSpellAbility.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();

                    MinionField.instance.setMinionPos = false;
                    GameEventManager.instance.EventSet(2f);
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);
                }
                break;
        }
    }
    #endregion

    #region[대상 선택 취소]
    bool spellSelectCancle = false;
    public void MinionSelectCancle()
    {
        if (!MinionManager.instance.selectMinionEvent)
            return;
        MinionManager.instance.selectMinionEvent = false;
        selectSpellEvent = false;
        spellSelectCancle = true;
        GameEventManager.instance.EventAdd(1.4f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(nowSpellName));
        int mana = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
        string cardName = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드이름");
        int n = CardHand.instance.nowHandNum;
        CardHand.instance.DrawCard();
        CardHand.instance.CardMove(cardName, n,BattleUI.instance.playerSpellPos.transform.position, new Vector2(10.685f, 13.714f), 0);
        CardViewManager.instance.UpdateCardView(0.001f);
        ManaManager.instance.playerNowMana += mana;
        CardHand.instance.useCardNum--;

    }
    #endregion

    [HideInInspector] public int selectChoose;
    [HideInInspector] public SpellAbility nowSpellAbility;
    [HideInInspector] public string nowSpellName;
    [HideInInspector] public bool selectSpellEvent;

    private IEnumerator SpellEvent(List<SpellAbility> spellList,bool enemy)
    {
        while (GameEventManager.instance.GetEventValue() > 0.1f)
            yield return new WaitForSeconds(0.001f);
        GameEventManager.instance.EventAdd(0.1f);
        spellList.Sort((a, b) => 
        {
            if (a.Condition_type > b.Condition_type)
                return 1;
            else
                return -1;
        });

        bool checkCombo = false;
        List<SpellAbility> chooseOneList = new List<SpellAbility>();
        for(int i = 0; i < spellList.Count; i++)
        {
            List<SpellAbility> nowEvent = new List<SpellAbility>();

            if (spellList[i].Condition_type == SpellAbility.Condition.선택)
                chooseOneList.Add(spellList[i]);
            else if (CheckCombo() && spellList[i].Condition_type == SpellAbility.Condition.연계)
                checkCombo = true;
            else if (checkCombo && spellList[i].Condition_type == SpellAbility.Condition.연계시_작동안함)
                continue;

            #region[조건에 따른 주문처리]
            if (spellList[i].Condition_type == SpellAbility.Condition.선택)
            {
                if (i + 1 >= spellList.Count || spellList[i + 1].Condition_type != SpellAbility.Condition.선택)
                {
                    List<Vector3> chooseOneData = new List<Vector3>();
                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        Vector3 data = chooseAbility.Condition_data;
                        if (!chooseOneData.Contains(data))
                            chooseOneData.Add(data);
                    }

                    BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

                    for (int j = 0; j < 2; j++)
                    {
                        string ChooseName = DataMng.instance.ToString((DataMng.TableType)chooseOneData[j].x, (int)chooseOneData[j].y, "카드이름");
                        CardViewManager.instance.CardShow(ref BattleUI.instance.chooseCardView[j], ChooseName);
                        CardViewManager.instance.UpdateCardView(0.001f);
                    }

                    selectChoose = -1;

                    while (selectChoose == -1)
                    {
                        GameEventManager.instance.EventSet(1f);
                        yield return new WaitForSeconds(0.001f);
                    }

                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        if (chooseAbility.Condition_data.y == chooseOneData[selectChoose].y)
                            nowEvent.Add(chooseAbility);
                    }
                }
            }
            else if (CheckCombo() && spellList[i].Condition_type == SpellAbility.Condition.연계)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (!checkCombo && spellList[i].Condition_type == SpellAbility.Condition.연계시_작동안함)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].Condition_type == SpellAbility.Condition.조건없음)
            {
                nowEvent.Add(spellList[i]);
            }
            #endregion

            foreach (SpellAbility ability in nowEvent)
            {
                if (spellSelectCancle)
                    break;
                yield return new WaitForSeconds(0.25f);
                if (CheckEvent(ability) == EventType.대상선택)
                {
                    if (targetMinion == null && targetHero == -1)
                    {
                        SetSelectMask(ability.Ability_type);

                        bool targetExistence = false;
                        selectSpellEvent = true;

                        for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                            if ((DragLineRenderer.instance.CheckMask(타겟.아군하수인) && !MinionManager.instance.minionList[m].enemy) ||
                                (DragLineRenderer.instance.CheckMask(타겟.적하수인) && MinionManager.instance.minionList[m].enemy))
                                if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                                    targetExistence = targetExistence || CheckConditionMinion(MinionManager.instance.minionList[m], ability);

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
                            MinionManager.instance.selectMinionEvent = true;
                            nowSpellAbility = ability;
                            BattleUI.instance.grayFilterAni.SetBool("On", true);
                            BattleUI.instance.selectMinion.gameObject.SetActive(true);
                            DragLineRenderer.instance.activeObj = BattleUI.instance.playerSpellPos;
                            BattleUI.instance.selectMinionTxt.text = GetText(ability.Ability_type);

                            while (MinionManager.instance.selectMinionEvent)
                            {
                                GameEventManager.instance.EventSet(1f);
                                yield return new WaitForSeconds(0.001f);
                            }
                        }
                    }
                    else
                    {
                        nowSpellAbility = ability;
                        MinionManager.instance.selectMinionEvent = true;
                        if (targetMinion != null)
                            MinionSelect(targetMinion);
                        else if (targetHero != -1)
                            HeroSelect(targetHero == 2);
                    }
                }
                else if (CheckEvent(ability) == EventType.카드뽑기)
                {
                    for (int draw = 0; draw < ability.Ability_data.x; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((enemy && (ability.Ability_data.y == 0)) || (!enemy && (ability.Ability_data.y == 1)))
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
                else if(CheckEvent(ability) == EventType.아군광역피해)
                {
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                    AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                        AttackManager.instance.AttackEffectRun();
                        EnemyMinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = true;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (!MinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(MinionField.instance.minions[m]);
                            else if (MinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                    AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                        AttackManager.instance.AttackEffectRun();
                        MinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = false;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                }
                else if (CheckEvent(ability) == EventType.적군광역피해)
                {
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (!enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                        AttackManager.instance.AttackEffectRun();
                        EnemyMinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = true;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (!MinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(MinionField.instance.minions[m]);
                            else if (MinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                        AttackManager.instance.AttackEffectRun();
                        MinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = false;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                }
                else if (CheckEvent(ability) == EventType.광역피해)
                {
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();
                    EnemyMinionField.instance.setMinionPos = false;
                    reArrangementEnemy = true;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);

                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (!MinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(MinionField.instance.minions[m]);
                        else if (MinionField.instance.minions[m].final_hp <= (int)ability.Ability_data.x)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, (int)ability.Ability_data.x);
                    AttackManager.instance.AttackEffectRun();
                    MinionField.instance.setMinionPos = false;
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);

                    GameEventManager.instance.EventSet(2f);
                }
                else if(CheckEvent(ability) == EventType.하수인소환)
                {
                    //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                    bool enemyFlag = (int)ability.Ability_data.z == 1 ? false : true;
                    string minion_name = DataMng.instance.ToString((DataMng.TableType)ability.Ability_data.x, (int)ability.Ability_data.y, "카드이름");
                    string minion_ability = DataMng.instance.ToString((DataMng.TableType)ability.Ability_data.x, (int)ability.Ability_data.y, "명령어");
                    if ((enemyFlag && !enemy) || (!enemyFlag && enemy))
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                        if (minion_name.Equals("나무정령"))
                        {
                            yield return new WaitForSeconds(0.5f);
                            EnemyMinionField.instance.minions[index].abilityList.Clear();
                            EnemyMinionField.instance.minions[index].abilityList = MinionManager.instance.MinionAbilityParsing(minion_ability);
                            MinionManager.instance.BaseMinionAbility(EnemyMinionField.instance.minions[index]);
                        }
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                        if (minion_name.Equals("나무정령"))
                        {
                            yield return new WaitForSeconds(0.5f);
                            MinionField.instance.minions[index].abilityList.Clear();
                            MinionField.instance.minions[index].abilityList = MinionManager.instance.MinionAbilityParsing(minion_ability);
                            MinionManager.instance.BaseMinionAbility(MinionField.instance.minions[index]);
                        }
                    }             
                }
                else if(CheckEvent(ability) == EventType.하수인들에게_은신부여)
                {
                    if(enemy)
                    {
                        for(int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                EnemyMinionField.instance.minions[m].stealth = true;
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                MinionField.instance.minions[m].stealth = true;
                    }
                }
                else if (CheckEvent(ability) == EventType.하수인들에게_능력부여)
                {
                    string ability_string = DataMng.instance.ToString((DataMng.TableType)ability.Ability_data.x, (int)ability.Ability_data.y, "명령어");
                    List<MinionAbility> abilityList = MinionManager.instance.MinionAbilityParsing(ability_string);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                for (int a = 0; a < abilityList.Count; a++)
                                    EnemyMinionField.instance.minions[m].abilityList.Add(abilityList[a]);
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                for (int a = 0; a < abilityList.Count; a++)
                                    MinionField.instance.minions[m].abilityList.Add(abilityList[a]);
                    }
                }
                else if(CheckEvent(ability) == EventType.하수인들에게_능력치부여)
                {
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                EnemyMinionField.instance.minions[m].nowAtk += (int)ability.Ability_data.x;
                                EnemyMinionField.instance.minions[m].final_hp += (int)ability.Ability_data.y;
                                EnemyMinionField.instance.minions[m].nowSpell += (int)ability.Ability_data.z;
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                MinionField.instance.minions[m].nowAtk += (int)ability.Ability_data.x;
                                MinionField.instance.minions[m].final_hp += (int)ability.Ability_data.y;
                                MinionField.instance.minions[m].nowSpell += (int)ability.Ability_data.z;
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.마나획득)
                {
                    if (enemy)
                        ManaManager.instance.enemyNowMana += (int)ability.Ability_data.x;
                    else
                        ManaManager.instance.playerNowMana += (int)ability.Ability_data.x;
                }
                else if (CheckEvent(ability) == EventType.마나수정획득)
                {
                    if (enemy)
                        ManaManager.instance.enemyMaxMana += (int)ability.Ability_data.x;
                    else
                    {
                        if (ManaManager.instance.playerMaxMana >= 10)
                        {
                            GameEventManager.instance.EventAdd(1.4f);
                            Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName("넘치는마나"));
                            string cardName = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드이름");
                            int n = CardHand.instance.nowHandNum;
                            CardHand.instance.DrawCard();
                            CardHand.instance.CardMove(cardName, n, BattleUI.instance.playerSpellPos.transform.position, new Vector2(10.685f, 13.714f), 0);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }
                        else
                        {
                            ManaManager.instance.playerMaxMana += (int)ability.Ability_data.x;
                        }
                    }
                }
                else if (CheckEvent(ability) == EventType.모든하수인주인의패로)
                {
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                            MinionManager.instance.minionList[m].gotoHandTrigger = true;
                }
                else if (CheckEvent(ability) == EventType.모든하수인처치)
                {
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                            MinionManager.instance.minionList[m].animator.SetTrigger("Death");

                    yield return new WaitForSeconds(1.25f);

                    GameEventManager.instance.EventSet(2.5f);

                    #region[아군필드상황정리]
                    List<MinionObject> removeMinionList = new List<MinionObject>();
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        removeMinionList.Add(MinionField.instance.minions[m]);

                    int pos = 0;
                    for (int m = 0; m < removeMinionList.Count; m++)
                    {
                        MinionField.instance.minions[pos] = removeMinionList[m];
                        MinionManager.instance.DeathMinionAbility(MinionField.instance.minions[pos]);
                        pos++;
                    }
                    MinionField.instance.minionNum = 0;
                    removeMinionList.Clear();
                    #endregion

                    #region[적군필드상황정리]
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                    {
                        MinionManager.instance.DeathMinionAbility(EnemyMinionField.instance.minions[m]);
                        EnemyMinionField.instance.minions[m].MinionRemoveProcess();
                    }
                    EnemyMinionField.instance.minionNum = 0;
                    #endregion
                }
                else if (CheckEvent(ability) == EventType.방어도획득)
                {
                    if (enemy)
                    {
                        if (HeroManager.instance.heroHpManager.enemyShield < 0)
                            HeroManager.instance.heroHpManager.enemyShield = 0;
                        HeroManager.instance.heroHpManager.enemyShield += (int)ability.Ability_data.x;
                        HeroManager.instance.heroHpManager.enemyShieldAni.SetBool("Break", false);
                    }
                    else
                    {
                        if (HeroManager.instance.heroHpManager.playerShield < 0)
                            HeroManager.instance.heroHpManager.playerShield = 0;
                        HeroManager.instance.heroHpManager.playerShield += (int)ability.Ability_data.x;
                        HeroManager.instance.heroHpManager.playerShieldAni.SetBool("Break", false);
                    }
                }
                else if (CheckEvent(ability) == EventType.공격력획득)
                {
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyAtk += (int)ability.Ability_data.x;
                    else
                        HeroManager.instance.heroAtkManager.playerAtk += (int)ability.Ability_data.x;
                }
            }
         
        }

        selectSpellEvent = false;

    }
}
