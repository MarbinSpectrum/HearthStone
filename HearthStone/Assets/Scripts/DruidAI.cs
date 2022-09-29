using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidAI : MonoBehaviour
{
    public static DruidAI instance;

    [HideInInspector] public List<string> AIDeck;

    public enum AI_Act
    {
        SpawnMinion,  //하수인을 소환할 수 있다면
        SpellRun,     //주문을 사용 할 수 있다면 && 주문을 사용하는 것으로 이득이 생길경우
        AttackMinion, //필드에 공격 할 수 있는 하수인이 존재한다면 && 공격하는 것이 이득인 경우 
        UseHero,      //영웅능력을 사용할 수 있다면 && 영웅으로 공격하는것이 이득인 경우
        GameEnd,      //게임이 종료됨
        TurnEnd       //위의 사항이 모두 적용되지않을 경우
    };

    public class AI_ActData
    {
        public AI_Act act;
        public List<string> data = new List<string>();
        public AI_ActData(AI_Act pAct)
        {
            act = pAct;
        }
    }

    public enum AI_SpellAct
    {
        //위로 올라갈수록 발동 우선순위가 높고
        //아래로 내려갈수록 발동 우선순위가 낮다.

        LowHp,          //자신영웅의 체력이 적을경우
        StrongEnemy,    //상대필드에 강한 하수인이 있을경우
        ManyEnemy,      //상대필드에 하수인이 많을경우
        HandLack,       //패가 부족할경우
        ManaLack,       //마나수정이 적을경우
        MinionSpawn,    //마나를 보충하면 하수인을 소환할 수 있다면
        ManyMyMinion,   //자신필드에 하수인이 많을경우
        WeakMyMinion,   //하수인의 체력이 적을경우
        LowEnemyHp,     //상대영웅의 체력이 적을경우
        MAX
    }

    Dictionary<AI_SpellAct, List<string>> caseByCard = new Dictionary<AI_SpellAct, List<string>>();

    private int FieldValue(int num,int atk,int hp)
    {
        return num * atk * hp;
    }

    private AI_ActData SelectChoice()
    {
        AI_ActData aiData = new AI_ActData(AI_Act.GameEnd);

        if (!BattleUI.instance.gameStart)
        {
            aiData.act = AI_Act.GameEnd;
            return aiData;
        }

        #region[하수인이 내는것을 고려]
        if (EnemyMinionField.instance.minionNum < 7)
        {
            //필드가 꽉차지 않았다.

            int cardIdx = -1;
            int cmpMana = -1;
            int cmpStat = 0;

            for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
            {
                int nowMana = ManaManager.instance.enemyNowMana;
                string cardName = EnemyCardHand.instance.nowCard[i];
                Vector2Int pair = DataMng.instance.GetPairByName(
                    DataParse.GetCardName(cardName));

                //카드 정보 가져오기
                string cardType = DataMng.instance.ToString(pair.x, pair.y, "카드종류");
                int cardCost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");
                int atk = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
                int hp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
                int cardStat = atk + hp;

                if (cardType != "하수인")
                {
                    //하수인이 아니다.
                    continue;
                }
                if (nowMana < cardCost)
                {
                    //낼수 없는 카드다.
                    continue;
                }
                if(cmpMana > cardCost)
                {
                    //전에 찾은 카드보다 비용이 가볍다.
                    continue;
                }
                if (cmpStat > cardStat)
                {
                    //전에 찾은 카드보다 스텟이 약하다.
                    continue;
                }

                cmpMana = cardCost;
                cmpStat = cardStat;
                cardIdx = i;
            }
            if (cardIdx != -1)
            {
                Debug.Log("하수인 내는것을 고려");
                aiData.act = AI_Act.SpawnMinion;
                aiData.data.Add(cardIdx.ToString());
                return aiData;
            }
        }
        #endregion

        #region[주문을 쓰는것을 고려]
        {
            AI_SpellAct importantCase = AI_SpellAct.MAX;   
            int cardIdx = -1;

            for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
            {
                int nowMana = ManaManager.instance.enemyNowMana;
                string cardName = EnemyCardHand.instance.nowCard[i];
                Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(cardName));
                string cardType = DataMng.instance.ToString(pair.x, pair.y, "카드종류");
                int cardCost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");
                if (cardType != "주문")
                    continue;
                if (nowMana < cardCost)
                    continue;

                for (AI_SpellAct aCase = 0; aCase < AI_SpellAct.MAX; aCase++)
                {
                    if (CaseBySpell(cardName, aCase))
                    {
                        //상황에 해당하는 카드이고
                        if (cardIdx == -1 || importantCase >= aCase)
                        {
                            //우선순위가 높은 상황인 경우
                            importantCase = aCase;
                            cardIdx = i;
                        }
                    }
                }
            }

            if (cardIdx != -1)
            {
                Debug.Log("주문을 내는것을 고려");
                aiData.act = AI_Act.SpellRun;
                aiData.data.Add(cardIdx.ToString());
                aiData.data.Add(CaseBySpellText(importantCase));
                return aiData;
            }
        }
        #endregion

        #region[하수인으로 공격하는 것을 고려]
        {
            int hpSum = 0;
            int atkSum = 0;
            int minionSum = 0;

            List<int> Ai_MinionIdx = new List<int>();
            for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
            {
                //공격가능한 AI하수인 묶음을 미리 구해놓은다.
                if (EnemyMinionField.instance.minions[i].gameObject.activeSelf == false)
                    continue;
                if (EnemyMinionField.instance.minions[i].checkCanAttack == false)
                    continue;
                Ai_MinionIdx.Add(i);
                hpSum += EnemyMinionField.instance.minions[i].final_hp;
                atkSum += EnemyMinionField.instance.minions[i].final_atk;
                minionSum += 1;
            }

            int pHpSum = 0;
            int pAtkSum = 0;
            int pMinionSum = 0;
            List<int> playerNormalMinions = new List<int>();
            List<int> playerTauntMinions = new List<int>();
            for (int i = 0; i < MinionField.instance.minions.Length; i++)
            {
                if (MinionField.instance.minions[i].gameObject.activeSelf == false)
                    continue;
                if (MinionField.instance.minions[i].stealth)
                    continue;
                //공격대상으로 가능한 하수인 묶음을 미리 구해놓은다.
                //도발하수인은 먼저 공격해야하므로 먼저 고려
                if (MinionField.instance.minions[i].taunt)
                    playerTauntMinions.Add(i);
                else
                    playerNormalMinions.Add(i);
                pHpSum += MinionField.instance.minions[i].final_hp;
                pAtkSum += MinionField.instance.minions[i].final_atk;
                pMinionSum += 1;
            }

            int baseFieldValue = FieldValue(minionSum, atkSum, hpSum);
            int basePlayerFieldValue = FieldValue(pMinionSum, pAtkSum, pHpSum);
            int cmpFieldValue = baseFieldValue - basePlayerFieldValue;
            int cmpPlayerDamage = 0;

            List<string> resultCase0 = null; //데미지를 많이 넣은경우
            List<string> resultCase1 = null; //필드를 유리하게 만드는경우
            for (int bit = 0; bit < (1<< Ai_MinionIdx.Count); bit++)
            {
                List<int> aiGroupA = new List<int>();
                List<int> aiGroupB = new List<int>();
                for (int i = 0; i < Ai_MinionIdx.Count; i++)
                {
                    //비트 마스킹을 이용해서
                    //그룹A와 그룹B로 나눈다.
                    int andBit = bit & (1 << i);
                    if (andBit != 0)
                        aiGroupA.Add(Ai_MinionIdx[i]);
                    else
                        aiGroupB.Add(Ai_MinionIdx[i]);
                }

                do
                {
                    //도발하수인은 그룹C
                    List<int> playerGroupC = new List<int>(playerTauntMinions);
                    do
                    {
                        //일반하수인은 그룹D
                        List<int> playerGroupD = new List<int>(playerNormalMinions);
                        do
                        {
                            //전투 시뮬레이션
                            //하수인들을 스택에 넣는다.
                            List<string> resultData = new List<string>();
                            Stack<int> aiMinions = new Stack<int>();
                            Stack<int> playerMinions = new Stack<int>();
                            for (int i = 0; i < aiGroupA.Count; i++)
                                aiMinions.Push(aiGroupA[i]);

                            for (int i = 0; i < playerGroupD.Count; i++)
                            {
                                //일반하수인은 나중에 처리하므로 먼저 넣는다.
                                playerMinions.Push(playerGroupD[i]);
                            }
                            for (int i = 0; i < playerGroupC.Count; i++)
                                playerMinions.Push(playerGroupC[i]);

                            int resultAtk = atkSum;
                            int resultHp = hpSum;
                            int resultMinion = minionSum;
                            int pResultAtk = pAtkSum;
                            int pResultHp = pHpSum;
                            int pResultMinion = pMinionSum;

                            int resultPlayerDamage = 0;
                            int pIdx = -1;
                            
                            int tatk = -1;
                            int thp = -1;
                            while (aiMinions.Count > 0)
                            {
                                //ai하수인이 순차적으로 공격한다.
                                int aIdx = aiMinions.Pop();
                                int atk = EnemyMinionField.instance.minions[aIdx].final_atk;
                                int hp = EnemyMinionField.instance.minions[aIdx].final_hp;

                                if (playerMinions.Count > 0)
                                {
                                    if (pIdx == -1)
                                    {
                                        //전투중인 하수인이 없는경우
                                        pIdx = playerMinions.Peek();
                                        tatk = EnemyMinionField.instance.minions[pIdx].final_atk;
                                        thp = EnemyMinionField.instance.minions[pIdx].final_hp;
                                    }

                                    //하수인 공격정보 저장
                                    if(pIdx != -1)
                                    {
                                        resultData.Add(aIdx.ToString());
                                        resultData.Add(pIdx.ToString());
                                    }

                                    //AI 하수인 체력 변화
                                    if(hp - tatk <= 0)
                                    {
                                        resultHp -= hp;
                                        resultAtk -= atk;
                                        resultMinion--;
                                    }
                                    else
                                        resultHp -= tatk;

                                    //타겟 하수인 처리
                                    if (thp - atk <= 0)
                                    {
                                        pIdx = -1;
                                        playerMinions.Pop();
                                        pResultAtk -= tatk;
                                        pResultHp -= thp;
                                        pResultMinion--;
                                    }
                                    else
                                    {
                                        thp -= atk;
                                        pResultHp -= atk;
                                    }

                                }
                                else
                                {
                                    //하수인이 없으므로 공격
                                    resultPlayerDamage += atk;
                                    resultData.Add(aIdx.ToString());
                                    resultData.Add("Hero");
                                }                             
                            }

                            if(pResultMinion <= playerGroupD.Count)
                            {
                                //남은 하수인이 그룹D 이하라는것은
                                //그룹C가 다 소멸했다는것
                                //즉, 도발하수인이 남지 않았다는것
                                for (int i = 0; i < aiGroupB.Count; i++)
                                {
                                    //그룹B의 하수인들이 모두 영웅을 공격
                                    int aidx = aiGroupB[i];
                                    int atk = EnemyMinionField.instance.minions[aidx].final_atk;
                                    resultPlayerDamage += atk;
                                    resultData.Add(aidx.ToString());
                                    resultData.Add("Hero");
                                }
                            }

                            int resultFieldValue = FieldValue(resultMinion, resultAtk, resultHp);
                            int resultPlayerFieldValue = FieldValue(pResultMinion, pResultAtk, pResultHp);
                            int fieldCmp = resultFieldValue - resultPlayerFieldValue;
                            if (resultData.Count > 0)
                            {
                                if (resultPlayerDamage >= HeroManager.instance.heroHpManager.nowPlayerHp * 0.35f)
                                {
                                    //데미지를 많이 준다는 선택지
                                    if (cmpPlayerDamage <= resultPlayerDamage)
                                    {
                                        cmpPlayerDamage = resultPlayerDamage;
                                        resultCase0 = resultData;
                                    }
                                }
                                else if (cmpFieldValue <= fieldCmp && pIdx == -1)
                                {
                                    //필드 가치를 높이는 선택지
                                    //상대 필드와 비교해서 부유해지는
                                    //최대한 부유해지는 선택지를 고른다.
                                    //미니언을 처리한 경우만 검사
                                    cmpFieldValue = fieldCmp;
                                    resultCase1 = resultData;
                                }
                            }
                        }
                        while (MyLib.Algorithm.Next_Permutation(playerGroupD)); //그룹 D를 다른 순서로 배치
                    }
                    while (MyLib.Algorithm.Next_Permutation(playerGroupC)); //그룹 C를 다른 순서로 배치
                } 
                while (MyLib.Algorithm.Next_Permutation(aiGroupA)); //그룹 A를 다른 순서로 배치
            }

            if(resultCase0 != null)
            {
                aiData.act = AI_Act.AttackMinion;
                aiData.data = resultCase0;
                return aiData;
            }
            else if (resultCase1 != null)
            {
                aiData.act = AI_Act.AttackMinion;
                aiData.data = resultCase1;
                return aiData;
            }
        }
      
        #endregion

        #region[영웅능력을 사용 및 공격격]
        if (ManaManager.instance.enemyNowMana >= 2 && HeroManager.instance.heroPowerManager.enemyCanUse)
        {
            Debug.Log("영웅 능력 사용 고려");
            aiData.act = AI_Act.UseHero;
            aiData.data.Add("UseAbility");
            return aiData;
        }
        else if (HeroManager.instance.heroAtkManager.enemyAttackCheck && !HeroManager.instance.enemyFreezeObj.activeSelf)
        {
            List<int> playerMinions = new List<int>();

            int hpSum = 0;
            int atkSum = 0;
            int minionSum = 0;
            for (int i = 0; i < EnemyMinionField.instance.minions.Length; i++)
            {
                //현재 AI의 필드의 가치를 구한다.
                if (EnemyMinionField.instance.minions[i].gameObject.activeSelf == false)
                    continue;
                if (EnemyMinionField.instance.minions[i].checkCanAttack == false)
                    continue;
                hpSum += EnemyMinionField.instance.minions[i].final_hp;
                atkSum += EnemyMinionField.instance.minions[i].final_atk;
                minionSum += 1;
            }

            int pMinionSum = 0;
            int pAtkSum = 0;
            int pHpSum = 0;
            for (int i = 0; i < MinionField.instance.minions.Length; i++)
            {
                //공격가능한 하수인들을 가져온다.
                if (MinionField.instance.minions[i].gameObject.activeSelf == false)
                    continue;
                pMinionSum += 1;
                pAtkSum += MinionField.instance.minions[i].final_atk;
                pHpSum += MinionField.instance.minions[i].final_hp;

                if (MinionField.instance.minions[i].stealth)
                {
                    //은신상태이면 공격하지 못한다.
                    continue;
                }
                if (MinionField.instance.minions[i].final_hp > HeroManager.instance.heroAtkManager.enemyFinalAtk)
                {
                    //하수인의 체력이 영웅의 공격보다 높다.
                    //공격해도 하수인이 죽지않는다. 공격하지 않는다.
                    continue;
                }

                if (MinionField.instance.minions[i].final_atk > 5)
                {
                    //공격력이 높은 몬스터는 공격하지말자
                    continue;
                }
                if (MinionField.instance.minions[i].final_atk >= HeroManager.instance.heroHpManager.nowEnemyHp +
                    HeroManager.instance.heroHpManager.enemyShield)
                {
                    //공격력이 쉴드+체력보다 높다.
                    //공격하면 죽는다 공격하지 않는다.
                    continue;
                }

                if(MinionManager.instance.CheckTaunt(false))
                {
                    //도발하수인이 없다.
                    //공격가능한 아무 하수인을 목록에 추가
                    playerMinions.Add(i);
                }
                else if (MinionField.instance.minions[i].taunt)
                {
                    //도발하수인이다.
                    //리스트에는 도발하수인만 들어갈것이다.
                    playerMinions.Add(i);
                }
            }
            int aiFieldValue = FieldValue(minionSum, atkSum, hpSum); //AI의 필드가치
            int playerFieldValue = FieldValue(pMinionSum, pAtkSum, pHpSum); //플레이어의 현재 필드가치
            int cmpFieldValue = aiFieldValue - playerFieldValue; //필드의 격차

            int resultAtkIdx = -1;
            for (int i = 0; i < playerMinions.Count; i++)
            {
                int pIdx = playerMinions[i];
                int tAtk = MinionField.instance.minions[pIdx].final_atk;
                int tHp = MinionField.instance.minions[pIdx].final_hp;
                int pResultMinionSum = pMinionSum - 1;
                int pResultAtk = pAtkSum - tAtk;
                int pResultHp = pHpSum - tHp;
                int resultPlayerFieldValue = FieldValue(pResultMinionSum, pResultAtk, pResultHp); //공격했을시 필드가치
                int fieldCmp = aiFieldValue - resultPlayerFieldValue; //공격했을시 필드의 격차

                if (cmpFieldValue <= fieldCmp)
                {
                    //상대 필드와 비교해서 부유해지는
                    //최대한 부유해지는 선택지를 고른다.
                    cmpFieldValue = fieldCmp;
                    resultAtkIdx = pIdx;
                }
            }

            if (resultAtkIdx != -1)
            {
                Debug.Log("영웅 능력으로 공격");
                aiData.act = AI_Act.UseHero;
                aiData.data.Add("AttackHero");
                aiData.data.Add(resultAtkIdx.ToString());
                return aiData;
            }
            else if (MinionManager.instance.CheckTaunt(false))
            {
                Debug.Log("영웅 능력으로 공격");
                aiData.act = AI_Act.UseHero;
                aiData.data.Add("AttackHero");
                aiData.data.Add("Hero");
                return aiData;
            }
            else
            {
                //공격을 안하는것이 이득인 경우
            }
        }

        #endregion

        aiData.act = AI_Act.TurnEnd;
        return aiData;
    }

    #region[Awake]
    public void Awake()
    {
        instance = this;

        AI_SpellAct_Setting();
        AI_Deck_Setting();
    }
    #endregion

    #region[AI Deck Setting]
    private void AI_Deck_Setting()
    {
        //덱정보 읽어오기
        TextAsset AI_DeckData = (TextAsset)Resources.Load("AI/AI_Deck");

        //덱 등록
        Deck ai_deck = new Deck("AI", Job.드루이드, AI_DeckData.text);
        AIDeck = ai_deck.GetInGameDeck();

        //덱 셔플
        Deck.Shuffle(AIDeck, 1000);
    }
    #endregion

    #region[AI_SpellAct_Setting]
    private void AI_SpellAct_Setting()
    {
        //주문에 따른 행동방식 설정
        for (AI_SpellAct aCase = 0; aCase < AI_SpellAct.MAX; aCase++)
        {
            List<string> spellList = new List<string>();
            switch(aCase)
            {
                case AI_SpellAct.LowHp:
                {
                    //상황0 . 자신영웅의 체력이 적을경우
                    spellList.Add("치유의 손길");
                    spellList.Add("할퀴기");
                }
                break;
                case AI_SpellAct.StrongEnemy:
                {
                    //상황1 . 상대필드에 강한 하수인이 있을경우
                    spellList.Add("자연화");
                    spellList.Add("별빛섬광");
                    spellList.Add("별똥별");
                    spellList.Add("휘둘러치기");
                }
                break;
                case AI_SpellAct.ManyEnemy:
                    {
                        //상황2 . 상대필드에 하수인이 많을경우
                        spellList.Add("달빛 섬광");
                        spellList.Add("할퀴기");
                        spellList.Add("휘둘러치기");
                        spellList.Add("천벌");
                        spellList.Add("야생성");
                        spellList.Add("별똥별");
                        spellList.Add("자연의 군대");
                    }
                    break;
                case AI_SpellAct.HandLack:
                    {
                        //상황3 . 패가 부족할경우
                        spellList.Add("급속 성장");
                        spellList.Add("넘치는마나");
                        spellList.Add("육성");
                    }
                    break;
                case AI_SpellAct.ManaLack:
                    {
                        //상황4 . 마나수정이 적을경우
                        spellList.Add("급속 성장");
                        spellList.Add("육성");
                    }
                    break;
                case AI_SpellAct.MinionSpawn:
                    {
                        //상황5 . 마나를 보충하면 하수인을 소환할 수 있다면
                        spellList.Add("정신 자극");
                        spellList.Add("동전 한 닢");
                    }
                    break;
                case AI_SpellAct.ManyMyMinion:
                    {
                        //상황6 . 자신필드에 하수인이 많을경우
                        spellList.Add("야생의 징표");
                        spellList.Add("야생의 힘");
                        spellList.Add("자연의 징표");
                        spellList.Add("숲의 영혼");
                        spellList.Add("야생의 포효");
                    }
                    break;
                case AI_SpellAct.WeakMyMinion:
                    {
                        //상황7 . 하수인의 체력이 적을경우
                        spellList.Add("치유의 손길");
                    }
                    break;
                case AI_SpellAct.LowEnemyHp:
                    {
                        //상황8 . 상대영웅의 체력이 적을경우
                        spellList.Add("자연의 군대");
                    }
                    break;
            }

            caseByCard[aCase] = spellList;
        }
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

        while (TurnManager.instance.turn == Turn.상대방)
        {
            yield return new WaitWhile(() => GameEventManager.instance.GetEventValue() > 0.001f);
            GameEventManager.instance.EventAdd(0.1f);

            yield return new WaitForSeconds(1f);

            AI_ActData aiData = SelectChoice();

            #region[하수인 소환]
            if (aiData.act == AI_Act.SpawnMinion)
            {
                int cardIdx = -1;
                if(int.TryParse(aiData.data[0], out cardIdx))
                {
                    EnemyCardHand.instance.UseCard(cardIdx);
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    Debug.LogError("하수인 소환 버그");
                }
            }
            #endregion

            #region[주문 사용]
            else if (aiData.act == AI_Act.SpellRun)
            {
                int cardIdx = -1;
                if (int.TryParse(aiData.data[0], out cardIdx))
                {
                    Debug.Log(aiData.data[1] + " : " + EnemyCardHand.instance.nowCard[cardIdx]);
                    EnemyCardHand.instance.UseCard(cardIdx);
                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    Debug.LogError("주문 발동 버그");
                }
            }
            #endregion

            #region[하수인으로 공격]
            else if (aiData.act == AI_Act.AttackMinion)
            {
                int attackIdx = -1;
                if (int.TryParse(aiData.data[0], out attackIdx))
                {
                    string targetName = aiData.data[1];
                    AttackOrder(attackIdx, targetName);
                    yield return new WaitForSeconds(2f);
                }
                else
                {
                    Debug.LogError("공격 버그");
                }
            }
            #endregion

            #region[영웅 사용]
            else if (aiData.act == AI_Act.UseHero)
            {
                if (aiData.data[0] == "UseAbility")
                {
                    //영웅능력사용
                    HeroManager.instance.heroPowerManager.UseHeroAbility(true);
                    yield return new WaitForSeconds(2f);
                }
                else if (aiData.data[0] == "AttackHero")
                {
                    if (aiData.data[1]== "Hero")
                    {
                        AttackManager.instance.PopAllDamageObj();
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, HeroManager.instance.heroAtkManager.enemyFinalAtk);
                        HeroManager.instance.heroAtkManager.HeroAttack(true, HeroManager.instance.playerHero.transform.position);
                        yield return new WaitForSeconds(2f);
                    }
                    else 
                    {
                        int attackIdx = -1;
                        if (int.TryParse(aiData.data[1], out attackIdx))
                        {
                            AttackManager.instance.PopAllDamageObj();
                            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, 
                                MinionField.instance.minions[attackIdx].final_atk);
                            AttackManager.instance.AddDamageObj(MinionField.instance.minions[attackIdx].damageEffect, 
                                HeroManager.instance.heroAtkManager.enemyFinalAtk);
                            HeroManager.instance.heroAtkManager.HeroAttack(true, MinionField.instance.minions[attackIdx].transform.position);
                            yield return new WaitForSeconds(2f);
                        }
                        else
                        {
                            Debug.LogError("공격 버그");
                        }
                    }
                }
            }
            #endregion

            #region[턴 종료]
            else if (aiData.act == AI_Act.TurnEnd)
            {
                Debug.Log("턴끝냄");
                SoundManager.instance.PlaySE("상대가턴종료버튼누름");
                TurnManager.instance.turnBtnAni.SetTrigger("내턴");
                TurnManager.instance.turnEndTrigger = true;
                break;
            }
            #endregion

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
                if (HeroManager.instance.heroHpManager.nowEnemyHp < 
                    MinionManager.instance.eventMininon.abilityList[MinionManager.instance.eventNum].Ability_data[0])
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
        int spellDamage = spellAbility.AbilityData[0];
        MinionManager.instance.selectMinionEvent = true;
        switch (spellAbility.AbilityType)
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
    public bool CaseBySpell(string cardName, AI_SpellAct aCase)
    {
        if(caseByCard[aCase].Contains(cardName) == false)
        {
            //해당 상황에 포함되지 않는 카드
            return false;
        }

        //spellName주문카드가 aCase에 맞는 카드인지 검사
        switch (aCase)
        {
            case AI_SpellAct.LowHp:
                //상황0 . 자신영웅의 체력이 적을경우
                {
                    int sumAtk = 0;
                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                            sumAtk += MinionField.instance.minions[m].final_atk;

                    if (sumAtk >= HeroManager.instance.heroHpManager.nowEnemyHp)
                    {
                        //상대필드 공격력 합계가 자신의 체력을 넘는다.
                        return true;
                    }

                    if (HeroManager.instance.heroHpManager.nowEnemyHp 
                        <= HeroManager.instance.heroHpManager.maxEnemyHp / 2)
                    {
                        //영웅의 체력이 절반이하다.
                        return true;
                    }
                }
                break;
            case AI_SpellAct.StrongEnemy:
                //상황1 . 상대필드에 강한 하수인이 있을경우
                {

                    for (int m = 0; m < MinionField.instance.minions.Length; m++)
                        if (MinionField.instance.minions[m].gameObject.activeSelf)
                        {
                            int sumStat = MinionField.instance.minions[m].final_atk +
                                MinionField.instance.minions[m].final_hp;

                            if (sumStat >= 6)
                            {
                                //스텟의 총합이 6이상인 하수인은 강한 하수인이라고 판단.
                                return true;
                            }
                        }
                }
                break;
            case AI_SpellAct.ManyEnemy:
                //상황2 . 상대필드에 하수인이 많을경우
                {

                    if (MinionField.instance.minionNum >= MinionField.MINION_SLOT_NUM / 2)
                    {
                        //상대필드에 하수인이 절반이상 차있을 경우
                        return true;
                    }
                    if (MinionField.instance.minionNum >= 2 &&
                        MinionField.instance.minionNum > EnemyMinionField.instance.minionNum)
                    {
                        //자신필드보다 하수인이 많을 경우
                        return true;
                    }
                }
                break;
            case AI_SpellAct.HandLack:
                //상황3 . 패가 부족할경우
                {
                    if(EnemyCardHand.instance.nowHandNum <= 5)
                    {
                        if (cardName == "급속 성장" && ManaManager.instance.enemyMaxMana >= 10)
                        {
                            if (ManaManager.instance.enemyMaxMana >= 10)
                            {

                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                break;
            case AI_SpellAct.ManaLack:
                //상황4 . 마나수정이 적을경우
                {
                    if (ManaManager.instance.enemyMaxMana <= 4)
                        return true;
                }
                break;
            case AI_SpellAct.MinionSpawn:
                //상황5 . 마나를 보충하면 하수인을 소환할 수 있다면
                {
                    int addMana = 0;
                    if (cardName == "동전 한 닢")
                        addMana = 1;
                    else if (cardName == "정신 자극")
                        addMana = 2;

                    for (int i = 0; i < EnemyCardHand.instance.nowCard.Count; i++)
                    {
                        int nowMana = ManaManager.instance.enemyNowMana + addMana;
                        string nowCardName = EnemyCardHand.instance.nowCard[i];
                        Vector2Int pair = DataMng.instance.GetPairByName(DataParse.GetCardName(nowCardName));
                        string cardType = DataMng.instance.ToString(pair.x, pair.y, "카드종류");
                        int cardCost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");
                        if (cardType != "하수인")
                            continue;
                        if (nowMana >= cardCost)
                            return true;
                    }
                }
                break;
            case AI_SpellAct.ManyMyMinion:
                //상황6 . 자신필드에 하수인이 많을경우
                {
                    if (EnemyMinionField.instance.minionNum >= 4)
                        return true;
                    if (EnemyMinionField.instance.minionNum >= 2 &&
                        EnemyMinionField.instance.minionNum > MinionField.instance.minionNum)
                        return true;
                }
                break;
            case AI_SpellAct.WeakMyMinion:
                //상황7 . 하수인의 체력이 적을경우
                {
                    int hpAbs = 0;
                    for (int m = 0; m < EnemyMinionField.instance.minions.Length; m++)
                        if (EnemyMinionField.instance.minions[m].gameObject.activeSelf)
                            Mathf.Max(hpAbs, Mathf.Abs(EnemyMinionField.instance.minions[m].baseHp -
                                EnemyMinionField.instance.minions[m].final_hp));

                    if (hpAbs > 3)
                        return true;
                }
                break;
            case AI_SpellAct.LowEnemyHp:
                //상황8 . 상대영웅의 체력이 적을경우
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
                break;
            default:
                {
                    Debug.LogError("사용조건을 적어주세요!!");
                }
                break;
        }
        return false;
    }

    #region[주문 사용 조건 TEXT]
    public string CaseBySpellText(AI_SpellAct aCase)
    {
        switch (aCase)
        {
            case AI_SpellAct.LowHp:
                return "자신영웅의 체력이 적음";
            case AI_SpellAct.StrongEnemy:
                return "상대필드에 강한 하수인이 있음";
            case AI_SpellAct.ManyEnemy:
                return "상대필드에 하수인이 많음";
            case AI_SpellAct.HandLack:
                return "패가 부족함";
            case AI_SpellAct.ManaLack:
                return "마나수정이 적음";
            case AI_SpellAct.MinionSpawn:
                return "하수인 소환을 위해서 마나 회복";
            case AI_SpellAct.ManyMyMinion:
                return "필드에 하수인이 많음";
            case AI_SpellAct.WeakMyMinion:
                return "에이스 하수인의 체력이 적음";
            case AI_SpellAct.LowEnemyHp:
                return "상대 영웅 체력이 적음";

        }
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
