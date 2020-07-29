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
                    minionObject.baseHp += (int)minionObject.abilityList[j].Ability_data.y;
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
    [HideInInspector] public bool selectMinionEvent;
    MinionObject eventMininon;
    int eventNum;
    public void SpawnMinionAbility(MinionObject minionObject)
    {
        //전투의 함성 및 연계

        BattlecryEventList.Clear();

        #region[대상선택이 필요한 이벤트]
        bool selectMinionEvent = false;
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 || (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
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
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 || (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.능력치를얻음)
                    BattlecryEventList.Add(j);
                else    if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                    BattlecryEventList.Add(j);
            }
        }
        #endregion

        #region[하수인 소환 이벤트]
        for (int j = 0; j < minionObject.abilityList.Count; j++)
        {
            if (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.전투의함성 || (minionObject.abilityList[j].Condition_type == MinionAbility.Condition.연계 && CheckCombo()))
            {
                if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                    BattlecryEventList.Add(j);
            }
        }

        #endregion

        StartCoroutine(BattlecryEvent(minionObject));
    }
    #endregion

    #region[연계인지 검사]
    bool CheckCombo()
    {
        return CardHand.instance.useCardNum > 1;
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
            #endregion

            #region[하수인 소환 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.하수인소환)
                NowEvent = 3;
            #endregion

            #region[카드뽑기 이벤트]
            else if (minionObject.abilityList[j].Ability_type == MinionAbility.Ability.카드뽑기)
                NowEvent = 4;
            #endregion

            #region[이벤트 처리]
            if (NowEvent == 1)
            {
                SetSelectMask(minionObject.abilityList[j].Ability_type);

                bool targetExistence = false;
                if (DragLineRenderer.instance.CheckMask(타겟.적하수인))
                {
                    for (int m = 0; m < minionList.Count; m++)
                        if (minionList[m].gameObject.activeSelf && minionList[m].enemy && !minionList[m].Equals(minionObject))
                        {
                            targetExistence = true;
                            break;
                        }
                }
                if (DragLineRenderer.instance.CheckMask(타겟.아군하수인))
                {
                    for (int m = 0; m < minionList.Count; m++)
                        if (minionList[m].gameObject.activeSelf && !minionList[m].enemy && !minionList[m].Equals(minionObject))
                        {
                            targetExistence = true;
                            break;
                        }
                }
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
            else if (NowEvent == 4)
            {
                if (minionObject.enemy)
                {
                    EnemyCardHand.instance.DrawCard();
                }
                else
                {
                    for (int c = 0; c < BattleUI.instance.playerCardAni.Length; c++)
                    {
                        if (BattleUI.instance.playerCardAni[c].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
                        {
                            Debug.Log("카드 드로우");
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
                minionObject.freeze = true;
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                minionObject.gotoHandTrigger = true;
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                break;
            case MinionAbility.Ability.침묵시키기:
                minionObject.ActSilence();
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
        }
    }
    #endregion

    #region[선택할 객체 설정]
    public void SetSelectMask(MinionAbility.Ability ability)
    {
        switch (ability)
        {
            case MinionAbility.Ability.빙결시키기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case MinionAbility.Ability.생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case MinionAbility.Ability.아군하수인_주인의패로되돌리기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.영웅에게_피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case MinionAbility.Ability.영웅의_생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                break;
            case MinionAbility.Ability.적군하수인_주인의패로되돌리기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                break;
            case MinionAbility.Ability.침묵시키기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.하수인의_생명력회복:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.하수인에게_피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.피해주기:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적영웅);
                DragLineRenderer.instance.AddMask(타겟.아군영웅);
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.대상의_공격력_생명력_교환:
                DragLineRenderer.instance.InitMask();
                DragLineRenderer.instance.AddMask(타겟.적하수인);
                DragLineRenderer.instance.AddMask(타겟.아군하수인);
                break;
            case MinionAbility.Ability.해당턴동안_능력치부여:
            case MinionAbility.Ability.능력치부여:
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


}

