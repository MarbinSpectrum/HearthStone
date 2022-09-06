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
            //else
            //{
            //    cardview.RemoveAt(i);
            //    break;
            //}
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
        Vector2 pair = DataMng.instance.GetPairByName(name);
        string cardType = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToString((int)pair.y, "카드종류");
        int cost = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToInteger((int)pair.y, "코스트");
        int power = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToInteger((int)pair.y, "공격력");
        int hp = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToInteger((int)pair.y, "체력");
        string cardExplain = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToString((int)pair.y, "카드설명");
        string level = DataMng.instance.m_dic[(DataMng.TableType)pair.x].ToString((int)pair.y, "등급");

        if (cardType.Equals("하수인"))
        {
            card.cardType = CardType.하수인;
            card.MinionsCostData = cost;
            card.MinionsAttackData = power;
            card.MinionsHpData = hp;
            card.MinionsCardNameData = name;
            card.MinionsCardExplainData = cardExplain;
        }
        else if (cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SpellCostData = cost;
            card.SpellCardNameData = name;
            card.SpellCardExplainData = cardExplain;
        }
        else if (cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.WeaponCostData = cost;
            card.WeaponAttackData = power;
            card.WeaponHpData = hp;
            card.WeaponCardNameData = name;
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
            card.MinionsCostData = cardCopy.MinionsCostData;
            card.MinionsAttackData = cardCopy.MinionsAttackData;
            card.MinionsHpData = cardCopy.MinionsHpData;
            card.MinionsCardNameData = cardCopy.MinionsCardNameData;
            card.MinionsCardExplainData = cardCopy.MinionsCardExplainData;
        }
        else if (cardType.Equals("주문"))
        {
            card.cardType = CardType.주문;
            card.SpellCostData = cardCopy.SpellCostData;
            card.SpellCardNameData = cardCopy.SpellCardNameData;
            card.SpellCardExplainData = cardCopy.SpellCardExplainData;
        }
        else if (cardType.Equals("무기"))
        {
            card.cardType = CardType.무기;
            card.WeaponCostData = cardCopy.WeaponCostData;
            card.WeaponAttackData = cardCopy.WeaponAttackData;
            card.WeaponHpData = cardCopy.WeaponHpData;
            card.WeaponCardNameData = cardCopy.WeaponCardNameData;
            card.WeaponCardExplainData = cardCopy.WeaponCardExplainData;
        }
        card.cardCostOffset = cardCopy.cardCostOffset;
        card.cardLevel = cardCopy.cardLevel;
        card.cardJob = cardCopy.cardJob;
    }
    #endregion

}
