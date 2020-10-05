using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidAI : MonoBehaviour
{
    public static DruidAI instance;

    public enum AI_Act
    {
        SpawnMinion,  //하수인을 소환할 수 있다면
        SpellRun,     //주문을 사용 할 수 있다면 && 주문을 사용하는 것으로 이득이 생길경우
        AttackMinion, //필드에 공격 할 수 있는 하수인이 존재한다면 && 공격하는 것이 이득인 경우 
        AttackHero,   //영웅능력을 사용할 수 있다면 && 공격하는것이 이득인 경우
        GameEnd,      //게임이 종료됨
        TurnEnd       //위의 사항이 모두 적용되지않을 경우
    };
    AI_Act act = AI_Act.TurnEnd;

    public static int caseNum = 9;

    List<string>[] caseByCard = new List<string>[caseNum];

    AI_Act SelectChoice()
    {
        if(!BattleUI.instance.gameStart)
            return AI_Act.GameEnd;

        #region[하수인이 내는것을 고려]
        if (EnemyMinionField.instance.minionNum < 7)
            for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
            {
                int nowMana = ManaManager.instance.enemyNowMana;
                string cardName = EnemyCardHand.instance.nowCard[i];
                Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
                string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
                int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
                if (cardType != "하수인")
                    continue;
                if (nowMana >= cardCost)
                    return AI_Act.SpawnMinion;
            }
        #endregion

        #region[주문을 쓰는것을 고려]
        for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
        {
            int nowMana = ManaManager.instance.enemyNowMana;
            string cardName = EnemyCardHand.instance.nowCard[i];
            Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
            string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
            int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
            if (cardType != "주문")
                continue;
            if (nowMana < cardCost)
                continue;

            for(int c = 0; c < caseNum; c++)
                if (caseByCard[c].Contains(cardName) && CaseBySpell(cardName,c))
                    return AI_Act.SpellRun;
        }
        #endregion

        #region[하수인으로 공격하는 것을 고려]
        //공격하는하수인,타겟번호,돌진여부,우선순위(남은체력)
        List<Vector4> targetList = new List<Vector4>();
        List<Vector4> targetTauntList = new List<Vector4>();
        for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
        {
            if (!EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                continue;
            if (!EnemyMinionField.instance.minions[i].checkCanAtttack)
                continue;
            int atk = EnemyMinionField.instance.minions[i].final_atk;
            int hp = EnemyMinionField.instance.minions[i].final_hp;
            bool charge = false;
            for (int j = 0; j < EnemyMinionField.instance.minions[i].abilityList.Count; j++)
                if (EnemyMinionField.instance.minions[i].abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                    charge = true;

            for (int j = 0; j < MinionField.instance.minions.Length; j++)
            {
                if (!MinionField.instance.minions[j].gameObject.activeSelf)
                    continue;
                if (MinionField.instance.minions[j].stealth)
                    continue;
                int targetAtk = MinionField.instance.minions[j].final_atk;
                int targetHp = MinionField.instance.minions[j].final_hp;
                if (targetHp - atk > 0 && !charge)
                    continue;
                if (MinionField.instance.minions[j].taunt)
                    targetTauntList.Add(new Vector4(i, j, charge ? +1 : -1, hp - targetAtk));
                else
                    targetList.Add(new Vector4(i, j, charge ? +1 : -1, hp - targetAtk));
            }
        }

        targetList.Sort(delegate (Vector4 A, Vector4 B)
        {
            if (A.z < B.z)
                return 1;
            if (A.z == B.z)
            {
                if (A.w > B.w)
                    return 1;
                else
                    return -1;
            }
            return -1;
        });
        targetTauntList.Sort(delegate (Vector4 A, Vector4 B)
        {
            if (A.z < B.z)
                return 1;
            if (A.z == B.z)
            {
                if (A.w > B.w)
                    return 1;
                else
                    return -1;
            }
            return -1;
        });

        if (targetTauntList.Count > 0)
        {
            if (!MinionManager.instance.CheckTaunt(false))
            {
                if (targetTauntList[0].z > 0)
                {
                    Debug.Log("돌진하수인이고 도발하수인존재");
                    return AI_Act.AttackMinion;
                }
                //돌진하수인은 아니고 공격하기 좋은경우
                else if (targetTauntList[0].w >= 0)
                {
                    Debug.Log("(적합한대상공격)돌진하수인은 아니고 도발하수인존재");
                    return AI_Act.AttackMinion;
                }
            }
        }
        else if (targetList.Count > 0)
        {
            //도발하수인이없다면
            if (MinionManager.instance.CheckTaunt(false))
            {
                if (targetList[0].z > 0)
                {
                    Debug.Log("돌진하수인이고 도발하수인이 없음");
                    return AI_Act.AttackMinion;
                }
                else if (targetList[0].w >= 0)
                {
                    Debug.Log("(적합한대상공격)돌진하수인이 아니고 도발하수인이 없음");
                    return AI_Act.AttackMinion;
                }
                else
                {
                    Debug.Log("(영웅을 공격)돌진하수인이 아니고 도발하수인이 없음");
                    return AI_Act.AttackMinion;
                }
            }
        }
        else if (EnemyMinionField.instance.minionNum > 0)
        {
            if (MinionManager.instance.CheckTaunt(false))
            {
                bool canAttackFlag = false;
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                {
                    if (!EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        continue;
                    if (EnemyMinionField.instance.minions[i].checkCanAtttack)
                    {
                        Debug.Log("필드에 하수인이 없어서 영웅본체공격");
                        return AI_Act.AttackMinion;
                    }
                }
            }
        }

        #endregion

        #region[영웅능력을 사용하는 것을 고려]
        if (ManaManager.instance.enemyNowMana >= 2 && HeroManager.instance.heroPowerManager.enemyCanUse)
        {
            if (HeroManager.instance.heroPowerManager.enemyHeroName.Equals("발리라"))
            {
                //무기공격력이 1이하이고 내구도가 2미만일때
                if (HeroManager.instance.heroAtkManager.enemyWeaponAtk <= 1 && HeroManager.instance.heroAtkManager.enemyWeaponDurability < 2)
                    return AI_Act.AttackHero;
            }
            else if (HeroManager.instance.heroPowerManager.enemyHeroName.Equals("말퓨리온"))
                return AI_Act.AttackHero;
        }
        else if (HeroManager.instance.heroAtkManager.enemyAttackCheck && !HeroManager.instance.enemyFreezeObj.activeSelf)
        {
            return AI_Act.AttackHero;
        }

        #endregion


        return AI_Act.TurnEnd;
    }

    #region[Awake]
    public void Awake()
    {
        instance = this;
        for (int i = 0; i < caseNum; i++)
            caseByCard[i] = new List<string>();
        //상황0 . 자신영웅의 체력이 적을경우
        caseByCard[0].Add("치유의 손길");
        caseByCard[0].Add("할퀴기");

        //상황1 . 상대필드에 강한 하수인이 있을경우
        caseByCard[1].Add("자연화");
        caseByCard[1].Add("별빛섬광");
        caseByCard[1].Add("별똥별");
        caseByCard[1].Add("휘둘러치기");

        //상황2 . 상대필드에 하수인이 많을경우
        caseByCard[2].Add("달빛 섬광");
        caseByCard[2].Add("할퀴기");
        caseByCard[2].Add("휘둘러치기");
        caseByCard[2].Add("천벌");
        caseByCard[2].Add("야생성");
        caseByCard[2].Add("별똥별");
        caseByCard[2].Add("자연의 군대");

        //상황3 . 패가 부족할경우
        caseByCard[3].Add("급속 성장");
        caseByCard[3].Add("넘치는마나");
        caseByCard[3].Add("육성");

        //상황4 . 마나수정이 적을경우
        caseByCard[4].Add("급속 성장");
        caseByCard[4].Add("육성");

        //상황5 . 마나를 보충하면 하수인을 소환할 수 있다면
        caseByCard[5].Add("정신 자극");
        caseByCard[5].Add("동전 한 닢");

        //상황6 . 자신필드에 하수인이 많을경우
        caseByCard[6].Add("야생의 징표");
        caseByCard[6].Add("야생의 힘");
        caseByCard[6].Add("자연의 징표");
        caseByCard[6].Add("숲의 영혼");
        caseByCard[6].Add("야생의 포효");

        //상황7 . 하수인의 체력이 적을경우
        caseByCard[7].Add("치유의 손길");

        //상황8 . 상대영웅의 체력이 적을경우
        caseByCard[8].Add("자연의 군대");
    }
    #endregion

    #region[AI_Run]
    public void AI_Run()
    {
        StartCoroutine(AI_Running());
    }

    private IEnumerator AI_Running()
    {
        yield return new WaitForSeconds(2f);

        while (TurnManager.instance.turn == 턴.상대방)
        {
            while (GameEventManager.instance.GetEventValue() > 0.001f)
                yield return new WaitForSeconds(0.001f);
            GameEventManager.instance.EventAdd(0.1f);

            yield return new WaitForSeconds(1f);

            act = SelectChoice();

            #region[하수인 소환]
            if (act == AI_Act.SpawnMinion)
            {
                int index = -1;
                int minionCost = -1;
                for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
                {
                    int nowMana = ManaManager.instance.enemyNowMana;
                    string cardName = EnemyCardHand.instance.nowCard[i];
                    Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
                    string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
                    int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
                    if (cardType != "하수인")
                        continue;
                    if (cardCost >= minionCost && nowMana >= cardCost)
                    {
                        minionCost = cardCost;
                        index = i;
                    }
                }

                EnemyCardHand.instance.UseCard(index);

                yield return new WaitForSeconds(3f);

            }
            #endregion

            #region[주문 사용]
            else if (act == AI_Act.SpellRun)
            {
                int index = -1;
                int importantCase = int.MaxValue;
                for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
                {
                    int nowMana = ManaManager.instance.enemyNowMana;
                    string cardName = EnemyCardHand.instance.nowCard[i];
                    Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
                    string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
                    int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
                    if (cardType != "주문")
                        continue;
                    for(int c = 0; c < caseNum; c++)
                        if (caseByCard[c].Contains(cardName) && CaseBySpell(cardName, c))
                            if (importantCase >= c && nowMana >= cardCost)
                            {
                                importantCase = c;
                                index = i;
                            }
                }

                Debug.Log(CaseBySpellText(importantCase) + " : " + EnemyCardHand.instance.nowCard[index]);
                EnemyCardHand.instance.UseCard(index);

                yield return new WaitForSeconds(3f);
            }
            #endregion

            #region[하수인으로 공격]
            else if (act == AI_Act.AttackMinion)
            {
                //공격하는하수인,타겟번호,돌진여부,우선순위(남은체력)
                List<Vector4> targetList = new List<Vector4>();
                List<Vector4> targetTauntList = new List<Vector4>();
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                {
                    if (!EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        continue;
                    if (!EnemyMinionField.instance.minions[i].checkCanAtttack)
                        continue;
                    int atk = EnemyMinionField.instance.minions[i].final_atk;
                    int hp = EnemyMinionField.instance.minions[i].final_hp;
                    bool charge = false;
                    for (int j = 0; j < EnemyMinionField.instance.minions[i].abilityList.Count; j++)
                        if (EnemyMinionField.instance.minions[i].abilityList[j].Ability_type == MinionAbility.Ability.돌진)
                            charge = true;

                    for (int j = 0; j < MinionField.instance.minions.Length; j++)
                    {
                        if (!MinionField.instance.minions[j].gameObject.activeSelf)
                            continue;
                        if (MinionField.instance.minions[j].stealth)
                            continue;
                        int targetAtk = MinionField.instance.minions[j].final_atk;
                        int targetHp = MinionField.instance.minions[j].final_hp;
                        if (targetHp - atk > 0 && !charge)
                            continue;
                        if (MinionField.instance.minions[j].taunt)
                            targetTauntList.Add(new Vector4(i, j, charge ? +1 : -1, hp - targetAtk));
                        else
                            targetList.Add(new Vector4(i, j, charge ? +1 : -1, hp - targetAtk));
                    }
                }

                targetList.Sort(delegate (Vector4 A, Vector4 B)
                {
                    if (A.z < B.z)
                        return 1;
                    if (A.z == B.z)
                    {
                        if (A.w > B.w)
                            return 1;
                        else
                            return -1;
                    }
                    return -1;
                });
                targetTauntList.Sort(delegate (Vector4 A, Vector4 B)
                {
                    if (A.z < B.z)
                        return 1;
                    if (A.z == B.z)
                    {
                        if (A.w > B.w)
                            return 1;
                        else
                            return -1;
                    }
                    return -1;
                });

                if (targetTauntList.Count > 0)
                {
                    //도발하수인이있다면
                    if (!MinionManager.instance.CheckTaunt(false))
                    {
                        //돌진하수인이면
                        if (targetTauntList[0].z > 0)
                        {
                            string targetName = "" + targetTauntList[0].y;
                            AttackOrder((int)targetTauntList[0].x, targetName);
                            yield return new WaitForSeconds(2f);
                        }
                        //돌진하수인은 아니고 공격하기 좋은경우
                        else if (targetTauntList[0].w >= 0)
                        {
                            string targetName = "" + targetTauntList[0].y;
                            AttackOrder((int)targetTauntList[0].x, targetName);
                            yield return new WaitForSeconds(2f);
                        }
                    }
                }
                else if (targetList.Count > 0)
                {
                    //도발하수인이없다면
                    if (MinionManager.instance.CheckTaunt(false))
                    {
                        //돌진하수인이면
                        if (targetList[0].z > 0)
                        {
                            string targetName = "Hero";
                            AttackOrder((int)targetList[0].x, targetName);
                            yield return new WaitForSeconds(2f);
                        }
                        //돌진하수인은 아니고 공격하기 좋은경우
                        else if (targetList[0].w >= 0)
                        {
                            string targetName = "" + targetList[0].y;
                            AttackOrder((int)targetList[0].x, targetName);
                            yield return new WaitForSeconds(2f);
                        }
                        //돌진하수인은 아니고 공격하기 힘든경우
                        else
                        {
                            string targetName = "Hero";
                            AttackOrder((int)targetList[0].x, targetName);
                            yield return new WaitForSeconds(2f);
                        }
                    }
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    if (MinionManager.instance.CheckTaunt(false))
                    {
                        bool canAttackFlag = false;
                        for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        {
                            if (!EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                                continue;
                            if (EnemyMinionField.instance.minions[i].checkCanAtttack)
                            {
                                string targetName = "Hero";
                                AttackOrder(i, targetName);
                                yield return new WaitForSeconds(2f);
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region[영웅능력 사용]
            else if (act == AI_Act.AttackHero)
            {
                //영웅능력사용조건
                if (ManaManager.instance.enemyNowMana >= 2 && HeroManager.instance.heroPowerManager.enemyCanUse)
                {
                    if (HeroManager.instance.heroPowerManager.enemyHeroName.Equals("발리라"))
                    {
                        //무기공격력이 1이하이고 내구도가 2미만일때
                        if (HeroManager.instance.heroAtkManager.enemyWeaponAtk <= 1 && HeroManager.instance.heroAtkManager.enemyWeaponDurability < 2)
                        {
                            HeroManager.instance.heroPowerManager.UseHeroAbility(true);
                            yield return new WaitForSeconds(2f);
                        }

                    }
                    else if (HeroManager.instance.heroPowerManager.enemyHeroName.Equals("말퓨리온"))
                    {
                        HeroManager.instance.heroPowerManager.UseHeroAbility(true);
                        yield return new WaitForSeconds(2f);
                    }
                }
                else if (HeroManager.instance.heroAtkManager.enemyAttackCheck && !HeroManager.instance.enemyFreezeObj.activeSelf)
                {
                    List<int> targetList = new List<int>();
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf && !MinionField.instance.minions[i].stealth)
                        {
                            if (MinionField.instance.minions[i].stealth)
                                continue;
                            if (MinionField.instance.minions[i].final_hp > HeroManager.instance.heroAtkManager.enemyFinalAtk)
                                continue;
                            if(!MinionManager.instance.CheckTaunt(false) && !MinionField.instance.minions[i].taunt)
                                continue;
                            if (MinionField.instance.minions[i].final_atk > 5)
                                continue;
                            if (MinionField.instance.minions[i].final_atk >= HeroManager.instance.heroHpManager.nowEnemyHp + HeroManager.instance.heroHpManager.enemyShield)
                                continue;

                            targetList.Add(i);
                        }

                    targetList.Sort(delegate (int A, int B)
                    {
                        if (MinionField.instance.minions[A].baseHp + MinionField.instance.minions[A].baseAtk < MinionField.instance.minions[B].baseHp + MinionField.instance.minions[B].baseAtk)
                            return -1;
                        return +1;
                    });


                    if (targetList.Count > 0)
                    {
                        AttackManager.instance.PopAllDamageObj();
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, MinionField.instance.minions[targetList[0]].final_atk);
                        AttackManager.instance.AddDamageObj(MinionField.instance.minions[targetList[0]].damageEffect, HeroManager.instance.heroAtkManager.enemyFinalAtk);
                        HeroManager.instance.heroAtkManager.HeroAttack(true, MinionField.instance.minions[targetList[0]].transform.position);
                        yield return new WaitForSeconds(2f);
                    }
                    else if (MinionManager.instance.CheckTaunt(false))
                    {
                        AttackManager.instance.PopAllDamageObj();
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, HeroManager.instance.heroAtkManager.enemyFinalAtk);
                        HeroManager.instance.heroAtkManager.HeroAttack(true, HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(2f);
                    }
                    else
                        HeroManager.instance.heroAtkManager.enemyCanAttackNum--;

                }
            }
            #endregion

            else if (act == AI_Act.TurnEnd)
            {
                Debug.Log("턴끝냄");
                SoundManager.instance.PlaySE("상대가턴종료버튼누름");
                TurnManager.instance.turnBtnAni.SetTrigger("내턴");
                TurnManager.instance.turnEndTrigger = true;
                break;
            }


        }
    }
    #endregion

    #region[AI Select 하수인]
    public void AI_Select(MinionAbility.Ability minionAbility)
    {
        int searchMinionIndex = -1;
        int minionAtk = 0;
        int minionHp = 0;
        int minionAbilityCount = 0;
        int hpAds = 0;
        int hpAtkAds = 0;
        MinionManager.instance.selectMinionEvent = true;
        switch (minionAbility)
        {
            #region[빙결시키기]
            case MinionAbility.Ability.빙결시키기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                //하수인이 없으면 영웅빙결
                else
                    MinionManager.instance.HeroSelect(false);
                break;
            #endregion

            #region[아군하수인_주인의패로되돌리기]
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                //전투의함성하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                        for (int j = 0; j < EnemyMinionField.instance.minions[i].abilityList.Count; j++)
                            if (EnemyMinionField.instance.minions[i].abilityList[j].Condition_type == MinionAbility.Condition.전투의함성)
                            {
                                searchMinionIndex = i;
                                break;
                            }
                //체력이깍인하수인찾기
                if (searchMinionIndex == -1)
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                            if (EnemyMinionField.instance.minions[i].final_hp <= EnemyMinionField.instance.minions[i].baseHp &&
                                    hpAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp))
                            {
                                searchMinionIndex = i;
                                break;
                            }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex]);
                break;
            #endregion

            #region[적군하수인_주인의패로되돌리기]
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                break;
            #endregion

            #region[침묵시키기]
            case MinionAbility.Ability.침묵시키기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp &&
                            MinionField.instance.minions[i].baseAtk + MinionField.instance.minions[i].baseHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                else
                {
                    //능력이 많은 하수인
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                            if (minionAbilityCount < MinionField.instance.minions[i].abilityList.Count)
                            {
                                minionAbilityCount = MinionField.instance.minions[i].abilityList.Count;
                                searchMinionIndex = i;
                            }
                    if (searchMinionIndex != -1)
                        MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                    else
                        MinionManager.instance.MinionSelect(MinionField.instance.minions[Random.Range(0, MinionField.instance.minionNum)]);
                }
                break;
            #endregion

            #region[하수인처치]
            case MinionAbility.Ability.하수인처치:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                else
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[Random.Range(0, MinionField.instance.minionNum)]);
                break;
            #endregion

            #region[생명력회복]
            case MinionAbility.Ability.생명력회복:
                //강한하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                        if (EnemyMinionField.instance.minions[i].final_hp <= EnemyMinionField.instance.minions[i].baseHp &&
                                hpAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp))
                        {
                            hpAds = Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp);
                            searchMinionIndex = i;
                            break;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex]);
                else
                {
                    MinionManager.instance.HeroSelect(true);
                }
                break;
            case MinionAbility.Ability.하수인의_생명력회복:
                //강한하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                        if (EnemyMinionField.instance.minions[i].final_hp <= EnemyMinionField.instance.minions[i].baseHp &&
                                hpAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp))
                        {
                            hpAds = Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp);
                            searchMinionIndex = i;
                            break;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex]);
                break;
            #endregion

            #region[피해주기]
            case MinionAbility.Ability.피해주기:
            case MinionAbility.Ability.하수인에게_피해주기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                else
                    MinionManager.instance.HeroSelect(false);
                break;
            #endregion

            #region[대상의_공격력_생명력_교환]
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (hpAtkAds < Mathf.Abs(MinionField.instance.minions[i].final_atk - MinionField.instance.minions[i].final_hp) && MinionField.instance.minions[i].final_atk <= MinionField.instance.minions[i].final_hp)
                        {
                            hpAtkAds = Mathf.Abs(MinionField.instance.minions[i].final_atk - MinionField.instance.minions[i].final_hp);
                            searchMinionIndex = i;
                        }

                if(searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);

                if (searchMinionIndex == -1)
                {
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                            if (hpAtkAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_atk - EnemyMinionField.instance.minions[i].final_hp) && EnemyMinionField.instance.minions[i].final_atk >= EnemyMinionField.instance.minions[i].final_hp)
                            {
                                hpAtkAds = Mathf.Abs(EnemyMinionField.instance.minions[i].final_atk - EnemyMinionField.instance.minions[i].final_hp);
                                searchMinionIndex = i;
                            }

                    if (searchMinionIndex != -1)
                        MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex]);
                }
                break;
            #endregion

            #region[능력치부여]
            case MinionAbility.Ability.해당턴동안_능력치부여:
            case MinionAbility.Ability.능력치부여:
                //체력높은하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                        if (EnemyMinionField.instance.minions[i].final_hp >= minionHp)
                        {
                            minionHp = EnemyMinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                            break;
                        }
                if (searchMinionIndex != -1)
                    MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex]);
                else if (EnemyMinionField.instance.minionNum > 0)
                    MinionManager.instance.MinionSelect(EnemyMinionField.instance.minions[Random.Range(0, EnemyMinionField.instance.minionNum)]);

                break;
            #endregion

            #region[영웅 영웅에게_피해주기]
            case MinionAbility.Ability.영웅에게_피해주기:
                MinionManager.instance.HeroSelect(false);
                break;
            #endregion

            #region[영웅 영웅의_생명력회복]
            case MinionAbility.Ability.영웅의_생명력회복:
                MinionManager.instance.HeroSelect(true);
                break;
            #endregion

            #region[영웅 생명력설정]
            case MinionAbility.Ability.영웅의_생명력설정:
                if (HeroManager.instance.heroHpManager.nowEnemyHp < (int)MinionManager.instance.eventMininon.abilityList[MinionManager.instance.eventNum].Ability_data.x)
                    MinionManager.instance.HeroSelect(true);
                else
                    MinionManager.instance.HeroSelect(false);
                break;
                #endregion
        }
    }
    #endregion

    #region[AI Select 주문]

    public int targetMinionNum = -2;
    public void AI_Select(SpellAbility spellAbility)
    {
        int searchMinionIndex = -1;
        int minionAtk = 0;
        int minionHp = 0;
        int minionAbilityCount = 0;
        int hpAds = 0;
        int hpAtkAds = 0;
        int spellDamage = (int)spellAbility.Ability_data.x;
        MinionManager.instance.selectMinionEvent = true;
        switch (spellAbility.Ability_type)
        {
            #region[빙결시키기]
            case SpellAbility.Ability.빙결시키기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                            {
                                minionAtk = MinionField.instance.minions[i].final_atk;
                                minionHp = MinionField.instance.minions[i].final_hp;
                                searchMinionIndex = i;
                            }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                    targetMinionNum = searchMinionIndex;
                }
                //하수인이 없으면 영웅빙결
                else
                    SpellManager.instance.HeroSelect(false,true);
                break;
            #endregion

            #region[아군하수인_주인의패로되돌리기]
            case SpellAbility.Ability.아군하수인_주인의패로되돌리기:
            case SpellAbility.Ability.하수인_주인의패로되돌리면서_비용감소:
                //전투의함성하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        for (int j = 0; j < EnemyMinionField.instance.minions[i].abilityList.Count; j++)
                            if (EnemyMinionField.instance.minions[i].abilityList[j].Condition_type == MinionAbility.Condition.전투의함성)
                            {
                                searchMinionIndex = i;
                                break;
                            }
                //체력이깍인하수인찾기
                if (searchMinionIndex == -1)
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                            if (EnemyMinionField.instance.minions[i].final_hp <= EnemyMinionField.instance.minions[i].baseHp &&
                                    hpAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_hp - EnemyMinionField.instance.minions[i].baseHp))
                            {
                                searchMinionIndex = i;
                                break;
                            }
                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                break;
            #endregion

            #region[적군하수인_주인의패로되돌리기]
            case SpellAbility.Ability.적군하수인_주인의패로되돌리기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                break;
            #endregion

            #region[돌진부여]
            case SpellAbility.Ability.돌진부여:
                //잠든하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        if (EnemyMinionField.instance.minions[i].sleep)
                        {
                            searchMinionIndex = i;
                            break;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, EnemyMinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[도발부여]
            case SpellAbility.Ability.도발부여:
                //체력높은 하수인 찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        if (EnemyMinionField.instance.minions[i].final_hp > minionHp)
                        {
                            minionHp = EnemyMinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, EnemyMinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[은신부여]
            case SpellAbility.Ability.은신부여:
                //체력낮은 하수인 찾기
                minionHp = int.MaxValue;
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        if (EnemyMinionField.instance.minions[i].final_hp < minionHp)
                        {
                            minionHp = EnemyMinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, EnemyMinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[침묵시키기]
            case SpellAbility.Ability.침묵시키기:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp &&
                            MinionField.instance.minions[i].baseAtk + MinionField.instance.minions[i].baseHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex]);
                    targetMinionNum = searchMinionIndex;
                }
                else
                {
                    //능력이 많은 하수인
                    for (int i = 0; i < MinionField.instance.minions.Length; i++)
                        if (MinionField.instance.minions[i].gameObject.activeSelf)
                            if (!MinionField.instance.minions[i].stealth)
                                if (minionAbilityCount < MinionField.instance.minions[i].abilityList.Count)
                            {
                                minionAbilityCount = MinionField.instance.minions[i].abilityList.Count;
                                searchMinionIndex = i;
                            }
                    if (searchMinionIndex != -1)
                    {
                        SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                        targetMinionNum = searchMinionIndex;
                    }
                    else
                    {
                        int r = Random.Range(0, MinionField.instance.minionNum);
                        SpellManager.instance.MinionSelect(MinionField.instance.minions[r], true);
                        targetMinionNum = r;
                    }
                }
                break;
            #endregion

            #region[하수인처치]
            case SpellAbility.Ability.하수인처치:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else
                {
                    int r = Random.Range(0, MinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[생명력회복]
            case SpellAbility.Ability.생명력회복:
            case SpellAbility.Ability.하수인의_생명력회복:
                SpellManager.instance.HeroSelect(true, true);
                break;
            #endregion

            #region[다른모든_적군에게_피해주기]
            case SpellAbility.Ability.다른모든_적군에게_피해주기:
                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else
                    SpellManager.instance.HeroSelect(false, true);
                break;
            #endregion

            #region[피해주기]
            case SpellAbility.Ability.적에게피해주기:
            case SpellAbility.Ability.피해주기:
            case SpellAbility.Ability.영웅의공격력만큼_피해주기:
                //주문을 사용하기 적합한 하수인 선택(데미지 대비 효율이 좋고,강한 하수인)
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (MinionField.instance.minions[i].final_hp <= spellDamage &&
                            Mathf.Abs(MinionField.instance.minions[i].final_hp - spellDamage) <= 2 &&
                            minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (MinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, MinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                else
                {
                    SpellManager.instance.HeroSelect(false, true);
                    targetMinionNum = -1;
                }
                break;
            #endregion

            #region[하수인에게_피해주기]
            case SpellAbility.Ability.하수인에게_피해주기:
            case SpellAbility.Ability.피해받지않은하수인에게_피해주기:
                //주문을 사용하기 적합한 하수인 선택(데미지 대비 효율이 좋고,강한 하수인)
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (MinionField.instance.minions[i].final_hp <= spellDamage &&
                            Mathf.Abs(MinionField.instance.minions[i].final_hp - spellDamage) <= 2 &&
                            minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            minionHp = MinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (MinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, MinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[대상의_공격력_생명력_교환]
            case SpellAbility.Ability.대상의_공격력_생명력_교환:
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (hpAtkAds < Mathf.Abs(MinionField.instance.minions[i].final_atk - MinionField.instance.minions[i].final_hp) && MinionField.instance.minions[i].final_atk <= MinionField.instance.minions[i].final_hp)
                        {
                            hpAtkAds = Mathf.Abs(MinionField.instance.minions[i].final_atk - MinionField.instance.minions[i].final_hp);
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (searchMinionIndex == -1)
                {
                    for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                        if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                            if (hpAtkAds < Mathf.Abs(EnemyMinionField.instance.minions[i].final_atk - EnemyMinionField.instance.minions[i].final_hp) && EnemyMinionField.instance.minions[i].final_atk >= EnemyMinionField.instance.minions[i].final_hp)
                            {
                                hpAtkAds = Mathf.Abs(EnemyMinionField.instance.minions[i].final_atk - EnemyMinionField.instance.minions[i].final_hp);
                                searchMinionIndex = i;
                            }

                    if (targetMinionNum >= 0)
                        SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                    else if (searchMinionIndex != -1)
                    {
                        SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                        targetMinionNum = searchMinionIndex;
                    }
                }
                break;
            #endregion

            #region[무기의_공격력만큼능력부여]
            case SpellAbility.Ability.무기의_공격력만큼능력부여:
                //체력높은 하수인 찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf)
                        if (EnemyMinionField.instance.minions[i].final_hp > minionHp)
                        {
                            minionHp = EnemyMinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, EnemyMinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[능력치부여]
            case SpellAbility.Ability.해당턴동안_능력치부여:
            case SpellAbility.Ability.능력치부여:
            case SpellAbility.Ability.능력부여:
                //체력높은하수인찾기
                for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
                    if (EnemyMinionField.instance.minions[i].gameObject.activeSelf && !EnemyMinionField.instance.minions[i].Equals(MinionManager.instance.eventMininon))
                        if (EnemyMinionField.instance.minions[i].final_hp >= minionHp)
                        {
                            minionHp = EnemyMinionField.instance.minions[i].final_hp;
                            searchMinionIndex = i;
                            break;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else if (EnemyMinionField.instance.minionNum > 0)
                {
                    int r = Random.Range(0, EnemyMinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(EnemyMinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
            #endregion

            #region[대상이_양옆하수인을_공격]
            case SpellAbility.Ability.대상이_양옆하수인을_공격:
                //강한하수인찾기
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (!MinionField.instance.minions[i].stealth)
                            if (minionAtk < MinionField.instance.minions[i].final_atk)
                        {
                            minionAtk = MinionField.instance.minions[i].final_atk;
                            searchMinionIndex = i;
                        }

                if (targetMinionNum >= 0)
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[targetMinionNum], true);
                else if (searchMinionIndex != -1)
                {
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[searchMinionIndex], true);
                    targetMinionNum = searchMinionIndex;
                }
                else
                {
                    int r = Random.Range(0, MinionField.instance.minionNum);
                    SpellManager.instance.MinionSelect(MinionField.instance.minions[r], true);
                    targetMinionNum = r;
                }
                break;
                #endregion
        }
    }
    #endregion

    #region[AI ChoiceSelect]
    public int AI_ChoiceSelect(string card_name)
    {
        #region[발톱의 드루이드]
        if (card_name == "발톱의 드루이드")
            return 1;
        #endregion

        #region[숲의 수호자]
        else if (card_name == "숲의 수호자")
        {
            int searchMinionIndex = -1;
            int minionAtk = 0;
            int minionHp = 0;
            int minionAbilityCount = 1;
            for (int i = 0; i < MinionField.instance.minions.Length; i++)
                if (MinionField.instance.minions[i].gameObject.activeSelf)
                    if (minionAtk + minionHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp && 
                        MinionField.instance.minions[i].baseAtk + MinionField.instance.minions[i].baseHp < MinionField.instance.minions[i].final_atk + MinionField.instance.minions[i].final_hp)
                    {
                        minionAtk = MinionField.instance.minions[i].final_atk;
                        minionHp = MinionField.instance.minions[i].final_hp;
                        searchMinionIndex = i;
                    }

            if (searchMinionIndex != -1)
                return 1;
            else
            {
                //능력이 많은 하수인
                for (int i = 0; i < MinionField.instance.minions.Length; i++)
                    if (MinionField.instance.minions[i].gameObject.activeSelf)
                        if (minionAbilityCount < MinionField.instance.minions[i].abilityList.Count)
                        {
                            minionAbilityCount = MinionField.instance.minions[i].abilityList.Count;
                            searchMinionIndex = i;
                        }
                if (searchMinionIndex != -1)
                    return 1;
            }
            return 0;
        }
        #endregion

        #region[전쟁의 고대정령]
        else if(card_name == "전쟁의 고대정령")
            return 1;
        #endregion

        #region[지식의 고대정령]
        else if(card_name == "지식의 고대정령")
        {
            if(EnemyCardHand.instance.nowHandNum <= 5)
                return 1;
            else
                return 0;
        }
        #endregion

        #region[세나리우스]
        else if (card_name == "세나리우스")
        {
            if (EnemyMinionField.instance.minionNum >= 4)
                return 0;
            else
                return 1;
        }
        #endregion

        #region[야생의 힘]
        else if (card_name == "야생의 힘")
        {
            if (EnemyMinionField.instance.minionNum >= 4)
                return 0;
            else
                return 1;
        }
        #endregion

        #region[천벌]
        else if (card_name == "천벌")
        {
            if (EnemyCardHand.instance.nowHandNum <= 5)
                return 1;
            else
                return 0;
        }
        #endregion

        #region[자연의 징표]
        else if (card_name == "자연의 징표")
            return 1;
        #endregion

        #region[별똥별]
        else if (card_name == "별똥별")
        {
            if (MinionField.instance.minionNum >= 4)
                return 1;
            else
                return 0;
        }
        #endregion

        #region[육성]
        else if (card_name == "육성")
        {
            if (EnemyCardHand.instance.nowHandNum <= 5)
                return 1;
            else
                return 0;
        }
        #endregion

        return 0;
    }

    public int AI_ChoiceSelect(MinionObject minionObject)
    {
        return AI_ChoiceSelect(minionObject.minion_name);
    }


    #endregion

    #region[주문 사용 조건]
    public bool CaseBySpell(string spellName,int n)
    {
        //상황0 . 자신영웅의 체력이 적을경우
        if (n == 0)
        {
            int sumAtk = 0;
            for (int m = 0; m < MinionField.instance.minions.Length; m++)
                if (MinionField.instance.minions[m].gameObject.activeSelf)
                    sumAtk += MinionField.instance.minions[m].final_atk;

            if (sumAtk >= HeroManager.instance.heroHpManager.nowEnemyHp)
                return true;

            if(HeroManager.instance.heroHpManager.nowEnemyHp <= 15)
                return true;
        }
        //상황1 . 상대필드에 강한 하수인이 있을경우
        else if (n == 1)
        {
            int sumStat = 0;
            for (int m = 0; m < MinionField.instance.minions.Length; m++)
                if (MinionField.instance.minions[m].gameObject.activeSelf)
                    sumStat = Mathf.Max(sumStat,MinionField.instance.minions[m].final_atk + MinionField.instance.minions[m].final_hp);

            if (sumStat >= 6)
                return true;
        }
        //상황2 . 상대필드에 하수인이 많을경우
        else if (n == 2)
        {
            if (MinionField.instance.minionNum >= 4)
                return true;
            if (MinionField.instance.minionNum >= 2 && MinionField.instance.minionNum > EnemyMinionField.instance.minionNum)
                return true;
        }
        //상황3 . 패가 부족할경우
        else if (n == 3)
        {
            if (!spellName.Equals("급속 성장"))
                if (EnemyCardHand.instance.nowHandNum <= 5)
                    return true;

            if (spellName.Equals("급속 성장") && ManaManager.instance.enemyMaxMana >= 10)
                if (EnemyCardHand.instance.nowHandNum <= 5)
                    return true;
        }
        //상황4 . 마나수정이 적을경우
        else if (n == 4)
        {
            if (ManaManager.instance.enemyMaxMana <= 4)
                return true;
        }
        //상황5 . 마나를 보충하면 하수인을 소환할 수 있다면
        else if (n == 5)
        {
            int addMana = 0;
            if (spellName.Equals("동전 한 닢"))
                addMana = 1;
            else if (spellName.Equals("정신 자극"))
                addMana = 2;

            for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
            {
                    int nowMana = ManaManager.instance.enemyNowMana + addMana;
                string cardName = EnemyCardHand.instance.nowCard[i];
                Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(cardName));
                string cardType = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "카드종류");
                int cardCost = DataMng.instance.ToInteger((DataMng.TableType)pair.x, (int)pair.y, "코스트");
                if (cardType != "하수인")
                    continue;
                if (nowMana >= cardCost)
                    return true;
            }

        }
        //상황6 . 자신필드에 하수인이 많을경우
        else if (n == 6)
        {
            if (EnemyMinionField.instance.minionNum >= 4)
                return true;
            if (EnemyMinionField.instance.minionNum >= 2 && EnemyMinionField.instance.minionNum > MinionField.instance.minionNum)
                return true;

        }
        //상황7 . 하수인의 체력이 적을경우
        else if (n == 7)
        {
            int hpAbs = 0;
            for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                    Mathf.Max(hpAbs, Mathf.Abs(EnemyMinionField.instance.minions[m].baseHp - EnemyMinionField.instance.minions[m].final_hp));

            if(hpAbs > 3)
                return true;
        }
        //상황8 . 상대영웅의 체력이 적을경우
        else if (n == 8)
        {
            int sumAtk = 0;
            for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                    sumAtk += EnemyMinionField.instance.minions[m].final_atk;

            if (sumAtk >= HeroManager.instance.heroHpManager.nowPlayerHp)
                return true;

            if (HeroManager.instance.heroHpManager.nowPlayerHp <= 14)
                return true;
        }
        else
        {
            Debug.LogError("사용조건을 적어주세요!!");
        }
        return false;
    }

    #region[주문 사용 조건 TEXT]
    public string CaseBySpellText(int n)
    {
        if (n == 0)
            return "자신영웅의 체력이 적음";
        else if (n == 1)
            return "상대필드에 강한 하수인이 있음";
        else if (n == 2)
            return "상대필드에 하수인이 많음";
        else if (n == 3)
            return "패가 부족함음";
        else if (n == 4)
            return "마나수정이 적음";
        else if (n == 5)
            return "하수인 소환을 위해서 마나 회복";
        //상황6 . 자신필드에 하수인이 많을경우
        else if (n == 6)
            return "필드에 하수인이 많음";
        //상황7 . 하수인의 체력이 적을경우
        else if (n == 7)
            return "에이스 하수인의 체력이 적음";
        //상황8 . 상대영웅의 체력이 적을경우
        else if (n == 8)
            return "상대 영웅 체력이 적음";
        return "예외";

    }
    #endregion

    #endregion

    #region[드루이드 공격 명령]
    public void AttackOrder(int n, string target)
    {
        GameEventManager.instance.EventAdd(3f);
        EnemyMinionField.instance.attack_ready = 1f;
        EnemyMinionField.instance.minions[n].stealth = false;
        SoundManager.instance.PlayMinionSE(EnemyMinionField.instance.minions[n].minion_name, 미니언상태.공격);
        if (target.Equals("Hero"))
        {
            AttackManager.instance.PopAllDamageObj();
            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, EnemyMinionField.instance.minions[n].final_atk);
            Debug.Log(EnemyMinionField.instance.minions[n].minion_name + " : " + "Hero");
            Vector3 targetPos = HeroManager.instance.playerHero.transform.position;
            targetPos -= new Vector3(0, 0, -10);
            EnemyMinionField.instance.minions_Attack_pos[n] = targetPos;
        }
        else
        {
            int targetN = 0;
            int.TryParse(target, out targetN);
            AttackManager.instance.PopAllDamageObj();
            AttackManager.instance.AddDamageObj(EnemyMinionField.instance.minions[n].damageEffect, MinionField.instance.minions[targetN].final_atk);
            AttackManager.instance.AddDamageObj(MinionField.instance.minions[targetN].damageEffect, EnemyMinionField.instance.minions[n].final_atk);
            Vector3 targetPos = MinionField.instance.minions[targetN].transform.position;
            targetPos -= new Vector3(0, 0, -10);
            Debug.Log(EnemyMinionField.instance.minions[n].minion_name + " : " + MinionField.instance.minions[targetN].minion_name);
            EnemyMinionField.instance.minions_Attack_pos[n] = targetPos;
        }

    }
    #endregion
}
