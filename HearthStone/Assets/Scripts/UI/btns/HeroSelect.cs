using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HeroSelect : Btn
{
    public GameObject select;

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
        btnImg.raycastTarget =
            //카드를 드래그중이 아니고
            (!DragCardObject.instance.dragCard || (DragCardObject.instance.dragCard && DragCardObject.instance.dragSelectCard)) && 
            //적영웅이 아닌오브젝트인데 패를 축소시켰을 경우
            ((!enemy && (CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패 기본상태") || CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패아래로내리기"))) || enemy);
        if (Input.GetMouseButtonUp(0))
            SetSelect(false);
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
        if(Input.GetMouseButton(0))
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

    #region[영웅 선택 이펙트 설정]
    public void SetSelect(bool flag)
    {
        if (MinionManager.instance.selectMinionEvent)
            return;

        if (!flag)
        {
            DragLineRenderer.instance.selectTarget = false;
            DragLineRenderer.instance.dragTargetPos = Vector2.zero;
            select.SetActive(false);
        }
        else if ((enemy && DragLineRenderer.instance.CheckMask(타겟.적영웅)) || (!enemy && DragLineRenderer.instance.CheckMask(타겟.아군영웅)))
        {
            if (!DragLineRenderer.instance.CheckMask(타겟.실행주체))
                if (DragLineRenderer.instance.CheckActObj(gameObject))
                    return;
            if (!DragCardObject.instance.dragSelectCard)
            {
                if (MinionManager.instance.CheckTaunt(enemy))
                {
                    DragLineRenderer.instance.selectTarget = true;
                    DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);
                    if (enemy)
                    {
                        if (MinionDrag.dragMinionNum != -1)
                        {
                            MinionField.instance.minions[MinionDrag.dragMinionNum].stealth = false;
                            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, MinionField.instance.minions[MinionDrag.dragMinionNum].final_atk);
                        }
                        else
                        {
                            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, HeroManager.instance.heroAtkManager.playerFinalAtk);
                        }
                    }
                    else
                    {
                        AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, 4);
                    }
                }
            }
            else
            {
                DragLineRenderer.instance.selectTarget = true;
                DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);
                if (enemy)
                    SpellManager.instance.targetHero = 2;
                else
                    SpellManager.instance.targetHero = 1;
                SpellManager.instance.targetMinion = null;

            }
            select.SetActive(true);
        }
    }
    #endregion

    #region[ActBtn]
    public override void ActBtn()
    {

    }
    #endregion


}
