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
            !DragCardObject.instance.dragCard && 
            //적영웅이 아닌오브젝트인데 패를 축소시켰을 경우
            ((!enemy && CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패 기본상태")) || enemy);

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
        {
            if (enemy)
                AttackManager.instance.PopDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage);
            else
                AttackManager.instance.PopDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage);
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
            if (MinionManager.instance.CheckTaunt(enemy))
            {
                DragLineRenderer.instance.selectTarget = true;
                DragLineRenderer.instance.dragTargetPos = new Vector2(transform.position.x, transform.position.y);
                if (enemy)
                {
                    MinionField.instance.minions[MinionDrag.dragMinionNum].stealth = false;
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.enemyHeroDamage, MinionField.instance.minions[MinionDrag.dragMinionNum].final_atk);
                }
                else
                {
                    AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, 4);
                }
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
