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

    public List<GoToHandEffect> gotoHandList = new List<GoToHandEffect>();

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

        if ((dragCardView.cardType == CardType.무기 || dragCardView.cardType == CardType.주문) 
            && dragCard)
        {
            dragCardName = dragCardView.GetName();
            Vector2Int pair = DataMng.instance.GetPairByName(
                DataMng.instance.playData.GetCardName(dragCardName));
            string ability_string = DataMng.instance.ToString(pair.x, pair.y, "명령어");
            spellList = SpellManager.instance.SpellParsing(ability_string);
            for (int i = 0; i < spellList.Count; i++)
                if (spellList[i].Condition_type != SpellAbility.Condition.선택 &&
                    SpellManager.instance.CheckEvent(spellList[i]) == SpellManager.EventType.대상선택)
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
                Debug.Log("드래그미니언");
                SpellManager.instance.RunSpellTargetMinion(
                    dragCardName, dragCardNum, SpellManager.instance.targetMinion, false);
            }
            else if (SpellManager.instance.targetHero != -1)
            {
                Debug.Log("드래그영웅");
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
        {
            //드래그 중인 카드가 사용가능하다고
            //내려놓았을때
            CardHand.instance.UseCard(dragCardNum);
        }
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

    //(StartPos) Screen좌표   //
    public void ShowDropEffectMinion(Vector2 startPos,Vector2 pos, int n)
    {
        dropEffect.dropPos = pos;
        Vector2 v = Camera.main.ScreenToWorldPoint(startPos);
        dropEffect.dropRectTransform.transform.position = v;
        dropEffect.dropEffectAni.SetTrigger("Effect_Minion_" + n);
    }

    public void ShowDropEffectMinion(Vector2 pos,int n)
    {
        ShowDropEffectMinion(Input.mousePosition, pos, n);
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
        ShowDropEffectSpell(Input.mousePosition, pos, n);
    }

    public void ShowDropEffectSpell(Vector2 startPos, Vector2 pos, int n)
    {
        dropEffect.dropPos = pos;
        Vector2 v = Camera.main.ScreenToWorldPoint(startPos);
        dropEffect.dropRectTransform.transform.position = v;
        dropEffect.dropEffectAni.SetTrigger("Effect_Spell_" + n);
        dropEffect.effectArrive = false;
    }

    public void GotoHandEffect(Vector2 pos,string s,bool enemy)
    {
        for (int i = 0; i < gotoHandList.Count; i++)
        {
            if (gotoHandList[i].cardHide)
            {
                gotoHandList[i].cardHide = false;
                gotoHandList[i].transform.position = pos;
                CardViewManager.instance.CardShow(ref gotoHandList[i].dropEffectCardView, s);
                CardViewManager.instance.UpdateCardView(0.001f);
                if (enemy || CardHand.instance.handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                    gotoHandList[i].dropEffectAni.SetTrigger("GoHand");
                else
                    gotoHandList[i].dropEffectAni.SetTrigger("GoHand_Small");
                break;
            }
        }
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
