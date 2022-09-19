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
        적영웅에게피해,
        적군광역피해, 아군광역피해, 광역피해,
        모든하수인주인의패로, 모든하수인처치,
        하수인소환,
        하수인들에게_은신부여,
        하수인들에게_능력부여,
        하수인들에게_능력치부여,
        하수인들에게_해당턴_능력치부여,
        마나획득, 마나수정획득,
        공격력획득, 방어도획득,
        무기장착, 무기공격력부여,
        무기파괴,무기공격력만큼적군광역피해,
        다음주문카드비용감소,
        내손으로다시가져오기
    }

    public void Awake()
    {
        instance = this;
    }

    public int playerSpellPower = 0;
    public int enemySpellPower = 0;

    public void Update()
    {
        int temp = 0;
        for (int m = 0; m < MinionField.instance.minions.Length; m++)
            if (MinionField.instance.minions[m].gameObject.activeSelf)
                temp += MinionField.instance.minions[m].final_spellAtk;
        playerSpellPower = temp;

        temp = 0;
        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                temp += EnemyMinionField.instance.minions[m].final_spellAtk;
        enemySpellPower = temp;
    }

    #region[스펠 능력 파싱]
    public List<SpellAbility> SpellParsing(string ability_string)
    {
        List<SpellAbility> abilityList = new List<SpellAbility>();

        //괄호기준으로 분할
        string[] aSplit = ability_string.Split('[', ']');
        List<string> abilityData = new List<string>();
        for (int i = 0; i < aSplit.Length; i++)
        {
            if(string.IsNullOrEmpty(aSplit[i]))
            {
                continue;
            }
            abilityData.Add(aSplit[i]);
        }


        for(int i = 0; i < abilityData.Count; i++)
        {
            //명령어를 게임에 쓸수 있도록 변환

            //주문능력 객체를 생성
            SpellAbility newAbility = new SpellAbility();

            //조건을 확인
            SpellAbility.Condition condition = SpellAbility.GetCondition(abilityData[i]);
            newAbility.ConditionType = condition;

            //조건 명령어의 파라미터 갯수를 가져온다.
            int cparaNum = SpellAbility.GetParameterNum(condition);
            for (int j = 1; j <= cparaNum; j++)
            {
                //파라미터를 대입
                int value = int.Parse(abilityData[i + j]);
                newAbility.ConditionData[j - 1] = value;
            }
            i += cparaNum;

            //능력을 확인
            SpellAbility.Ability ability = SpellAbility.GetAbility(abilityData[i + 1]);
            newAbility.AbilityType = ability;

            //능력 명령어의 파라미터 갯수를 가져온다.
            int aparaNum = SpellAbility.GetParameterNum(ability);
            for (int j = 1; j <= aparaNum; j++)
            {
                //파라미터를 대입
                int value = int.Parse(abilityData[i + 1 + j]);
                newAbility.AbilityData[j - 1] = value;
            }
            i += aparaNum + 1;
            abilityList.Add(newAbility);
        }

        abilityList.Sort((a, b) =>
        {
            if (a.ConditionType > b.ConditionType)
                return 1;
            else if (a.ConditionType < b.ConditionType)
                return -1;
            else
            {
                if (a.AbilityType == SpellAbility.Ability.무기장착)
                    return +1;
                else
                    return -1;
            }
        });
        return abilityList;
    }
    #endregion

    #region[대상이 아닌]
    public void RunSpell(string name, bool enemy = false)
    {
        RunSpell(name, enemy, false);
    }

    public void RunSpell(string name, bool enemy, bool heroPower)
    {
        targetMinion = null;
        targetHero = -1;
        spellSelectCancle = false;
        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(name));
        string ability_string = DataMng.instance.ToString(pair.x, pair.y, "명령어");
        List<SpellAbility> spellList = SpellParsing(ability_string);
        nowSpellName = name;
        StartCoroutine(SpellEvent(spellList, enemy, heroPower));
    }

    [HideInInspector] public int selectChoose;
    [HideInInspector] public SpellAbility nowSpellAbility;
    [HideInInspector] public string nowSpellName;
    [HideInInspector] public bool selectSpellEvent;

    private IEnumerator SpellEvent(List<SpellAbility> spellList, bool enemy,bool heroPower)
    {
        yield return new WaitWhile(() => GameEventManager.instance.GetEventValue() > 0.1f);
        GameEventManager.instance.EventAdd(0.1f);

        bool checkCombo = false;
        bool checkPreparation = false;

        List<SpellAbility> chooseOneList = new List<SpellAbility>();
        for (int i = 0; i < spellList.Count; i++)
        {
            List<SpellAbility> nowEvent = new List<SpellAbility>();

            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
                chooseOneList.Add(spellList[i]);
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
                checkCombo = true;
            else if (checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
                continue;

            #region[조건에 따른 주문처리]
            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
            {
                if (i + 1 >= spellList.Count || spellList[i + 1].ConditionType != SpellAbility.Condition.선택)
                {
                    List<ParaData> chooseOneData = new List<ParaData>();
                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        ParaData data = chooseAbility.ConditionData;
                        if (!chooseOneData.Contains(data))
                            chooseOneData.Add(data);
                    }

                    if (enemy)
                    {
                        selectChoose = DruidAI.instance.AI_ChoiceSelect(nowSpellName);
                    }
                    else
                    {

                        BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

                        for (int j = 0; j < 2; j++)
                        {
                            string ChooseName = DataMng.instance.ToString(chooseOneData[j][0], chooseOneData[j][1], "카드이름");
                            CardViewManager.instance.CardShow(ref BattleUI.instance.chooseCardView[j], ChooseName);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }

                        selectChoose = -1;

                        while (selectChoose == -1)
                        {
                            GameEventManager.instance.EventSet(1f);
                            yield return new WaitForSeconds(0.001f);
                        }
                    }

                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        if (chooseAbility.ConditionData[1] == chooseOneData[selectChoose][1])
                            nowEvent.Add(chooseAbility);
                    }
                }
            }
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (!checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.조건없음)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.피해입지않은하수인)
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
                        SetSelectMask(ability.AbilityType);

                        bool targetExistence = false;
                        selectSpellEvent = true;

                        if(enemy)
                        {
                            for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                                if ((DragLineRenderer.instance.CheckMask(타겟.아군하수인) && MinionManager.instance.minionList[m].enemy) ||
                                    (DragLineRenderer.instance.CheckMask(타겟.적하수인) && !MinionManager.instance.minionList[m].enemy))
                                    if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                                        targetExistence = targetExistence || CheckConditionMinion(MinionManager.instance.minionList[m], ability);
                        }
                        else
                        {
                            for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                                if ((DragLineRenderer.instance.CheckMask(타겟.아군하수인) && !MinionManager.instance.minionList[m].enemy) ||
                                    (DragLineRenderer.instance.CheckMask(타겟.적하수인) && MinionManager.instance.minionList[m].enemy))
                                    if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                                        targetExistence = targetExistence || CheckConditionMinion(MinionManager.instance.minionList[m], ability);
                        }

                        if (DragLineRenderer.instance.CheckMask(타겟.아군영웅))
                            targetExistence = true;
                        if (DragLineRenderer.instance.CheckMask(타겟.적영웅))
                            targetExistence = true;
                        if (DragLineRenderer.instance.CheckMask(타겟.실행주체))
                            targetExistence = true;

                        if (targetExistence)
                        {
                            if(enemy)
                            {
                                DruidAI.instance.targetMinionNum = -2;
                                nowSpellAbility = ability;
                                DruidAI.instance.AI_Select(nowSpellAbility);
                            }
                            else
                            {
                                if (DragLineRenderer.instance.CheckMask(타겟.아군영웅) || DragLineRenderer.instance.CheckMask(타겟.적영웅))
                                    CardHand.instance.handAni.SetTrigger("축소");
                                MinionManager.instance.selectMinionEvent = true;
                                nowSpellAbility = ability;
                                BattleUI.instance.grayFilterAni.SetBool("On", true);
                                BattleUI.instance.selectMinion.gameObject.SetActive(true);
                                DragLineRenderer.instance.activeObj = BattleUI.instance.playerSpellPos;
                                BattleUI.instance.selectMinionTxt.text = GetText(ability.AbilityType);

                                while (MinionManager.instance.selectMinionEvent)
                                {
                                    GameEventManager.instance.EventSet(1f);
                                    yield return new WaitForSeconds(0.001f);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (enemy)
                        {
                            nowSpellAbility = ability;
                            DruidAI.instance.AI_Select(nowSpellAbility);
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
                }
                else if (CheckEvent(ability) == EventType.카드뽑기)
                {
                    for (int draw = 0; draw < ability.AbilityData[0]; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((enemy && (ability.AbilityData[1] == 0)) || (!enemy && (ability.AbilityData[1] == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (CheckEvent(ability) == EventType.아군광역피해)
                {
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= 
                                ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, 
                                    ability.AbilityData[0] + enemySpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (!enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(
                                    EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(
                                    MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + enemySpellPower);
                        AttackManager.instance.AttackEffectRun();
                        MinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = false;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                }
                else if (CheckEvent(ability) == EventType.적영웅에게피해)
                {
                    if (nowSpellName == "사악한 일격")
                    {
                        if (!enemy)
                        {
                            EffectManager.instance.CutEffect(HeroManager.instance.enemyHero.transform.position, new Vector2(+1, 1));
                            EffectManager.instance.CutEffect(HeroManager.instance.enemyHero.transform.position, new Vector2(-1, 1));
                        }
                        else
                        {
                            EffectManager.instance.CutEffect(HeroManager.instance.playerHero.transform.position, new Vector2(+1, 1));
                            EffectManager.instance.CutEffect(HeroManager.instance.playerHero.transform.position, new Vector2(-1, 1));
                        }
                        GameEventManager.instance.EventAdd(1f);
                        yield return new WaitForSeconds(1f);
                    }

                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    AttackManager.instance.PopAllDamageObj();
                    if (!enemy)
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, ability.AbilityData[0] + playerSpellPower);
                    else
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, ability.AbilityData[0] + enemySpellPower);
                    AttackManager.instance.AttackEffectRun();
                }
                else if (CheckEvent(ability) == EventType.광역피해)
                {
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= 
                            ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                        else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AttackEffectRun();
                    MinionField.instance.setMinionPos = false;
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);

                    GameEventManager.instance.EventSet(2f);
                }
                else if (CheckEvent(ability) == EventType.하수인소환)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                    bool enemyFlag =( ability.AbilityData[2] == 1) ? false : true;
                    string minion_name = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                    string minion_ability = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
                    if ((enemyFlag && !enemy) || (!enemyFlag && enemy))
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                    }
                }
                else if (CheckEvent(ability) == EventType.하수인들에게_은신부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
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
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    string ability_string = 
                        DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
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
                else if (CheckEvent(ability) == EventType.하수인들에게_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                EnemyMinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                EnemyMinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                MinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                MinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.마나획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (nowSpellName == "정신 자극")
                    {
                        EffectManager.instance.SpiralEffect(enemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                        GameEventManager.instance.EventSet(1f);
                    }
                    else if (nowSpellName == "육성")
                    {
                        EffectManager.instance.SpiralEffect(enemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                        GameEventManager.instance.EventSet(1f);
                    }

                    if (enemy)
                        ManaManager.instance.enemyNowMana += ability.AbilityData[0];
                    else
                        ManaManager.instance.playerNowMana += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.마나수정획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (nowSpellName == "급속 성장")
                    {
                        EffectManager.instance.SpiralEffect(enemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                        GameEventManager.instance.EventSet(1f);
                    }

                    if (enemy)
                    {
                        if (ManaManager.instance.enemyMaxMana >= 10)
                        {
                            GameEventManager.instance.EventAdd(1.4f);
                            Vector2Int pair = DataMng.instance.GetPairByName(
                                DataParse.GetCardName("넘치는마나"));
                            string cardName = DataMng.instance.ToString(pair.x, pair.y, "카드이름");
                            int n = EnemyCardHand.instance.nowHandNum;
                            EnemyCardHand.instance.DrawCard();
                            EnemyCardHand.instance.nowCard.Add(cardName);
                            EnemyCardHand.instance.CardMove(n, BattleUI.instance.enemySpellPos.transform.position, EnemyCardHand.instance.defaultSize, 180);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }
                        else
                        {
                            ManaManager.instance.enemyMaxMana += ability.AbilityData[0];
                        }
                    }
                    else
                    {
                        if (ManaManager.instance.playerMaxMana >= 10)
                        {
                            GameEventManager.instance.EventAdd(1.4f);
                            Vector2Int pair = DataMng.instance.GetPairByName(
                               DataParse.GetCardName("넘치는마나"));
                            string cardName = DataMng.instance.ToString(pair.x, pair.y, "카드이름");
                            int n = CardHand.instance.nowHandNum;
                            CardHand.instance.DrawCard();
                            CardHand.instance.SetCardHand(cardName, n, BattleUI.instance.playerSpellPos.transform.position,
                                CardHand.instance.defaultSize, 0);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }
                        else
                        {
                            ManaManager.instance.playerMaxMana += ability.AbilityData[0];
                        }
                    }
                }
                else if (CheckEvent(ability) == EventType.모든하수인주인의패로)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                        {
                            MinionManager.instance.minionList[m].gotoHandTrigger = true;
                        }

                }
                else if (CheckEvent(ability) == EventType.모든하수인처치)
                {
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                            MinionManager.instance.minionList[m].animator.SetTrigger("Death");

                    yield return new WaitForSeconds(1.25f);
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);

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
                else if (CheckEvent(ability) == EventType.하수인들에게_해당턴_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                EnemyMinionField.instance.minions[m].buffList.Add(buff);
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                MinionField.instance.minions[m].buffList.Add(buff);
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.방어도획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        if (HeroManager.instance.heroHpManager.enemyShield < 0)
                            HeroManager.instance.heroHpManager.enemyShield = 0;
                        HeroManager.instance.heroHpManager.enemyShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.enemyShieldAni.SetBool("Break", false);
                    }
                    else
                    {
                        if (HeroManager.instance.heroHpManager.playerShield < 0)
                            HeroManager.instance.heroHpManager.playerShield = 0;
                        HeroManager.instance.heroHpManager.playerShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.playerShieldAni.SetBool("Break", false);
                    }
                }
                else if (CheckEvent(ability) == EventType.공격력획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyAtk += ability.AbilityData[0];
                    else
                        HeroManager.instance.heroAtkManager.playerAtk += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.무기장착)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    GameEventManager.instance.EventAdd(1f);
                    if (enemy)
                    {
                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.enemyWeaponName = weapon_name;
                        HeroManager.instance.SetEnemyDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk = weaponAtk;
                    }
                    else
                    {
                        if (CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                            CardHand.instance.handAni.SetTrigger("축소");
                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");

                        DragCardObject.instance.ShowDragCard(weapon_name);
                        Vector2 v = Camera.main.WorldToScreenPoint(HeroManager.instance.heroAtkManager.playerWeapon.transform.position - new Vector3(0, 50, 0));
                        DragCardObject.instance.ShowDropEffecWeapon(v, 0);

                        yield return new WaitWhile(() => !DragCardObject.instance.dropEffect.effectArrive);


                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.playerWeaponName = weapon_name;
                        HeroManager.instance.SetPlayerDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.playerWeaponAtk = weaponAtk;

                    }
                }
                else if (CheckEvent(ability) == EventType.무기공격력부여)
                {
                    if (nowSpellName == "맹독")
                    {
                        if (enemy)
                            EffectManager.instance.MagicEffect(HeroManager.instance.heroAtkManager.enemyWeapon.transform.position);
                        else
                            EffectManager.instance.MagicEffect(HeroManager.instance.heroAtkManager.playerWeapon.transform.position);
                        GameEventManager.instance.EventAdd(1f);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk += ability.AbilityData[0];
                    else
                    {
                        if (CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                            CardHand.instance.handAni.SetTrigger("축소");
                        HeroManager.instance.heroAtkManager.playerWeaponAtk += ability.AbilityData[0];
                    }
                }
                else if (CheckEvent(ability) == EventType.다음주문카드비용감소)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    checkPreparation = true;
                    CardHand.instance.nextSpellCostOffset = ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.무기파괴)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.SetEnemyDurability(0);
                    else
                    {
                        if(CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                            CardHand.instance.handAni.SetTrigger("축소");
                        HeroManager.instance.SetPlayerDurability(0);
                    }
                }
                else if (CheckEvent(ability) == EventType.무기공격력만큼적군광역피해)
                {
                    if (!enemy)
                        if (CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                            CardHand.instance.handAni.SetTrigger("축소");
                    if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (!enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= (int)HeroManager.instance.heroAtkManager.playerWeaponAtk + playerSpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, (int)HeroManager.instance.heroAtkManager.playerWeaponAtk + playerSpellPower);
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, (int)HeroManager.instance.heroAtkManager.playerWeaponAtk + playerSpellPower);

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
                            else if (MinionField.instance.minions[m].final_hp <= (int)HeroManager.instance.heroAtkManager.enemyWeaponAtk + enemySpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, (int)HeroManager.instance.heroAtkManager.enemyWeaponAtk + enemySpellPower);
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, (int)HeroManager.instance.heroAtkManager.playerWeaponAtk + enemySpellPower);

                        AttackManager.instance.AttackEffectRun();
                        MinionField.instance.setMinionPos = false;
                        GameEventManager.instance.EventSet(2f);
                        reArrangementEnemy = false;
                        ReArrangement();
                        Invoke("SetMinionPos", 1.25f);
                    }
                }
                else if (CheckEvent(ability) == EventType.내손으로다시가져오기)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (CardHand.instance.nowHandNum < 10)
                        for (int c = 0; c < BattleUI.instance.playerCardAni.Length; c++)
                        {
                            if (BattleUI.instance.playerCardAni[c].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                            {
                                CardHand.instance.DrawCard();
                                CardHand.instance.SetCardHand(nowSpellName, CardHand.instance.nowHandNum - 1, 
                                    CardHand.instance.drawCardPos.position, CardHand.instance.defaultSize, 0);
                                CardViewManager.instance.UpdateCardView();
                                break;
                            }
                        }
                }
            }

        }

        selectSpellEvent = false;

        if (!heroPower)
            for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                if (MinionManager.instance.minionList[m].gameObject.activeSelf && 
                    MinionManager.instance.minionList[m].enemy == enemy)
                    MinionManager.instance.minionList[m].spellRun = true;

        if(!enemy && !heroPower)
        {
            QuestManager.instance.SpellCard();
            Vector2 pair = DataMng.instance.GetPairByName(DataParse.GetCardName(nowSpellName));
            if (pair.x == 0)
                QuestManager.instance.CharacterCard(Job.드루이드);
            else if (pair.x == 1)
                QuestManager.instance.CharacterCard(Job.도적);
        }

        if (!checkPreparation && !heroPower)
            CardHand.instance.nextSpellCostOffset = 0;

    }
    #endregion

    #region[하수인대상 선택]
    public void RunSpellTargetMinion(string name,int handNum, MinionObject minionObject, bool enemy)
    {
        targetMinion = null;
        targetHero = -1;
        spellSelectCancle = false;
        nowSpellName = name;

        int cost = CardHand.instance.GetCardCost(handNum);
        ManaManager.instance.playerNowMana -= cost;

        CardHand.instance.useCardNum++;
        CardHand.instance.CardRemove(handNum);

        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(name));
        string ability_string = DataMng.instance.ToString(pair.x, pair.y, "명령어");
        List<SpellAbility> spellList = SpellParsing(ability_string);

        StartCoroutine(RunSpellTargetMinion(spellList, minionObject, enemy));
    }

    private IEnumerator RunSpellTargetMinion(List<SpellAbility> spellList,
        MinionObject minionObject,bool enemy)
    {
        bool checkCombo = false;
        List<SpellAbility> chooseOneList = new List<SpellAbility>();
        for (int i = 0; i < spellList.Count; i++)
        {
            List<SpellAbility> nowEvent = new List<SpellAbility>();

            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
                chooseOneList.Add(spellList[i]);
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
                checkCombo = true;
            else if (checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
                continue;

            #region[조건에 따른 주문처리]
            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
            {
                if (i + 1 >= spellList.Count || spellList[i + 1].ConditionType != SpellAbility.Condition.선택)
                {
                    List<ParaData> chooseOneData = new List<ParaData>();
                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        ParaData data = chooseAbility.ConditionData;
                        if (!chooseOneData.Contains(data))
                            chooseOneData.Add(data);
                    }

                    BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

                    for (int j = 0; j < 2; j++)
                    {
                        string ChooseName = DataMng.instance.ToString(chooseOneData[j][0], chooseOneData[j][1], "카드이름");
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
                        if (chooseAbility.ConditionData[1] == chooseOneData[selectChoose][1])
                            nowEvent.Add(chooseAbility);
                    }
                }
            }
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (!checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.조건없음)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.피해입지않은하수인)
            {
                nowEvent.Add(spellList[i]);
            }
            #endregion

            foreach (SpellAbility ability in nowEvent)
            {
                yield return new WaitForSeconds(0.25f);
                nowSpellAbility = ability;
                if (CheckEvent(ability) == EventType.대상선택)
                {
                    MinionManager.instance.selectMinionEvent = true;
                    selectSpellEvent = true;
                    MinionSelect(minionObject);
                }
                else if (CheckEvent(ability) == EventType.카드뽑기)
                {
                    for (int draw = 0; draw < ability.AbilityData[0]; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((enemy && (ability.AbilityData[1] == 0)) || (!enemy && (ability.AbilityData[1] == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (CheckEvent(ability) == EventType.아군광역피해)
                {
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + enemySpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (!enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + enemySpellPower);
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
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                        else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AttackEffectRun();
                    MinionField.instance.setMinionPos = false;
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);

                    GameEventManager.instance.EventSet(2f);
                }
                else if (CheckEvent(ability) == EventType.하수인소환)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                    bool enemyFlag = ability.AbilityData[2] == 1 ? false : true;
                    string minion_name = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                    string minion_ability = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
                    if ((enemyFlag && !enemy) || (!enemyFlag && enemy))
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                    }
                }
                else if (CheckEvent(ability) == EventType.하수인들에게_은신부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
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
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    string ability_string = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
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
                else if (CheckEvent(ability) == EventType.하수인들에게_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                EnemyMinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                EnemyMinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                MinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                MinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.마나획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        ManaManager.instance.enemyNowMana += ability.AbilityData[0];
                    else
                        ManaManager.instance.playerNowMana += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.마나수정획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        ManaManager.instance.enemyMaxMana += ability.AbilityData[0];
                    else
                    {
                        if (ManaManager.instance.playerMaxMana >= 10)
                        {
                            GameEventManager.instance.EventAdd(1.4f);
                            Vector2Int pair = DataMng.instance.GetPairByName(
                                DataParse.GetCardName("넘치는마나"));
                            string cardName = DataMng.instance.ToString(pair.x, pair.y, "카드이름");
                            int n = CardHand.instance.nowHandNum;
                            CardHand.instance.DrawCard();
                            CardHand.instance.SetCardHand(cardName, n, BattleUI.instance.playerSpellPos.transform.position,
                                CardHand.handCardSize, 0);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }
                        else
                        {
                            ManaManager.instance.playerMaxMana += ability.AbilityData[0];
                        }
                    }
                }
                else if (CheckEvent(ability) == EventType.모든하수인주인의패로)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                            MinionManager.instance.minionList[m].gotoHandTrigger = true;
                }
                else if (CheckEvent(ability) == EventType.모든하수인처치)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
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
                else if (CheckEvent(ability) == EventType.하수인들에게_해당턴_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                EnemyMinionField.instance.minions[m].buffList.Add(buff);
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                MinionField.instance.minions[m].buffList.Add(buff);
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.방어도획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        if (HeroManager.instance.heroHpManager.enemyShield < 0)
                            HeroManager.instance.heroHpManager.enemyShield = 0;
                        HeroManager.instance.heroHpManager.enemyShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.enemyShieldAni.SetBool("Break", false);
                    }
                    else
                    {
                        if (HeroManager.instance.heroHpManager.playerShield < 0)
                            HeroManager.instance.heroHpManager.playerShield = 0;
                        HeroManager.instance.heroHpManager.playerShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.playerShieldAni.SetBool("Break", false);
                    }
                }
                else if (CheckEvent(ability) == EventType.공격력획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyAtk += ability.AbilityData[0];
                    else
                        HeroManager.instance.heroAtkManager.playerAtk += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.무기장착)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    GameEventManager.instance.EventAdd(1f);
                    if (enemy)
                    {
                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.enemyWeaponName = weapon_name;
                        HeroManager.instance.SetEnemyDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk = weaponAtk;
                    }
                    else
                    {
                        CardHand.instance.handAni.SetTrigger("축소");
                        Vector2 v = Camera.main.WorldToScreenPoint(HeroManager.instance.heroAtkManager.playerWeapon.transform.position - new Vector3(0, 50, 0));
                        DragCardObject.instance.ShowDropEffecWeapon(v, 0);

                        yield return new WaitWhile(() => !DragCardObject.instance.dropEffect.effectArrive);

                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.playerWeaponName = weapon_name;
                        HeroManager.instance.SetPlayerDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.playerWeaponAtk = weaponAtk;

                    }
                }
                else if (CheckEvent(ability) == EventType.무기공격력부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk += ability.AbilityData[0];
                    else
                        HeroManager.instance.heroAtkManager.playerWeaponAtk += ability.AbilityData[0];
                }
            }
        }

        selectSpellEvent = false;

        for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
            if (MinionManager.instance.minionList[m].gameObject.activeSelf && MinionManager.instance.minionList[m].enemy == enemy)
                MinionManager.instance.minionList[m].spellRun = true;

        if (!enemy)
        {
            QuestManager.instance.SpellCard();
            Vector2 pair = DataMng.instance.GetPairByName(DataParse.GetCardName(nowSpellName));
            if (pair.x == 0)
                QuestManager.instance.CharacterCard(Job.드루이드);
            else if (pair.x == 1)
                QuestManager.instance.CharacterCard(Job.도적);
        }

        CardHand.instance.nextSpellCostOffset = 0;
    }
    #endregion

    #region[영웅대상선택]
    public void RunSpellTargetHero(string name, int handNum, bool runHero, bool tarHero)
    {
        targetMinion = null;
        targetHero = -1;
        spellSelectCancle = false;

        //명령어 데이터를 읽고 파싱한다.
        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(name));
        string ability_string = DataMng.instance.ToString(pair.x, pair.y, "명령어");
        List<SpellAbility> spellList = SpellParsing(ability_string);

        //카드이름을 얻고 카드의 비용을 구한다.
        nowSpellName = name;
        int cost = CardHand.instance.GetCardCost(handNum);

        //사용처리
        ManaManager.instance.playerNowMana -= cost;
        CardHand.instance.useCardNum++;
        CardHand.instance.CardRemove(handNum);

        //카드효과 실행
        StartCoroutine(RunSpellTargetHero(spellList, runHero, tarHero));
    }

    private IEnumerator RunSpellTargetHero(List<SpellAbility> spellList, bool runHero,bool tarHero)
    {
        bool enemy = runHero;
        yield return new WaitWhile(() => GameEventManager.instance.GetEventValue() > 0.1);
        GameEventManager.instance.EventAdd(0.1f);

        bool checkCombo = false;
        List<SpellAbility> chooseOneList = new List<SpellAbility>();
        for (int i = 0; i < spellList.Count; i++)
        {
            //현재 작동이 가능한 이벤트 리스트
            List<SpellAbility> nowEvent = new List<SpellAbility>();

            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
                chooseOneList.Add(spellList[i]);
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
                checkCombo = true;
            else if (checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
                continue;

            #region[조건에 따른 주문처리]
            if (spellList[i].ConditionType == SpellAbility.Condition.선택)
            {
                if (i == spellList.Count - 1 || //현재가 가장 끝이거나
                    spellList[i + 1].ConditionType != SpellAbility.Condition.선택) //햔제 항목이 마지막 선택 항목인 경우
                {
                    //더이상의 선택 항목이 없다고 판단
                    List<ParaData> chooseOneData = new List<ParaData>();
                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        ParaData data = chooseAbility.ConditionData;
                        if (!chooseOneData.Contains(data))
                            chooseOneData.Add(data);
                    }

                    BattleUI.instance.chooseOneDruid.SetBool("Hide", false);

                    for (int j = 0; j < chooseOneData.Count; j++)
                    {
                        string ChooseName = DataMng.instance.ToString(chooseOneData[j][0], chooseOneData[j][1], "카드이름");
                        CardViewManager.instance.CardShow(ref BattleUI.instance.chooseCardView[j], ChooseName);
                        CardViewManager.instance.UpdateCardView(0.001f);
                    }

                    selectChoose = -1;

                    while (selectChoose == -1)
                    {
                        GameEventManager.instance.EventSet(1f);
                        yield return new WaitForSeconds(0.001f);
                    }

                    //선택이 끝났다면
                    foreach (SpellAbility chooseAbility in chooseOneList)
                    {
                        if (chooseAbility.ConditionData[1] == chooseOneData[selectChoose][1])
                        {
                            nowEvent.Add(chooseAbility);
                            break;
                        }
                    }
                }
            }
            else if (CheckCombo() && spellList[i].ConditionType == SpellAbility.Condition.연계)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (!checkCombo && spellList[i].ConditionType == SpellAbility.Condition.연계시_작동안함)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.조건없음)
            {
                nowEvent.Add(spellList[i]);
            }
            else if (spellList[i].ConditionType == SpellAbility.Condition.피해입지않은하수인)
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
                    MinionManager.instance.selectMinionEvent = true;
                    selectSpellEvent = true;
                    nowSpellAbility = ability;
                    HeroSelect(tarHero);
                }
                else if (CheckEvent(ability) == EventType.카드뽑기)
                {
                    for (int draw = 0; draw < ability.AbilityData[0]; draw++)
                    {
                        //0이면 발동한 플레이어가 뽑고 
                        //1이면 상대플레이어가 카드를 뽑음
                        if ((enemy && (ability.AbilityData[1] == 0)) || (!enemy && (ability.AbilityData[1] == 1)))
                            EnemyCardHand.instance.DrawCard();
                        else
                            CardHand.instance.CardDrawAct();
                        SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                        GameEventManager.instance.EventAdd(0.5f);
                        yield return new WaitForSeconds(0.5f);
                    }
                }
                else if (CheckEvent(ability) == EventType.아군광역피해)
                {
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(
                                    EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + enemySpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    if (!enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                emptyList.Add(EnemyMinionField.instance.minions[m]);
                            else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                                deathList.Add(EnemyMinionField.instance.minions[m]);
                            else
                                survivalList.Add(EnemyMinionField.instance.minions[m]);

                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(
                                    EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                            else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + enemySpellPower)
                                deathList.Add(MinionField.instance.minions[m]);
                            else
                                survivalList.Add(MinionField.instance.minions[m]);

                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(
                                    MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + enemySpellPower);
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
                    if (nowSpellName == "칼날 부채")
                    {
                        if (enemy)
                            EffectManager.instance.FanofKnives(HeroManager.instance.enemyHero.transform.position, 180);
                        else
                            EffectManager.instance.FanofKnives(HeroManager.instance.playerHero.transform.position, 0);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "폭풍의 칼날")
                    {
                        if (enemy)
                            EffectManager.instance.TornadoEffect(HeroManager.instance.enemyHero.transform.position);
                        else
                            EffectManager.instance.TornadoEffect(HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(1f);
                    }
                    else if (nowSpellName == "별똥별")
                    {
                        GameEventManager.instance.EventSet(1f);
                        EffectManager.instance.StarFall(!enemy);
                        yield return new WaitForSeconds(1f);
                    }
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    deathList.Clear();
                    survivalList.Clear();
                    emptyList.Clear();
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
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
                        else if (MinionField.instance.minions[m].final_hp <= ability.AbilityData[0] + playerSpellPower)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[m].damageEffect, ability.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AttackEffectRun();
                    MinionField.instance.setMinionPos = false;
                    reArrangementEnemy = false;
                    ReArrangement();
                    Invoke("SetMinionPos", 1.25f);

                    GameEventManager.instance.EventSet(2f);
                }
                else if (CheckEvent(ability) == EventType.하수인소환)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    //미니언이 적군 하수인인지 아군 하수인인지 결정(1아군 ,-1적군)
                    bool enemyFlag = ability.AbilityData[2] == 1 ? false : true;
                    string minion_name = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                    string minion_ability = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
                    if ((enemyFlag && !enemy) || (!enemyFlag && enemy))
                    {
                        int index = EnemyMinionField.instance.minionNum;
                        EnemyMinionField.instance.AddMinion(EnemyMinionField.instance.minionNum, minion_name, false);
                    }
                    else
                    {
                        int index = MinionField.instance.minionNum;
                        MinionField.instance.AddMinion(MinionField.instance.minionNum, minion_name, false);
                    }
                }
                else if (CheckEvent(ability) == EventType.하수인들에게_은신부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
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
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    string ability_string = DataMng.instance.ToString(ability.AbilityData[0], ability.AbilityData[1], "명령어");
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
                else if (CheckEvent(ability) == EventType.하수인들에게_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                EnemyMinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                EnemyMinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                MinionField.instance.minions[m].nowAtk += ability.AbilityData[0];
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                                MinionField.instance.minions[m].nowSpell += ability.AbilityData[2];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.마나획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        ManaManager.instance.enemyNowMana += ability.AbilityData[0];
                    else
                        ManaManager.instance.playerNowMana += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.마나수정획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        ManaManager.instance.enemyMaxMana += ability.AbilityData[0];
                    else
                    {
                        if (ManaManager.instance.playerMaxMana >= 10)
                        {
                            GameEventManager.instance.EventAdd(1.4f);
                            Vector2Int pair = DataMng.instance.GetPairByName(
                                DataParse.GetCardName("넘치는마나"));
                            string cardName = DataMng.instance.ToString(pair.x, pair.y, "카드이름");
                            int n = CardHand.instance.nowHandNum;
                            CardHand.instance.DrawCard();
                            CardHand.instance.SetCardHand(cardName, n, BattleUI.instance.playerSpellPos.transform.position,
                                CardHand.handCardSize, 0);
                            CardViewManager.instance.UpdateCardView(0.001f);
                        }
                        else
                        {
                            ManaManager.instance.playerMaxMana += ability.AbilityData[0];
                        }
                    }
                }
                else if (CheckEvent(ability) == EventType.모든하수인주인의패로)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
                        if (MinionManager.instance.minionList[m].gameObject.activeSelf)
                            MinionManager.instance.minionList[m].gotoHandTrigger = true;
                }
                else if (CheckEvent(ability) == EventType.모든하수인처치)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
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
                else if (CheckEvent(ability) == EventType.하수인들에게_해당턴_능력치부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                            if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                EnemyMinionField.instance.minions[m].buffList.Add(buff);
                                EnemyMinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                    else
                    {
                        for (int m = 0; m < MinionField.instance.minions.Length; m++)
                            if (MinionField.instance.minions[m].gameObject.activeSelf)
                            {
                                Vector4 buff = new Vector4(ability.AbilityData[0], ability.AbilityData[1], ability.AbilityData[2], 1);
                                MinionField.instance.minions[m].buffList.Add(buff);
                                MinionField.instance.minions[m].final_hp += ability.AbilityData[1];
                            }
                    }
                }
                else if (CheckEvent(ability) == EventType.방어도획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                    {
                        if (HeroManager.instance.heroHpManager.enemyShield < 0)
                            HeroManager.instance.heroHpManager.enemyShield = 0;
                        HeroManager.instance.heroHpManager.enemyShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.enemyShieldAni.SetBool("Break", false);
                    }
                    else
                    {
                        if (HeroManager.instance.heroHpManager.playerShield < 0)
                            HeroManager.instance.heroHpManager.playerShield = 0;
                        HeroManager.instance.heroHpManager.playerShield += ability.AbilityData[0];
                        HeroManager.instance.heroHpManager.playerShieldAni.SetBool("Break", false);
                    }
                }
                else if (CheckEvent(ability) == EventType.공격력획득)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyAtk += ability.AbilityData[0];
                    else
                        HeroManager.instance.heroAtkManager.playerAtk += ability.AbilityData[0];
                }
                else if (CheckEvent(ability) == EventType.무기장착)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    GameEventManager.instance.EventAdd(1f);
                    if (enemy)
                    {
                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.enemyWeaponName = weapon_name;
                        HeroManager.instance.SetEnemyDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk = weaponAtk;
                    }
                    else
                    {
                        CardHand.instance.handAni.SetTrigger("축소");
                        Vector2 v = Camera.main.WorldToScreenPoint(HeroManager.instance.heroAtkManager.playerWeapon.transform.position - new Vector3(0, 50, 0));
                        DragCardObject.instance.ShowDropEffecWeapon(v, 0);

                        yield return new WaitWhile(() => !DragCardObject.instance.dropEffect.effectArrive);

                        string weapon_name = DataMng.instance.ToString(
                            ability.AbilityData[0], ability.AbilityData[1], "카드이름");
                        Vector2Int pair = DataMng.instance.GetPairByName(
                            DataParse.GetCardName(weapon_name));
                        int weaponHp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                        int weaponAtk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                        HeroManager.instance.heroAtkManager.playerWeaponName = weapon_name;
                        HeroManager.instance.SetPlayerDurability(weaponHp);
                        HeroManager.instance.heroAtkManager.playerWeaponAtk = weaponAtk;

                    }
                }
                else if (CheckEvent(ability) == EventType.무기공격력부여)
                {
                    SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                    if (enemy)
                        HeroManager.instance.heroAtkManager.enemyWeaponAtk += ability.AbilityData[0];
                    else
                        HeroManager.instance.heroAtkManager.playerWeaponAtk += ability.AbilityData[0];
                }
            }
        }

        selectSpellEvent = false;

        for (int m = 0; m < MinionManager.instance.minionList.Count; m++)
            if (MinionManager.instance.minionList[m].gameObject.activeSelf && MinionManager.instance.minionList[m].enemy == enemy)
                MinionManager.instance.minionList[m].spellRun = true;

        if (!enemy)
        {
            QuestManager.instance.SpellCard();
            Vector2 pair = DataMng.instance.GetPairByName(DataParse.GetCardName(nowSpellName));
            if (pair.x == 0)
                QuestManager.instance.CharacterCard(Job.드루이드);
            else if (pair.x == 1)
                QuestManager.instance.CharacterCard(Job.도적);
        }

        CardHand.instance.nextSpellCostOffset = 0;
    }
    #endregion

    #region[연계인지 검사]
    bool CheckCombo()
    {
        return CardHand.instance.useCardNum > 1;
    }
    #endregion

    #region[이벤트 타입]
    public EventType CheckEvent(SpellAbility spellAbility)
    {
        switch(spellAbility.AbilityType)
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
            case SpellAbility.Ability.모든하수인에게_해당턴동안_능력치부여:
                return EventType.하수인들에게_해당턴_능력치부여;
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
            case SpellAbility.Ability.무기장착:
                return EventType.무기장착;
            case SpellAbility.Ability.무기에_공격력부여:
                return EventType.무기공격력부여;
            case SpellAbility.Ability.다음카드비용감소:
            case SpellAbility.Ability.무작위_패_버리기:
            case SpellAbility.Ability.무작위_하수인뺏기:
            case SpellAbility.Ability.다음턴에다시가져오기:
                return EventType.없음;
            case SpellAbility.Ability.적영웅에게_피해주기:
                return EventType.적영웅에게피해;
            case SpellAbility.Ability.다음주문카드비용감소:
                return EventType.다음주문카드비용감소;
            case SpellAbility.Ability.무기의_공격력만큼_모든_적군에게피해:
                return EventType.무기공격력만큼적군광역피해;
            case SpellAbility.Ability.무기파괴:
                return EventType.무기파괴;
            case SpellAbility.Ability.내손으로다시가져오기:
                return EventType.내손으로다시가져오기;
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
        if (spellAbility.ConditionType != SpellAbility.Condition.피해입지않은하수인)
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
    [HideInInspector] public MinionObject targetMinion = null;
    [HideInInspector] public int targetHero = -1;
    List<MinionObject> deathList = new List<MinionObject>();
    List<MinionObject> survivalList = new List<MinionObject>();
    List<MinionObject> emptyList = new List<MinionObject>();

    //대상하수인 , 발동한 플레이어
    public void MinionSelect(MinionObject minionObject, bool enemy = false)
    {
        if (!MinionManager.instance.selectMinionEvent)
            return;
        targetMinion = minionObject;
        selectSpellEvent = false;
        MinionManager.instance.selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (nowSpellAbility.AbilityType)
        {
            case SpellAbility.Ability.빙결시키기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.freezeTrigger = true;
                break;
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.gotoHandTrigger = true;
                break;
            case SpellAbility.Ability.하수인_주인의패로되돌리면서_비용감소:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.gotoHandTrigger = true;
                CardHand.instance.handCostOffset[CardHand.instance.nowHandNum] = -nowSpellAbility.AbilityData[0];
                break;
            case SpellAbility.Ability.돌진부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.sleep = false;
                break;
            case SpellAbility.Ability.도발부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.taunt = true;
                break;
            case SpellAbility.Ability.은신부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.stealth = true;
                break;
            case SpellAbility.Ability.침묵시키기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.ActSilence();
                break;
            case SpellAbility.Ability.하수인처치:
                invokeMinion = minionObject;
                if (nowSpellName == "암살")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(-1, 1));
                    Invoke("MinionDeath", 1f);
                }
                else
                    Invoke("MinionDeath", 0f);
                break;
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.하수인의_생명력회복:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.final_hp += nowSpellAbility.AbilityData[0];
                minionObject.final_hp = Mathf.Min(minionObject.final_hp, minionObject.baseHp);
                EffectManager.instance.HealEffect(minionObject.transform.position, nowSpellAbility.AbilityData[0]);
                break;
            case SpellAbility.Ability.하수인의_생명력설정:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                minionObject.final_hp = nowSpellAbility.AbilityData[0];
                minionObject.baseHp = nowSpellAbility.AbilityData[0];
                break;
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                if (nowSpellName == "휘둘러치기")
                    EffectManager.instance.SwipeEffect(enemy);
                if (!enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp <= 
                            nowSpellAbility.AbilityData[0] + playerSpellPower && !EnemyMinionField.instance.minions[m].Equals(minionObject))
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                            if (!EnemyMinionField.instance.minions[i].Equals(minionObject))
                                AttackManager.instance.AddDamageObj(
                                    EnemyMinionField.instance.minions[i].damageEffect, nowSpellAbility.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AddDamageObj(
                        HeroManager.instance.heroHpManager.enemyHeroDamage, nowSpellAbility.AbilityData[0] + playerSpellPower);
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
                        else if (MinionField.instance.minions[m].final_hp <= nowSpellAbility.AbilityData[0] + enemySpellPower && !MinionField.instance.minions[m].Equals(minionObject))
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);

                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                            if (!MinionField.instance.minions[i].Equals(minionObject))
                                AttackManager.instance.AddDamageObj(
                                    MinionField.instance.minions[i].damageEffect, nowSpellAbility.AbilityData[0] + enemySpellPower);
                    AttackManager.instance.AddDamageObj(
                        HeroManager.instance.heroHpManager.playerHeroDamage, nowSpellAbility.AbilityData[0] + enemySpellPower);
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
                invokeMinion = minionObject;
                invokeEnemy = enemy;
                if (nowSpellName == "기습")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "독칼")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "절개")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeMinion.transform.position, new Vector2(+1, 1));
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "달빛 섬광")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.MagicEffect(invokeMinion.transform.position);
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "별빛섬광")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.MagicEffect(invokeMinion.transform.position);
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "별똥별")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.EnergyEffect(invokeMinion.transform.position + new Vector3(0, 800, 0), invokeMinion.transform.position);
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "천벌")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.MagicEffect(invokeMinion.transform.position);
                    Invoke("MinionDamage", 1f);
                }
                else if (nowSpellName == "휘둘러치기")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.SwipeTargetEffect(invokeMinion.transform.position);
                    Invoke("MinionDamage", 0f);
                }
                else
                    Invoke("MinionDamage", 0f);
                break;
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                if (nowSpellName == "야생성")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.SwipeTargetEffect(minionObject.transform.position);
                }
                AttackManager.instance.PopAllDamageObj();
                AttackManager.instance.AddDamageObj(minionObject.damageEffect, (enemy ? HeroManager.instance.heroAtkManager.enemyFinalAtk + enemySpellPower : HeroManager.instance.heroAtkManager.playerFinalAtk + playerSpellPower));
                AttackManager.instance.AttackEffectRun();
                break;
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                int temp = minionObject.final_hp;
                minionObject.baseHp = minionObject.final_atk;
                minionObject.final_hp = minionObject.final_atk;
                minionObject.baseAtk = temp;
                minionObject.nowAtk = temp;
                minionObject.final_atk = temp;
                for (int i = 0; i < minionObject.buffList.Count; i++)
                {
                    //공격력 체력 버프 소멸
                    minionObject.buffList[i] = new Vector4(0, 0, minionObject.buffList[i].z, minionObject.buffList[i].w);
                }
                break;
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                Vector4 weaponBuff = new Vector4(HeroManager.instance.heroAtkManager.playerWeaponAtk, 0, 0, 0);
                minionObject.buffList.Add(weaponBuff);
                break;
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.능력치부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                Vector4 buff = new Vector4(nowSpellAbility.AbilityData[0],
                    nowSpellAbility.AbilityData[1],
                    nowSpellAbility.AbilityData[2], 1);
                if (nowSpellAbility.AbilityType == SpellAbility.Ability.능력치부여)
                    buff.w = 0;
                minionObject.buffList.Add(buff);
                minionObject.final_hp += nowSpellAbility.AbilityData[1];
                break;
            case SpellAbility.Ability.능력부여:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                string ability_string = DataMng.instance.ToString(
                    nowSpellAbility.AbilityData[0], nowSpellAbility.AbilityData[1], "명령어");
                List<MinionAbility> abilityList = MinionManager.instance.MinionAbilityParsing(ability_string);
                for (int i = 0; i < abilityList.Count; i++)
                    minionObject.abilityList.Add(abilityList[i]);
                break;
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                int left = minionObject.num - 1;
                int right = minionObject.num + 1;
                if (!enemy)
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

    MinionObject invokeMinion;
    bool invokeEnemy;
    bool invokeRunEnemy;

    public void MinionDamage()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
        AttackManager.instance.PopAllDamageObj();
        AttackManager.instance.AddDamageObj(invokeMinion.damageEffect, 
            nowSpellAbility.AbilityData[0] + (invokeEnemy ? enemySpellPower : playerSpellPower));
        AttackManager.instance.AttackEffectRun();
        invokeMinion = null;
    }

    public void MinionDeath()
    {
        if (invokeMinion == null)
            return;
        SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
        invokeMinion.MinionDeath();
        invokeMinion = null;
    }

    public void HeroDamage()
    {
        AttackManager.instance.PopAllDamageObj();
        SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
        if (invokeEnemy)
            AttackManager.instance.AddDamageObj
                (HeroManager.instance.heroHpManager.enemyHeroDamage, 
                nowSpellAbility.AbilityData[0] + (invokeRunEnemy ? enemySpellPower : playerSpellPower));
        else
            AttackManager.instance.AddDamageObj(
                HeroManager.instance.heroHpManager.playerHeroDamage, 
                nowSpellAbility.AbilityData[0] + (invokeRunEnemy ? enemySpellPower : playerSpellPower));
        AttackManager.instance.AttackEffectRun();
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

    public void HeroSelect(bool enemy, bool runEnemy = false)
    {
        if (!MinionManager.instance.selectMinionEvent)
            return;
        selectSpellEvent = false;
        targetHero = enemy ? 2 : 1;
        MinionManager.instance.selectMinionEvent = false;
        GameEventManager.instance.EventAdd(0.3f);
        BattleUI.instance.grayFilterAni.SetBool("On", false);
        BattleUI.instance.selectMinion.gameObject.SetActive(false);
        switch (nowSpellAbility.AbilityType)
        {
            case SpellAbility.Ability.빙결시키기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                HeroManager.instance.SetFreeze(enemy);
                break;
            case SpellAbility.Ability.적에게피해주기:
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
                invokeEnemy = enemy;
                invokeRunEnemy = runEnemy;
                if (nowSpellName == "독칼")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeEnemy ? HeroManager.instance.enemyHero.transform.position : 
                        HeroManager.instance.playerHero.transform.position, new Vector2(+1, 1));
                    Invoke("HeroDamage", 1f);
                }
                else if (nowSpellName == "절개")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.CutEffect(invokeEnemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position, new Vector2(+1, 1));
                    Invoke("HeroDamage", 1f);
                }
                else if (nowSpellName == "달빛 섬광")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.MagicEffect(invokeEnemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                    Invoke("HeroDamage", 1f);
                }
                else if (nowSpellName == "별빛섬광")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.MagicEffect(invokeEnemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                    Invoke("HeroDamage", 1f);
                }
                else if (nowSpellName == "휘둘러치기")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.SwipeTargetEffect(invokeEnemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                    Invoke("HeroDamage", 0f);
                }
                else
                    Invoke("HeroDamage", 0f);
                break;
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                if (nowSpellName == "야생성")
                {
                    GameEventManager.instance.EventSet(1f);
                    EffectManager.instance.SwipeTargetEffect(enemy ? HeroManager.instance.enemyHero.transform.position : HeroManager.instance.playerHero.transform.position);
                }
                AttackManager.instance.PopAllDamageObj();
                AttackManager.instance.AddDamageObj(
                    enemy ? HeroManager.instance.heroHpManager.enemyHeroDamage : HeroManager.instance.heroHpManager.playerHeroDamage
                    , runEnemy ? HeroManager.instance.heroAtkManager.enemyFinalAtk + enemySpellPower : HeroManager.instance.heroAtkManager.playerFinalAtk + playerSpellPower);
                AttackManager.instance.AttackEffectRun();
                break;
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.영웅의_생명력회복:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                if (enemy)
                {
                    HeroManager.instance.heroHpManager.nowEnemyHp += 
                        nowSpellAbility.AbilityData[0];
                    EffectManager.instance.HealEffect(
                        HeroManager.instance.enemyHero.transform.position, nowSpellAbility.AbilityData[0]);
                }
                else
                {
                    HeroManager.instance.heroHpManager.nowPlayerHp += nowSpellAbility.AbilityData[0];
                    EffectManager.instance.HealEffect(
                        HeroManager.instance.playerHero.transform.position, nowSpellAbility.AbilityData[0]);
                }
                break;
            case SpellAbility.Ability.영웅의_생명력설정:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                if (enemy)
                    HeroManager.instance.heroHpManager.nowEnemyHp = nowSpellAbility.AbilityData[0];
                else
                    HeroManager.instance.heroHpManager.nowPlayerHp = nowSpellAbility.AbilityData[0];
                break;
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                SoundManager.instance.PlaySpellSE(nowSpellName, 주문상태.효과);
                deathList.Clear();
                survivalList.Clear();
                emptyList.Clear();
                if (nowSpellName == "휘둘러치기")
                    EffectManager.instance.SwipeEffect(!enemy);
                if (enemy)
                {
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (!EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            emptyList.Add(EnemyMinionField.instance.minions[m]);
                        else if (EnemyMinionField.instance.minions[m].final_hp 
                            <= nowSpellAbility.AbilityData[0] + playerSpellPower)
                            deathList.Add(EnemyMinionField.instance.minions[m]);
                        else
                            survivalList.Add(EnemyMinionField.instance.minions[m]);
                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[i].damageEffect, 
                                    nowSpellAbility.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AttackEffectRun();

                    EnemyMinionField.instance.setMinionPos = false;
                    if (emptyList.Count != 7)
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
                        else if (MinionField.instance.minions[m].final_hp <= nowSpellAbility.AbilityData[0] + playerSpellPower)
                            deathList.Add(MinionField.instance.minions[m]);
                        else
                            survivalList.Add(MinionField.instance.minions[m]);
                    AttackManager.instance.PopAllDamageObj();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                                AttackManager.instance.AddDamageObj(MinionField.instance.minions[i].damageEffect, nowSpellAbility.AbilityData[0] + playerSpellPower);
                    AttackManager.instance.AttackEffectRun();

                    MinionField.instance.setMinionPos = false;
                    if (emptyList.Count != 7)
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
        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(nowSpellName));
        int mana = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");
        string cardName = DataMng.instance.ToString(pair.x, pair.y, "카드이름");
        int n = CardHand.instance.nowHandNum;
        CardHand.instance.DrawCard();
        CardHand.instance.SetCardHand(cardName, n,BattleUI.instance.playerSpellPos.transform.position,
            CardHand.handCardSize, 0);
        CardViewManager.instance.UpdateCardView(0.001f);
        mana += CardHand.instance.removeCostOffset;
        mana = mana < 0 ? 0 : mana;
        ManaManager.instance.playerNowMana += mana;
        CardHand.instance.handCostOffset[CardHand.instance.nowHandNum] = CardHand.instance.removeCostOffset;
        CardHand.instance.useCardNum--;

    }
    #endregion
}
