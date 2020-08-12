using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragCardObject : MonoBehaviour
{
    public static DragCardObject instance;


    public CardView dragCardView;
    RectTransform rectTransform;
    public RectTransform parentsRectTransform;

    public GameObject dragPoint;

    public Image glowImg;

    public DropEffect dropEffect;

    public int dragCardNum = 0;
    public bool dragCard;
    public string dragCardName;
    public bool mouseInMyField;
    public bool mouseInEnemyField;
    public bool mouseInField;

    public bool dragSelectCard;

    public Image playerfield;
    public Image enemyfield;

    [HideInInspector] public bool checkNotDamageMinion = false;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        instance = this;
        dragCardView.hide = true;
        parentsRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    public void Update()
    {
        if (!Input.GetMouseButton(0))
            HideDragCard();

        dragPoint.SetActive(!dragCardView.hide);

        if(dragCardView.hide)
            rectTransform.anchoredPosition = new Vector2(-10000,-10000);
        else
        {
            Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rectTransform.transform.position = v;
        }

        if (dragCardView.cardType == CardType.무기)
            glowImg.sprite = CardHand.instance.weaponImg;
        else if (dragCardView.cardType == CardType.주문)
            glowImg.sprite = CardHand.instance.spellImg;
        else if (dragCardView.cardType == CardType.하수인)
        {
            if (dragCardView.cardLevel.Equals("전설"))
                glowImg.sprite = CardHand.instance.minionImg_legend;
            else
                glowImg.sprite = CardHand.instance.minionImg;
        }
        glowImg.enabled = !dragCardView.hide && CardHand.instance.canUse[dragCardNum] && mouseInField;

        dragCard = !dragCardView.hide;

        List<SpellAbility> spellList = new List<SpellAbility>();

        if ((dragCardView.cardType == CardType.무기 || dragCardView.cardType == CardType.주문) && dragCard)
        {
            dragCardName = dragCardView.cardType == CardType.주문 ? dragCardView.SpellCardNameData : dragCardView.WeaponCardNameData;
            Vector2 pair = DataMng.instance.GetPairByName(DataMng.instance.playData.GetCardName(dragCardName));
            string ability_string = DataMng.instance.ToString((DataMng.TableType)pair.x, (int)pair.y, "명령어");
            spellList = SpellManager.instance.SpellParsing(ability_string);
            spellList.Sort((a, b) =>
            {
                if (a.Condition_type > b.Condition_type)
                    return 1;
                else
                {
                    if (a.Ability_type == SpellAbility.Ability.무기장착)
                        return +1;
                    else
                        return -1;
                }
            });
            for (int i = 0; i < spellList.Count; i++)
                if (spellList[i].Condition_type != SpellAbility.Condition.선택 && SpellManager.instance.CheckEvent(spellList[i]) == SpellManager.EventType.대상선택)
                {
                    dragSelectCard = true;
                    CardHand.instance.handAni.SetBool("패내리기", true);
                    DragLineRenderer.instance.activeObj = gameObject;
                    DragLineRenderer.instance.lineRenderer.enabled = true;
                    DragLineRenderer.instance.startPos = BattleUI.instance.playerSpellPos.transform.position;
                    SpellManager.instance.SetSelectMask(spellList[i].Ability_type);
                    break;
                }

            for (int i = 0; i < spellList.Count; i++)
                if (spellList[i].Condition_type == SpellAbility.Condition.피해입지않은하수인)
                    checkNotDamageMinion = true;
        }
        else if(dragSelectCard)
        {
            if (SpellManager.instance.targetMinion)
            {
                //CardHand.instance.removeCostOffset = CardHand.instance.handCostOffset[dragCardNum];
                //CardHand.instance.handCostOffset[dragCardNum] = 0;
                //for (int i = dragCardNum; i < 9; i++)
                //    CardHand.instance.handCostOffset[i] = CardHand.instance.handCostOffset[i + 1];
                SpellManager.instance.RunSpellTargetMinion(dragCardName, dragCardNum, SpellManager.instance.targetMinion, false);
            }
            else if (SpellManager.instance.targetHero != -1)
            {
                //CardHand.instance.removeCostOffset = CardHand.instance.handCostOffset[dragCardNum];
                //CardHand.instance.handCostOffset[dragCardNum] = 0;
                //for (int i = dragCardNum; i < 9; i++)
                //    CardHand.instance.handCostOffset[i] = CardHand.instance.handCostOffset[i + 1];
                SpellManager.instance.RunSpellTargetHero(dragCardName, dragCardNum, false, SpellManager.instance.targetHero == 2);
            }
            CardHand.instance.handAni.SetBool("패내리기", false);
            dragSelectCard = false;
            DragLineRenderer.instance.InitMask();
            DragLineRenderer.instance.lineRenderer.enabled = false;
            checkNotDamageMinion = false;
        }

        playerfield.raycastTarget = !dragSelectCard;
        enemyfield.raycastTarget = !dragSelectCard;

        dragCardView.gameObject.SetActive(!dragSelectCard);
        glowImg.gameObject.SetActive(!dragSelectCard);
    }

    public void DragAndDropCard()
    {
        if (CardHand.instance.canUse[dragCardNum])
            CardHand.instance.UseCard(dragCardNum);
        HideDragCard();
    }

    public void HideDragCard()
    {
        dragCardView.hide = true;
        mouseInMyField = false;
        mouseInEnemyField = false;
        mouseInField = false;
    }

    public void CheckMouseInMyField(bool b)
    {
        if (!dragCard)
            return;
        mouseInMyField = b && dragCard;
    }

    public void CheckMouseInEnemyField(bool b)
    {
        if (!dragCard)
            return;
        mouseInEnemyField = b && dragCard;
    }

    public void CheckMouseInField(bool b)
    {
        if (!dragCard)
            return;
        mouseInField = b && dragCard;
    }

    public void ShowDropEffectMinion(Vector2 pos,int n)
    {
        dropEffect.dropPos = pos;
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dropEffect.dropRectTransform.transform.position = v;
        dropEffect.dropEffectAni.SetTrigger("Effect_Minion_" + n);
    }

    public void ShowDropEffecWeapon(Vector2 pos, int n)
    {
        dropEffect.dropPos = pos;
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dropEffect.dropRectTransform.transform.position = v;
        dropEffect.dropEffectAni.SetTrigger("Effect_Minion_" + n);
    }

    public void ShowDropEffectSpell(Vector2 pos, int n)
    {
        dropEffect.dropPos = pos;
        Vector2 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dropEffect.dropRectTransform.transform.position = v;
        dropEffect.dropEffectAni.SetTrigger("Effect_Spell_" + n);
        dropEffect.effectArrive = false;
    }

    public void GotoHandEffect(Vector2 pos,string s)
    {
        dropEffect.dropPos = pos;
        dropEffect.dropRectTransform.anchoredPosition = dropEffect.dropPos;
        ShowDragCard(s);
        if (CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
            dropEffect.dropEffectAni.SetTrigger("GoHand");
        else
            dropEffect.dropEffectAni.SetTrigger("GoHand_Small");
        dropEffect.effectArrive = false;
    }

    public void ShowDragCard(string s)
    {
        //cardView.hide = true;
        dragCardView.hide = false;
        CardViewManager.instance.CardShow(ref dragCardView, s);
        CardViewManager.instance.CardShow(ref dropEffect.dropEffectCardView, s);
        CardViewManager.instance.UpdateCardView(0.001f);
    }

    public void ShowDragCard(CardView cardView)
    {
        //cardView.hide = true;
        dragCardView.hide = false;
        CardViewManager.instance.CardShow(ref dragCardView, cardView);
        CardViewManager.instance.CardShow(ref dropEffect.dropEffectCardView, cardView);
        CardViewManager.instance.UpdateCardView(0.001f);
    }
}
