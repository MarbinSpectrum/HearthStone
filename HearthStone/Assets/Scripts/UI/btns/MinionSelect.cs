using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinionSelect : Btn
{
    public MinionObject minionObject;

    public GameObject select_normal;
    public GameObject select_taunt;
    public GameObject select_legend;
    bool select = false;

    public bool enemy;

    #region[Awake]
    public override void Awake()
    {
        AddEvent();
        EventTrigger.Entry pUp = new EventTrigger.Entry();
        pUp.eventID = EventTriggerType.PointerUp;
        pUp.callback.AddListener((data) =>
        {
            pointerUp();
        });
        btnEvent.triggers.Add(pUp);
    }
    #endregion

    #region[Update]
    public override void Update()
    {
        btnImg.raycastTarget = !DragCardObject.instance.dragCard || (DragCardObject.instance.dragCard && DragCardObject.instance.dragSelectCard);
        enemy = minionObject.enemy;
        if(Input.GetMouseButtonUp(0))
            SetSelect(false);
        if (minionObject.taunt)
        {
            select_taunt.SetActive(select);
            select_normal.SetActive(false);
        }
        else
        {
            select_taunt.SetActive(false);
            select_normal.SetActive(select);
        }
        select_legend.SetActive(minionObject.legend && select);

    }
    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region[pointerEnter]
    public override void pointerEnter()
    {
        if (Input.GetMouseButton(0))
            SetSelect(true);
    }
    #endregion

    #region[pointerDown]
    public override void pointerDown()
    {

    }
    #endregion

    #region[pointerExit]
    public override void pointerExit()
    {
        SetSelect(false);
        if (Input.GetMouseButton(0))
            AttackManager.instance.PopAllDamageObj();
        if (DragCardObject.instance.dragSelectCard)
        {
            SpellManager.instance.targetHero = -1;
            SpellManager.instance.targetMinion = null;
        }
    }
    #endregion

    #region[pointerClick]
    public override void pointerClick()
    {

    }
    #endregion

    #region[pointerUp]
    public void pointerUp()
    {
        SetSelect(false);
    }
    #endregion

    #region[하수인 선택 이펙트 설정]
    public void SetSelect(bool flag)
    {
        if (!flag)
        {
            DragLineRenderer.instance.selectTarget = false;
            DragLineRenderer.instance.dragTargetPos = Vector2.zero;
            select = false;
        }
        else if ((enemy && DragLineRenderer.instance.CheckMask(타겟.적하수인)) ||
            (!enemy && DragLineRenderer.instance.CheckMask(타겟.아군하수인)))
        {
            if (!DragLineRenderer.instance.CheckMask(타겟.실행주체) && //실행주체는 선택하지 않는상태
                DragLineRenderer.instance.CheckActObj(gameObject)) //드래그한 대상이 실행주체다.
            {
                //실행주체다. 선택불가.
                return;
            }
            if (DragCardObject.instance.checkNotDamageMinion && //피해입지않은 하수인만 선택하자는 상태
                minionObject.baseHp > minionObject.final_hp) //하수인이 피해입은 상태이다.
            {
                //피해입은 하수인이다. 선택불가.
                return;
            }
            if (DragCardObject.instance.dragSelectCard == false && //카드를 드롭한 상태
                MinionManager.instance.CheckTaunt(minionObject) == false) //도발 하수인 때문에 공격불가.
            {
                //도발 하수인이 있어서 공격실패
                return;
            }
            if(minionObject.stealth)
            {
                //은신상태이다. 선택불가.
                return;
            }
            if (DragCardObject.instance.dragSelectCard == false)
            {
                if (MinionDrag.dragMinionNum != -1)
                {
                    //하수인으로 공격
                    DragLineRenderer.instance.selectTarget = true;
                    DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);

                    //적과 아군 미니언에게 서로 공격력만큼 주고 받는다.
                    AttackManager.instance.AddDamageObj(minionObject.damageEffect,
                        MinionField.instance.minions[MinionDrag.dragMinionNum].final_atk);
                    AttackManager.instance.AddDamageObj(MinionField.instance.minions[MinionDrag.dragMinionNum].damageEffect, 
                        minionObject.final_atk);
                }
                else
                {
                    //영웅으로 공격
                    DragLineRenderer.instance.selectTarget = true;
                    DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);

                    //적과 영웅이 서로 공격력만큼 주고받는다.
                    AttackManager.instance.AddDamageObj(minionObject.damageEffect,
                        HeroManager.instance.heroAtkManager.playerFinalAtk);
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage,
                        minionObject.final_atk);
                }
            }
            else
            {
                DragLineRenderer.instance.selectTarget = true;
                DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);
                SpellManager.instance.targetMinion = minionObject;
                SpellManager.instance.targetHero = -1;
            }
            select = true;
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
