using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewManager : MonoBehaviour
{
    public static CardViewManager instance;

   [HideInInspector] public List<CardView> cardview = new List<CardView>();

    #region[Awake]
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    #region[UpdateCardView]
    public void UpdateCardView()
    {
        for (int i = 0; i < cardview.Count; i++)
            if(cardview[i] != null)
                cardview[i].updateCard = true;
    }

    public void UpdateCardView(float waitTime)
    {
        StartCoroutine(UpdateCardViewEvent_C(waitTime));
    }

    IEnumerator UpdateCardViewEvent_C(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        UpdateCardView();
    }

    #endregion

    #region[카드표시]
    public void CardShow(ref CardView card, string name)
    {
        Vector2Int pair = DataMng.instance.GetPairByName(name);
        string cardType = DataMng.instance.ToString(pair.x, pair.y, "카드종류");
        int cost = DataMng.instance.ToInteger(pair.x, pair.y, "코스트");
        int power = DataMng.instance.ToInteger(pair.x, pair.y, "공격력");
        int hp = DataMng.instance.ToInteger(pair.x, pair.y, "체력");
        string cardExplain = DataMng.instance.ToString(pair.x, pair.y, "카드설명");
        string level = DataMng.instance.ToString(pair.x, pair.y, "등급");

        if (cardType.Equals("하수인"))
        {
            card.cardType = CardType.하수인;
            card.SetCost(cost);
            card.MinionsAttackData = power;
            card.MinionsHpData = hp;
            card.SetName(name);
            card.MinionsCardExplainData = cardExplain;
        }
        else if (cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SetCost(cost);
            card.SetName(name);
            card.SpellCardExplainData = cardExplain;
        }
        else if (cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.SetCost(cost);
            card.WeaponAttackData = power;
            card.WeaponHpData = hp;
            card.SetName(name);
            card.WeaponCardExplainData = cardExplain;
        }
        card.cardLevel = level;
        card.cardJob = ((DataMng.TableType)pair.x).ToString();
    }

    public void CardShow(ref CardView card, CardView cardCopy)
    {
        string cardType = cardCopy.cardType.ToString();

        if (cardType.Equals("하수인"))
        {
            card.cardType = CardType.하수인;
            card.SetCost(cardCopy.GetCost());
            card.MinionsAttackData = cardCopy.MinionsAttackData;
            card.MinionsHpData = cardCopy.MinionsHpData;
            card.SetName(cardCopy.GetName());
            card.MinionsCardExplainData = cardCopy.MinionsCardExplainData;
        }
        else if (cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SetCost(cardCopy.GetCost());
            card.SetName(cardCopy.GetName());
            card.SpellCardExplainData = cardCopy.SpellCardExplainData;
        }
        else if (cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.SetCost(cardCopy.GetCost());
            card.WeaponAttackData = cardCopy.WeaponAttackData;
            card.WeaponHpData = cardCopy.WeaponHpData;
            card.SetName(cardCopy.GetName());
            card.WeaponCardExplainData = cardCopy.WeaponCardExplainData;
        }
        card.cardCostOffset = cardCopy.cardCostOffset;
        card.cardLevel = cardCopy.cardLevel;
        card.cardJob = cardCopy.cardJob;
    }

    public void CardShow(ref CardView card, CardData cardData)
    {
        string cardType = cardData.cardType;

        if (cardType.Equals("하수인"))
        {
            card.cardType = CardType.하수인;
            card.SetCost(cardData.cardCost);
            card.MinionsAttackData = cardData.cardAttack;
            card.MinionsHpData = cardData.cardHp;
            card.SetName(cardData.cardName);
            card.MinionsCardExplainData = cardData.cardExplain;
        }
        else if (cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SetCost(cardData.cardCost);
            card.SetName(cardData.cardName);
            card.SpellCardExplainData = cardData.cardExplain;
        }
        else if (cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.SetCost(cardData.cardCost);
            card.WeaponAttackData = cardData.cardAttack;
            card.WeaponHpData = cardData.cardHp;
            card.SetName(cardData.cardName);
            card.WeaponCardExplainData = cardData.cardExplain;
        }
        card.cardLevel = cardData.cardLevel;
        card.cardJob = cardData.cardJob.ToString();
    }
    #endregion

}
