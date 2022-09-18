using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class CardHand : MonoBehaviour
{
    public static CardHand instance;

    private static Vector2 HandCardSize = new Vector2(10.685f, 13.714f);
    public static Vector2 handCardSize
    {
        get
        {
            return HandCardSize;
        }
    }


    [Header("패 오브젝트")]
    public List<RectTransform> card = new List<RectTransform>();
    private CardView[] handCard;

    List<Vector3> cardStartPos = new List<Vector3>();
    List<Vector2> cardStartSize = new List<Vector2>();
    [HideInInspector] public List<bool> canUse = new List<bool>();
    List<float> cardStartAngle = new List<float>();
    List<float> handLerp = new List<float>();
    public Animator handAni;

    [Header("패 글로우")]
    public List<RectTransform> card_glow = new List<RectTransform>();
    Image[] glowImg;
    public Sprite minionImg_legend;
    public Sprite minionImg;
    public Sprite spellImg;
    public Sprite weaponImg;

    [Header("패의 크기")]
    public Vector2 defaultSize;

    [Header("패 사이의 각도")]
    [Range(0, 90)]
    public float angle;

    [Header("최대 패의 각도")]
    [Range(0, 180)]
    public float maxAngle;

    [Header("중심적까지의 거리")]
    public float range;

    [Header("현재패의수")]
    [Range(0, 10)]
    public int nowHandNum;
    public Transform drawCardPos;

    public Material glowNormalMat;
    public Material glowComboMat;

    [HideInInspector] public int useCardNum = 0;

    //손패 카드 코스트 증감처리
    [HideInInspector] public int[] handCostOffset;

    //다음주문카드 비용감소
    [HideInInspector] public int nextSpellCostOffset;


    [HideInInspector] public int removeCostOffset = 0;

    int exhaustion = 0;

    #region[Awake]
    private void Awake()
    {
        instance = this;
        if (card.Count <= 0)
            return;
        handCard = new CardView[card.Count];
        handCostOffset = new int[card.Count];

        for (int i = 0; i < card.Count; i++)
        {
            cardStartPos.Add(Vector4.zero);
            cardStartSize.Add(Vector2.zero);
            cardStartAngle.Add(0);
            canUse.Add(false);
            handCard[i] = card[i].transform.Find("Card").GetComponent<CardView>();
            handLerp.Add(1);
        }

        if (card_glow.Count <= 0)
            return;
        glowImg = new Image[card_glow.Count];
        for (int i = 0; i < card_glow.Count; i++)
            glowImg[i] = card_glow[i].GetComponent<Image>();

    }
    #endregion

    #region[Update]
    public void Update()
    {
        UpdateCardHand();
        SetCardUse();
    }
    #endregion

    #region[카드 UI 업데이트]
    public void UpdateCardHand()
    {
        for (int i = 0; i < card.Count; i++)
        {
            card[i].gameObject.SetActive(i < nowHandNum);
            if (!card[i].gameObject.activeSelf)
                card[i].transform.position = drawCardPos.position;
            handCard[i].cardCostOffset = handCostOffset[i];
            if (handCard[i].cardType == CardType.주문)
                handCard[i].cardCostOffset -= nextSpellCostOffset;
        }

        for (int i = 0; i < card.Count; i++)
        {
            if (Application.isPlaying)
            {
                int cost = GetCardCost(i);

                if (handCard[i].cardType == CardType.무기)
                    glowImg[i].sprite = weaponImg;
                else if (handCard[i].cardType == CardType.주문)
                    glowImg[i].sprite = spellImg;
                else if (handCard[i].cardType == CardType.하수인)
                {
                    if (handCard[i].cardLevel.Equals("전설"))
                        glowImg[i].sprite = minionImg_legend;
                    else
                        glowImg[i].sprite = minionImg;
                }

                card_glow[i].gameObject.SetActive(
                    canUse[i] &&
                    !GameEventManager.instance.EventCheck() &&
                    BattleUI.instance.gameStart &&
                    TurnManager.instance.turnAniEnd &&
                    TurnManager.instance.turn == Turn.플레이어 &&
                    !handCard[i].hide &&
                    card[i].gameObject.activeSelf &&
                    cost <= ManaManager.instance.playerNowMana &&
                    DragCardObject.instance.dropEffect.dropEffectAni.GetCurrentAnimatorStateInfo(0).IsName("DropEffect_Stop") &&
                    ((handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대중") && 
                    handAni.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99) ||
                    handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대") ||
                    handAni.GetCurrentAnimatorStateInfo(0).IsName("패 기본상태")));

                if (useCardNum > 0 && handCard[i].cardType == CardType.무기 &&
                    handCard[i].WeaponCardExplainData.Contains("연계"))
                    glowImg[i].material = glowComboMat;
                else if (useCardNum > 0 && handCard[i].cardType == CardType.주문 &&
                    handCard[i].SpellCardExplainData.Contains("연계"))
                    glowImg[i].material = glowComboMat;
                else if (useCardNum > 0 && handCard[i].cardType == CardType.하수인 &&
                    handCard[i].MinionsCardExplainData.Contains("연계"))
                    glowImg[i].material = glowComboMat;
                else
                    glowImg[i].material = glowNormalMat;
            }
            else
                card_glow[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < nowHandNum; i++)
        {
            float fullAngle;
            float addAngle;
            if ((nowHandNum - 1) * angle > maxAngle)
            {
                fullAngle = maxAngle;
                if (nowHandNum <= 1)
                    addAngle = 0;
                else
                    addAngle = maxAngle / (nowHandNum - 1);
            }
            else
            {
                fullAngle = (nowHandNum - 1) * angle;
                addAngle = angle;
            }
            float tempAngle = fullAngle / 2f;
            tempAngle -= i * addAngle;
            Vector3 destinationPos = Quaternion.Euler(0, 0, tempAngle) * Vector3.up;
            destinationPos = transform.position + (Vector3)destinationPos * range * transform.localScale.x;

            if (!Application.isPlaying)
            {
                card[i].transform.position = destinationPos;
                card[i].transform.rotation = Quaternion.Euler(0, 0, tempAngle);
            }
            else
            {
                Vector3 nowPos = Vector3.Lerp(cardStartPos[i], destinationPos, handLerp[i]);
                float nowAngle = Mathf.Lerp(cardStartAngle[i], tempAngle, handLerp[i]);
                Vector2 nowSize = Vector2.Lerp(cardStartSize[i], defaultSize, handLerp[i]);

                if (handLerp[i] < 1)
                    handLerp[i] += Time.deltaTime * 2;
                else
                {
                    cardStartPos[i] = destinationPos;
                    cardStartAngle[i] = tempAngle;
                    cardStartSize[i] = defaultSize;
                    handLerp[i] = 1;
                }

                card[i].transform.position = nowPos;
                card[i].transform.rotation = Quaternion.Euler(0, 0, nowAngle);
                card[i].localScale = new Vector3(nowSize.x, nowSize.y, 0);
            }
        }
    }
    #endregion

    #region[카드 사용 가능 여부]
    public void SetCardUse()
    {
        if (Application.isPlaying == false)
            return;

        for (int i = 0; i < card.Count; i++)
        {
            //게임이 시작되어야지 카드를 사용 할 수 있다.
            canUse[i] = BattleUI.instance.gameStart;

            //마나가 충분해야지 사용가능
            int cost = GetCardCost(i);
            canUse[i] = (cost <= ManaManager.instance.playerNowMana);

            if (handCard[i].cardType == CardType.무기)
            {
                //무기는 사용할 마나만 충분하면 바로 사용가능하다.
            }
            else if (handCard[i].cardType == CardType.주문)
            {
                //주문의 명령어 코드를 읽어들인다.
                DataMng dataMng = DataMng.instance;
                Vector2Int pair = dataMng.GetPairByName(DataParse.
                    GetCardName(handCard[i].GetName()));
                string ability_string = dataMng.ToString(pair.x, pair.y, "명령어");
                List<SpellAbility> spellList =
                    SpellManager.instance.SpellParsing(ability_string);

                foreach (SpellAbility sAbility in spellList)
                {
                    //명령어를 탐색해본다.
                    if (HeroManager.instance.EquipWeapon() == false)
                    {
                        //무기를 장착중이지 않은 상태
                        //무기관련 효과는 발동 할 수 없다.
                        switch (sAbility.AbilityType)
                        {
                            case SpellAbility.Ability.무기에_공격력부여:
                            case SpellAbility.Ability.무기파괴:
                            case SpellAbility.Ability.무기의_공격력만큼능력부여:
                                {
                                    canUse[i] &= false;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
            }
            else if (handCard[i].cardType == CardType.하수인)
            {
                if (MinionField.instance.FullField())
                {
                    //필드가 꽉차면 하수인 카드를 사용할 수 없다.
                    canUse[i] &= false;
                }
            }
        }
    }
    #endregion

    #region[카드 비용 계산]
    public int GetCardCost(int n)
    {
        //카드 비용 계산
        int cost = handCard[n].GetCost() + handCostOffset[n];
        if (handCard[n].cardType == CardType.주문)
        {
            //주문카드 비용감소     처리
            cost -= nextSpellCostOffset;
        }
        cost = Mathf.Max(0, cost);
        return cost;
    }
    #endregion

    #region[카드 드로우]
    public void CardDrawAct()
    {
        StartCoroutine(CardDrawActRun());
    }

    private IEnumerator CardDrawActRun()
    {
        int index = -1;

        for (int i = 0; i < BattleUI.instance.playerCardAni.Length; i++)
        {
            if (BattleUI.instance.playerCardAni[i].GetCurrentAnimatorStateInfo(0).IsName("카드일반"))
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            while (GameEventManager.instance.GetEventValue() > 0.1f)
                yield return new WaitForSeconds(0.001f);

            exhaustion++;
            BattleUI.instance.exhaustionUI.SetActive(true);
            BattleUI.instance.exhaustionText.text = "카드가 없습니다! " + exhaustion + "의\n 피해를 입습니다.";

            GameEventManager.instance.EventAdd(2f);
            yield return new WaitForSeconds(2f);
            BattleUI.instance.exhaustionUI.SetActive(false);

            if(handAni.GetCurrentAnimatorStateInfo(0).IsName("패확대"))
                handAni.SetTrigger("축소");

            AttackManager.instance.PopAllDamageObj();
            AttackManager.instance.AddDamageObj(HeroManager.instance.heroHpManager.playerHeroDamage, exhaustion);
            AttackManager.instance.AttackEffectRun();
            GameEventManager.instance.EventAdd(0.5f);
        }
        else if (nowHandNum >= 10)
        {
            int pDeckNum = InGameDeck.instance.GetDeckCardNum();
            if (pDeckNum == 1)
            {
                if (HeroManager.instance.heroPowerManager.playerHeroName == "발리라")
                    BattleUI.instance.playerText.ShowText("카드가 없어!!");
                else if (HeroManager.instance.heroPowerManager.playerHeroName == "말퓨리온")
                    BattleUI.instance.playerText.ShowText("카드가 없다!!");
                SoundManager.instance.PlayCharacterSE(HeroManager.instance.heroPowerManager.playerHeroName, 영웅상태.덱이없다);
            }
            else if (pDeckNum == 2)
            {
                if (HeroManager.instance.heroPowerManager.playerHeroName == "발리라")
                    BattleUI.instance.playerText.ShowText("카드가 거의 없어.");
                else if (HeroManager.instance.heroPowerManager.playerHeroName == "말퓨리온")
                    BattleUI.instance.playerText.ShowText("카드가 거의 없다.");
                SoundManager.instance.PlayCharacterSE(HeroManager.instance.heroPowerManager.playerHeroName, 영웅상태.덱이적다);
            }

            GameEventManager.instance.EventAdd(0.3f);
            BattleUI.instance.playerCardAni[index].SetTrigger("Draw");
            string topCard = InGameDeck.instance.GetTopCard();
            InGameDeck.instance.PopTopCard();
            DrawCardRemove.instance.RemoveCard(topCard, false);
        }
        else
        {
            int pDeckNum = InGameDeck.instance.GetDeckCardNum();
            if (pDeckNum == 1)
            {
                if (HeroManager.instance.heroPowerManager.playerHeroName == "발리라")
                    BattleUI.instance.playerText.ShowText("카드가 없어!");
                else if (HeroManager.instance.heroPowerManager.playerHeroName == "말퓨리온")
                    BattleUI.instance.playerText.ShowText("카드가 없다!");
                SoundManager.instance.PlayCharacterSE(HeroManager.instance.heroPowerManager.playerHeroName, 영웅상태.덱이없다);
            }
            else if (pDeckNum == 2)
            {
                if (HeroManager.instance.heroPowerManager.playerHeroName == "발리라")
                    BattleUI.instance.playerText.ShowText("카드가 거의 없어.");
                else if (HeroManager.instance.heroPowerManager.playerHeroName == "말퓨리온")
                    BattleUI.instance.playerText.ShowText("카드가 거의 없다.");
                SoundManager.instance.PlayCharacterSE(HeroManager.instance.heroPowerManager.playerHeroName, 영웅상태.덱이적다);
            }
            
            GameEventManager.instance.EventAdd(0.3f);
            BattleUI.instance.playerCardAni[index].SetTrigger("Draw");
            SoundManager.instance.PlaySE("카드드로우");
            DrawCard();
            string topCard = InGameDeck.instance.GetTopCard();
            InGameDeck.instance.PopTopCard();
            SetCardHand(topCard, nowHandNum - 1, drawCardPos.transform.position, defaultSize, 0);
            CardViewManager.instance.UpdateCardView(0.001f);
        }

        yield return new WaitForSeconds(0.001f);
    }


    public void DrawCard()
    {
        if (nowHandNum >= 10)
            return;
        nowHandNum++;
        for (int i = 0; i < nowHandNum-1; i++)
        {
            SetCardHand(i, card[i].transform.position,
                defaultSize,
                card[i].transform.rotation.eulerAngles.z > 180 ?
                instance.card[i].transform.rotation.eulerAngles.z - 360 : 
                instance.card[i].transform.rotation.eulerAngles.z);
        }
    }
    #endregion

    #region[패에 카드 배치]
    public void SetCardHand(int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        //n번째 카드를 pos위치,size크기,angle각도인 상태에서
        //원래 있어야할 패위치로 이동한다.
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
    }

    public void SetCardHand(string name, int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        //n번째 카드를 name기준으로 변환하고
        //pos위치,size크기,angle각도인 상태에서
        //원래 있어야할 패위치로 이동한다.
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
        CardViewManager.instance.CardShow(ref handCard[n], name);
        CardViewManager.instance.UpdateCardView();
    }

    public void SetCardHand(CardView cardView, int n, Vector3 pos, Vector2 size, float angle = 0)
    {
        //n번째 카드를 cardView와 같이 변환하고
        //pos위치,size크기,angle각도인 상태에서
        //원래 있어야할 패위치로 이동한다.
        handLerp[n] = 0;
        cardStartPos[n] = pos;
        cardStartSize[n] = size;
        cardStartAngle[n] = angle;
        CardViewManager.instance.CardShow(ref handCard[n], cardView);
        CardViewManager.instance.UpdateCardView();
    }
    public void SetCardHand(int n, string name)
    {
        //n번째 카드를 name기준으로 변환하고
        CardViewManager.instance.CardShow(ref handCard[n], name);
        CardViewManager.instance.UpdateCardView();
    }
    #endregion

    #region[카드 사용]
    public void UseCard(int n)
    {
        if (DragCardObject.instance.dragSelectCard)
        {
            //카드를 드래그중이면 발동이 안된다.
            return;
        }

        //카드 비용 계산
        int cost = GetCardCost(n);
        ManaManager.instance.playerNowMana -= cost;

        //사용갯수 
        useCardNum++;

        //카드 사용이 취소될때 받을값
        removeCostOffset = handCostOffset[n];

        if (handCard[n].cardType == CardType.주문)
        {
            //주문 이펙트가 켜지고
            //주문이 실행된다.
            DragCardObject.instance.ShowDropEffectSpell(Input.mousePosition, 0);
            SpellManager.instance.RunSpell(handCard[n].GetName());
        }
        else if (handCard[n].cardType == CardType.무기)
        {
            //무기카드도 주문카드의 일종으로 취급되서 발동된다.
            SpellManager.instance.RunSpell(handCard[n].GetName());
        }
        else if (handCard[n].cardType == CardType.하수인)
        {
            //하수인을  소환
            MinionField.instance.AddMinion(handCard[n].GetName(), true);
        }

        //n번째 카드를 패에서 제거
        CardRemove(n);
    }
    #endregion

    #region[카드없애기]
    public void CardRemove(int n)
    {
        handCostOffset[n] = 0;
        for (int i = n; i < 9; i++)
            handCostOffset[i] = handCostOffset[i + 1];
        nowHandNum--;
        for (int i = 0; i < nowHandNum; i++)
        {
            float fullAngle;
            float addAngle;
            if ((nowHandNum - 1) * angle > maxAngle)
            {
                fullAngle = maxAngle;
                if (nowHandNum <= 1)
                    addAngle = 0;
                else
                    addAngle = maxAngle / (nowHandNum - 1);
            }
            else
            {
                fullAngle = (nowHandNum - 1) * angle;
                addAngle = angle;
            }
            float tempAngle = fullAngle / 2f;
            tempAngle -= i * addAngle;
            int cardViewNum = (i >= n) ? i + 1 : i;
            SetCardHand(handCard[cardViewNum], i, card[cardViewNum].transform.position, defaultSize, tempAngle);
        }
        CardViewManager.instance.UpdateCardView(0.001f);
    }
    #endregion

    #region[OnDrawGizmosSelected]
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < nowHandNum; i++)
            Gizmos.DrawLine(transform.position, card[i].transform.position);

        Gizmos.color = Color.green;

        Vector3 v1 = Quaternion.Euler(0, 0, -maxAngle / 2f) * Vector3.up * range * transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + v1);
        Vector3 v2 = Quaternion.Euler(0, 0, maxAngle / 2f) * Vector3.up * range * transform.localScale.x;
        Gizmos.DrawLine(transform.position, transform.position + v2);
    }
    #endregion
}
